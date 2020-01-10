using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Threading;
using System.Drawing.Printing;

namespace Forms_MyNote
{
    /// <summary>
    /// function:   1.打开txt文档，保存txt文档，导出pdf文档
    /// </summary>
    public partial class form : Form
    {
        private static int col = 0, row = 0;     //状态栏中行和列
        private static bool condition = false;  //状态栏和自动换行切换
        private static float fontSize;     //字体大小 快捷键ctrl +/-
        private static float fontSizeInit;

        public form()
        {
            InitializeComponent();
            handleRowCol();         //初始化状态栏，状态栏默认开启
            fontSizeInit = TextBox.Font.Size;   //字体初始化大小
            fontSize = TextBox.Font.Size;
            //把工具箱中printDocument的事件处理函数，添加到打印页面去
            this.textToPrint.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
        }

        /// <summary>
        /// 新建        逻辑类
        /// </summary>
        public void handleCreate()
        {
            needToSave();
            createOptions();
        }

        /// <summary>
        /// 当前文档有改动时，判断是否保存操作
        /// </summary>
        public void needToSave()
        {
            if (statusRead.Text == "read-in")
            {
                DialogResult dialogResult = MessageBox.Show("内容已改变，是否先保存?",
                    "Note express", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    // MessageBox.Show("you right");
                    handleSave();
                    //createOptions();  //保存了不要再创建新文件的
                }
                else if(dialogResult == DialogResult.No)
                {
                    createOptions();
                }
                else
                {
                    //取消后应没有变化
                }
            }
            else
            { 
                //如果是不需要保存文件，即文本没有改动，也应没有变化
                /*FileStream fileStream = new FileStream(Filename.Text,
                    FileMode.OpenOrCreate, FileAccess.Write);*/
            }
            /*int index = Filename.Text.IndexOf(".");
            if(Filename.Text.Substring(index + 1) == "txt")
            {
                handleSave();
            }*/
        }

        /// <summary>
        /// 新建文件后清空文本页面
        /// </summary>
        public void createOptions()
        {
            TextBox.Text = "";
            Filename.Text = "NewFile";
            statusRead.Text = "unchanged";
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// 
        public void handleSave()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "请选择要保存的文件路径";
            saveFileDialog.InitialDirectory = @"C:\Users\MTSW\Desktop\vs";
            saveFileDialog.Filter = "doc文档|*.txt";
            //如果你点了保存，那就要生成一个文件
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                //定义一个范围，在范围结束时处理对象，其功能和try,catch,Finally完全相同
                using (FileStream fileStream = new FileStream(fileName, 
                    FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = Encoding.Default.GetBytes(TextBox.Text.Trim());
                    fileStream.Write(buffer, 0, buffer.Length);
                }
            }
            //保存后修改状态栏文件名与文件状态
            int index = saveFileDialog.FileName.IndexOf(@"\vs");
            string TruncateFilename = saveFileDialog.FileName.Substring(index + 1);
            //string TruncateFilename = Path.GetFileName(saveFileDialog.FileName);
            Filename.Text = TruncateFilename;
            statusRead.Text = "unchanged";
            handleRowCol();
        }

        /// <summary>
        /// 得到标准行列和文本进度
        /// </summary>
        /// 
        public void handleRowCol()
        {
            int index = TextBox.GetFirstCharIndexOfCurrentLine();   //找到当前行第一个字符
            row = TextBox.GetLineFromCharIndex(index) + 1;  //对当前字符检索行号
            col = TextBox.SelectionStart - index + 1;       //当前字符的文件位置 - 当前行第一个字符位置
            statusCol.Text = "Col " + col;
            statusLn.Text = "Row " + row;
            if(TextBox.TextLength == 0)
            {
                statusRate.Text = "100%";
            }
            else
            {
                statusRate.Text = ((int)((double)(index + col - 1) 
                    / (double)TextBox.TextLength * 100)).ToString()
                    + " %";
            }
        }

        /// <summary>
        /// 自定义对话框实现打印功能
        /// </summary>
        private StreamReader StreamToPrint1;    //文件流
        private PrintDocument textToPrint = new PrintDocument();//创建一个PrintDocument的实例
        
        public void handlePrint(StreamReader StreamToPrint)
        {
            /*NoteCopy noteCopy = new NoteCopy();
            noteCopy.Show();*/
            this.StreamToPrint1 = StreamToPrint;
            PrintDialog printDialog = new PrintDialog();//创建一个PrintDialog的实例
            printDialog.AllowSomePages = true;
            printDialog.ShowHelp = true;
            //this.StreamToPrint = StreamToPrint;
            //把PrintDialog的Document属性设为上面配置好的PrintDocument的实例
            printDialog.Document = textToPrint;
            //调用PrintDialog的ShowDialog函数显示打印对话框
            DialogResult result = printDialog.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                textToPrint.Print();
            }
        }
        
