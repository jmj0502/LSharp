using LSharp.Interpreter;
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
            moduleBody.Define("unwrapOrElse", new UnwrapOrElse());
            return moduleBody;
        }
    }

    class UnwrapOrElse : ICallable
    {
        public int Arity()
        {
            throw new NotImplementedException();
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var resultObj = (Interpreter.Result)arguments[0];
            var fun = (LSFunction)arguments[1];
            if (resultObj.isOk()) return resultObj.Value;
            var args = new List<object> { resultObj };
            return fun.Call(interpreter, args);
        }
    }
}
