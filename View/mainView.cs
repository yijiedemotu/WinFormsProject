using Sunny.UI;
using System.ComponentModel;
using System.Globalization;
using TICP.Model;
using TICP.Common;
using TICP.Helper;

namespace WinFormsApp3.View
{
    public partial class mainView : UIPage
    {
        //创建一个simpleView的集合，存放所有创建的simpleView实例
        private List<simpleView> simpleViews = new List<simpleView>();

        // 页面被反复换 Parent 时 WinForms 可能再次触发 Load，防止重复加卡片 / 重复开采集任务
        private bool initialized;

        // 通道卡片区是否可见。采集后台线程要读它，用 volatile 保证可见性，
        // 不要直接在后台线程里读控件的 Visible（跨线程读控件属性是不安全的）。
        private volatile bool channelsVisible = true;

        //创建一个UILight的实例，用于显示连接状态
        private UILight uiLight1;

        // 用户主动断开后不再自动重连；后台任务读得到这个 volatile 标志
        private volatile bool manualDisconnect;

        //data里是下位机的数据
        //private ushort[] data = new ushort[8];    已经用全局变量替代

        private BindingList<dataModel> bindingList = new BindingList<dataModel>();


        //mainView的构造函数
        public mainView(UILight uiLight1)
        {
            InitializeComponent();
            //将传递过来的UILight实例赋值给mainView中的uiLight1,
            //this.uiLight1是mainView中的uiLight1，uiLight1是Form1中的uiLight1
            this.uiLight1 = uiLight1;

            dataGridView1.DataSource = bindingList;//数据源绑定

            // 启动时读一次配置：串口参数、轮询周期、重连间隔都从 appconfig.json 来
            AppConfigStore.Load();
            mainCommon.modbusRtuClient.SlaveId = AppConfigStore.Current.SlaveId;

            // 页面真正销毁时才停止采集。切页只是把页面从 uiPanel2 里摘下来（换 Parent），
            // 若在这里就取消，从别的页面切回来后采集任务永远不会重新启动。
            // 只 Cancel 不 Dispose：后台任务还要读 Token，提前 Dispose 会抛 ObjectDisposedException。
            Disposed += (s, e) => readCTS.Cancel();
        }


        //创建一个Task类型的变量readTask，用于异步读取下位机数据
        private Task? readTask;
        private readonly CancellationTokenSource readCTS = new CancellationTokenSource();

        //`StartRead()` 是 UI 线程（mainView_Load 触发，Load 事件是 UI 线程）
        private void StartRead()
        {
            //`Task.Run( async ()=>{ ... })` 里面整个 while 循环 = **后台子线程**
            CancellationToken token = readCTS.Token;   // 先取出来，避免 CTS 释放后取 Token 抛异常
            if (token.IsCancellationRequested) return;
            if (readTask != null && !readTask.IsCompleted) return;   // 已经在跑就不重复启动

            readTask = Task.Run(async () =>
            {
                long lastErrorLog = -10000;      // 保证第一次失败就会记日志
                long lastReconnect = -10000;     // 重连限流，避免端口占用时疯狂重试
                long lastNgLog = -10000;         // 超差日志限流
                AppConfig cfg = AppConfigStore.Current;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        // 断线自动重连：只有用户没主动断开、配置允许、且距上次尝试超过间隔才试
                        if (!mainCommon.modbusRtuClient.IsConnected &&
                            !manualDisconnect && cfg.AutoReconnect &&
                            !string.IsNullOrWhiteSpace(cfg.PortName) &&
                            Environment.TickCount64 - lastReconnect >= cfg.ReconnectIntervalMs)
                        {
                            lastReconnect = Environment.TickCount64;
                            TryConnect(cfg.PortName, false);   // 静默重连，不弹窗
                            UpdateConnectionUi(mainCommon.modbusRtuClient.IsConnected,
                                mainCommon.modbusRtuClient.IsConnected ? cfg.PortName + " 已连接（自动）" : "重连中…");
                        }

                        if (mainCommon.modbusRtuClient.IsConnected)
                        {
                            ushort[] raw = mainCommon.modbusRtuClient.ReadHoldingRegisters(
                                cfg.StartAddress, (ushort)cfg.ChannelCount);

                            // 原始值 → 标定/判定（纯逻辑，不碰控件）
                            QualityResult q = QualityCommon.Evaluate(raw, yuzhiCommon.Current);
                            for (int k = 0; k < q.Values.Length && k < mainCommon.data.Length; k++)
                            {
                                Volatile.Write(ref mainCommon.oriData[k], raw[k]);
                                Volatile.Write(ref mainCommon.data[k], q.Values[k]); // 全局数组存物理量
                            }

                            // 切到别的页面 / 句柄已销毁时不碰控件，只继续更新全局数据
                            if (!IsDisposed && IsHandleCreated && channelsVisible)
                            {
                                int n = Math.Min(simpleViews.Count, q.Values.Length);
                                for (int i = 0; i < n; i++)
                                    simpleViews[i].refreshQuality(q.Values[i], q.Ok[i]);
                            }

                            // 超差报警写日志（限流 10 秒），不在后台线程弹 MessageBox
                            if (q.HasNg && Environment.TickCount64 - lastNgLog > 10000)
                            {
                                Log.Warn($"通道 [{q.NgChannels}] 超差");
                                lastNgLog = Environment.TickCount64;
                            }
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        // ModbusRtuClient 读失败时已经自行 Disconnect，下一轮会走自动重连分支
                        long now = Environment.TickCount64;
                        if (now - lastErrorLog > 10000)
                        {
                            Log.Error("采集失败（已断线，将按配置自动重连）", ex);
                            lastErrorLog = now;
                        }
                        UpdateConnectionUi(false, "重连中…");
                    }

                    try { await Task.Delay(cfg.PollIntervalMs, token); }
                    catch (OperationCanceledException) { break; }
                }
            }, token);
        }



