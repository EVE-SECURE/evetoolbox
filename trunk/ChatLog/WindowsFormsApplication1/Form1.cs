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

            //string[] s = StarDict.DisStrWithEngOrOther("英语测试GY5有红");
            //if (s != null)
            //{
            //   for (int i = 0; i < s.Length; i++) Debug(s[i]);
            //}
            //else {
            //   Debug("OneHoleWorld");
            //}
            testbutton_Click(null, null);

        }
        public void Debug(string str)
        {
            //DebugOutput.Text = str + "\r\n" + DebugOutput.Text;
            AddTextToObj(DebugOutput, str);
        }
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
            oThread.Start();*/
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
        }

        public delegate void VoidFuncDelegate(object[] objs);
        public delegate void SeekFileDelegate(string str);
        
        private void SeekFile()
        {
            string fileName = GetDefaultFileName();
            FileStream stream = null;
            int returnStatus = 0;
            try
            {
                stream = new FileStream(fileName, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(stream);
                sr.ReadToEnd();
               
                do
                {
                    if (!sr.EndOfStream)
                    {
                        string bufstr = sr.ReadToEnd();
                        Debug(bufstr);
                    }
                    if (loop)
                    {
                        Thread.Sleep(1000);
                    }
                } while (true);
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                Debug(ex.ToString());
                Console.WriteLine("Encountered some error.");
                returnStatus = -1;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public string GetDefaultFileName()
        {
            string default_path = "C:\\Users\\hqfyll\\Documents\\EVE\\logs\\Chatlogs";

            DirectoryInfo dires = new DirectoryInfo(default_path);
            FileInfo[] files = dires.GetFiles();
            FileInfo result = null;
            foreach (FileInfo f in files)
            {
                //Debug(f.Name);
                //int nSearch = f.Name.IndexOf("联盟");
                int nSearch = f.Name.IndexOf("本地");
                if (nSearch >= 0)
                {
                    bool bChange = result == null || f.LastWriteTime > result.LastWriteTime;
                    if (bChange)
                    {
                        result = f;
                    }
                }
            }
            Debug(result.FullName);
            return result.FullName;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (oThread != null) 
            //{
            //   oThread.Abort();
                //oThread.Interrupt();
            //}
            ThreadMgr.TerminateAllThreads();
        }

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

        private void PickedResult_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
