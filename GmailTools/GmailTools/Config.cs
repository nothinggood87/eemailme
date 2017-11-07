using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools
{
    class Config
    {
        private static List<string> andrewPCs = new List<string>
        {
            "ANDREW-LAPTOP",
            "ANDREW-DESKTOP"
        };
        private static List<string> jacobPCs = new List<string>
        {
            "ENVY10",
            "DT10",
            "DTGAME",
            "ENVY13"
        };
        public static string User
        {
            get
            {
                string pc = Environment.MachineName;
                if (jacobPCs.Contains(pc)) { return "Jacob"; }
                else if (andrewPCs.Contains(pc)) { return "Andrew"; }
                else { throw new Exception("Your computer name is not supported!"); }
            }
        }
        public static string ProjectPath
        {
            get
            {
                switch (Environment.MachineName)
                {
                    // Jacob
                    case "ENVY10": return "C:\\Dropbox\\Projects\\eemailme";
                    case "DT10": return "V:\\Dropbox\\Projects\\eemailme";
                    // Andrew
                    case "ANDREW-LAPTOP": return "C:\\Users\\andre\\Dropbox\\eemailme";
                    case "ANDREW-DESKTOP": return "C:\\Users\\andre\\Dropbox\\eemailme";
                }
                throw new Exception("Your computer name is not supported!");
            }
        }
        public static string ReportsPath => $"{ProjectPath}\\Reports\\{User}";
        public static string DataPath => $"{ProjectPath}\\Data";
        public static List<string> GmailSystemLabels => new List<string>
        {
            "INBOX",
            "SPAM",
            "TRASH",
            "UNREAD",
            "STARRED",
            "IMPORTANT",
            "SENT",
            "DRAFTS",
            "CATEGORY_PERSONAL",
            "CATEGORY_SOCIAL",
            "CATEGORY_PROMOTIONS",
            "CATEGORY_UPDATES",
            "CATEGORY_FORUMS"
        };
    }
}
