using System;
using System.Collections.Generic;
using LSharp.Interpreter;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalFunctions
{

    public class ResultOK : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            if (arguments.Count > 0)
            {
                return new Result(arguments[0]);
            }
            return new Result(new List<object>());
        }

        public override string ToString()
        {
            return "<Result.OK>";
        }
    }

    public class ResultError : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var message = (string)arguments[0];
            return new Result(message);
        }

        public override string ToString()
        {
            return "<Result.Err>";
        }
    }
}
