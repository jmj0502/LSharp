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
        public readonly LSClass Superclass;
        public readonly Dictionary<string, LSFunction> methods = new();

        public LSClass(string name, Dictionary<string, LSFunction> methods, LSClass superclass)
        {
            Name = name;
            this.methods = methods;
            Superclass = superclass;
        }

        /// <summary>
        /// Checks if a method exist in a class. If so, the method is resoved; otherwise we return null.
        /// </summary>
        /// <param name="name">The name of the method to be resolved from the class.</param>
        public LSFunction FindMethod(string name)
        {
            if (methods.ContainsKey(name))
            {
                return methods[name];
            }

            if (Superclass != null)
            {
                return Superclass.FindMethod(name);
            }

            return null;
        }

        /// <summary>
        /// Generates a new lox instance of the current class. To do so, checks if there's a init method defined; in case it is, the 
        /// initializer will bind the instance to the current interpreter and proceed to forward the provided arguments. Otherwise
        /// returns an instance with no define state (other than it's methods).
        /// </summary>
        /// <param name="interpreter">The LS interpreter.</param>
        /// <param name="arguments">The arguments that must be provided to the 'init' method of the class.</param>
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

        /// <summary>
        /// Check if the 'init' method of the class has any arity and proceeds to forward it. In case the class doesn't have an init method
        /// it just returns 0.
        /// </summary>
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
