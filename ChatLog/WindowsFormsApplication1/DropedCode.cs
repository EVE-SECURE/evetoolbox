/*
 * public delegate void VoidFuncDelegate(object[] objs);
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
 * 
 * 
 *         public delegate void PlayByNameDelegate(string str);
        public PlayByNameDelegate func = null;
        /// <summary>
        /// 线程死循环函数
        /// </summary>
        public void TalkThreadFun() 
        {
            //return;
            for (bool loop = true; loop; ) 
            {
                if (msgqueue.Count > 0)
                {
                    MsgdMutex.WaitOne();
                    string[] namelist = msgqueue.ToArray();
                    msgqueue.Clear();
                    MsgdMutex.ReleaseMutex();

                    for (int i = 0; i < namelist.Length; i++)
                    {
                        int nSleep = 400;
                        if (namelist[i].Equals(alart_name)) {
                            nSleep = 1500;
                        }
                        //
                        //func.Invoke(namelist[i]);
                        //func.DynamicInvoke(new object[] { namelist[i] });
                        //func.
                        //PlayWithName(namelist[i]);
                        //Thread.Sleep(1500);
                    }
                }
                Thread.Sleep(1000);
            }
        }

*/