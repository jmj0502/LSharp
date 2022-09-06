using System;
using System.Collections.Generic;
using LSharp.Parser;
using LSharp.Scanner;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.GlobalFunctions;

namespace LSharp.GlobalModules
{
    class JSON : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("parse", new Parse());
            moduleBody.Define("into", new Into());
            return moduleBody;
        }
    }

    public class Parse : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var json = (string)arguments[0];
            try
            {
                var tokens = new JSONScanner(json).ScanTokens();
                var parsedJson =  new JSONParser(tokens).Parse();
                return new ResultOK().Call(interpreter, new List<object> { parsedJson });
            }
            catch(JSONError e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
            }
            catch(JSONScanError e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
            }
        }

        public override string ToString()
        {
            return "<native function json.parse>";
        }
    }

    public class Into : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        private string stringify(object value)
        {
            if (value == null) return "null";
            if (value is true) return "true";
            if (value is false) return "false";
            if (value is string) return $"\"{value}\"";
            if (value is double)
            {
                var text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                    return text;
                }
            }
            if (value is List<object>)
            {
                var sb = new StringBuilder();
                var list = (List<object>)value;
                sb.Append("[");
                for (int i = 0; i < list.Count; i++)
                {
                    sb.Append(stringify(list[i]));
                    if (!Equals(i, list.Count - 1))
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("]");
                return sb.ToString();
            }
            if (value is Dictionary<object, object>)
            {
                var sb = new StringBuilder();
                var dict = (Dictionary<object, object>)value;
                var dictKeys = dict.Keys.ToList();
                var dictValues = dict.Values.ToList();
                sb.Append("{");
                for (var i = 0; i < dictKeys.Count; i++)
                {
                    sb.Append($"{stringify(dictKeys[i])}:{stringify(dictValues[i])}");
                    if (!Equals(i, dictKeys.Count - 1))
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("}");
                return sb.ToString();
            }

            throw new InvalidJSONError("Invalid JSON value provided.");
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var json = arguments[0];
            try
            {
                var jsonString = stringify(json);
                return new ResultOK().Call(interpreter, new List<object> { jsonString });
            }
            catch(InvalidJSONError e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
            }
        }

        public override string ToString()
        {
            return "<native function json.into>";
        }
    }

    class InvalidJSONError : ApplicationException
    {
        public InvalidJSONError(string message) : base(message)
        {
        }
    }
}
