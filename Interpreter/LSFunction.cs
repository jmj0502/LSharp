using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSFunction : ICallable
    {
        private readonly Stmt.Function declaration;

        public LSFunction(Stmt.Function declaration)
        {
            this.declaration = declaration;
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var enviroment = new Enviroment.Enviroment(interpreter.Globals);
            for (var i = 0; i < declaration.Parameters.Count; i++)
            {
                enviroment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(declaration.Body, enviroment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name}>";
        }
    }
}
