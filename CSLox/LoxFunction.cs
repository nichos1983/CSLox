namespace CSLox
{
    // Stmt.Function is compile-time syntax node of function declaration/definition.
    // Expr.Call is compile-time syntax node of function call.
    // LoxFunction is run-time wrapper of Stmt.Function.
    class LoxFunction : LoxCallable
    {
        private readonly Stmt.Function _declaration;
        
        public LoxFunction(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        public int Arity()
        {
            return _declaration.Params.Count;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new Environment(interpreter.Globals);
            for(int i = 0; i < _declaration.Params.Count; i++)
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            
            interpreter.ExecuteBlock(_declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {_declaration.Name.Lexeme}>";
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