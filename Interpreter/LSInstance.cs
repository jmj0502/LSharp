using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class LSInstance
    {
        private LSClass lsClass;

        public LSInstance(LSClass lsClass)
        {
            this.lsClass = lsClass;
        }

        public override string ToString()
        {
            return $"{lsClass} instance";
        }
    }
}
