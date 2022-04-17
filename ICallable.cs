using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Interpreter;

namespace LSharp
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter.Interpreter interpreter, List<object> arguments);
    }
}
