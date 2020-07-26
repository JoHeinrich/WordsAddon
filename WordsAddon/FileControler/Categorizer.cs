using System.Collections.Generic;
using System.Linq;

namespace VoiceControl
{
    public class Categorizer
    {
        Dictionary<string, string> settings = new Dictionary<string, string>();

        public Categorizer(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> Words => settings.Where(x => x.Key[0] != '!' && x.Key[0] != '?');
        public IEnumerable<KeyValuePair<string, string>> Settings => settings.Where(x => x.Key[0] == '!').Select(x=>new KeyValuePair<string, string>(x.Key.Remove(0, 1),x.Value));
        public IEnumerable<KeyValuePair<string, string>> Commands => settings.Where(x => x.Key[0] == '?').Select(x => new KeyValuePair<string, string>(x.Key.Remove(0, 1), x.Value));

    }
}
