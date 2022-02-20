using System.Text;

namespace CSLox
{
    class ASTPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value is null ? "nil" : expr.Value.ToString()!;
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitSetExpr(Expr.Set expr)
        {
            return Parenthesize(expr.Name.Lexeme, expr.Object, expr.Value);
        }

        public string VisitThisExpr(Expr.This expr)
        {
            return expr.Keyword.Lexeme;
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
        }

        public string VisitFunctionExpr(Expr.Function expr)
        {
            return "function";
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            return Parenthesize("call", expr.Callee);
        }

        public string VisitGetExpr(Expr.Get expr)
        {
            return Parenthesize(expr.Name.Lexeme, expr.Object);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return Parenthesize(expr.Name.Lexeme);
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize(expr.Name.Lexeme, expr.Value);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach(Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}