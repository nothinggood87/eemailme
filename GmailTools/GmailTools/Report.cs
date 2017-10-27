using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools
{
    /// <summary>
    /// Intended to automate as much of report processing as possible.
    /// Report data will all be kept in-memory until complete.
    /// </summary>
    abstract class Report
    {
        public void Process(string sourceDataPath, string destinationCsvPath)
        {
            var data = new List<List<string>>();
            using (var fs = new FileStream(sourceDataPath, FileMode.Open, FileAccess.Read))
            {
                var parser = new MimeParser(fs, MimeFormat.Mbox);
                int count = 0;
                while (!parser.IsEndOfStream)
                {
                    MimeMessage message = parser.ParseMessage();
                    data.Add(ProcessMessage(message));
                    // data.Add(new MailMessage(message.From.ToString(), message.Subject, message.TextBody, message.Date));
                    if (count % 10000 == 0)
                    {
                        Console.WriteLine(count / 1000 + "k complete");
                    }
                    count++;
                }
            }
            var csvReport = new CsvReport(Headers, data);
            File.WriteAllLines(destinationCsvPath, csvReport.GetCsvText());
        }
        public abstract List<string> ProcessMessage(MimeMessage message);
        public List<string> Headers { get; set; }
    }
}
