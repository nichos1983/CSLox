using System.Collections.Generic;

namespace CSLox
{
    interface LoxCallable
    {
        int Arity();
        object? Call(Interpreter interpreter, List<object?> arguments);
    }

    // C# can not define an instance of interface in Interpreter()
    class NativeClockFunction : LoxCallable
    {
        public int Arity()
        {
            return 0;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            return (double)System.Environment.TickCount / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn>";
        }
    }
}