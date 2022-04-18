using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Return : ApplicationException
    {
        public object Value;

        public Return(object value)
        {
            Value = value;
        }
    }
}
