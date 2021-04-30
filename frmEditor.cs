using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
  
namespace test4
{

    public partial class frmEditor : Form
    {
        
        public frmEditor()
        {
            InitializeComponent();
        }
        

        /// <summary>     
        /// 保存原始内容
        /// </summary>
        private String originalContent = "";
        private String _FileName = "";
        public string OriginalContent { get => originalContent; set => originalContent = value; }
        public string FileName { get => _FileName; set => _FileName = value; }
        public TreeNode refBtn;
        public ListView lv;
        public ListView lv2;
        public int[] fatt;
        public System.Windows.Forms.DataVisualization.Charting.Chart chart;


        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = DateTime.Now.ToString();
        }

        public void setReadOnly()
        {
            textBox1.ReadOnly = true;
        }

        private void frmEditor_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "";
            lblTimer.Text = "";
            Text = FileName+"-我的记事本";
            textBox1.Text = originalContent;
        }



        private void frmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Save();
            if (textBox1.Text != originalContent
                    && MessageBox.Show("文件已修改，保存吗？",
                    "保存文件", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                save();

            }
            for (int i = lv.Items.Count - 1; i >= 0; --i)
                if (lv.Items[i].Text == refBtn.Text)
                    lv.Items[i].Remove();
            
        }

        private void baocunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }


        private void chartRefresh()
        {
            int count = 0;
            for (int i = 0; i < 128; i++)
                if (lv2.Items[i].SubItems[1].Text.Equals("0")) count++;
            chart.Series["s1"].Points.ElementAt(0).SetValueY(count);
            chart.Series["s1"].Points.ElementAt(1).SetValueY(128 - count);
            chart.Refresh();
        }


        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void save()
        {
            int count = 0;
            for (int i = 0; i < 128; i++)
                if (lv2.Items[i].SubItems[1].Text.Equals("0")) count++;
            
            double size = System.Text.Encoding.Default.GetByteCount(textBox1.Text);
            double size1 = System.Text.Encoding.Default.GetByteCount(originalContent);
            double block = Math.Ceiling(size / 64);
            double block1 = Math.Ceiling((size-size1) / 64);
            if (count < block1)
            {
                MessageBox.Show("磁盘空间不足，请缩减文本或释放磁盘空间", "磁盘使用警告",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (block == 0) block++;

            refBtn.Tag = textBox1.Text;
            originalContent = textBox1.Text;
            Boolean isNull = true;
            int firstBlock = 127;




            for (int i = 2; i < 128; i++)
                if (fatt[i] == (refBtn.GetHashCode() - 1))
                {
                    isNull = false;
                    firstBlock = i;
                    block--;
                    break;
                }

            if (isNull)
            {
                int i;
                for (i = 2; i < 128; i++)
                    if (lv2.Items[i].SubItems[1].Text.Equals("0"))
                    {
                        fatt[i] = refBtn.GetHashCode() - 1;
                        lv2.Items[i].SubItems[1].Text="255";
                        firstBlock = i;
                        block--;
                        break;
                    }
                if (i == 128)
                    MessageBox.Show("磁盘空间不足，请缩减文本或释放磁盘空间", "磁盘使用警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }





            int next = Int32.Parse(lv2.Items[firstBlock].SubItems[1].Text);
            int currentBlock = firstBlock;
            while (block > 0||next!=255)
            {

                if (next == 255)
                {
                    for (int i = 2; i < 128; i++)
                    {
                        if (lv2.Items[i].SubItems[1].Text.Equals("0"))
                        {
                            lv2.Items[currentBlock].SubItems[1].Text = i.ToString();
                            currentBlock = i;
                            lv2.Items[currentBlock].SubItems[1].Text = "255";
                            fatt[i] = refBtn.GetHashCode();
                            block--;
                            break;
                            //next = i;
                            //fatt[i] = refBtn.GetHashCode();
                            //block--;
                        }
                    }
                }
                else
                {
                    int temp = Int32.Parse(lv2.Items[currentBlock].SubItems[1].Text);
                    if (block==0)
                    {
                        lv2.Items[currentBlock].SubItems[1].Text = "255";
                    }
                    if (block < 0)
                    {
                        lv2.Items[currentBlock].SubItems[1].Text = "0";
                        fatt[currentBlock] =0;
                    }
                    currentBlock = temp;
                    block--;
                }
                next = Int32.Parse(lv2.Items[currentBlock].SubItems[1].Text);
                if (next==255&&block<0)
                {
                    lv2.Items[currentBlock].SubItems[1].Text = "0";
                    fatt[currentBlock] = 0;
                }
            }

            chartRefresh();
            //for (int i = 2; i < 128; i++)
            //{

            //}
            ////int firstIndex=0;
            //int beginIndex = 0;
            //int lastIndex = 127;
            //for (int i = 2; i < 128; i++)
            //{
            //    if (fatt[i] == refBtn.GetHashCode())
            //    {
            //        beginIndex = i;
            //        break;
            //    }
            //}
            //if (beginIndex == 0)
            //{
            //    beginIndex = 2;
            //}


            //for (int i = beginIndex; i < 128; i++)
            //{
            //    if (block > 0)
            //    {
            //        if (lv2.Items[i].SubItems[1].Text.Equals("0"))
            //        {
            //            fatt[i] = refBtn.GetHashCode();
            //            block--;
            //            lv2.Items[lastIndex].SubItems[1].Text = i.ToString();
            //            lv2.Items[i].SubItems[1].Text = "255";
            //            lastIndex = i;
            //        }
            //        else
            //        {
            //            if (fatt[i].Equals(refBtn.GetHashCode()))
            //            {
            //                block--;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (fatt[i].Equals(refBtn.GetHashCode()))
            //            lv2.Items[i].SubItems[1].Text = "0";
            //    }
            //}


        }
    }
}
