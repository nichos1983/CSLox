using System.Collections.Generic;

namespace CSLox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        public readonly Environment Globals = new Environment(null);
        private Environment _environment;
        private readonly Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();

        public Interpreter()
        {
            _environment = Globals; // C# can not assign it in field initializer
            Globals.Define("clock", new NativeClockFunction());
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(Stmt statement in statements)
                {
                    Execute(statement);
                }
            }
            catch(RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            object? left = Evaluate(expr.Left);
            
            if(expr.Operator.Type == TokenType.OR)
            {
                if(IsTruthy(left))
                    return left;
            }
            else
            {
                if(!IsTruthy(left))
                    return left;
            }

            return Evaluate(expr.Right);
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            object? right = Evaluate(expr.Right);
            switch(expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    return -(double)right;
                
                default:
                    break;
            }

            return null;
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left)!;
            object right = Evaluate(expr.Right)!;

            switch(expr.Operator.Type)
            {
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if(left is double d1 && right is double d2)
                        return d1 + d2;
                    if(left is string s1 && right is string s2)
                        return s1 + s2;
                    // Uncomment it to implement Challenges 2 in chapter <Evaluating expressions>
                    // But this is not a good design idea
                    // if(left is string || right is string)
                    //     return left.ToString() + right.ToString();
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
            }

            return null;
        }

        public object? VisitFunctionExpr(Expr.Function expr)
        {
            return new LoxFunction(null, expr, _environment);
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.Callee)!;

            List<object?> arguments = new List<object?>();
            foreach(Expr argument in expr.Arguments)
                arguments.Add(Evaluate(argument));
            
            if(callee is not LoxCallable)
                throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
            
            LoxCallable function = (LoxCallable)callee;
            if(arguments.Count != function.Arity())
                throw new RuntimeError(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
            
            return function.Call(this, arguments);
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.Value);
            
            int distance;
            if(_locals.ContainsKey(expr))
            {
                distance = _locals[expr];
                _environment.AssignAt(distance, expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }
            
            return value;
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            string fnName = stmt.Name.Lexeme;
            _environment.Define(fnName, new LoxFunction(fnName, stmt.FunctionBody, _environment));
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            if(IsTruthy(Evaluate(stmt.Condition)))
                Execute(stmt.ThenBranch);
            else if(stmt.ElseBranch != null)
                Execute(stmt.ElseBranch);
            
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            object? value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            object? value = null;
            if(stmt.Value != null)
                value = Evaluate(stmt.Value);
            
            throw new Return(value);
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if(stmt.Initializer != null)
                value = Evaluate(stmt.Initializer);
            
            _environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            while(IsTruthy(Evaluate(stmt.Condition)))
                Execute(stmt.Body);
            
            return null;
        }

        private bool IsTruthy(object? obj)
        {
            if(obj == null)
                return false;
            if(obj is bool o)
                return o;
            return true;
        }

        private bool IsEqual(object? a, object? b)
        {
            if(a == null && b == null)
                return true;
            if(a == null)
                return false;

            return a.Equals(b);
        }

        private void CheckNumberOperand(Token op, object operand)
        {
            if(operand is double)
                return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private void CheckNumberOperands(Token op, object left, object right)
        {
            if(left is double && right is double)
                return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }

        private string? Stringify(object? obj)
        {
            if(obj == null)
                return "nil";
            
            if(obj is double)
            {
                string text = obj.ToString()!;
                if(text.EndsWith(".0", System.StringComparison.Ordinal))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }

            // C# bool ToString() returns capitalized word, so lox works around it
            if(obj is bool o)
                return o ? "true" : "false";

            return obj.ToString();
        }

        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = _environment;
            try
            {
                _environment = environment;
                foreach(Stmt statement in statements)
                    Execute(statement);
            }
            finally
            {
                _environment = previous;
            }
        }

        public void Resolve(Expr expr, int depth)
        {
            _locals.Add(expr, depth);
        }

        private object? LookUpVariable(Token name, Expr expr)
        {
            int distance;
            if(_locals.ContainsKey(expr))
            {
                distance = _locals[expr];
                return _environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }
    }
}