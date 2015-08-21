using System.Reflection;
using System.Reflection.Emit;

namespace BFCompiler
{
    static class BFCompiler
    {
        internal static void Compile(string assemblyName, string outputFileName, string sourceCode)
        {
            var assembly = new BFAssembly(assemblyName, outputFileName);

            var memory = new BFMemory(assembly.MainTypeBuilder);

            MethodBuilder mainMethod = assembly.MainTypeBuilder.DefineMethod("Execute", MethodAttributes.Public | MethodAttributes.Static);

            GenerateMainMethod(mainMethod, memory, sourceCode);
            assembly.SetEntryPoint(mainMethod);
            assembly.BuildToFile();
        }

        private static void GenerateMainMethod(MethodBuilder mainMethod, BFMemory memory, string sourceCode)
        {
            ILGenerator ilGen = mainMethod.GetILGenerator();
            var charray = sourceCode
                            .Replace(" ", string.Empty)
                            .Replace("\r", string.Empty)
                            .Replace("\n", string.Empty)
                            .Replace("\t", string.Empty)
                            .ToCharArray();
            using (var generator = new BFGenerator(ilGen, memory))
            {
                foreach (var t in charray)
                {
                    switch (t)
                    {
                        case '>':
                            generator.GenerateMoveRight();
                            break;
                        case '<':
                            generator.GenerateMoveLeft();
                            break;
                        case '+':
                            generator.GenerateIncrement();
                            break;
                        case '-':
                            generator.GenerateDecrement();
                            break;
                        case '.':
                            generator.GenerateWrite();
                            break;
                        case ',':
                            generator.GenerateRead();
                            break;
                        case '[':
                            generator.GenerateOpenLoop();
                            break;
                        case ']':
                            generator.GenerateCloseLoop();
                            break;
                    }
                }
            }
        }
    }
}
