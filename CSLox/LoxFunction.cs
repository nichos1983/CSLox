namespace CSLox
{
    // 1. Stmt.Function is a compile-time syntax node of function statement definition.
    // 2. Expr.Function is a compile-time syntax node of function expression, Added in Challenges 10.2.
    //    It's separated from Stmt.Function to be reusable between named function and anonymous function.
    // 3. Expr.Call is compile-time syntax node of function call.
    // 4. LoxFunction is run-time wrapper of Stmt.Function.
    public class LoxFunction : LoxCallable
    {
        private readonly string? _name;
        private readonly Expr.Function _declaration;
        private readonly Environment _closure;
        
        public LoxFunction(string? name, Expr.Function declaration, Environment closure)
        {
            _name = name;
            _declaration = declaration;
            _closure = closure;
        }

        public int Arity()
        {
            return _declaration.Params.Count;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new Environment(_closure);
            for(int i = 0; i < _declaration.Params.Count; i++)
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            
            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch(Return returnValue)
            {
                // Fib example test shows exception is running deadly slow.
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(_name))
                return "<fn>";
            return $"<fn {_name}>";
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_name, _declaration, environment);
        }
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