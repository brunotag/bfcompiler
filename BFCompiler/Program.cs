using System;
using System.IO;

namespace BFCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Wrong usage. Type <<bfcompiler /?>> for help");
                return;
            }

            var sourceFileName = args[0];
            var outputName = args[1];

            string sourceCode = null;

            #region sourceCode = File.ReadAllText(sourceFileName); with exception handling
            try
            {
                sourceCode = File.ReadAllText(sourceFileName);
            }
            catch (System.ArgumentException)
            {
                Console.WriteLine("The filename is invalid or was not provided");
                return;
            }
            catch (System.IO.PathTooLongException)
            {
                Console.WriteLine("The provided path is too long");
                return;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("A directory in the path was not found");
                return;
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Can't find a file with the provided name");
                return;
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Provided path is in an invalid format");
                return;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Apparently you are not authorized to access the file with the provided name");
                return;
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("Apparently you are not authorized to access the file with the provided name");
                return;
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Unspecified IO Error. See details:");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("Unspecified Error. See details:");
                throw;
            }
            #endregion

            try
            {
                BFCompiler.Compile(outputName, outputName + ".exe", sourceCode);
            }
            catch (CompilerException ex)
            {
                Console.WriteLine("Compiler error:");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("{0} compiled to {1}.exe", sourceFileName, outputName);
        }
    }
}