        public void handlePdf(StreamReader StreamToPrint)
        {
            this.StreamToPrint1 = StreamToPrint;
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintToFile = true;
            printDialog.Document = textToPrint;
            textToPrint.Print();
            
        }

        /// <summary>
        /// 另存为，再保存一次
        /// </summary>
        public void handleSaveAs()
        {
            handleSave();
        }

        /// <summary>
        /// 打开一个已存在的文件，打开文件对话框
        /// </summary>
        public void handleOpen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件|*.txt";
            openFileDialog.InitialDirectory = @"C:\Users\MTSW\Desktop\vs";
            //你选择一个文件后，确定打开
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
            int index = openFileDialog.FileName.IndexOf(@"\vs");
            string TruncateFilename = openFileDialog.FileName.Substring(index + 1);
            Filename.Text = TruncateFilename;
            statusRead.Text = "unchanged";
            handleRowCol();
        }

        //F5快捷输出时间
        public void handleTime()
        {
            TextBox.Text += statusTime.Text;
            TextBox.SelectionStart = col + statusTime.Text.Length;  //光标出现在时间后面
        }

        //剪切
        public void handleCut()
        {           //选中拖动内容
            TextBox.Cut();
        }
        //复制
        public void handleCopy()
        {
            TextBox.Copy();
        }
        //粘贴
        public void handlePaste()
        {
            TextBox.Paste();
        }
        //撤销
        public void handleRevoke()
        {       //剪切板中不能撤销时间的快捷键操作
            TextBox.Redo();
        }

        /// <summary>
        /// 自定义对话框完成查找功能，非模态调用
        /// </summary>
        //private FormSearch formSearch;
        private static string searchText;
        private static int searchTextInt;
        private FormSearch formSearch = new FormSearch();
        public void handleSearch(EventArgs e)
        {
            //在类中继续实例一个委托类型的事件
            formSearch.MyEvent += new MyDelegate(toSearchToText);
            formSearch.MyEventInt += new MyDelegateInt(messageInt);
            formSearch.Show();
            formSearch.StartPosition = FormStartPosition.CenterParent;
            //formSearch.Modal = true;
            //formSearch.Visible = false;
            if (searchTextInt == 1)
            {
                MessageBox.Show(searchTextInt + " right");
                //MessageBox.Show(searchText + " right");
                findWords(searchText);  //查找字符串 
                OnLostFocus(e);
                formSearch.Activate();
                //formSearch.Close();
            }
            //TextBox.SelectionColor = Color.Black;
        }

        public void messageInt(int message)
        {
            //MessageBox.Show(message.ToString());
            searchTextInt = message;
        }
        public void toSearchToText(string message)
        {
            //MessageBox.Show(message);
            searchText = message;
        }

        private static int ver; //定义游标（当前光标位置）

        //查找字符串
        public void findWords(string searchText)
        {
            //在文本中从ver开始匹配查找searchText，返回值是找到的第一个索引
            int start = TextBox.Text.IndexOf(searchText, ver);
            if(start == -1)
            {
                ver = 0;
                MessageBox.Show("找不到 " + searchText, "MyNote Note:");
            }
            else
            {
                /*List<string> findList = new List<string>();
                IEnumerable<string> query = TextBox.Text.Select();*/
                //从字符串中把子字符串截取出来
                //var query = TextBox.Text.Substring(start, searchText.Length);
                TextBox.SelectionStart = start;
                TextBox.SelectionLength = searchText.Length;
                    //selection实现选中功能
                //TextBox.SelectionColor = Color.Red;
                ver = start + 1;
            }
            searchTextInt = 0;
            //从最后往前面开始找
            //int last = TextBox.Text.LastIndexOf(searchText, var);
        }

        //字体对话框
        public void handleFont()
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowDialog();
            TextBox.SelectionFont = fontDialog.Font;
        }

        //颜色对话框
        public void handleColor()
        {
            ColorDialog fontDialog = new ColorDialog();
            //fontDialog.ShowColor = true;    //字体对话框显示颜色
            fontDialog.ShowDialog();
            TextBox.SelectionColor = fontDialog.Color;
        }

        public void handleFontAdd()
        {
            fontSize++;
            changeFontSize();
            //FontDialog fontDialog = new FontDialog();
        }
        public void handleFontSub()
        {
            fontSize--;
            changeFontSize();
        }

        public void handleFontSet()
        {
            fontSize = fontSizeInit;
            changeFontSize();
            //TextBox.Font. = (uint)fontSizeInit;
        }

        public void changeFontSize()
        {
            Font font = TextBox.Font;
            FontStyle fontStyle = new FontStyle();
            TextBox.SelectionFont = new Font(font.Name,
                fontSize, fontStyle);
        }

        public void handleAbout()
        {
            About about = new About();
            about.Show();
        }

        /// <summary>
        /// 实现快捷键完成操作
        ///     richTextBox 自身支持选择内容及复制剪切粘贴撤销快捷键
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.N && e.Modifiers == Keys.Control)
            {
                handleCreate();
            }
            if (e.KeyCode == Keys.P && e.Modifiers == Keys.Control)
            {
                handlePrint(fileStream());
            }
            if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                if(statusRead.Text == "unchanged")
                {       //如果文件没改动，保存不会有操作

                }
                else
                {
                    statusRead.Text = "unchanged";
                    string fileName = Filename.Text;
                    //定义一个范围，在范围结束时处理对象，其功能和try,catch,Finally完全相同
                    using (FileStream fileStream = new FileStream(fileName,
                        FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buffer = Encoding.Default.GetBytes(TextBox.Text.Trim());
                        fileStream.Write(buffer, 0, buffer.Length);
                    }
                }
                if(Filename.Text == "NewFile")
                {
                    handleSave();
                }
                
            }
            if (e.KeyCode == Keys.S && (int)e.Modifiers == ((int)Keys.Control + (int)Keys.Shift))
            {                               //同一类型的快捷键需要强转int进行加操作
                handleSaveAs();
            }
            if (e.KeyCode == Keys.O && e.Modifiers == Keys.Control)
            {
                handleOpen();
            }
            if (e.KeyCode == Keys.F6)   //快速导出pdf，快捷键f6
            {
                handlePdf(fileStream());
            }
            if (e.KeyCode == Keys.F5)
            {
                handleTime();
            }
            if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
            {
                //handleCut();
            }
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                //handleCopy();
            }
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                //handlePaste();
            }
            if (e.KeyCode == Keys.H && e.Modifiers == Keys.Control)
            {
                handleSearch(e);
            }
            if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                //handleRevoke();
            }
            if (e.KeyCode == Keys.F && e.Modifiers == Keys.Alt)
            {       //对文本选中后通过shift快捷键修改字体将键位冲突
                handleFont();
            }
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Alt)
            {
                handleColor();
            }
            if (e.KeyCode == Keys.Add && e.Modifiers == Keys.Control)
            {
                handleFontAdd();
            }
            if (e.KeyCode == Keys.Subtract && e.Modifiers == Keys.Control)
            {
                handleFontSub();
            }
        }

        //打印参数中需要的文件流
        private StreamReader fileStream()
        {
            needToSave();   //判断是否需要保存文件
            string fileName = @"C:\Users\MTSW\Desktop\" + this.Filename;
            StreamReader streamReader = new StreamReader(fileName, Encoding.Default);
            return streamReader;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            statusRead.Text = "read-in";
        }

        /// <summary>
        /// 键盘按任意键时事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.Enter)
            {
                //MessageBox.Show("you right1");
            }*/
            handleRowCol();
        }
        // 当鼠标移动时事件
        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            handleRowCol();
        }

        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleCreate();
        }
        private void 打印ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handlePrint(fileStream());
        }
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleOpen();
        }
        private void 导出PDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handlePdf(fileStream());
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleSaveAs();
        }

        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleSearch(e);
        }

        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleRevoke();
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleFont();
        }

        private void 状态栏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            状态栏ToolStripMenuItem.Checked = condition;
            if (状态栏ToolStripMenuItem.Checked == false)
            {
                statusStrip1.Visible = false;
                condition = true;
            }
            else
            {
                statusStrip1.Visible = true;
                condition = false;
            }
        }
        
        private void 自动换行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            自动换行ToolStripMenuItem.Checked = condition;
            if (自动换行ToolStripMenuItem.Checked == false)
            {
                TextBox.WordWrap = false;
                condition = true;
            }
            else
            {
                TextBox.WordWrap = true;
                condition = false;
            }
        }

        private void 默认大小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleFontSet();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleAbout();
        }

        private void form_Load(object sender, EventArgs e)
        {
            timer1.Start();     //窗体加载时开启定时器
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            statusTime.Text = System.DateTime.Now.ToString();
        }

        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleColor();
        }
        
        //打印控件
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            string text = StreamToPrint1.ReadToEnd();
            MessageBox.Show(text);
            FontStyle fontStyle = new FontStyle();
            Font font = new Font(TextBox.Font, fontStyle);
            e.Graphics.DrawString(text, font, Brushes.Red,
                e.MarginBounds.X, e.MarginBounds.Y);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            handleSave();
        }
    }
}
