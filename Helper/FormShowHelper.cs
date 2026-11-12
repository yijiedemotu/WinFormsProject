using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp3.Helper
{
    public class FormShowHelper
    {
        /// <summary>
        /// 把UIPage页面嵌入到UIPanel里面（SunnyUI 页面嵌套，代替弹出新窗口）
        /// 把新建的页面嵌入到目标的UIPanel里
        /// </summary>
        public static void formShow(UIPage uIPage, UIPanel uIPanel)
        {
            if (uIPanel != null)
            {
                uIPanel.Controls.Clear();    // 清空面板里旧控件
                uIPage.Parent = uIPanel;     // 设置父容器
                uIPage.Dock = DockStyle.Fill;// 铺满面板
                uIPage.Show();
            }
        }

        /// <summary>
        /// 反射开启双缓冲，减少控件刷新闪烁（上位机DataGridView、动态绘制常用）
        /// </summary>
        public static void SetDoubleBuffered(Control control)
        {
            // DoubleBuffered 是Control的非公开属性，只能反射访问
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(control, true, null);
        }
    }
}
