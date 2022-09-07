using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class Dictionaries : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("keys", new Keys());
            moduleBody.Define("values", new Values());
            moduleBody.Define("containsKey", new ContainsKey());
            moduleBody.Define("delete", new Delete());
            moduleBody.Define("clear", new Clear());
            moduleBody.Define("toList", new ToList());
            moduleBody.Define("at", new At("dictionary"));
            return moduleBody;
        }

        public override string ToString()
        {
            return "<native module Dictionary>";
        }
    }

    public class Keys : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            return dict.Keys.ToList();
        }

        public override string ToString()
        {
            return "<native function dictionary.keys>";
        }
    }

    public class Values : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            return dict.Values.ToList();
        }

        public override string ToString()
        {
            return "<native function dictionary.values>";
        }
    }

    public class ContainsKey : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            var key = arguments[1];
            return dict.ContainsKey(key);
        }

        public override string ToString()
        {
            return "<native function dictionary.containsKey>";
        }
    }

    public class Delete : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            var key = arguments[1];
            return dict.Remove(key);
        }

        public override string ToString()
        {
            return "<native function dictionary.delete>";
        }
    }

    public class Clear : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            dict.Clear();
            return null;
        }

        public override string ToString()
        {
            return "<native function dictionary.clear>";
        }
    }

    public class ToList : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var dict = (Dictionary<object, object>)arguments[0];
            var keyValuePairs = dict.ToList();
            var entryList = new List<object>();
            foreach (var (key, value) in keyValuePairs)
            {
                entryList.Add(new List<object> { key, value });
            }
            return entryList;
        }

        public override string ToString()
        {
            return "<native function dictionary.toList>";
        }
    }

}
