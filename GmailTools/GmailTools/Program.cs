using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;

namespace GmailTools
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessData();
        }
        public static void ProcessData()
        {
            FileStream stream = new FileStream("C:\\TMP\\Mail\\mail.mbox", FileMode.Open);
            var parser = new MimeParser(stream, MimeFormat.Mbox);
            MimeMessage message;
            List<MailMessage> data = new List<MailMessage>();
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"C:\TMP\Mail\stats.csv"))
            {
                file.WriteLine(MailMessage.GetCsvHeaders());
                while (!parser.IsEndOfStream)
                {
                    message = parser.ParseMessage();
                    var msg = new MailMessage();
                    msg.To = message.To.ToString();
                    msg.From = message.From.ToString();
                    msg.Subject = message.Subject;
                    msg.Date = message.Date;
                    msg.OriginalTo = message.Headers.Where(x => x.Field == "X-Gm-Original-To").First().Value;
                    msg.DeliveredTo = message.Headers.Where(x => x.Field == "Delivered-To").First().Value;
                    msg.GmailLabels = message.Headers.Where(x => x.Field == "X-Gmail-Labels").First().Value;
                    file.WriteLine(msg.ToCsvString());
                    // data.Add(new MailMessage(message.From.ToString(), message.Subject, message.TextBody, message.Date));
                    if (data.Count % 10000 == 0)
                    {
                        Console.WriteLine(data.Count / 1000 + "k complete");
                    }
                }
            }
        }
        class MailMessage
        {
            
            public string To { get; set; }
            public string From { get; set; }
            public string Subject { get; set; }
            public DateTimeOffset Date { get; set; }
            public string OriginalTo { get; set; }
            public string DeliveredTo { get; set; }
            public string GmailLabels { get; set; }

            public string ToCsvString(bool Stats = true)
            {
                return $"\"{To}\",\"{From}\",\"{Subject}\",\"{Date}\",\"{OriginalTo}\",\"{DeliveredTo}\",\"{GmailLabels}\"";
            }
            public static string GetCsvHeaders()
            {
                string[] fields = {
                    "To",
                    "From",
                    "Subject",
                    "Date",
                    "X-Gm-Original-To",
                    "Delivered-To",
                    "X-Gmail-Labels"
                };
                var str = "";
                foreach(var f in fields)
                {
                    str += $",\"{f}\"";
                }
                return str.Substring(1,str.Length-1);
            }
        }
    }
    
}
