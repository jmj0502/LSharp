using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalFunctions
{
    public class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var now = DateTime.Now;
            return now.Millisecond / 1000.0;
        }

        public override string ToString()
        {
            return "<native function clock>";
        }
    }
}
