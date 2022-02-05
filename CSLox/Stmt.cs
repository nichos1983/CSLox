using System.Collections.Generic;

namespace CSLox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R VisitExpressionStmt(Expression stmt);
            R VisitPrintStmt(Print stmt);
        }

        public abstract R Accept<R>(Visitor<R> visitor);

        public class Expression : Stmt
        {
            public readonly Expr Expr;

            public Expression(Expr expression)
            {
                Expr = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Print : Stmt
        {
            public readonly Expr Expr;

            public Print(Expr expression)
            {
                Expr = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }
    }
}