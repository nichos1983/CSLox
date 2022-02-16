using System.Collections.Generic;

namespace CSLox
{
    public class LoxClass : LoxCallable
    {
        public readonly string Name;

        public LoxClass(string name)
        {
            Name = name;
        }

        public int Arity()
        {
            return 0;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            return instance;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}