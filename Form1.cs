using Sunny.UI;
using WinFormsApp3.Helper;
using WinFormsApp3.View;

namespace WinFormsApp3
{
    public partial class Form1 : UIForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private mainView mainView = new mainView(); // 创建mainView的实例

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            //当点击按钮时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
            //Helper.FormShowHelper.formShow(new View.mainView(), uiPanel1);
            FormShowHelper.formShow(mainView, uiPanel2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //在窗体加载时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
            FormShowHelper.formShow(mainView, uiPanel2);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }
    }
}
