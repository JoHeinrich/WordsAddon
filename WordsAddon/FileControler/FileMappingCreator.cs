using System;
using System.Collections.Generic;
using System.IO;

namespace VoiceControl
{
    public class FileMappingCreator
    {
        public Dictionary<string, string> CreateMapping(string path)
        {
            Dictionary<string, string> mapping = new Dictionary<string, string>();
                    
            foreach (var word in File.ReadLines(path))
            {
                try
                {
                    if (word.Contains(":"))
                    {
                        var split = word.Split(':');
                        var selector = split[0];
                        var rest = word.Remove(0, selector.Length + 1);
                        selector = selector.Trim();
                        rest = rest.Trim();
                        mapping.Add(selector, rest);
                    }
                    else
                    {
                        var wordt = word.Trim();
                        if (wordt != "")
                        {
                            mapping.Add(wordt, wordt);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("coud not add word " + word + " => " + e.Message);
                    throw;
                }
            }

            return mapping;

        }
    }
}
