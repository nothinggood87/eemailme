using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;

namespace GmailTools.Reports
{
    class BasicToFromSubjectEtc : Report
    {
        public BasicToFromSubjectEtc(string sourceDataPath, string destinationCsvPath)
        {
            _sourceDataPath = sourceDataPath;
            _destinationCsvPath = destinationCsvPath;
        }
        internal override List<string> ProcessMessage(MimeMessage message)
        {
            var msg = new MailMessage
            {
                To = message.To.ToString(),
                From = message.From.ToString(),
                Subject = message.Subject,
                Date = message.Date,
                OriginalTo = message.Headers.Where(x => x.Field == "X-Gm-Original-To").FirstOrDefault()?.Value ?? "",
                DeliveredTo = message.Headers.Where(x => x.Field == "Delivered-To").FirstOrDefault()?.Value ?? "",
                GmailLabels = message.Headers.Where(x => x.Field == "X-Gmail-Labels").FirstOrDefault()?.Value ?? ""
            };
            return msg.ToList();
        }

        internal override List<string> GetHeaders()
        {
            return MailMessage.GetCsvHeaders();
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

        public List<string> ToList()
        {
            return new List<string>
            {
                To,
                From,
                Subject,
                Date.ToString(),
                OriginalTo,
                DeliveredTo,
                GmailLabels
            };
        }
        public static List<string> GetCsvHeaders()
        {
            return new List<string>
            {
                "To",
                "From",
                "Subject",
                "Date",
                "X-Gm-Original-To",
                "Delivered-To",
                "X-Gmail-Labels"
            };
        }
    }
}
