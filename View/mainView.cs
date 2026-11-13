using Sunny.UI;
using WinFormsApp3.Common;

namespace WinFormsApp3.View
{
    public partial class mainView : UIPage
    {
        public mainView()
        {
            InitializeComponent();
        }

        //创建一个simpleView的集合，存放所有创建的simpleView实例
        private List<simpleView> simpleViews = new List<simpleView>();

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
        }

        //mainView里的timer计时器，用于定时刷新simpleView中的数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            //如果与下位机连接，则刷新simpleView中的数据
            if(mainCommon.modbusRtuClient.IsConnected)
            {
                //读取下位机的保持寄存器数据，地址为0，长度为8,并将读取到的数据存放在data数组中，如[0,1,2,3,4,5,6,7]
                var data = mainCommon.modbusRtuClient.ReadHoldingRegisters(0, 8);
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
    }
}
