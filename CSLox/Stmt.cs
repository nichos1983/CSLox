
namespace CSLox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R VisitExpressionStmt(Expression stmt);
            R VisitPrintStmt(Print stmt);
            R VisitVarStmt(Var stmt);
        }

        public abstract R Accept<R>(Visitor<R> visitor);

        public class Expression : Stmt
        {
            public readonly Expr Expr;

            public Expression(Expr @expr)
            {
                Expr = @expr;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }
        }

        public class Print : Stmt
        {
            public readonly Expr Expr;

            public Print(Expr @expr)
            {
                Expr = @expr;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }
        }

        public class Var : Stmt
        {
            public readonly Token Name;
            public readonly Expr? Initializer;

            public Var(Token @name, Expr? @initializer)
            {
                Name = @name;
                Initializer = @initializer;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVarStmt(this);
            }
        }

    }
}