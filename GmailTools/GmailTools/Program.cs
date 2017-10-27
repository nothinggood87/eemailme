using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using GmailTools.Reports;

namespace GmailTools
{
    class Program
    {
        static void Main(string[] args)
        {
            string date = "2017-10-12";
            string data = $"{Config.DataPath}\\{date}";
            string report = Config.ReportsPath;
            var reports = new List<Report>
            {
                new BasicToFromSubjectEtc($"{data}\\jacob.mbox",
                    $"{report}\\BasicToFromSubjectEtc\\BasicToFromSubjectEtc.csv")
            };
            Console.WriteLine("Choose report to run");
            for(int i = 0; i < reports.Count; i++)
            {
                Console.WriteLine($"{i} : {reports[i].GetType().Name}");
            }
            int choice = Convert.ToInt32(Console.ReadLine());
            reports[choice].Process();
        }
    }
}
