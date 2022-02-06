using System.Collections.Generic;

namespace CSLox
{
    public class Interpreter : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        private Environment _environment = new Environment();

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

        public object? VisitLiteralExpr(Expr.Literal literal)
        {
            return literal.Value;
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

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return _environment.Get(expr.Name);
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            object? value = Evaluate(expr.Value);
            _environment.Assign(expr.Name, value);
            return value;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            object? value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if(stmt.Initializer != null)
                value = Evaluate(stmt.Initializer);
            
            _environment.Define(stmt.Name.Lexeme, value);
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
    }
}