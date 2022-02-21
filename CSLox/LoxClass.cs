using System.Collections.Generic;

namespace CSLox
{
    public class LoxClass : LoxCallable
    {
        public readonly string Name;
        private readonly Dictionary<string, LoxFunction> _methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            Name = name;
            _methods = methods;
        }

        public int Arity()
        {
            LoxFunction? initializer = FindMethod("init");
            if(initializer == null)
                return 0;
            return initializer.Arity();
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            LoxInstance instance = new LoxInstance(this);
            LoxFunction? initializer = FindMethod("init");
            if(initializer != null)
                initializer.Bind(instance).Call(interpreter, arguments);
            
            return instance;
        }

        public override string ToString()
        {
            return Name;
        }

        public LoxFunction? FindMethod(string name)
        {
            if(_methods.ContainsKey(name))
                return _methods[name];
            
            return null;
        }
    }
}