using System;
using System.Collections.Generic;
using LSharp.Parser;
using LSharp.Scanner;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class JSON : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("parse", new Parse());
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
            var tokens = new JSONScanner(json).ScanTokens();
            return new JSONParser(tokens).Parse();
        }

        public override string ToString()
        {
            return "<native function json.parse>";
        }
    }
}
