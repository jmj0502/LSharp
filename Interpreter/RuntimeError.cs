using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Interpreter
{
    public class RuntimeError : ApplicationException
    {
        public readonly Token token;
        public readonly string FileName;

        public RuntimeError(Token token, string message) : base(message)
        {
            this.token = token;
        }

        public RuntimeError(Token token, string message, string fileName) : base(message)
        {
            this.token = token;
            FileName = fileName;
        }
    }
}
