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
            moduleBody.Define("createFile", new CreateFile());
            moduleBody.Define("writeFile", new WriteFile());
            moduleBody.Define("fileExists", new FileExists());
            moduleBody.Define("directoryExists", new DirExist());
            moduleBody.Define("createDirectory", new CreateDir());
            moduleBody.Define("moveDirectory", new MoveDir());
            moduleBody.Define("deleteDirectory", new DeleteDir());
            moduleBody.Define("getDirectoryFiles", new GetDirFiles());
            moduleBody.Define("getParentDirectory", new GetParentPath());
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

    public class CreateFile : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            var content = (string)arguments[1];
            try
            {
                System.IO.File.WriteAllText(path, content);
                return true;
            } 
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.createFile>";
        }
    }

    public class WriteFile : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            var content = (string)arguments[1];
            try
            {
                System.IO.File.AppendAllText(path, content);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.writeFile>";
        }
    }

    public class FileExists : ICallable
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
                return System.IO.File.Exists(path);
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.fileExists>";
        }
    }

    public class DirExist : ICallable
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
                return System.IO.Directory.Exists(path);
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.directoryExist>";
        }
    }

    public class CreateDir : ICallable
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
                System.IO.Directory.CreateDirectory(path);
                return true;
            } 
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.createDirectory>";
        }
    }

    public class MoveDir : ICallable
    {
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var originPath = (string)arguments[0];
            var destinationPath = (string)arguments[1];
            try
            {
                System.IO.Directory.Move(originPath, destinationPath);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.moveDirectory>";
        }
    }

    public class DeleteDir : ICallable
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
                System.IO.Directory.Delete(path);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public override string ToString()
        {
            return "<native function io.deleteDirectory>";
        }
    }

    public class GetDirFiles : ICallable
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
                return System.IO.Directory.GetFiles(path);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return "<native function io.getDirectoryFiles>";
        }
    }

    public class GetParentPath : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            var directoryInfo = System.IO.Directory.GetParent(path);
            return new Dictionary<object, object>()
            {
                ["name"] = directoryInfo.FullName,
                ["exists"] = directoryInfo.Exists,
                ["creationDate"] = directoryInfo.CreationTimeUtc,
            };
        }

        public override string ToString()
        {
            return "<native function io.getParentPath>";
        }
    }

}
