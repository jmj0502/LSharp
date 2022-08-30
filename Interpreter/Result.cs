using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Result
    {
        public object Value;
        public string ErrorMessage;

        public Result(object value)
        {
            Value = value;
        }

        public Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool isOk()
        {
            return Value != null;
        }

        public bool isError()
        {
            return ErrorMessage != null;
        }
    }
}