        /// <summary>填充串口下拉框，避免「下拉框没有东西」和 PortName 为空的异常。</summary>
        private void FillPorts()
        {
            try
            {
                uiComboBox1.Items.Clear();
                foreach (string port in ModbusRtuClient.GetAvailablePorts())
                    uiComboBox1.Items.Add(port);

                // 优先选中上次连接成功的端口（appconfig.json），否则选第一个
                string preferred = AppConfigStore.Current.PortName;
                if (!string.IsNullOrWhiteSpace(preferred) && uiComboBox1.Items.Contains(preferred))
                    uiComboBox1.SelectedItem = preferred;
                else if (uiComboBox1.Items.Count > 0)
                    uiComboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // 枚举串口失败不能拖垮页面加载（如无串口驱动、非 Windows 运行时）
                Log.Error("枚举串口失败", ex);
            }
        }

        /// <summary>用配置里的串口参数连接。autoReconnect 调用时静默失败，手动点击才弹窗。</summary>
        private bool TryConnect(string portName, bool showMessage)
        {
            AppConfig cfg = AppConfigStore.Current;
            cfg.PortName = portName;
            try
            {
                bool ok = mainCommon.modbusRtuClient.Connect(portName, cfg.BaudRate, cfg.DataBits,
                    cfg.ToStopBits(), cfg.ToParity());
                if (ok) AppConfigStore.Save();   // 连接成功才记住端口，避免写进一个错误端口
                else if (showMessage)
                    MessageBox.Show($"无法连接 {portName}，请检查端口占用、波特率与设备供电。\n详细堆栈已写入 Logs 目录。",
                        "连接失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ok;
            }
            catch (Exception ex)
            {
                Log.Error("连接串口失败", ex);
                if (showMessage)
                    MessageBox.Show("连接失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>后台任务也要更新指示灯/状态文字，统一走 BeginInvoke 回 UI 线程。</summary>
        private void UpdateConnectionUi(bool connected, string text)
        {
            Action update = () =>
            {
                try
                {
                    if (uiLight1 != null && !uiLight1.IsDisposed)
                        uiLight1.State = connected ? UILightState.On : UILightState.Off;
                }
                catch (InvalidOperationException) { /* 主窗体正在关闭 */ }

                if (!IsDisposed)
                {
                    uiLabel3.Text = text;
                    uiButton1.Text = connected ? "断开" : "连接";
                }
            };

            try
            {
                if (InvokeRequired) BeginInvoke(update);
                else update();
            }
            catch (InvalidOperationException) { /* 句柄在投递瞬间被销毁 */ }
        }

        //事件，创建8个simpleView
        private void mainView_Load(object sender, EventArgs e)
        {
            if (initialized) return;
            initialized = true;

            FillPorts();
            for (int i = 0; i < 8; i++)
            {
                //创建一个simpleView实例，创建顺序就是通道顺序（先左后右、先上后下）
                var simpleView = new simpleView();
                simpleView.SetChannelNumber(i + 1);   // 让卡片显示「通道1」…「通道8」
                uiTableLayoutPanel1.Controls.Add(simpleView);
                //将simpleView实例显示出来
                simpleView.Show();
                //将创建的simpleView实例添加到集合中，在需要的时候可以进行操作
                simpleViews.Add(simpleView);
            }

            //启动定时器，定时刷新simpleView中的数据
            //timer1.Start();   会卡顿，已用异步方式代替

            StartRead();
        }



        //mainView里的timer计时器，用于定时刷新simpleView中的数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (mainCommon.modbusRtuClient.IsConnected)
            {
                //读取下位机的保持寄存器数据，地址为0，长度为8,并将读取到的数据存放在data数组中，如[0,1,2,3,4,5,6,7]
                 //data = mainCommon.modbusRtuClient.ReadHoldingRegisters(0, 8);
                //定义一个变量i，用于遍历simpleViews集合中的simpleView实例
                int i = 0;
                foreach (var item in simpleViews)
                {
                    //将存放在数组中的每一位数据传递给simpleView进行刷新
                    //item.refreshData(data[i]);
                    i++;
                }
            }
        }

        //跨页面访问控件：信号灯在 Form1，由构造函数注入；这里通过 BeginInvoke 更新
        private void uiButton1_Click(object sender, EventArgs e)
        {
            // 已连接 → 主动断开，并关掉自动重连（否则下一秒又连上了）
            if (mainCommon.modbusRtuClient.IsConnected)
            {
                manualDisconnect = true;
                mainCommon.modbusRtuClient.Disconnect();
                UpdateConnectionUi(false, "无连接（已手动断开）");
                Log.Info("用户主动断开串口");
                return;
            }

            string comName = uiComboBox1.Text.Trim();
            if (string.IsNullOrEmpty(comName))
            {
                MessageBox.Show("请先选择串口号", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            manualDisconnect = false;
            bool ok = TryConnect(comName, true);
            UpdateConnectionUi(ok, ok ? comName + " 已连接" : "无连接");
        }

        /// <summary>
        /// 「隐藏通道 / 显示通道」按钮：把上方 4×2 的通道卡片区收起 / 展开。
        /// 只切 Visible，不 Dispose 卡片，也不停采集任务 —— 采集线程继续写全局数据，
        /// 只是隐藏时跳过控件刷新；重新展开后下一秒数据就会自己回来。
        /// uiTitlePanel1 是 Dock=Fill，uiPanel2 隐藏后它会自动占满空出来的高度。
        /// </summary>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            channelsVisible = !channelsVisible;
            uiPanel2.Visible = channelsVisible;
            uiButton2.Text = channelsVisible ? "隐藏通道" : "显示通道";
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //当选择下拉框中的选项时，获取选中的值，并将其转换为整数类型


        }



        //当点击uiButton2按钮时，读取下位机的数据，并将数据存放在dataModel中
        private void uiButton3_Click(object sender, EventArgs e)
        {
            // 保存的是「这一刻的采样 + 判定」，不是界面上看到的某个数字：
            // 从 oriData 快照重新算一次，保证保存到库里的物理量与判定一致。
            ushort[] snapshot = new ushort[8];
            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i] = Volatile.Read(ref mainCommon.oriData[i]);
            QualityResult q = QualityCommon.Evaluate(snapshot, yuzhiCommon.Current);
            string[] physical = QualityCommon.ToStrings(q.Values);

            dataModel record = new dataModel()
            {
                testTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                projectName = uiTextBox1.Text,
                data1 = physical.Length > 0 ? physical[0] : "0",
                data2 = physical.Length > 1 ? physical[1] : "0",
                data3 = physical.Length > 2 ? physical[2] : "0",
                data4 = physical.Length > 3 ? physical[3] : "0",
                data5 = physical.Length > 4 ? physical[4] : "0",
                data6 = physical.Length > 5 ? physical[5] : "0",
                data7 = physical.Length > 6 ? physical[6] : "0",
                data8 = physical.Length > 7 ? physical[7] : "0",
                result = q.Result,
                ngChannels = q.NgChannels
            };

            try
            {
                // 先写库再上表格：写库失败时表格里不该留一条「看着已保存」的假记录
                record.Id = mainCommon.sqlHelper.Insert(record);
                bindingList.Add(record);
            }
            catch (Exception ex)
            {
                Log.Error("保存检测记录失败", ex);
                MessageBox.Show("保存失败：" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
