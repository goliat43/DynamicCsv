using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DynamicCsv
{
    public static class Constants
    {
        public const char Separator = ',';
    }

    /// <summary>
    /// Property of Niclas Eriksson 
    /// </summary>
    public sealed class DynamicCsvFile : IEnumerable<DynamicCsvRow>
    {
        private int count;

        private DynamicCsvFile()
        {
        }

        public List<string> Headers { get; } = new List<string>();
        public List<DynamicCsvRow> Rows { get; } = new List<DynamicCsvRow>();

        public int Count => count;

        public static DynamicCsvFile Load(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));

            var fs = new StreamReader(filename);

            return Load(fs);
        }


        public static DynamicCsvFile Load(StreamReader filestream)
        {
            if (filestream == null) throw new ArgumentNullException(nameof(filestream));

            var csvFile = new DynamicCsvFile();
            string row;

            //Read the header and store it.
            string header = filestream.ReadLine();
            if (string.IsNullOrWhiteSpace(header)) throw new InvalidDataException("Header in CSV was empty");
            csvFile.Headers.AddRange(header.Split(Constants.Separator));

            //Read the rest of the file.
            while ((row = filestream.ReadLine()) != null)
            {
                csvFile.count++; //TODO: Ugly!
                //Skip empty rows.
                if (string.IsNullOrWhiteSpace(row)) continue;

                csvFile.Rows.Add(new DynamicCsvRow(row, csvFile.Headers));
            }

            return csvFile;
        }

        public IEnumerator<DynamicCsvRow> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


