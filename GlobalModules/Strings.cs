using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class Strings
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("split", new Split());
            moduleBody.Define("reverse", new Reverse());
            moduleBody.Define("substring", new Substring());
            return moduleBody;
        }
    }

    public class Split : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = ((string)arguments[0]);
            var separator = ((string)arguments[1]);
            var splittedStr = str.Split(separator);
            return splittedStr.ToList<object>();
        }

        public override string ToString()
        {
            return "<native function string.split>";
        }
    }

    public class Reverse : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var strChars = str.ToCharArray();
            Array.Reverse(strChars);
            return new string(strChars);
        }

        public override string ToString()
        {
            return "<native function string.reverse>";
        }
    }

    public class Substring : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var indexValue = (double)arguments[1];
            var index = (int)indexValue;
            return str.Substring(index);
        }

        public override string ToString()
        {
            return "native function string.substring";
        }
    }
}
