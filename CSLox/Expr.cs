
namespace CSLox
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            R VisitLiteralExpr(Literal expr);
            R VisitLogicalExpr(Logical expr);
            R VisitSetExpr(Set expr);
            R VisitUnaryExpr(Unary expr);
            R VisitBinaryExpr(Binary expr);
            R VisitFunctionExpr(Function expr);
            R VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R VisitGroupingExpr(Grouping expr);
            R VisitVariableExpr(Variable expr);
            R VisitAssignExpr(Assign expr);
        }

        public abstract R Accept<R>(Visitor<R> visitor);

        public class Literal : Expr
        {
            public readonly object? Value;

            public Literal(object? @value)
            {
                Value = @value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }

        public class Logical : Expr
        {
            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;

            public Logical(Expr @left, Token @operator, Expr @right)
            {
                Left = @left;
                Operator = @operator;
                Right = @right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }

        public class Set : Expr
        {
            public readonly Expr Object;
            public readonly Token Name;
            public readonly Expr Value;

            public Set(Expr @object, Token @name, Expr @value)
            {
                Object = @object;
                Name = @name;
                Value = @value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }

        public class Unary : Expr
        {
            public readonly Token Operator;
            public readonly Expr Right;

            public Unary(Token @operator, Expr @right)
            {
                Operator = @operator;
                Right = @right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }

        public class Binary : Expr
        {
            public readonly Expr Left;
            public readonly Token Operator;
            public readonly Expr Right;

            public Binary(Expr @left, Token @operator, Expr @right)
            {
                Left = @left;
                Operator = @operator;
                Right = @right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }

        public class Function : Expr
        {
            public readonly List<Token> Params;
            public readonly List<Stmt> Body;

            public Function(List<Token> @params, List<Stmt> @body)
            {
                Params = @params;
                Body = @body;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitFunctionExpr(this);
            }
        }

        public class Call : Expr
        {
            public readonly Expr Callee;
            public readonly Token Paren;
            public readonly List<Expr> Arguments;

            public Call(Expr @callee, Token @paren, List<Expr> @arguments)
            {
                Callee = @callee;
                Paren = @paren;
                Arguments = @arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }

        public class Get : Expr
        {
            public readonly Expr Object;
            public readonly Token Name;

            public Get(Expr @object, Token @name)
            {
                Object = @object;
                Name = @name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public readonly Expr Expression;

            public Grouping(Expr @expression)
            {
                Expression = @expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }

        public class Variable : Expr
        {
            public readonly Token Name;

            public Variable(Token @name)
            {
                Name = @name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }

        public class Assign : Expr
        {
            public readonly Token Name;
            public readonly Expr Value;

            public Assign(Token @name, Expr @value)
            {
                Name = @name;
                Value = @value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }

    }
}