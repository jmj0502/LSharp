using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class ErrorVariant : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("new", new New());
            return moduleBody;
        }
    }

    class New : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var message = (string)arguments[0];
            return new ApplicationException(message);
        }

        public override string ToString()
        {
            return "<native function Error.new>";
        }
    }
}
