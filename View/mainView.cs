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
                long lastErrorLog = -10000;   // 保证第一次失败就会记日志
                while (!token.IsCancellationRequested)
                {
                    // try 放在 while 里面：一次超时/掉线只跳过本轮，不能把整个采集循环终结掉
                    try
                    {
                        if (mainCommon.modbusRtuClient.IsConnected)
                        {
                            ushort[] tempData = mainCommon.modbusRtuClient.ReadHoldingRegisters(0, 8);
                            // ushort[] -> double[] 逐项赋值，避免 Array.Copy 的类型转换歧义
                            // Volatile：采集线程写、UI 线程读，避免 32 位环境下 double 撕裂读
                            for (int k = 0; k < tempData.Length && k < mainCommon.data.Length; k++)
                                Volatile.Write(ref mainCommon.data[k], tempData[k]);

                            // 切到别的页面 / 句柄已销毁时不碰控件，只继续更新全局数据
                            if (!IsDisposed && IsHandleCreated && channelsVisible)
                            {
                                for (int i = 0; i < simpleViews.Count && i < tempData.Length; i++)
                                {
                                    //`item.refreshData(data[i])`：**后台线程调用 simpleView 的方法**
                                    //`refreshData` 里面把赋值投递回 UI 线程
                                    simpleViews[i].refreshData(tempData[i]);
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException) { break; }
                    catch (Exception ex)
                    {
                        // 后台线程里 MessageBox.Show 会弹出无属主窗口甚至卡住退出，改为写日志；
                        // 连续失败时限流，避免每秒刷一条日志。
                        long now = Environment.TickCount64;
                        if (now - lastErrorLog > 10000)
                        {
                            Log.Error("采集数据失败（已断线，下一轮自动跳过读取）", ex);
                            lastErrorLog = now;
                        }
                    }

                    try { await Task.Delay(1000, token); }
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
                if (uiComboBox1.Items.Count > 0) uiComboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // 枚举串口失败不能拖垮页面加载（如无串口驱动、非 Windows 运行时）
                Log.Error("枚举串口失败", ex);
            }
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

        //跨页面访问控件
        //当点击uiButton1按钮时，连接下位机，并将连接状态显示在uiLight1上
        //信号灯在Form1界面，在mainView界面中无法直接访问uiLight1，
        private void uiButton1_Click(object sender, EventArgs e)
        {
            //获取下拉框中选中的值，并将其传递给modbusRtuClient进行连接
            string comName = uiComboBox1.Text.Trim();
            if (string.IsNullOrEmpty(comName))
            {
                MessageBox.Show("请先选择串口号", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //当窗体加载时，拿到通讯对象modbusRtuClient，并传递给mainView、totalView、plotView、yvzhiView
            bool isConnected;
            try
            {
                isConnected = mainCommon.modbusRtuClient.Connect(comName);
            }
            catch (Exception ex)
            {
                Log.Error("连接串口失败", ex);
                isConnected = false;
            }

            // Connect 内部会先 Disconnect，重连失败时原来的绿灯必须复位，否则状态是假的
            uiLight1.State = isConnected ? UILightState.On : UILightState.Off;
            uiLabel3.Text = isConnected ? comName : "无连接";   // 状态文字与指示灯同步
            if (!isConnected)
            {
                MessageBox.Show($"无法连接 {comName}，请检查端口是否被占用、设备是否上电。\n详细堆栈已写入 Logs 目录。",
                    "连接失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            //创建dataModel的实例，用于存放读取到的数据（Id 交给数据库自增，不用 bindingList.Count）
            dataModel record = new dataModel()
            {
                testTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                projectName = uiTextBox1.Text,
                data1 = Volatile.Read(ref mainCommon.data[0]).ToString(CultureInfo.InvariantCulture),
                data2 = Volatile.Read(ref mainCommon.data[1]).ToString(CultureInfo.InvariantCulture),
                data3 = Volatile.Read(ref mainCommon.data[2]).ToString(CultureInfo.InvariantCulture),
                data4 = Volatile.Read(ref mainCommon.data[3]).ToString(CultureInfo.InvariantCulture),
                data5 = Volatile.Read(ref mainCommon.data[4]).ToString(CultureInfo.InvariantCulture),
                data6 = Volatile.Read(ref mainCommon.data[5]).ToString(CultureInfo.InvariantCulture),
                data7 = Volatile.Read(ref mainCommon.data[6]).ToString(CultureInfo.InvariantCulture),
                data8 = Volatile.Read(ref mainCommon.data[7]).ToString(CultureInfo.InvariantCulture)
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
