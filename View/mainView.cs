using Sunny.UI;
using System.ComponentModel;
using WinFormsApp3.Common;
using WinFormsApp3.Model;

namespace WinFormsApp3.View
{
    public partial class mainView : UIPage
    {
        //创建一个simpleView的集合，存放所有创建的simpleView实例
        private List<simpleView> simpleViews = new List<simpleView>();

        //创建一个UILight的实例，用于显示连接状态
        private UILight uiLight1;


        //data里是下位机的数据
        private ushort[] data = new ushort[8];

        private BindingList<dataModel> bindingList = new BindingList<dataModel>();


        //mainView的构造函数
        public mainView(UILight uiLight1)
        {
            InitializeComponent();
            //将传递过来的UILight实例赋值给mainView中的uiLight1,
            //this.uiLight1是mainView中的uiLight1，uiLight1是Form1中的uiLight1
            this.uiLight1 = uiLight1;

            dataGridView1.DataSource = bindingList;//数据源绑定
        }


        //创建一个Task类型的变量readTask，用于异步读取下位机数据
        private Task readTask;
        private CancellationTokenSource readCTS = new CancellationTokenSource();

        //`StartRead()` 是 UI 线程（mainView_Load 触发，Load 事件是 UI 线程）
        private void StartRead()
        {
            //`Task.Run( async ()=>{ ... })` 里面整个 while 循环 = **后台子线程**
            readTask = Task.Run(async () =>
            {
                try
                {
                    while (!readCTS.Token.IsCancellationRequested)
                    {
                        if (mainCommon.modbusRtuClient.IsConnected)
                        {
                            ushort[] tempData = mainCommon.modbusRtuClient.ReadHoldingRegisters(0, 8);
                            Array.Copy(tempData, data, tempData.Length);
                            int i = 0;
                            foreach (var item in simpleViews)
                            {
                                //`item.refreshData(data[i])`：**后台线程调用 simpleView 的方法**
                                //`refreshData` 里面 `this.Invoke(...)`：把 lambda 里面赋值代码**扔回 UI 线程**
                                item.refreshData(tempData[i]);
                                i++;
                            }
                        }
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"读取数据时发生异常: {ex.Message}");
                }
            }, readCTS.Token);
        }



        //事件，创建8个simpleView
        private void mainView_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                //创建一个simpleView实例
                var simpleView = new simpleView();
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
                data = mainCommon.modbusRtuClient.ReadHoldingRegisters(0, 8);
                //定义一个变量i，用于遍历simpleViews集合中的simpleView实例
                int i = 0;
                foreach (var item in simpleViews)
                {
                    //将存放在数组中的每一位数据传递给simpleView进行刷新
                    item.refreshData(data[i]);
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
            string comName = uiComboBox1.Text;

            //当窗体加载时，拿到通讯对象modbusRtuClient，并传递给mainView、totalView、plotView、yvzhiView
            bool isConnected = mainCommon.modbusRtuClient.Connect(comName);
            //如果连接成功，小灯变为绿色，否则不变色
            if (isConnected)
            {
                uiLight1.State = UILightState.On;
            }
        }

        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //当选择下拉框中的选项时，获取选中的值，并将其转换为整数类型


        }



        //当点击uiButton2按钮时，读取下位机的数据，并将数据存放在dataModel中
        private void uiButton3_Click(object sender, EventArgs e)
        {
            //创建dataModel的实例，用于存放读取到的数据
            dataModel dataModel = new dataModel()
            {
                Id = bindingList.Count + 1,
                testTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                projectName = uiTextBox1.Text,
                data1 = data[0].ToString(),
                data2 = data[1].ToString(),
                data3 = data[2].ToString(),
                data4 = data[3].ToString(),
                data5 = data[4].ToString(),
                data6 = data[5].ToString(),
                data7 = data[6].ToString(),
                data8 = data[7].ToString()
            };
            bindingList.Add(dataModel);
        }
    }
}
