using System.Reflection;
using System.Reflection.Emit;

namespace BFCompiler
{
    class BFMemory
    {
        public FieldBuilder PointerFieldBuilder { get; private set; }
        public FieldBuilder MemoryFieldBuilder { get; private set; }

        public BFMemory(TypeBuilder typeBuilder)
        {
            PointerFieldBuilder = typeBuilder.DefineField("pointer", typeof(short), FieldAttributes.Static | FieldAttributes.Private);
            MemoryFieldBuilder = typeBuilder.DefineField("memory", typeof(byte[]), FieldAttributes.Static | FieldAttributes.Private);

            var staticConstructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Static,CallingConventions.Standard, null);
            GenerateStaticConstructorBody(staticConstructorBuilder.GetILGenerator());
        }

        private void GenerateStaticConstructorBody(ILGenerator ilGen)
        {
            // construct the memory as byte[short.MaxValue]
            ilGen.Emit(OpCodes.Ldc_I4, 0x777f);
            ilGen.Emit(OpCodes.Newarr, typeof(byte));
            ilGen.Emit(OpCodes.Stsfld, MemoryFieldBuilder);

            // Construct the pointer as short = 0
            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Stsfld, PointerFieldBuilder);

            //Is this required? Technically, it's not. But in order to create verifiable assemblies, you have to emit it.
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
