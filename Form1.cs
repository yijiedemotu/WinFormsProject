using gkHelper;
using Sunny.UI;
using WinFormsApp3.Common;
using WinFormsApp3.Helper;
using WinFormsApp3.View;

namespace WinFormsApp3
{
    public partial class Form1 : UIForm
    {
        public Form1()
        {
            InitializeComponent();

            //当窗体加载时，拿到通讯对象modbusRtuClient，并传递给mainView、totalView、plotView、yvzhiView
            var isConnected = mainCommon.modbusRtuClient.Connect("com2");
            //如果连接成功，小灯变为绿色，否则不变色
            if (isConnected)
            {
                uiLight1.State = UILightState.On;
            }

        }
        // 创建mainView的实例
        private mainView mainView = new mainView();

        //当点击按钮时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
        //Helper.FormShowHelper.formShow(new View.mainView(), uiPanel1);
        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            FormShowHelper.formShow(mainView, uiPanel2);
        }

        //在窗体加载时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
        private void Form1_Load(object sender, EventArgs e)
        {
            FormShowHelper.formShow(mainView, uiPanel2);
        }

        //在窗体显示时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
        private void Form1_Shown(object sender, EventArgs e)
        {

        }

        // 创建totalView的实例
        private totalView totalView = new totalView();

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            //当点击按钮时，调用FormShowHelper.formShow方法，将totalView嵌入到uiPanel1中
            FormShowHelper.formShow(totalView, uiPanel2);
        }

        // 创建plotView的实例
        private plotView plotView = new plotView();
        private void uiSymbolButton3_Click(object sender, EventArgs e)
        {
            FormShowHelper.formShow(plotView, uiPanel2);
        }

        // 创建yvzhiView的实例
        private yvzhiView yvzhiView = new yvzhiView();
        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            FormShowHelper.formShow(yvzhiView, uiPanel2);
        }
    }
}
