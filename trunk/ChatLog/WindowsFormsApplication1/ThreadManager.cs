using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
namespace WindowsFormsApplication1
{
    class ThreadManager
    {
        /// <summary>
        /// 父窗体
        /// </summary>
        public Form     Parent              = null;
        /// <summary>
        /// 线程表
        /// </summary>
        LinkedList<Thread> ThreadSet        = new LinkedList<Thread>();

        /// <summary>
        /// 委托原型 废弃
        /// </summary>
        /// <param name="objs">参数列表</param>
        public delegate void VoidFuncDelegate(object[] objs);

        public void RunBackground(Thread th) 
        {
            //Thread th = new Thread(new ThreadStart(func.));
            if (th != null)
            {
                ThreadSet.AddFirst(th);
                th.Start();
            }
        }

        public void TerminateAllThreads() 
        {
            foreach (Thread th in ThreadSet) 
            {
                if (th != null
                    && th.ThreadState != ThreadState.Aborted
                    && th.ThreadState != ThreadState.Unstarted) 
                {
                    th.Abort();
                }
            }
        }
        
    }
    
    
}
