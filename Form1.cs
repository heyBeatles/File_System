using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace test4
{
    public partial class Form1 : Form
    {
        int folderIndex = 1;
        int fileIndex = 1;
        //ArrayList fileList = new ArrayList();
        //Hashtable mainHash = new Hashtable();
        TreeNode pathForNow;
        Hashtable main = new Hashtable();
        //String[] fat = new String[128];
        int[] fat = new int[128];

        private void firstNode()
        {
            foreach (TreeNode nodes in treeView1.Nodes)
            {
                if (nodes.Name == "root")       //判断符合条件的节点 
                {
                    pathForNow = nodes;
                    pathForNow.Tag = main;
                }

            }
        }

        private void InitializeFatString()
        {
            for (int i = 0; i < fat.Length; i++)
            {
                fat[i] = 0;
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitializeListview();
            InitializeFatString();
            InitializeChart();
            treeView1.ExpandAll();
            firstNode();
            label4.Text = @"C:\";
        }


        private void InitializeChart()
        {
            chart1.Titles.Add("磁盘占用比");
            comboBox1.SelectedIndexChanged+= new System.EventHandler(ComboBox1_SelectedIndexChanged);
            chart1.Series["s1"].IsValueShownAsLabel = true;
            chart1.Series["s1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            chart1.Series["s1"].Points.AddXY("空闲", "126");
            chart1.Series["s1"].Points.AddXY("占用", "2");
            comboBox1.SelectedIndex = 0;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 1:chart1.Series["s1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;break;
                case 2: chart1.Series["s1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;break;
                case 3: chart1.Series["s1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar; break;
                case 4: chart1.Series["s1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Funnel; break;
                default:    break;
            }
        }



        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < 128; i++)
                if (listView1.Items[i].SubItems[1].Text.Equals("0")) count++;
            if (count==0)
            {
                MessageBox.Show("磁盘空间不足，请释放磁盘空间", "磁盘使用警告",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            fileGenerate();
            chartRefresh();
        }

        
        private void 新建文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < 128; i++)
                if (listView1.Items[i].SubItems[1].Text.Equals("0")) count++;
            if (count == 0)
            {
                MessageBox.Show("磁盘空间不足，请释放磁盘空间", "磁盘使用警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            folderGenerate();
            chartRefresh();
        }


        private void button1_Click_2(object sender, EventArgs e)
        {
            //listView1.Items.Add(new ListViewItem(new string[] { "John dsfsfsdfs ", "0"}));
            flowLayoutPanel1.Controls.Clear();
        }

        private void fileClicked(object sender, EventArgs e)
        {
            //TreeNode clickedBt = (TreeNode)sender;
            TreeNode folder;
            if (sender.GetType() == typeof(DoubleClickButton))
            {
                Button bt = (Button)sender;
                folder = (TreeNode)bt.Tag;
            }
            else
            {
                folder = (TreeNode)sender;
            }



            frmEditor a = new frmEditor();
            if (folder.Text[0] == '*') a.setReadOnly();
            a.chart = chart1;
            a.refBtn = folder;
            a.OriginalContent = (String)folder.Tag;
            a.FileName = folder.Text;
            a.lv = listView2;
            a.StartPosition = FormStartPosition.CenterScreen;
            a.Show();
            a.fatt = fat;
            a.lv2 = listView1;

            String beginBlock="?";
            for (int i = 2; i < 128; i++)
                if (fat[i]==(folder.GetHashCode()-1))
                {
                    beginBlock = i.ToString();
                    break;
                }
            listView2.Items.Add(new ListViewItem(new string[] { folder.Text,"读写",beginBlock,pathLabelRefresh1(pathForNow) }));
            //listView2.Items.RemoveByKey(folder.Text);
        }

        private void folderClicked(object sender, EventArgs e)
        {
            //TreeNode folderClicked = (TreeNode)sender;
            TreeNode folder;
            if (sender.GetType()==typeof(DoubleClickButton))
            {
                Button bt = (Button)sender;
                folder = (TreeNode)bt.Tag;
            }
            else
            {
                folder = (TreeNode)sender;
            }
            //TreeNode folderTag = (TreeNode)folderClicked.Tag;
            pathForNow = folder;
            Hashtable folderContains = (Hashtable)folder.Tag;
            folderContainsDisplay(folderContains);
            pathLabelRefresh();
        }

        //当前路径显示更新
        private void pathLabelRefresh()
        {
            String path = pathForNow.Text;
            TreeNode path1 = pathForNow;
            while(path1.Parent != null)
            {
                path1 = path1.Parent;
                path = path.Insert(0, path1.Text + @"\");
            }
            label4.Text = path;
        }

        //带参数的路径更新
        public String pathLabelRefresh1(TreeNode tr)
        {
            String path = tr.Text;
            TreeNode path1 = tr;
            while (path1.Parent != null)
            {
                path1 = path1.Parent;
                path = path.Insert(0, path1.Text + @"\");
            }
            return path;
        }


        private void folderContainsDisplay(Hashtable toTraverse)
        {
            flowLayoutPanel1.Controls.Clear();
            foreach (object btn in toTraverse.Values)
            {
                Button bt = (Button)btn;
                this.flowLayoutPanel1.Controls.Add(bt);
            }
        }


        //生成文件夹方法
        private void folderGenerate()
        {
            treeView1.LabelEdit = false;//不可编辑
            DoubleClickButton button1 = new DoubleClickButton();
            button1.Name = "folder" + folderIndex;
            button1.Text = "新建文件夹" + folderIndex;
            folderIndex++;
            button1.ImageList = this.imageList1;
            button1.ImageKey = "64x64.png";
            //button1.Image = global::test4.Properties.Resources.folder;
            button1.Location = new System.Drawing.Point(3, 3);
            //button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 77);
            //button1.Size = new System.Drawing.Size(78, 77);
            button1.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //button1.Size = new System.Drawing.Size(59, 68);
            //button1.Size = new System.Drawing.Size(103, 120);
            button1.TabIndex = 0;
            //button1.Text = "新建文件夹";
            button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            //button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            button1.UseVisualStyleBackColor = true;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            button1.FlatStyle = FlatStyle.Flat;//样式  
            button1.BackColor = Color.Transparent;//去背景  
            button1.FlatAppearance.BorderSize = 0;//去边线
            button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            button1.ForeColor = System.Drawing.Color.Black;
            button1.Padding = new System.Windows.Forms.Padding(0);
            button1.ContextMenuStrip = this.文件右键菜单;
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode(button1.Text);
            treeNode2.ImageIndex = 0;
            treeNode2.SelectedImageIndex = 0;
            //treeNode2.ContextMenuStrip = this.文件右键菜单;
            //treeNode2.Tag = button1;
            button1.Tag = treeNode2;
            //button1.Tag = treeNode2;
            Hashtable folderContains = new Hashtable();
            treeNode2.Tag = folderContains;




            //this.button5.ImageKey = "64x64.png";
            //this.button5.ImageList = this.imageList1;
            //this.button5.Location = new System.Drawing.Point(3, 3);
            //this.button5.Name = "button5";
            //this.button5.Size = new System.Drawing.Size(59, 68);
            //this.button5.TabIndex = 0;
            //this.button5.Text = "button5";
            //this.button5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            //this.button5.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            //this.button5.UseVisualStyleBackColor = true;




            //DoubleClickButton button2 = new DoubleClickButton();
            //button2.Name = "folder11" ;
            //button2.Text = "新建文件夹11";
            //DoubleClickButton button3 = new DoubleClickButton();
            //button3.Name = "folder111";
            //button3.Text = "新建文件夹111";
            //folderContains.Add(1, button2);
            //folderContains.Add(2, button3);




            //button1.Tag = folderContains;
            //foreach (TreeNode nodes in treeView1.Nodes)
            //{
            //    if (nodes.Name == "root")       //判断符合条件的节点 
            //    {
            //        nodes.Nodes.Add(treeNode2);
            //    }

            //}
            pathForNow.Nodes.Add(treeNode2);


            button1.DoubleClick += new System.EventHandler(folderClicked);
            //fileList.Add(button1);
            //mainHash.Add(button1.Name, button1);
            Hashtable folderForNow = (Hashtable)pathForNow.Tag;
            folderForNow.Add(button1.Name, button1);
            this.flowLayoutPanel1.Controls.Add(button1);



            for (int i = 2; i < 128; i++)
                if (listView1.Items[i].SubItems[1].Text.Equals("0"))
                {
                    fat[i] = treeNode2.GetHashCode();
                    listView1.Items[i].SubItems[1].Text = "255";
                    break;
                }
            //this.treeView1.treeNode1.Nodes.add
            treeView1.ExpandAll();
        }


        //生成文件方法
        private void fileGenerate()
        {
            
            DoubleClickButton button1 = new DoubleClickButton();
            button1.ImageList = this.imageList1;
            button1.ImageIndex = 1;
            button1.Size = new System.Drawing.Size(88, 77);
            button1.DoubleClick += new System.EventHandler(fileClicked);
            button1.Location = new System.Drawing.Point(3, 3);           
            button1.TabIndex = 0;
            String btText = "";
            //button1.Tag = btText;
            button1.Name = "file" + fileIndex;
            button1.Text = "新建文件" + fileIndex;
            fileIndex++;
            button1.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            button1.Margin = new System.Windows.Forms.Padding(7);
            button1.UseVisualStyleBackColor = true;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            button1.FlatStyle = FlatStyle.Flat;//样式  
            button1.ForeColor = Color.Transparent;//前景  
            button1.BackColor = Color.Transparent;//去背景  
            button1.FlatAppearance.BorderSize = 0;//去边线
            button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            //button1.ForeColor = System.Drawing.Color.Transparent;
            button1.ForeColor = System.Drawing.Color.Black;
            button1.Padding = new System.Windows.Forms.Padding(0);
            button1.ContextMenuStrip = this.文件右键菜单;
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode(button1.Text);
            //treeNode2.ContextMenuStrip = this.文件右键菜单;
            //treeNode2.Tag = button1;
            button1.Tag = treeNode2;
            treeNode2.Tag = btText;
            treeNode2.ImageIndex = 2;
            treeNode2.SelectedImageIndex = 2;
            //foreach (TreeNode nodes in treeView1.Nodes)
            //{
            //    if (nodes.Name == "root")       //判断符合条件的节点 

            //    {
            //        nodes.Nodes.Add(treeNode2);
            //    }


            //}
            pathForNow.Nodes.Add(treeNode2);



            //this.treeView1.Nodes.Add(treeNode2);
            Hashtable folderForNow = (Hashtable)pathForNow.Tag;
            folderForNow.Add(button1.Name, button1);
            this.flowLayoutPanel1.Controls.Add(button1);


            for (int i = 2; i < 128; i++)
                    if (listView1.Items[i].SubItems[1].Text.Equals("0"))
                    {
                        fat[i] = treeNode2.GetHashCode()-1;
                        listView1.Items[i].SubItems[1].Text = "255";
                        break; 
                    }
            treeView1.ExpandAll();
        }

        //初始化FAT表
        private void InitializeListview()
        {
            listView1.Items.Add(new ListViewItem(new string[] { "0", "255" }));
            listView1.Items.Add(new ListViewItem(new string[] { "1", "255"}));

            for (int i = 2; i < 128; i++)
            {
                listView1.Items.Add(new ListViewItem(new string[] {i.ToString(), "0" }));
            }
        }


        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("测试listview双击事件相应");
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            TreeNode clickedNode = treeView1.SelectedNode;
            //Button contains = (Button)clickedNode.Tag;
            //MessageBox.Show(clickedNode.Parent.Text);
                if (clickedNode.Tag.GetType() == typeof(String))
                {
                    fileClicked(clickedNode, e);
                }
                else
                {
                    pathForNow = clickedNode;
                    folderClicked(clickedNode, e);
                }
            
            //else
            //{
            //    folderContainsDisplay(mainHash);
            //}
            //fileClicked(clickedNode.Tag, e);
        }

        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Button sourceControl = owner.SourceControl as Button;
                    String a = Interaction.InputBox("请输入文件名", "文件名修改", sourceControl.Text, 100, 100);

                    Boolean isRepeated = false;
                    Hashtable hs = (Hashtable)pathForNow.Tag;
                    foreach (object btn in hs.Values)
                    {
                        Button bt = (Button)btn;
                        if (bt.Text.Equals(a)&&(!(a.Equals(sourceControl.Text))))
                        {
                            isRepeated = true;
                            MessageBox.Show("文件名重复!");
                        }
                    }



                    if ((!(a.Equals("")))&&(!isRepeated))
                    {
                        sourceControl.Text = a;
                    }
                    TreeNode node = (TreeNode)sourceControl.Tag;
                    node.Text = sourceControl.Text;
                }
            }
            pathLabelRefresh();
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Button sourceControl = owner.SourceControl as Button;
                    TreeNode node = (TreeNode)sourceControl.Tag;
                    if (node.Tag.GetType() == typeof(String))
                    {
                        fileClicked(sourceControl, e);
                    }
                    else
                    {
                        folderClicked(sourceControl, e);
                    }
                }
            }
            
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Button sourceControl = owner.SourceControl as Button;
                    TreeNode node = (TreeNode)sourceControl.Tag;
                    object oj = node.Tag;
                    if (oj.GetType()==typeof(Hashtable)&&((Hashtable)oj).Count>0)
                    {
                        MessageBox.Show("文件夹不为空，不能删除");
                    }
                    else
                    {
                        this.flowLayoutPanel1.Controls.RemoveByKey(sourceControl.Name);
                        Hashtable ht = (Hashtable)pathForNow.Tag;
                        ht.Remove(sourceControl.Name);
                        node.Remove();
                        for (int i = 2; i < 128; i++)
                            if (fat[i].Equals(node.GetHashCode())|| fat[i].Equals(node.GetHashCode()-1))
                            {
                                listView1.Items[i].SubItems[1].Text = "0";
                                fat[i] = 0;
                            }
                        sourceControl.Dispose();
                    }
                }
            }
            chartRefresh();
        }


        private void chartRefresh()
        {
            int count = 0;
            for (int i = 0; i < 128; i++)
                if (listView1.Items[i].SubItems[1].Text.Equals("0")) count++;
            chart1.Series["s1"].Points.ElementAt(0).SetValueY(count);
            chart1.Series["s1"].Points.ElementAt(1).SetValueY(128 - count);
            chart1.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //propertyForm a = new propertyForm();
            //a.Show();
            //MessageBox.Show(DateTime.Now.ToLongDateString().ToString());
            //MessageBox.Show(DateTime.Now.ToString());
            //DateTime dt = DateTime.Now;
            //dt.AddMinutes(-10);
            //MessageBox.Show(dt.ToString(("yyyy-MM-dd-hh:mm")));
            //MessageBox.Show(dt.AddMinutes(-1).ToString(("yyyy-MM-dd-hh:mm")));
            //String a = "aaaaasdqqqqdc";
            //int i = System.Text.Encoding.Default.GetByteCount(a);
            //MessageBox.Show(i.ToString());
            //for (int i = listView2.Items.Count - 1; i >= 0; --i)
            //    if (listView2.Items[i].Text == "新建文件2")
            //        listView2.Items[i].Remove();
            //listView1.Items[2].SubItems[1].Text = "测试";
            //MessageBox.Show(listView1.Items[2].SubItems[1].Text);


            //int a = 300;
            //double b = Math.Ceiling((double)(a / 255));
            //MessageBox.Show(b.ToString());

            //MessageBox.Show(listView1.Items[2].SubItems[1].Text);


                if (pathForNow.Parent!=null)
                {
                    pathForNow = pathForNow.Parent;
                }
                Hashtable folderContains = (Hashtable)pathForNow.Tag;
                folderContainsDisplay(folderContains);
                pathLabelRefresh();
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Button sourceControl = owner.SourceControl as Button;
                    propertyForm pf = new propertyForm();
                    pf.btn = sourceControl;
                    pf.StartPosition = FormStartPosition.CenterScreen;
                    pf.ShowDialog();
                }
            }
        }




        private void button5_Click(object sender, EventArgs e)
        {
            //SeriesChartType
            //chart1.Titles.Add("饼图测试");
            //chart1.Series["s1"].Points.AddXY("1", "20");
            //chart1.Series["s1"].Points.AddXY("21", "20");
            chart1.Series["s1"].Points.ElementAt(0).SetValueY(40);
            chart1.Refresh();
            //this.chart1.Series.Clear();
        }

        private void dasdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            helpForm hf = new helpForm();
            hf.StartPosition = FormStartPosition.CenterScreen;
            hf.ShowDialog();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm ab = new aboutForm();
            ab.StartPosition = FormStartPosition.CenterScreen;
            ab.ShowDialog();
        }
    }

        
}
