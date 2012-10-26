using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{
    class FileOper
    {
        public string basepath = "C:\\Users\\hqfyll\\Documents\\EVE\\logs\\Chatlogs";
        public string filename = "本地";
        public bool AddType = true;
        public int SleepSpan = 1000;
        public bool SkipHistory = true;
        public int FileCheckTime = 30;
        public bool loop = true;

        public delegate void MessagePushbackDelegate(string str);
        public MessagePushbackDelegate MessageFunc = null;
        public void SeekFile() 
        {
            string LogFileName = GetDefaultFileName();
            FileStream stream = null;
            int returnStatus = 0;
            int CheckCnt = 0;
            try
            {
                stream = new FileStream(LogFileName, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(stream);
                if (SkipHistory) { sr.ReadToEnd(); }

                do
                {
                    CheckCnt++;
                    if (!sr.EndOfStream)
                    {
                        string bufstr = sr.ReadToEnd();
                        char[] split = { '\n' };
                        string[] lines = bufstr.Split(split);
                        for (int i = lines.Length - 1; i >= 0; i--)
                        {
                            // /BeginInvoke(MessageFunc, new object[] { bufstr });
                            if (lines[i].Length > 2)
                            {
                                if (this.AddType)
                                {

                                    lines[i] = "[" + filename + "]" + lines[i];
                                }

                                MessageFunc.Invoke(lines[i]);
                            }
                        }
                    }

                    if (CheckCnt > FileCheckTime)
                    {
                        string fn = GetDefaultFileName();
                        bool NeedReOpen = fn != LogFileName;
                        if (NeedReOpen)
                        {
                            LogFileName = fn;
                            sr.Close();
                            stream.Close();
                            stream = new FileStream(LogFileName, FileMode.Open,
                            FileAccess.Read, FileShare.ReadWrite);
                            sr = new StreamReader(stream);
                            if (SkipHistory) { sr.ReadToEnd(); }
                        }
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
                //Debug(ex.ToString());
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
            string path = basepath;

            DirectoryInfo dires = new DirectoryInfo(path);
            FileInfo[] files = dires.GetFiles();
            FileInfo result = null;
            foreach (FileInfo f in files)
            {
                int nSearch = f.Name.IndexOf(filename);
                if (nSearch >= 0)
                {
                    bool bChange = result == null || f.LastWriteTime > result.LastWriteTime;
                    if (bChange)
                    {
                        result = f;
                    }
                }
            }
            return result.FullName;
        }
    }
}
