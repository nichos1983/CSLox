
namespace CSLox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R VisitBlockStmt(Block stmt);
            R VisitExpressionStmt(Expression stmt);
            R VisitFunctionStmt(Function stmt);
            R VisitIfStmt(If stmt);
            R VisitPrintStmt(Print stmt);
            R VisitReturnStmt(Return stmt);
            R VisitVarStmt(Var stmt);
            R VisitWhileStmt(While stmt);
        }

        public abstract R Accept<R>(Visitor<R> visitor);

        public class Block : Stmt
        {
            public readonly List<Stmt> Statements;

            public Block(List<Stmt> @statements)
            {
                Statements = @statements;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }
        }

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

        public class Function : Stmt
        {
            public readonly Token Name;
            public readonly Expr.Function FunctionBody;

            public Function(Token @name, Expr.Function @functionbody)
            {
                Name = @name;
                FunctionBody = @functionbody;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }
        }

        public class If : Stmt
        {
            public readonly Expr Condition;
            public readonly Stmt ThenBranch;
            public readonly Stmt? ElseBranch;

            public If(Expr @condition, Stmt @thenbranch, Stmt? @elsebranch)
            {
                Condition = @condition;
                ThenBranch = @thenbranch;
                ElseBranch = @elsebranch;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
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

        public class Return : Stmt
        {
            public readonly Token Keyword;
            public readonly Expr? Value;

            public Return(Token @keyword, Expr? @value)
            {
                Keyword = @keyword;
                Value = @value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitReturnStmt(this);
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

        public class While : Stmt
        {
            public readonly Expr Condition;
            public readonly Stmt Body;

            public While(Expr @condition, Stmt @body)
            {
                Condition = @condition;
                Body = @body;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }
        }

    }
}