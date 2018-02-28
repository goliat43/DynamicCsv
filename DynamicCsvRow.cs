using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicCsv
{
    [System.Diagnostics.DebuggerDisplay("{" + nameof(ToDebugString) + "()}")]
    public sealed class DynamicCsvRow : DynamicObject, IEnumerable<DynamicCsvEntry>
    {
        private readonly List<string> header;
        private readonly List<string> row;
        public dynamic Dynamic => this;

        public DynamicCsvRow(string rowContent, List<string> header)
        {
            if (rowContent == null) throw new ArgumentNullException(nameof(rowContent));
            if (header == null) throw new ArgumentNullException(nameof(header));

            this.header = header;
            this.row = rowContent.Split(Constants.Separator).ToList();
            if (header.Count != row.Count) throw new InvalidDataException("CSV row content doesn't match header.");
        }


        #region Dynamic

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var indexToGet = header.IndexOf(binder.Name);

            result = null;
            if (indexToGet == -1) return false;

            result = row[indexToGet];

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var indexToSet = header.IndexOf(binder.Name);
            if (indexToSet == -1) return false;

            row[indexToSet] = value.ToString();
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;

            if (indexes[0] == null) return false;

            if (indexes[0] is int index1)
            {
                result = row[index1];
                return true;
            }
            if (indexes[0] is string att)
            {
                var index = header.IndexOf(att);
                result = row[index];
                return true;
            }

            return false;
        }


        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes[0] == null) return false;

            if (indexes[0] is int)
            {
                var index = (int)indexes[0];
                row[index] = value.ToString();

            }
            else if (indexes[0] is string)
            {
                var att = (string)indexes[0];
                var index = header.IndexOf(att);
                if (index == -1) return false;

                row[index] = value.ToString();
                return true;
            }

            return false;
        }


        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            if (binder.Type == typeof(List<string>))
            {
                result = row;
                return true;
            }
            if (binder.Type == typeof(string))
            {
                result = string.Join(Constants.Separator.ToString(), row);
                return true;
            }

            return false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var type = typeof(List<string>);
            try
            {
                result = type.InvokeMember(binder.Name,
                                           BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                                           null, row, args);

                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        #endregion Dynamic


        #region Enumerable
        public IEnumerator<DynamicCsvEntry> GetEnumerator()
        {
            return header.Select((t, i) => new DynamicCsvEntry(row[i], t)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<dynamic> AsDynamicEnumerable()
        {
            return this;
        }
        #endregion Enumerable

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            for(int i = 0; i < header.Count; i++)
            {
                dict.Add(header[i], row[i]);
            }

            return dict;
        }

        public string ToDebugString()
        {
            if (row == null || row.Count == 0) return string.Empty;

            var sb = new string[header.Count];
            for (var i = 0; i < header.Count; i++)
            {
                sb[i] = $"{header[i]}={row[i]}";
            }

            return string.Join(Constants.Separator.ToString(), sb);
        }

        public override string ToString()
        {
            return (row != null && row.Count > 0) ? string.Join(Constants.Separator.ToString(), row) : string.Empty;
        }
    }
}