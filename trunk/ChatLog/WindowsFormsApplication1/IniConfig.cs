using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
{
    public class IniConfig
    {
        public string filename = "Config.ini";
        System.Collections.Generic.Dictionary<string, string> config_values = new Dictionary<string, string>();

        public bool SkipTread           = true;
        public bool SpeakUnknow         = false;
        public string[] TreadKeyWards   = null;
        public bool DebugMsg            = false;
        public void load() 
        {
            System.IO.FileStream   fs   = new System.IO.FileStream(filename, System.IO.FileMode.Open);
            System.IO.StreamReader rs   = new System.IO.StreamReader(fs);
            rs.ReadLine(); // read [config]
            while (!rs.EndOfStream) 
            {
                string line = rs.ReadLine();
                string[] detail = line.Split(new char[] { '=','#' });
                if (detail.Length >= 2)
                {
                    config_values[detail[0]] = detail[1];
                }
            }

            SkipTread     = IsSkipTrade();
            SpeakUnknow   = IsSpeakUnknowSystem();
            TreadKeyWards = GetTreadKeyWords();
            DebugMsg      = IsDebugMode();
        }

        bool IsSkipTrade() 
        {
            try
            {
                return config_values["SkipTrade"] == "1";
            }
            catch { return true; }
        }

        bool IsSpeakUnknowSystem() 
        {
            try
            {
                return config_values["SpeakUnknowSystem"] == "1";
            }
            catch { return false; }
        }

        string[] GetTreadKeyWords() 
        {
            try
            {
                string value = config_values["TradeKeyWords"];
                if (value == null) { return null; }
                return value.Split(new char[] { ',' });
            }
            catch{
                return null;
            }
        }

        bool IsDebugMode() 
        {
            try
            {
                return config_values["DebugMessage"] == "1";
            }
            catch{
                return false;
            }
        }
    }
}
