using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public class StarSearch
    {
        public class StarSystem {
            public int GameLinkId;
            public string FullName;
            public string Ownername;
            public string[] Names;
            public StarSystem[] LinkedSystem;
            

            public bool InitWithString(string str) 
            {
                char[] split = { '\t' };
                char[] splitnames = { ',','"'};
                string[] substrs = str.Split(split);
                if (substrs.Length > 1) {
                    FullName = substrs[0];
                }
                if (substrs.Length > 2 && substrs[1].Length>0) {
                    string[] nicknames = substrs[1].Split(splitnames);
                    Names = nicknames;
                }
                if (substrs.Length > 3 && substrs[2].Length>0) {
                    Ownername = substrs[2];
                }
                return true;
            }
            public bool IsSameName(string str) {
                bool rlt = str.Contains(FullName);
                for (int i = 0; !rlt && i < Names.Length; i++) {
                    rlt = str.Contains(Names[i]);
                }
                return rlt;
            }
        };
        public char[] StaticSplit = { ' ', '\r', '\n', ',' };
        public System.Collections.Generic.Dictionary<string, StarSystem> dict = null;
        /// <summary>
        /// 读取数据文件
        /// </summary>
        public void Load() 
        {
            System.IO.FileStream fs = null;
            System.IO.StreamReader sr =null;
            try
            {
                fs = new System.IO.FileStream("Star.dat", System.IO.FileMode.Open);
                sr = new System.IO.StreamReader(fs);
                if (dict != null) { dict.Clear(); }
                else {
                    dict = new Dictionary<string,StarSystem>();
                }
                sr.ReadLine();
                while (!sr.EndOfStream) 
                {
                    string line = sr.ReadLine();
                    StarSystem ss = new StarSystem();
                    ss.InitWithString(line);
                    dict[ss.FullName] = ss;
                    dict[ss.FullName.ToLower()] = ss;
                    for (int i = 0; ss.Names != null && i < ss.Names.Length; i++) 
                    {
                        if (ss.Names[i].Length > 0)
                        {
                            dict[ss.Names[i]] = ss;
                            dict[ss.Names[i].ToLower()] = ss;
                        }
                    }
                }
            }
            catch (Exception) { }
            finally {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }

        }

        public StarSystem[] SearchStarSystem(string line) 
        {
            List<StarSystem> list = new List<StarSystem>();
            StarSystem[] result = null;
            string[] words = DisassembString(line);
            foreach (string w in words) {
                StarSystem ss = null;
                bool has =    dict.TryGetValue(w,out ss) 
                           || dict.TryGetValue(w.ToLower(),out ss)
                           || dict.TryGetValue(w.ToUpper(),out ss);
                if (has) 
                {
                    list.Add(ss);
                }
            }
            if (list.Count > 0) 
            {
                result = list.ToArray();
            }
            return result;
        }

        public string[] SearchUnkonwSystem(string line) 
        {
            List<string> list = new List<string>();
            string[] result = null;
            string[] words = DisassembString(line);
            foreach (string w in words) {
                bool bAllAscii = true;
                bool bHasMinus = false;
                for (int i = 0; i < w.Length; i++) {
                    if (w[i] == '-') { bHasMinus = true; }
                    if ((int)w[i] >127) {bAllAscii = false;}
                }
                if (bAllAscii && bHasMinus && w.Length > 3 && w.Length < 7) 
                {
                    list.Add(w);
                }
            }
            if (list.Count > 0)
            {
                result = list.ToArray();
            }
            return result;
        }

        public string[] DisassembString(string line) 
        {
            List<string> list = new List<string>();
            string[] words = line.Split(StaticSplit);
            foreach (string w in words) 
            {
                list.Add(w);
                DisStrWithEngOrOther(w, list);
            }
            return list.ToArray();
        }

        public int DisStrWithEngOrOther(string str,List<string> list) 
        {
            //List<string> list = new List<string>();
            int nStartPos = 0;
            int nCnt = 0;
            for (int i = 0; i < str.Length-1; i++) 
            {
                int nCheckCode = (127 - (int)str[i]) * (127 - (int)str[i + 1]);
                if (nCheckCode <= 0) 
                {
                    string substr = str.Substring(nStartPos, i - nStartPos+1);
                    nStartPos = i+1;
                    list.Add(substr);
                    nCnt++;
                }
            }
            if (nStartPos < str.Length - 1 && nStartPos!=0) {
                list.Add(str.Substring(nStartPos));
                nCnt++;
            }
            return nCnt;
        }
    }
}
