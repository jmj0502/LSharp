using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Interpreter;
using LSharp.Tokens;

namespace LSharp.Enviroment
{
    public class Enviroment
    {
        public readonly Dictionary<string, object> values = new();

        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                object value;
                var result = values.TryGetValue(name.Lexeme, out value);
                if (result) return value;
            }

            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}."); 
        }
    }
}
