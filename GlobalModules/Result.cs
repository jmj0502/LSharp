using LSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class Result : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("unwrapOrElse", new UnwrapOrElse());
            moduleBody.Define("unwrap", new Unwrap());
            moduleBody.Define("isOk", new IsOk());
            moduleBody.Define("isErr", new IsErr());
            return moduleBody;
        }

        public override string ToString()
        {
            return "<native module Result>";
        }
    }

    class UnwrapOrElse : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var resultObj = (Interpreter.Result)arguments[0];
            if (resultObj.IsHandled()) return null;
            var fun = (LSFunction)arguments[1];
            resultObj.Handle();
            if (resultObj.IsOk()) return resultObj.Value;
            var args = new List<object> { resultObj.ErrorMessage };
            return fun.Call(interpreter, args);
        }

        public override string ToString()
        {
            return "<native function Result.unwrapOrElse>";
        }
    }

    class Unwrap : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var resultObj = (Interpreter.Result)arguments[0];
            if (resultObj.IsHandled()) return null;
            if (resultObj.IsOk()) return resultObj.Value;
            throw new ErrorResult(resultObj.ErrorMessage);
        }

        public override string ToString()
        {
            return "<native function Result.unwrap>";
        }
    }

    class IsOk : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var resultObj = (Interpreter.Result)arguments[0];
            return resultObj.IsOk();
        }

        public override string ToString()
        {
            return "<native function Result.isOK>";
        }
    }

    class IsErr : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var resultObj = (Interpreter.Result)arguments[0];
            return resultObj.IsError();
        }

        public override string ToString()
        {
            return "<native function Result.isErr>";
        }
    }

    public class ErrorResult: ApplicationException
    {
        public ErrorResult(string message) : base(message)
        {
        }
    }
}
