using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSClass : ICallable
    {
        public readonly string Name;

        public LSClass(string name)
        {
            Name = name;
        }

        public object Call(Interpreter interpreter,
            List<object> arguments)
        {
            var instance = new LSInstance(this);
            return instance;
        }

        public int Arity()
        {
            return 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
