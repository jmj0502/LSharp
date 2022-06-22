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
            moduleBody.Define("join", new Join());
            moduleBody.Define("add", new Add());
            moduleBody.Define("prepend", new Prepend());
            moduleBody.Define("insert", new Insert());
            moduleBody.Define("concat", new Concat());
            return moduleBody;
        }
    }

    public class Join : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var separatorStr = (string)arguments[1];
            return string.Join(separatorStr, list);
        }

        public override string ToString()
        {
            return "<native function list.join>";
        }
    }

    public class Add : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var newItem = arguments[1];
            list.Add(newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.add>";
        }
    }

    public class Prepend : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var newItem = arguments[1];
            list.Insert(0, newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.prepend>";
        }
    }

    public class Insert : ICallable
    {
        public int Arity()
        {
            return 3;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var list = (List<object>)arguments[0];
            var indexValue = (double)arguments[1];
            var index = (int)indexValue;
            var newItem = arguments[2];
            list.Insert(index, newItem);
            return (double)list.Count;
        }

        public override string ToString()
        {
            return "<native function list.insert>";
        }
    }

    public class Concat : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var lhsList = (List<object>)arguments[0];
            var rhsList = (List<object>)arguments[1];
            return lhsList.Concat(rhsList).ToList();
        }

        public override string ToString()
        {
            return "<native function list.concat>";
        }
    }
}
