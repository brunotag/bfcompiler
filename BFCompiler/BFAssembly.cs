using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BFCompiler
{
    class BFAssembly
    {
        private readonly AssemblyBuilder _asmBuilder;
        private readonly TypeBuilder _typeBuilder;
        private readonly string _outputFileName;
        private MethodBuilder _methodBuilder;

        public TypeBuilder MainTypeBuilder
        {
            get { return _typeBuilder; }
        }

        public BFAssembly(string assemblyName, string outputFileName)
        {
            _asmBuilder = DefineAssemblyBuilder(assemblyName);
            _typeBuilder = DefineMainTypeBuilder(_asmBuilder, assemblyName, outputFileName);
            _outputFileName = outputFileName;
        }

        public void SetEntryPoint(MethodBuilder mainMethodBuilder)
        {
            _methodBuilder = mainMethodBuilder;
        }

        public void BuildToFile()
        {
            _typeBuilder.CreateType();
            _asmBuilder.SetEntryPoint(_methodBuilder);
            _asmBuilder.Save(_outputFileName);
        }

        private AssemblyBuilder DefineAssemblyBuilder(string assemblyName)
        {
            return AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(assemblyName),
                AssemblyBuilderAccess.Save
                );
        }

        private TypeBuilder DefineMainTypeBuilder(AssemblyBuilder asm, string assemblyName, string outputFileName)
        {
            ModuleBuilder mod = asm.DefineDynamicModule(assemblyName, outputFileName);
            return mod.DefineType(
                assemblyName + ".Program",
                TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract //static type = abstract sealed
                );
        }
    }
}
