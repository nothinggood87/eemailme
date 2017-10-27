using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using System.IO;

namespace GmailTools.Reports
{
    /*
     * Example CSV
     * Date, Time, GitHub, Lists, Junk
     * 2017, 10:56, 20,    10,     1
     */
    class MailVolumeByLabel : IReport
    {
        public MailVolumeByLabel(string sourceDataPath, string destinationCsvPath)
        {
            _sourceDataPath = sourceDataPath;
            _destinationCsvPath = destinationCsvPath;
        }
        string _sourceDataPath, _destinationCsvPath;
        private List<string> _headers = new List<string>();
        private class StatItem
        {
            public DateTimeOffset Date { get; set; }
            public string Label { get; set; }
            public int MessageCount { get; set; }
        }
        private class Stats
        {
            public List<StatItem> Items { get; set; } = new List<StatItem>();
            public void Add(StatItem item)
            {
                item.Date = item.Date.AddMinutes(item.Date.Minute * -1);
                item.Date = item.Date.AddSeconds(item.Date.Second * -1);
                item.Date = item.Date.AddMilliseconds(item.Date.Millisecond * -1);
                
                Items.Add(item);
            }
        }
        public void Process()
        {
            var data = new List<List<string>>();
            var headers = new List<string>();
            using (var fs = new FileStream(_sourceDataPath, FileMode.Open, FileAccess.Read))
            {
                var parser = new MimeParser(fs, MimeFormat.Mbox);
                int count = 0;
                var stats = new Stats();
                while (!parser.IsEndOfStream)
                {
                    MimeMessage message = parser.ParseMessage();
                    var userLabels = new List<string>();
                    string labels = message.Headers.Where(x => x.Field == "X-Gmail-Labels")
                        .FirstOrDefault()?.Value ?? "nouserlabels";
                    foreach (string l in labels.Split(','))
                    {
                        if (!Config.GmailSystemLabels.Contains(l.ToUpper()))
                        {
                            // Add a stat for that label
                            stats.Add(new StatItem()
                            {
                                Date = message.Date,
                                Label = l,
                                MessageCount = 1
                            });
                        }
                    }
                    // Print status
                    if (count % 10000 == 0)
                        Console.WriteLine(count / 1000 + "k complete");

                    count++;
                }
                // Agregate stats
                var statsDict = new Dictionary<DateTimeOffset, List<int>>();
                DateTimeOffset startDate = stats.Items.Min(x => x.Date);
                startDate = startDate.AddMilliseconds(startDate.Millisecond * -1);
                DateTimeOffset endDate = stats.Items.Max(x => x.Date);
                endDate = endDate.AddMilliseconds(endDate.Millisecond * -1);
                List<string> uniqueLabels = stats.Items.Select(x => x.Label)
                        .Distinct()
                        .ToList();
                _headers.Add("DateTime");
                _headers.AddRange(uniqueLabels);
                for (var date = startDate; date <= endDate; date = date.AddHours(1))
                {
                    statsDict.Add(date, new int[uniqueLabels.Count].ToList());
                }
                for(int i = 0; i < stats.Items.Count; i++)
                {
                    statsDict[stats.Items[i].Date][uniqueLabels.IndexOf(stats.Items[i].Label)]++;
                }
                for (var date = startDate; date <= endDate; date = date.AddHours(1))
                {
                    var row = new List<string>
                    {
                        date.ToString()
                    };
                    foreach (var c in statsDict[date])
                    {
                        row.Add(c.ToString());
                    }
                    data.Add(row);
                }
            }
            var csvReport = new CsvReport(_headers, data);
            File.WriteAllLines(_destinationCsvPath, csvReport.GetCsvText());
        }
    }
}
