using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3.View
{
    public partial class simpleView : UIPage
    {
        public simpleView()
        {
            InitializeComponent();
        }

        private void simpleView_Load(object sender, EventArgs e)
        {

        }

        //刷新simpleView中的数据
        public void refreshData(ushort data)
        {
            //重要：在异步任务里面，如果我们需要对控件进行一个更改，要把它抛出到 UI 线程才行
            this.Invoke(new Action(() =>
            {
                //将读取到的数据传递给simpleView中UI属性的uiTextBox1进行刷新赋值
                uiTextBox7.Text = data.ToString();
            }));
        }
    }
}
