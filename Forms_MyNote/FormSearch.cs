using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms_MyNote
{
    public delegate void MyDelegate(string text);   //定义委托实现传值
    public delegate void MyDelegateInt(int text);

    public partial class FormSearch : Form
    {
        
        public FormSearch()
        {
            InitializeComponent();
        }
        public event MyDelegate MyEvent;    //事件
        public event MyDelegateInt MyEventInt;
        private void button1_Click(object sender, EventArgs e)
        {               //button1就是确认按钮
            MyEventInt(1);
            MyEvent(this.searchText.Text);
            OnLostFocus(e);

            //this.Hide();
            //MyEventInt(0);
        }
        
        private void searchText_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MyEventInt(3);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            MyEventInt(2);
        }
    }
}
