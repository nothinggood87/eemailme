using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools.Reports
{
    class GithubUserVolumeByDate
    {
        public GithubUserVolumeByDate(string sourceDataPath, string destinationCsvPath)
        {
            _sourceDataPath = sourceDataPath;
            _destinationCsvPath = destinationCsvPath;
        }
        string _sourceDataPath, _destinationCsvPath;
        int StartQuarterHourOfYear { get; }
        int EndQuarterHourOfYear { get; }
        DateTime EndDate { get; }
        int AccountedQuarterHours => EndQuarterHourOfYear - StartQuarterHourOfYear;
        static int ToQuarterHour(DateTime n) => (n.Date.DayOfYear * 24 + n.Date.Hour) * 4 + n.Date.Minute / 15;
        static DateTime FromQuarterHour(int n)
        {
            var date = new DateTime(17, 0, 0, 0, 0, 0, 0);
            date = date.AddHours(n / 4);
            return date.AddMinutes(15 * (n - n / 4 * 4));
        }
        public void Process()
        {
            var headers = new List<string>();
            headers.Add("QuarterHourOfYear");
            var raw = new List<int>[AccountedQuarterHours];
            MimeParser parser;
            MimeMessage message;
            string name;
            int quarterHour;
            
            using (var fs = new FileStream(_sourceDataPath, FileMode.Open, FileAccess.Read))
            {
                parser = new MimeParser(fs, MimeFormat.Mbox);
                int i, k;
                for(i = 0; !parser.IsEndOfStream;i++)
                {
                    message = parser.ParseMessage();
                    k = message.From.Count - 1;
                    if (message.From[k].Name.ToLower().Contains("github"))
                    {
                        message.Date = message.Date.ToLocalTime();
                        name = message.From[k].Name;
                        name = name.Substring(name.IndexOf('['));
                        name = name.Substring(0, name.IndexOf('/')).ToLower();
                        quarterHour = (message.Date.DayOfYear * 24 + message.Date.Hour) * 4 + message.Date.Minute / 15 - StartQuarterHourOfYear;
                        if (raw[quarterHour] == null)
                            raw[quarterHour] = new List<int>(new int[headers.Count]);
                        else if (raw[quarterHour].Count < headers.Count)
                            raw[quarterHour].AddRange(new int[headers.Count - raw[quarterHour].Count]);
                        if (!headers.Contains(name))
                        {
                            headers.Add(name);
                            raw[quarterHour].Add(1);
                        }
                        else raw[quarterHour][headers.FindIndex(x => x == name)]++;
                    }
                    // Print status
                    if (i % 10000 == 0)
                        Console.WriteLine(i / 1000 + "k complete");
                }
            }
            List<List<string>> data = new List<List<string>>(AccountedQuarterHours);
            for (var date = StartQuarterHourOfYear; date <= EndQuarterHourOfYear; date++)
            {
                var row = new List<string>
                    {
                        FromQuarterHour(date).ToString()
                    };
                foreach (var c in raw[date])
                {
                    row.Add(c.ToString());
                }
                data.Add(row);
            }
            var csvReport = new CsvReport(headers, data);
            File.WriteAllLines(_destinationCsvPath, csvReport.GetCsvText());
        }
    }
}
