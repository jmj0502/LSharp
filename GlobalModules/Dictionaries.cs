using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class Dictionaries : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("keys", new Keys());
            return moduleBody;
        }
    }

    public class Keys : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            return dict.Keys.ToList();
        }

        public override string ToString()
        {
            return "<native function dictionary.keys>";
        }
    }
}
