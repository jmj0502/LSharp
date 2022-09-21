using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.GlobalFunctions;

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
            moduleBody.Define("readLine", new ReadLine());
            return moduleBody;
        }

        public override string ToString()
        {
            return "<native module IO>";
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
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var originPath = (string)arguments[0];
            var destinationPath = (string)arguments[1];
            ICallable result;
            try
            {
                System.IO.File.Copy(originPath, destinationPath);
                result = new ResultOK();
                return result.Call(interpreter, new List<object>());
            }
            catch (Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var originPath = (string)arguments[0];
            var destinationPath = (string)arguments[1];
            ICallable result;
            try
            {
                System.IO.File.Move(originPath, destinationPath);
                result = new ResultOK();
                return result.Call(interpreter, new List<object>());
            }
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            ICallable result;
            try
            {
                System.IO.File.Delete(path);
                result = new ResultOK();
                return result.Call(interpreter, new List<object>());
            }
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            var content = (string)arguments[1];
            ICallable result;
            try
            {
                System.IO.File.WriteAllText(path, content);
                result = new ResultOK();
                return result.Call(interpreter, new List<object>());
            } 
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            return 2;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var path = (string)arguments[0];
            var content = (string)arguments[1];
            ICallable result;
            try
            {
                System.IO.File.AppendAllText(path, content);
                result = new ResultOK();
                return result.Call(interpreter, new List<object>());
            }
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            ICallable result;
            try
            {
                var fileExists = System.IO.File.Exists(path);
                result = new ResultOK();
                return result.Call(interpreter, new List<object> { fileExists });
            }
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            ICallable result;
            try
            {
                var dirExist = System.IO.Directory.Exists(path);
                result = new ResultOK();
                return result.Call(interpreter, new List<object> { dirExist });
            }
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
            ICallable result;
            try
            {
                var dirInfo = System.IO.Directory.CreateDirectory(path);
                var parsedDirInfo = FileAndDirInfoParser.ParseDirInfo(dirInfo);
                result = new ResultOK();
                return result.Call(interpreter, new List<object> { parsedDirInfo });
            } 
            catch(Exception e)
            {
                result = new ResultError();
                return result.Call(interpreter, new List<object> { e.Message });
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
                return new ResultOK().Call(interpreter, new List<object>());
            }
            catch(Exception e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
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
                return new ResultOK().Call(interpreter, new List<object>());
            }
            catch(Exception e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
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
                var dirFiles = System.IO.Directory.GetFiles(path);
                var filePaths = new List<object>();
                foreach (string dirFile in dirFiles)
                {
                    filePaths.Add(dirFile);
                }
                return new ResultOK().Call(interpreter, filePaths);
            }
            catch(Exception e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
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
            try
            {
                var directoryInfo = System.IO.Directory.GetParent(path);
                var parsedDirInfo = FileAndDirInfoParser.ParseDirInfo(directoryInfo);
                return new ResultOK().Call(interpreter, new List<object> { parsedDirInfo });
            }
            catch (Exception e)
            {
                return new ResultError().Call(interpreter, new List<object> { e.Message });
            }
        }

        public override string ToString()
        {
            return "<native function io.getParentPath>";
        }
    }

    public class ReadLine : ICallable
    {
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter.Interpreter interpreter, List<object> arguments)
        {
            var prompt = (string)arguments[0];
            Console.WriteLine(prompt);
            var result = Console.ReadLine();
            if (result == null) return "";
            return result;
        }

        public override string ToString()
        {
            return "<native function io.readLine>";
        }
    }

    class FileAndDirInfoParser
    {
        public static Dictionary<object, object> ParseFileInfo(System.IO.FileInfo fileInfo)
        {
            return new Dictionary<object, object>
            {
                ["extension"] = fileInfo.Extension,
                ["name"] = fileInfo.Name,
                ["path"] = fileInfo.DirectoryName,
                ["creationDate"] = fileInfo.CreationTimeUtc,
                ["exists"] = fileInfo.Exists,
                ["lastUpdate"] = fileInfo.LastWriteTimeUtc
            };
        } 

        public static Dictionary<object, object> ParseDirInfo(System.IO.DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) return null;
            var files = directoryInfo.GetFiles();
            var parsedFilesInfo = new List<object>();
            foreach (var file in files)
            {
                parsedFilesInfo.Add(ParseFileInfo(file));
            }

            var subDirectories = directoryInfo.GetDirectories();
            var parsedDirsInfo = new List<object>();
            foreach (var dir in subDirectories)
            {
                parsedDirsInfo.Add(ParseDirInfo(dir));
            }

            return new Dictionary<object, object>()
            {
                ["directory"] = directoryInfo.FullName,
                ["exists"] = directoryInfo.Exists,
                ["creationDate"] = directoryInfo.CreationTimeUtc,
                ["files"] = parsedFilesInfo,
                ["subdirectories"] = parsedDirsInfo
            };
        }
            
    }

}
