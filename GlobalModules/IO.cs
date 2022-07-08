using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.GlobalModules
{
    public class IO : IModule
    {
        public Enviroment.Enviroment GenerateBody()
        {
            var moduleBody = new Enviroment.Enviroment();
            moduleBody.Define("readFileAsText", new ReadFileAsText());
            moduleBody.Define("copyFile", new CopyFile());
            moduleBody.Define("moveFile", new MoveFile());
            moduleBody.Define("deleteFile", new DeleteFile());
            return moduleBody;
        }
    }

    public class ReadFileAsText : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            return System.IO.File.ReadAllText(path);
        }

        public override string ToString()
        {
            return "<native function io.readFileAsText>";
        }
    }

    // TODO: ReadFileAsBytes. Pretty useful fuction, but the byte type must be introduced before we can actually have a way to
    // interact with them.

    public class CopyFile : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var originPath = (string)arguments[0];
            var destinationPath = (string)arguments[1];
            try
            {
                System.IO.File.Copy(originPath, destinationPath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.copyFile>";
        }
    }

    public class MoveFile : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var originPath = (string)arguments[0];
            var destinationPath = (string)arguments[1];
            try
            {
                System.IO.File.Move(originPath, destinationPath);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.moveFile>";
        }
    }

    public class DeleteFile : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            try
            {
                System.IO.File.Delete(path);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.deleteFile>";
        }
    }
}
