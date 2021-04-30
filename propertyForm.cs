using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test4
{
    public partial class propertyForm : Form
    {
        public Button btn;
        

        public propertyForm()
        {
            InitializeComponent();
        }

        private void propertyForm_Load(object sender, EventArgs e)
        {
            TreeNode tr = (TreeNode)btn.Tag;
            Form1 a = new Form1();
            labelDate.Text = DateTime.Now.AddMinutes(-1).ToString(("yyyy-MM-dd-hh:mm"));
            labelName.Text = tr.Text;
            labelPath.Text = a.pathLabelRefresh1(tr);
            //labelSize.Text = System.Text.Encoding.Default.GetByteCount((String)tr.Tag) + "字节";
            labelSize.Text = "unknown";
            if (tr.Tag.GetType()==typeof(String))
            {
                String b = (String)tr.Tag;
                labelSize.Text = System.Text.Encoding.Default.GetByteCount(b)+"字节";
            }
            //定义labelType
            if (tr.Tag.GetType() == typeof(String))
                labelType.Text = "文件";
            else
                labelType.Text = "文件夹";
        }


        //设置读写
        //private void radioButton1_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButton1.Checked)
        //        if (btn.Text[0] == '*') btn.Text = btn.Text.Substring(1);
        //}


        //设置只读
        //private void radioButton2_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (radioButton2.Checked)
        //        if (!(btn.Text[0] == '*'))   btn.Text = btn.Text.Insert(0, "*");
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            rwCheck();
        }

        private void rwCheck()
        {
            TreeNode tr = (TreeNode)btn.Tag;
            if (radioButton1.Checked)
                if (tr.Text[0] == '*') tr.Text = tr.Text.Substring(1);
            if (radioButton2.Checked)
                if (!(tr.Text[0] == '*')) tr.Text = tr.Text.Insert(0, "*");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rwCheck();
            this.Close();
        }
    }
}
