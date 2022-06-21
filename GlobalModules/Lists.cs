using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    class Lists : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("len", new Len("list"));
            moduleBody.Define("reverse", new Reverse("list"));
            moduleBody.Define("slice", new Slice("list"));
            moduleBody.Define("contains", new Contains("list"));
            moduleBody.Define("indexOf", new IndexOf("list"));
            return moduleBody;
        }
    }
}
