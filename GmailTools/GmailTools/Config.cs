using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools
{
    class Config
    {
        public static string ProjectPath => "C:\\Dropbox\\Projects\\eemailme";
        public static string ReportsPath => $"{ProjectPath}\\Reports\\Jacob";
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
            "DRAFT",
            "CATEGORY_PERSONAL",
            "CATEGORY_SOCIAL",
            "CATEGORY_PROMOTIONS",
            "CATEGORY_UPDATES",
            "CATEGORY_FORUMS"
        };
    }
}
