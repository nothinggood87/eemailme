using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailTools
{
    class CsvReport
    {
        public CsvReport(List<string> headers, List<List<string>> data)
        {
            // Convert headers to csv
            foreach (string h in headers)
            {
                Header += ",\"{h}\"";
            }
            Header = Header.Substring(1);
            // Convert data to csv
            foreach (List<string> row in data)
            {
                var csvRow = "";
                foreach (string col in row)
                {
                    csvRow += ",\"{col}\"";
                }
                Data.Add(csvRow.Substring(1));
            }
        }
        public string Header { get; set; }
        public List<string> Data { get; set; }
        public List<string> GetCsvText()
        {
            var text = new List<string>
            {
                Header
            };
            text.AddRange(Data);
            return text;
        }
    }
}
