﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSClass : ICallable
    {
        public readonly string Name;
        public readonly Dictionary<string, LSFunction> methods = new();

        public LSClass(string name, Dictionary<string, LSFunction> methods)
        {
            Name = name;
            this.methods = methods;
        }

        public object FindMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }

            return null;
        }

        public object Call(Interpreter interpreter,
            List<object> arguments)
        {
            var instance = new LSInstance(this);
            return instance;
        }

        public int Arity()
        {
            return 0;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
