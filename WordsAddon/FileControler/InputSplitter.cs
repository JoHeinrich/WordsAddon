using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VoiceControl
{
    public class SimpleSplitter : IEnumerable<KeyValuePair<string, string>>
    {
        Dictionary<string, string> mapping = new Dictionary<string, string>();

        public IEnumerator GetEnumerator()
        {
            return mapping.GetEnumerator();
        }

        void Add(string key, string value)
        {
            if (mapping.ContainsKey(key)) return;
            mapping[key] = value;
        }



        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, string>>)mapping).GetEnumerator();
        }

        public SimpleSplitter(IEnumerable<string> input, string splitter = "[A-Z][a-z]+")
        {
            //input = input.Select(x => x.ToLower());
            foreach (var item in input)
            {
                
                Add(item, item);
            }
            foreach (var item in input)
            {
                var match = Regex.Match(item, "[A-Z][a-z]+");
                List<string> words = new List<string>();
                while (match.Success)
                {
                    words.Add(match.Value);
                    Add(match.Value, item);
                    match = match.NextMatch();
                }
                for (int i = 0; i < words.Count; i++)
                {
                    string connected = "";
                    for (int j = i; j < words.Count; j++)
                    {
                        connected += words[j];
                        Add(connected, item);
                        connected += " ";

                    }
                }
            }
        }
    }
    public class InputSplitter
    {
        Dictionary<string, List<string>> mapping = new Dictionary<string, List<string>>();

        void Add(string key, string value)
        {
            List<string> list;
            if (!mapping.TryGetValue(key, out list))
            {
                list = new List<string>();
                mapping[key] = list;
            }
            if (!list.Contains(value)) list.Add(value);
        }
        public InputSplitter(IEnumerable<string> input, string splitter = "[A-Z][a-z]+")
        {
            foreach (var item in input)
            {
                Add(item, item);
            }
            foreach (var item in input)
            {
                var match = Regex.Match(item, "[A-Z][a-z]+");
                List<string> words = new List<string>();
                while (match.Success)
                {
                    words.Add(match.Value);
                    Add(match.Value, item);
                    match = match.NextMatch();
                }
                for (int i = 0; i < words.Count; i++)
                {
                    string connected = "";
                    for (int j = i; j < words.Count; j++)
                    {
                        connected += words[j];
                        Add(connected, item);
                        connected += " ";

                    }
                }
            }
        }
        public Dictionary <string, string> First()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (var item in mapping)
            {
                values[item.Key]= item.Value.First();
            }
            return values;
        }
        public List<KeyValuePair<string, string>> Get(int max)
        {

            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            foreach (var item in mapping)
            {
                if (item.Value.Count <= max)
                {
                    item.Value.ForEach(x => values.Add(new KeyValuePair<string, string>(item.Key, x)));
                }
            }
            return values;
        }
    }
}
