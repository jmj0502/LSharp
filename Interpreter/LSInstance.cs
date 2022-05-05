using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Interpreter
{
    public class LSInstance
    {
        private LSClass lsClass;
        private readonly Dictionary<string, object> fields = new();

        public LSInstance(LSClass lsClass)
        {
            this.lsClass = lsClass;
        }

        public object Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }

            var method = lsClass.FindMethod(name.Lexeme);
            if (method != null) return method;

            throw new RuntimeError(name, 
                $"Undefined property '{name.Lexeme}'.");
        }

        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }

        public override string ToString()
        {
            return $"{lsClass} instance";
        }
    }
}
