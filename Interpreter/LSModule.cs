using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Interpreter
{
    public class LSModule
    {
        private readonly Enviroment.Enviroment body = new();
        private readonly string name;

        public LSModule(Enviroment.Enviroment body, string name)
        {
            this.body = body;
            this.name = name;
        }

        public object Get(Token name)
        {

            var stmt = body.Get(name);

            if (stmt == null)
                throw new RuntimeError(name, 
                    $"Undefined property '{name.Lexeme}'.");

            return stmt;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
