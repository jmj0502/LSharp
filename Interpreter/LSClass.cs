using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSClass
    {
        public readonly string Name;

        public LSClass(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
