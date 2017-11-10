using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools.Reports
{
    class GithubUserVolumeByDate : IReport
    {
        public GithubUserVolumeByDate(string sourceDataPath, string destinationCsvPath)
        {
            _sourceDataPath = sourceDataPath;
            _destinationCsvPath = destinationCsvPath;
        }
        private string _sourceDataPath, _destinationCsvPath;
        private int _startQuarterHourOfYear = int.MaxValue, _endQuarterHourOfYear = int.MinValue;
        private static int ToQuarterHour(DateTime time) => (time.Date.DayOfYear * 24 + time.Date.Hour) * 4 + time.Date.Minute / 15;
        private List<string> _headers = new List<string>();
        private static DateTime FromQuarterHour(int quarterHourOfYear)
        {
            var date = new DateTime(DateTime.UtcNow.Year, 1, 1).AddDays(quarterHourOfYear / 4 - 1);
            quarterHourOfYear %= 4 * 24;
            date = date.AddHours(quarterHourOfYear/4);
            date = date.AddMinutes(15 * (quarterHourOfYear % 4));
            return date;
        }
        public void Process()
        {
            _headers.Add("QuarterHourOfYear");
            Dictionary<int,List<int>> raw = new Dictionary<int,List<int>>();
            
            using (var fs = new FileStream(_sourceDataPath, FileMode.Open, FileAccess.Read))
            {
                var parser = new MimeParser(fs, MimeFormat.Mbox);
                for(int i = 0; !parser.IsEndOfStream;i++)
                {
                    MimeMessage message = parser.ParseMessage();
                    int lastMessageIndex = message.From.Count - 1;
                    if (lastMessageIndex >= 0 &&message.From[lastMessageIndex].Name.ToLower().Contains("github"))
                    {
                        message.Date = message.Date.ToLocalTime();
                        var name = message.From[lastMessageIndex].Name;
                        int cropIndex = name.IndexOf('[');
                        if (cropIndex > 0)
                        {
                            name = name.Substring(cropIndex);
                            name = name.Substring(0, name.IndexOf('/')).ToLower();
                        }
                        var quarterHour = (message.Date.DayOfYear * 24 + message.Date.Hour) * 4 + message.Date.Minute / 15;
                        //setting min max for time
                        if (quarterHour < _startQuarterHourOfYear)
                            _startQuarterHourOfYear = quarterHour;
                        else if (quarterHour > _endQuarterHourOfYear)
                            _endQuarterHourOfYear = quarterHour;

                        if (!raw.ContainsKey(quarterHour))
                            raw.Add(quarterHour, new List<int>());
                        if (raw[quarterHour] == null)
                            raw[quarterHour] = new List<int>(new int[_headers.Count]);
                        else if (raw[quarterHour].Count < _headers.Count)
                            raw[quarterHour].AddRange(new int[_headers.Count - raw[quarterHour].Count]);
                        if (!_headers.Contains(name))
                        {
                            _headers.Add(name);
                            raw[quarterHour].Add(1);
                        }
                        else raw[quarterHour][_headers.FindIndex(x => x == name)]++;
                    }
                    // Print status
                    if (i % 10000 == 0)
                        Console.WriteLine(i / 1000 + "k complete");
                }
            }
            List<List<string>> data = new List<List<string>>(_endQuarterHourOfYear - _startQuarterHourOfYear);
            for (var date = _startQuarterHourOfYear; date <= _endQuarterHourOfYear; date++)
            {
                var row = new List<string>
                    {
                        FromQuarterHour(date).ToString()
                    };
                if(raw.ContainsKey(date))
                    for (int i = 0; i < raw[date].Count; i++)
                    {
                        row.Add(raw[date][i].ToString());
                    }
                data.Add(row);
            }
            File.WriteAllLines(_destinationCsvPath, new CsvReport(_headers, data).GetCsvText());
        }
    }
}
