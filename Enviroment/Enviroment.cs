using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Interpreter;
using LSharp.Tokens;

namespace LSharp.Enviroment
{
    public class Enviroment
    {
        public readonly Enviroment Enclosing;
        public readonly Dictionary<string, object> values = new();

        public Enviroment()
        {
            Enclosing = null;
        }

        public Enviroment(Enviroment enclosing)
        {
            Enclosing = enclosing;
        }

        /// <summary>
        /// Defines a new key in the values dictionary intended to represent the scope of the program.
        /// If a value is not provided, it'll turn the value into null (nil in lox semantics).
        /// </summary>
        /// <param name="name">Variable name. We'll be used as the key for the provided value.</param>
        /// <param name="value">Value tied to the variable. Nil, if no value is provided.</param>
        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        /// <summary>
        /// Returns a value from the dictionary that works as the program enviroment. If the variable 
        /// doesn't have an assigned value, it retuns null. If the variable is not defined, an runtime error
        /// is raised.
        /// </summary>
        /// <param name="name">The name of variable to be accessed.</param>
        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                object value;
                var result = values.TryGetValue(name.Lexeme, out value);
                if (result) return value;
            }

            //If the variable we're loking for isn't found on this enviroment, the
            //method proceed to check the enclosing enviroment recursively until we indeed find something.
            //If the variable isn't found in any scope, we proceed to raise a runtime exception.
            if (Enclosing != null) return Enclosing.Get(name);

            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}."); 
        }

        /// <summary>
        /// Updates the value of a variable contained in the enviroment. If the variable is not found, we raise an undefined 
        /// variable error.
        /// </summary>
        /// <param name="name">The token that represents the identifier which holds the variable name.</param>
        /// <param name="value">The value that will be tied up to the reasigned variable.</param>
        public void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
