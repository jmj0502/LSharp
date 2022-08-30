using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class Result : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            return moduleBody;
        }
    }

    class OrElse : ICallable
    {
        public int Arity()
        {
            throw new NotImplementedException();
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            throw new NotImplementedException();
        }
    }
}
