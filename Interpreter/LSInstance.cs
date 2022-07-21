using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Interpreter
{
    public class LSInstance
    {
        private LSClass lsClass;
        private readonly Dictionary<string, object> fields = new();

        public LSInstance(LSClass lsClass)
        {
            this.lsClass = lsClass;
        }

        /// <summary>
        /// Validates is a field with the provided name is contained on the fields of the instance. If so, returns
        /// true, otherwise, returns false.
        /// </summary>
        /// <param name="name">The token that represents the name of the field.</param>
        public bool HasField(Token name)
        {
            return fields.ContainsKey(name.Lexeme);
        }

        /// <summary>
        /// Performs a look up on the instance fields based on the identifier called on a get expression. 
        /// If the field is resolved as property, then its value is returned; otherwise it proceeds to check 
        /// if there's a method in the class with the given identifier, if so, proceeds to bind the method to the instance
        /// and return it, otherwise a runtime error is thrown, since Lox doesn't allow call on non-existing members.
        /// NOTE: based on this configuration, fields shadow methods.
        /// </summary>
        /// <param name="name">The token that holds the name of the property/method to be resolved.</param>
        public object Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }

            var method = lsClass.FindMethod(name.Lexeme);
            if (method != null) return method.Bind(this);

            throw new RuntimeError(name, 
                $"Undefined property '{name.Lexeme}'.");
        }

        /// <summary>
        /// Creates a new property on the instance (in case the property didn't exist) or updates an existing one.
        /// </summary>
        /// <param name="name">The token that holds the name of the property to be created/updated.</param>
        /// <param name="value">The value that such property will hold.</param>
        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }

        public override string ToString()
        {
            return $"{lsClass} instance";
        }
    }
}
