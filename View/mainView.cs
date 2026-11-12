using Sunny.UI;

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
                simpleView.Show();
                //将创建的simpleView实例添加到集合中，在需要的时候可以进行操作
                simpleViews.Add(simpleView);
            }
        }

    }
}
