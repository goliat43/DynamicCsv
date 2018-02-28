using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicCsv
{
    public class DynamicCsvEntry
    {
        public string Header { get; }
        public string Content { get; }

        public DynamicCsvEntry(string content, string header)
        {
            Content = content;
            Header = header;
        }
    }
}
