using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BFCompiler
{
    class BFGenerator : IDisposable
    {
        private readonly ILGenerator _generator;
        private readonly BFMemory _memory;
        private readonly Stack<Label> _bracketStack;

        public BFGenerator(ILGenerator ilGen, BFMemory memory)
        {
            _generator = ilGen;
            _memory = memory;
            _bracketStack = new Stack<Label>();
        }

        public void GenerateMoveRight()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldc_I4_1);
            _generator.Emit(OpCodes.Add);
            _generator.Emit(OpCodes.Conv_I2);
            _generator.Emit(OpCodes.Stsfld, _memory.PointerFieldBuilder);
        }

        public void GenerateMoveLeft()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldc_I4_1);
            _generator.Emit(OpCodes.Sub);
            _generator.Emit(OpCodes.Conv_I2);
            _generator.Emit(OpCodes.Stsfld, _memory.PointerFieldBuilder);
        }

        public void GenerateIncrement()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.MemoryFieldBuilder);
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldelema, typeof(byte));
            _generator.Emit(OpCodes.Dup);
            _generator.Emit(OpCodes.Ldobj, typeof(byte));
            _generator.Emit(OpCodes.Ldc_I4_1);
            _generator.Emit(OpCodes.Add);
            _generator.Emit(OpCodes.Conv_U1);
            _generator.Emit(OpCodes.Stobj, typeof(byte));
        }

        public void GenerateDecrement()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.MemoryFieldBuilder);
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldelema, typeof(Byte));
            _generator.Emit(OpCodes.Dup);
            _generator.Emit(OpCodes.Ldobj, typeof(Byte));
            _generator.Emit(OpCodes.Ldc_I4_1);
            _generator.Emit(OpCodes.Sub);
            _generator.Emit(OpCodes.Conv_U1);
            _generator.Emit(OpCodes.Stobj, typeof(Byte));
        }

        public void GenerateWrite()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.MemoryFieldBuilder);
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldelem_U1);
            //As we want the Console.Write(char) overload, we create a new Array containing the Char type.
            _generator.Emit(
                OpCodes.Call,
                typeof(Console).GetMethod(
                    "Write",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(Char) },
                    null
                    )
                );
        }

        public void GenerateRead()
        {
            _generator.Emit(OpCodes.Ldsfld, _memory.MemoryFieldBuilder);
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldelem_U1);
            _generator.Emit(OpCodes.Call, typeof(Console).GetMethod("Read", BindingFlags.Public | BindingFlags.Static));
        }

        public void GenerateOpenLoop()
        {
            var firstLabel = _generator.DefineLabel();
            var secondLabel = _generator.DefineLabel();
            _generator.Emit(OpCodes.Br, secondLabel);    //writes the "jump to the secondLabel"
            _generator.MarkLabel(firstLabel);   //Marks WITH label firstLabel
            _bracketStack.Push(firstLabel);
            _bracketStack.Push(secondLabel);
        }

        public void GenerateCloseLoop()
        {
            Label secondLabel;
            Label firstLabel;
            try
            {
                secondLabel = _bracketStack.Pop();
                firstLabel = _bracketStack.Pop();
            }
            catch (Exception e)
            {
                throw new CompilerException("Unbalanced Brackets. Unexpected \']\'", e);
            }
            _generator.MarkLabel(secondLabel); //Marks WITH label secondLabel
            _generator.Emit(OpCodes.Ldsfld, _memory.MemoryFieldBuilder);
            _generator.Emit(OpCodes.Ldsfld, _memory.PointerFieldBuilder);
            _generator.Emit(OpCodes.Ldelem_U1);
            _generator.Emit(OpCodes.Ldc_I4_0);
            _generator.Emit(OpCodes.Bgt, firstLabel);
        }

        public void Dispose()
        {
            if (_bracketStack.Count > 0)
            {
                throw new CompilerException(
                    string.Format("Unbalanced Brackets. {0} closed brackets ']' expected", (int)(_bracketStack.Count / 2))
                    );
            }
            _generator.Emit(OpCodes.Ret);
        }
    }
}
