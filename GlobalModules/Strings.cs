﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class Strings
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("split", new Split());
            return moduleBody;
        }
    }

    public class Split : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var str = ((string)arguments[0]);
            var separator = ((string)arguments[1]);
            var splittedStr = str.Split(separator);
            return splittedStr.ToList<object>();
        }

        public override string ToString()
        {
            return "<native function split>";
        }
    }
}