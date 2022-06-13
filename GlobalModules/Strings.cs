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
            moduleBody.Define("trim", new Trim());
            moduleBody.Define("len", new Len());
            moduleBody.Define("toUpper", new ToUpper());
            moduleBody.Define("toLower", new ToLower());
            moduleBody.Define("startsWith", new StartsWith());
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
            return "<native function string.substring>";
        }
    }

    public class Trim : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            return str.Trim();
        }

        public override string ToString()
        {
            return "<native function string.trim>";
        }
    }

    public class Len : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            return str.Length;
        }

        public override string ToString()
        {
            return "<native function string.len>";
        }
    }

    public class ToUpper : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            return str.ToUpper();
        }

        public override string ToString()
        {
            return "<native function string.toUpper>";
        }
    }

    public class ToLower : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            return str.ToLower();
        }

        public override string ToString()
        {
            return "<native function string.ToLower>";
        }
    }

    public class StartsWith : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var startingChar = (string)arguments[1];
            return str.StartsWith(startingChar[0]);
        }

        public override string ToString()
        {
            return "<native function string.startsWith>";
        }
    }
}
