using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceControl
{

    public interface IArgumentWord
    {
        void Build(string argument);
        Rule Builder { get; }

    }

    public class TextWord : IArgumentWord
    {
        public void Build(string argument)
        {
            Builder = new Text(argument);
        }

        public Rule Builder { get; private set; }

    }

    public class NumberWord : IArgumentWord
    {
        public void Build(string argument)
        {
            Builder = new Number(100);
        }

        public Rule Builder { get; private set; }

    }
}
