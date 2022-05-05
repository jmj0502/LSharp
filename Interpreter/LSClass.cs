using System;
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

        public LSFunction FindMethod(string name)
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
            var initializer = FindMethod("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }
            return instance;
        }

        public int Arity()
        {
            var initializer = FindMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
