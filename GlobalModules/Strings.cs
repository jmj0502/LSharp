using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class Strings : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("split", new Split());
            moduleBody.Define("reverse", new Reverse("string")); // Should also apply o list.
            moduleBody.Define("substring", new Substring());
            moduleBody.Define("trim", new Trim());
            moduleBody.Define("len", new Len("string")); //Should also apply to lists.
            moduleBody.Define("toUpper", new ToUpper());
            moduleBody.Define("toLower", new ToLower());
            moduleBody.Define("startsWith", new StartsWith());
            moduleBody.Define("slice", new Slice("string")); //Should also apply to lists.
            moduleBody.Define("endsWith", new EndsWith());
            moduleBody.Define("contains", new Contains("string")); //Should also apply to lists.
            moduleBody.Define("indexOf", new IndexOf("string")); //Should also apply to lists.
            moduleBody.Define("at", new At("string"));
            moduleBody.Define("match", new Match());
            moduleBody.Define("replace", new Replace());
            return moduleBody;
        }

        public override string ToString()
        {
            return "<native module String>";
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
            var str = (string)arguments[0];
            var separator = (string)arguments[1];
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
        private readonly string moduleName;

        public Reverse(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string)
            {
                var str = (string)arguments[0];
                var strChars = str.ToCharArray();
                Array.Reverse(strChars);
                return new string(strChars);
            }
            var list = (List<object>)arguments[0];
            list.Reverse();
            return list;
        }

        public override string ToString()
        {
            return $"<native function {moduleName}.reverse>";
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
        private readonly string moduleName;

        public Len(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string)
            {
                var str = (string)arguments[0];
                return (double)str.Length;
            }
            var list = (List<object>)arguments[0];
            return (double)list.Count;
        }

        public override string ToString()
        {
            return $"<native function {moduleName}.len>";
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
        private readonly string moduleName;

        public Slice(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var leftIndex = (int)((double)arguments[1]);
            var rightIndex = (int)((double)arguments[2]);
                
            if (arguments[0] is string)
            {
                var str = (string)arguments[0];
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

            var list = (List<object>)arguments[0];
            return list.GetRange(leftIndex, rightIndex);
        }

        public override string ToString()
        {
            return $"<native function {moduleName}.slice>";
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
        private readonly string moduleName;

        public Contains(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string)
            {
                var str = (string)arguments[0];
                var str2 = (string)arguments[1];
                return str.Contains(str2);
            }

            var list = (List<object>)arguments[0];
            return list.Contains(arguments[1]);
        }

        public override string ToString()
        {
            return $"<native function {moduleName}.contains>";
        }
    }

    public class IndexOf : ICallable
    {

        private readonly string moduleName;

        public IndexOf(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            if (arguments[0] is string)
            {
                var str = (string)arguments[0];
                var str2 = (string)arguments[1];
                return str.IndexOf(str2);
            }

            var list = (List<object>)arguments[0];
            var element = arguments[1];
            return list.IndexOf(element);
        }

        public override string ToString()
        {
            return $"<native function {moduleName}.indexOf>";
        }
    }

    public class At : ICallable
    {
        private readonly string moduleName;

        public At(string moduleName)
        {
            this.moduleName = moduleName;
        }

        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var obj = arguments[0];
            if (obj is string)
            {
                var strIndex = (int)((double)arguments[1]);
                var str = (string)arguments[0];
                if (strIndex < 0 || strIndex >= str.Length) throw new StringError("String index out of range.");
                return str[strIndex].ToString();
            }
            if (obj is List<object>)
            {
                var listIndex = (int)((double)arguments[1]);
                var list = (List<object>)arguments[1];
                if (listIndex < 0 || listIndex >= list.Count) throw new ListError("List index out of range");
            }
            var dict = (Dictionary<object, object>)obj;
            var key = (string)arguments[1];
            return dict[key];
        }

        public override string ToString()
        {
            return "<native function string.at>";
        }
    }

    public class Match : ICallable
    {
        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var pattern = (string)arguments[1];
            var options = (Dictionary<object, object>)arguments[2];
            Regex regex;
            if (options.ContainsKey("insensitive"))
            {
                regex = new Regex($"{pattern}", RegexOptions.IgnoreCase);
            }
            else
            {
                regex = new Regex($"{pattern}");
            }
            var matches = regex.Match(str);
            var results = new List<object>();
            foreach (var match in matches.Groups)
            {
                results.Add(match.ToString());
            }
            return results;
        }

        public override string ToString()
        {
            return "<native function string.match>";
        }
    }

    public class Replace : ICallable
    {
        public int Arity()
        {
            return 4;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = (string)arguments[0];
            var pattern = (string)arguments[1];
            var replacement = (string)arguments[2];
            var options = (Dictionary<object, object>)arguments[3];
            Regex regex;
            if (options.ContainsKey("insensitive"))
            {
                regex = new Regex($"{pattern}", RegexOptions.IgnoreCase);
            }
            else
            {
                regex = new Regex($"{pattern}");
            }
            return regex.Replace(str, replacement);
        }

        public override string ToString()
        {
            return "<native function string.replace>";
        }
    }

    class StringError : ApplicationException
    {
        public StringError(string message) : base(message)
        {
        }
    }
}
