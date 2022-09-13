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
        private bool isHandled = false;

        public Result(object value)
        {
            Value = value;
        }

        public Result(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool IsOk()
        {
            return Value != null;
        }

        public bool IsError()
        {
            return ErrorMessage != null;
        }

        public bool IsHandled()
        {
            return isHandled;
        }

        public void Handle()
        {
            if (isHandled) return;
            isHandled = true;
        }
    }
}
