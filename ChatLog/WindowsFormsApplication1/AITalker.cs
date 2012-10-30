using System.Runtime.InteropServices;
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
    public class AITalker
    {
        

        /// <summary>
        /// 消息锁
        /// </summary>
        Mutex MsgdMutex = null;

        private System.Collections.Generic.Queue<string> msgqueue = new Queue<string>();

        /// <summary>
        /// 增加需要朗读的队列
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool AddMessageToRead(string msg) 
        {
            MsgdMutex.WaitOne();
            msgqueue.Enqueue(msg);
            MsgdMutex.ReleaseMutex();
            return true;
        }

        /// <summary>
        /// 需要朗读的消息
        /// </summary>
        /// <param name="starname"></param>
        public void AddStarSystemAlart(string starname) 
        {
            char[] namel = starname.ToLower().ToCharArray();
            AddMessageToRead(alart_name);
            for (int i = 0; i < namel.Length; i++) 
            {
                if (namel[i] == '-') 
                {
                    AddMessageToRead("gang");
                }
                else
                {
                    AddMessageToRead(namel[i].ToString());
                }
            }
            AddMessageToRead("emeny");
        }

        public int TalkOneStep()
        {
            MsgdMutex.WaitOne();
            string name = null;
            if (msgqueue.Count > 0) {
                name = msgqueue.Dequeue();
            } 
            MsgdMutex.ReleaseMutex();
            
            return PlayWithName(name);
        }


        /// <summary>
        /// 简易执行命令
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [STAThread]
        public string mciSendStringEasy(string str) 
        {
            string msg = "";
            const int msg_length = 2560;
            msg.PadLeft(msg_length);

            int nErrCode = mciSendString(str, msg, msg_length - 1, 0);
            if (nErrCode!=0) {
                mciGetErrorString(nErrCode, msg, msg_length - 1);
            }
            return msg;
        }

        public string namepro = "chat_log_sound_";

        /// <summary>
        /// 调用者可见名字到内部名字准换
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetInnerName(string name) {
            return namepro + name;
        }

        /// <summary>
        /// 打开文件并赋予名字
        /// </summary>
        /// <param name="file"></param>
        /// <param name="name"></param>
        public void OpenFileAsName(string file, string name) {
            string cmd = "open \"" + file + "\" alias " + GetInnerName(name);
            mciSendStringEasy(cmd);
            mciSendStringEasy("set " + GetInnerName(name) + " time format milliseconds");
        }

        /// <summary>
        /// 播放指定字段
        /// </summary>
        /// <param name="name"> 名字</param>
        /// <returns>需要等待时常.毫秒</returns>
        public int PlayWithName(string name) {
            if (name == null) 
            {
                return 0;
            }
            string cmd = "play " + GetInnerName(name) + " from 0";
            mciSendStringEasy(cmd);
            if (name == alart_name) {
                return 1400;
            }
            return 400;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AITalker() {
            CloseAll();
            bool createdNew = false;
            MsgdMutex = new Mutex(false, "AiTalkerMsg", out createdNew);
            LoadFile();
        }
        /// <summary>
        /// 结束时候关闭所有.需要主动调用.
        /// 不调用我也不知道会怎么样:)
        /// </summary>
        public void CloseAll()
        {
            mciSendStringEasy("close all");
        }

        string alart_name = "alart";
        /// <summary>
        /// 打开所有声音文件
        /// </summary>
        public void LoadFile()
        {
            for (int i = 0; i <= 9; i++)
            {
                OpenFileAsName("LetterVoice\\" + i.ToString() + ".mp3", i.ToString());
            }
            for (char i = 'a'; i <= 'z'; i++)
            {
                OpenFileAsName("LetterVoice\\" + i + ".mp3", "" + i);
            }
            OpenFileAsName("LetterVoice\\gang.mp3", "gang");
            OpenFileAsName("LetterVoice\\enemy.mp3", "emeny");
            OpenFileAsName("LetterVoice\\alart.mp3", alart_name);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 引入mciSendString @ winmm.dll
        /// </summary>
        /// <param name="lpstrCommand">命令串</param>
        /// <param name="lpstrReturnString">返回串</param>
        /// <param name="uReturnLength">返回串长度最大值</param>
        /// <param name="hwndCallback">回调</param>
        /// <returns></returns>
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]
        public static extern int mciSendString(
            string lpstrCommand,
            string lpstrReturnString,
            int uReturnLength,
            int hwndCallback
            );
        [DllImport("winmm.dll", EntryPoint = "mciGetErrorString", CharSet = CharSet.Auto)]
        public static extern bool mciGetErrorString
            (
            int fdwError,
            string lpszErrorText,
            int uReturnLength
            );       
    }
}
