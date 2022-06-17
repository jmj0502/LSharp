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
            moduleBody.Define("slice", new Slice());
            moduleBody.Define("endsWith", new EndsWith());
            moduleBody.Define("contains", new Contains());
            moduleBody.Define("indexOf", new IndexOf());
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
            var leftValue = (double)arguments[1];
            var leftIndex = (int)leftValue;
            return str.Substring(leftIndex);
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
            var str1 = (string)arguments[0];
            var str2 = (string)arguments[1];
            return str1.StartsWith(str2);
        }

        public override string ToString()
        {
            return "<native function string.startsWith>";
        }
    }

    public class Slice : ICallable
    {
        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {

            var str = (string)arguments[0];
            var leftValue = (double)arguments[1];
            var leftIndex = (int)leftValue;
            var rightValue = (double)arguments[2];
            var rightIndex = (int)rightValue;
            if (rightIndex < leftIndex)
            {
                if (leftIndex >= str.Length) leftIndex = str.Length;
                if (rightIndex < 0) rightIndex = str.Length + rightIndex;
                if (leftIndex < 0) leftIndex = str.Length - 1;
                return str.Substring(rightIndex, leftIndex - rightIndex);
            }
            if (rightIndex >= str.Length) rightIndex = str.Length;
            if (leftIndex < 0) leftIndex = str.Length + rightIndex;
            if (rightIndex < 0) rightIndex = str.Length - 1;
            return str.Substring(leftIndex, rightIndex - leftIndex);
        }

        public override string ToString()
        {
            return "<native function string.slice>";
        }
    }

    public class EndsWith : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var str2 = (string)arguments[1];
            return str.EndsWith(str2);
        }

        public override string ToString()
        {
            return "<native function string.endsWith>";
        }
    }

    public class Contains : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var str2 = (string)arguments[1];
            return str.Contains(str2);
        }

        public override string ToString()
        {
            return "<native function string.contains>";
        }
    }

    public class IndexOf : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var str2 = (string)arguments[1];
            return str.IndexOf(str2);
        }

        public override string ToString()
        {
            return "<native function string.indexOf>";
        }
    }
}
