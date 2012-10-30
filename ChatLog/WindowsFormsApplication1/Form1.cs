using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private static Encoding encoding = Encoding.BigEndianUnicode;
        private static bool loop = true;
        Thread oThread = null;
        ThreadManager ThreadMgr = new ThreadManager();
        public StarSearch StarDict;
        Mutex RichLineReadMutex = null;
        public string MyDocPath = "";
        public AITalker talker = null;
        private int TalkerTimerSkip = 0;

        AiTalkerForm soundform = new AiTalkerForm();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls   =   false;
            ThreadMgr.Parent = this;
            StarDict = new StarSearch();
            StarDict.Load();
            bool createdNew = false;
            RichLineReadMutex = new Mutex(false, "RichReadLine", out createdNew);
            MyDocPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\EVE\\logs\\Chatlogs";
            talker = new AITalker();

            //soundform.aitalker = talker;
            
            StartWatchLog();
            SoundTimer.Enabled = true;
            int tid = Thread.CurrentThread.ManagedThreadId;
            Debug("Main tid is : "+ tid.ToString());
            this.Text = "FDK 报警器 by 无脑战士";
            this.Width = 451;
            this.Height = 219;
            PickedResult.Text = "长安内部试用版\r\n斐德克地区有效";
            //PickedResult.Text = "";
            PickedResult.Enabled = false;

        }

        public void Debug(string str)
        {
            //DebugOutput.Text = str + "\r\n" + DebugOutput.Text;
            AddTextToObj(DebugOutput, str);
        }

        /// <summary>
        /// 监视器的回到函数
        /// </summary>
        /// <param name="str"></param>
        public void RichReadLine(string str) 
        {
            Debug(str);

            RichLineReadMutex.WaitOne();
            StarSearch.StarSystem[] StarSet = StarDict.SearchStarSystem(str);
            if (StarSet != null && StarSet.Length > 0)
            {
                string outline = "";
                foreach (StarSearch.StarSystem ss in StarSet)
                {
                    if (outline.Length > 0) { outline = outline + ","; }
                    outline = outline + ss.FullName;
                    talker.AddStarSystemAlart(ss.FullName);
                }
                outline = "发现 (" + outline + ") @" + str;
                AddTextToObj(PickedResult, outline);
            }
            else 
            {
                string[] starnames = StarDict.SearchUnkonwSystem(str);
                if (starnames != null && starnames.Length > 0) 
                {
                    string outline = "";
                    foreach (string ss in starnames)
                    {
                        if (outline.Length > 0) { outline = outline + ","; }
                        outline = outline + ss;
                        talker.AddStarSystemAlart(ss);
                    }
                    outline = "发现未收录星系 (" + outline + ") @" + str;
                    AddTextToObj(PickedResult, outline);
                    
                }

            }
            RichLineReadMutex.ReleaseMutex();
        }

        private void testbutton_Click(object sender, EventArgs e)
        {
            /*if (oThread == null)
            {
                oThread = new Thread(new ThreadStart(this.SeekFile));
            }
            */
          
          
        }

        /// <summary>
        /// 启动所有的频道监视
        /// </summary>
        public void StartWatchLog()
        {
            FileOper fo1 = new FileOper();
            fo1.filename = "联盟";
            FileOper.MessagePushbackDelegate mpd = new FileOper.MessagePushbackDelegate(RichReadLine);
            fo1.MessageFunc = mpd;
            Thread th1 = new Thread(new ThreadStart(fo1.SeekFile));
            th1.Name = "th1";
            fo1.basepath = MyDocPath;
            ThreadMgr.RunBackground(th1);

            FileOper fo2 = new FileOper();
            fo2.filename = "军团";
            FileOper.MessagePushbackDelegate mpd2 = new FileOper.MessagePushbackDelegate(RichReadLine);
            fo2.MessageFunc = mpd2;
            Thread th2 = new Thread(new ThreadStart(fo2.SeekFile));
            th2.Name = "th2";
            fo2.basepath = MyDocPath;
            ThreadMgr.RunBackground(th2);

            FileOper fo3 = new FileOper();
            fo3.filename = "本地";
            //fo3.filename = "程序员们的";
            FileOper.MessagePushbackDelegate mpd3 = new FileOper.MessagePushbackDelegate(RichReadLine);
            fo3.MessageFunc = mpd3;
            Thread th3 = new Thread(new ThreadStart(fo3.SeekFile));
            th3.Name = "th3";
            fo3.basepath = MyDocPath;
            ThreadMgr.RunBackground(th3);


            FileOper fo4 = new FileOper();
            
            fo4.filename = "程序员们的";
            FileOper.MessagePushbackDelegate mpd4 = new FileOper.MessagePushbackDelegate(RichReadLine);
            fo4.MessageFunc = mpd4;
            Thread th4 = new Thread(new ThreadStart(fo4.SeekFile));
            th4.Name = "th4";
            fo4.basepath = MyDocPath;
            ThreadMgr.RunBackground(th4);

       
        }

        /// <summary>
        /// 窗体关闭的时候删除所有线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ThreadMgr.TerminateAllThreads();
            talker.CloseAll();
        }

        /// <summary>
        /// ui代理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="str"></param>
        public delegate void AddTextToObjInvoke(object obj, string str);
        public void AddTextToObj(object obj, string str) 
        {
            AddTextToObjInvoke func = new AddTextToObjInvoke(AddTextToObjInvokeFunc);
            BeginInvoke(func,new object[]{obj,str});
        }
        public void AddTextToObjInvokeFunc(object obj, string str) 
        {
            if (obj is TextBox) {
                TextBox tb = (TextBox)obj;
                if (str.EndsWith("\n"))
                {
                    tb.Text = str + tb.Text;
                }
                else { 
                    tb.Text = str + "\r\n" + tb.Text;
                }
            }
        }

        //private void PickedResult_TextChanged(object sender, EventArgs e)
        //{

        //}


        private void SoundTimer_Tick(object sender, EventArgs e)
        {
            if (TalkerTimerSkip > 0) 
            {
                TalkerTimerSkip--;
                return;
            }
            int nWaitTime = talker.TalkOneStep();
            if (nWaitTime >= 1400) {
                TalkerTimerSkip = 2;
            }
        }
    }
}
