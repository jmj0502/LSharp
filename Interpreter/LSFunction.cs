using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSFunction : ICallable
    {
        private readonly Expression.Function declaration;
        private readonly string name;
        private readonly Enviroment.Enviroment closure;
        private readonly bool isInitializer;

        public LSFunction(Expression.Function declaration, string name, Enviroment.Enviroment closure, 
            bool isInitializer)
        {
            this.declaration = declaration;
            this.name = name;
            this.closure = closure;
            this.isInitializer = isInitializer;
        }

        public LSFunction Bind(LSInstance instance)
        {
            var enviroment = new Enviroment.Enviroment(closure);
            enviroment.Define("this", instance);
            return new LSFunction(declaration, "this", enviroment, 
                isInitializer);
        }

        public int Arity()
        {
            return declaration.Parameters.Count;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var enviroment = new Enviroment.Enviroment(closure);
            for (var i = 0; i < declaration.Parameters.Count; i++)
            {
                enviroment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, enviroment);
            }
            catch (Return returnValue)
            {
                if (isInitializer) return closure.GetAt(0, "this");

                return returnValue.Value;
            }

            if (isInitializer) return closure.GetAt(0, "this");
            return null;
        }

        public override string ToString()
        {
            return $"<fn {name}>";
        }
    }
}
