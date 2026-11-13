using gkHelper;
using Sunny.UI;
using WinFormsApp3.Common;
using WinFormsApp3.Helper;
using WinFormsApp3.View;

namespace WinFormsApp3
{
    //窗体的大框架
    public partial class Form1 : UIForm
    {

        // 创建mainView的实例
        private mainView mainView;

        //Form1的构造函数
        public Form1()
        {
            InitializeComponent();

        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            //当点击按钮时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
            FormShowHelper.formShow(mainView, uiPanel2);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // 创建mainView的实例，并将uiLight1传递给mainView的构造函数
            mainView = new mainView(uiLight1); 
            //在窗体加载时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
            FormShowHelper.formShow(mainView, uiPanel2);
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            //在窗体显示时，调用FormShowHelper.formShow方法，将mainView嵌入到uiPanel1中
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
            //当点击按钮时，调用FormShowHelper.formShow方法，将plotView嵌入到uiPanel1中
            FormShowHelper.formShow(plotView, uiPanel2);
        }

        // 创建yvzhiView的实例
        private yvzhiView yvzhiView = new yvzhiView();
        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            //当点击按钮时，调用FormShowHelper.formShow方法，将yvzhiView嵌入到uiPanel1中
            FormShowHelper.formShow(yvzhiView, uiPanel2);
        }
    }
}
