using System.Collections.Generic;

namespace CSLox
{
    public class Resolver : Expr.Visitor<object?>, Stmt.Visitor<object?>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        private readonly Interpreter _interpreter;
        // C# doesn't support accessing Stack by index, so we use List instead.
        private readonly List<Dictionary<string, bool>> _scopes = new List<Dictionary<string, bool>>();
        private FunctionType _currentFunction = FunctionType.NONE;
        
        public Resolver(Interpreter interpreter)
        {
            _interpreter = interpreter;
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }

        public object? VisitClassStmt(Stmt.Class stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);

            if(stmt.Superclass != null && stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme))
                Lox.Error(stmt.Superclass.Name, "A class can't inherit from itself.");

            if(stmt.Superclass != null)
                Resolve(stmt.Superclass);

            BeginScope();
            _scopes[_scopes.Count - 1]["this"] = true;

            foreach(Stmt.Function method in stmt.Methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if(method.Name.Lexeme.Equals("init"))
                    declaration = FunctionType.INITIALIZER;
                
                ResolveFunction(method.FunctionBody, declaration);
            }

            EndScope();

            return null;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if(stmt.Initializer != null)
                Resolve(stmt.Initializer);
            Define(stmt.Name);
            return null;
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            
            Resolve(stmt.FunctionBody);
            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.ThenBranch);
            if(stmt.ElseBranch != null)
                Resolve(stmt.ElseBranch);
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            if(_currentFunction == FunctionType.NONE)
                Lox.Error(stmt.Keyword, "Can't return from top-level code.");
            
            if(stmt.Value != null)
            {
                if(_currentFunction == FunctionType.INITIALIZER)
                    Lox.Error(stmt.Keyword, "Can't return a value from an initializer.");
                
                Resolve(stmt.Value);
            }
            return null;
        }
        
        public object? VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return null;
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return null;
        }

        public object? VisitThisExpr(Expr.This expr)
        {
            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitFunctionExpr(Expr.Function expr)
        {
            ResolveFunction(expr, FunctionType.FUNCTION);
            return null;
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach(Expr argument in expr.Arguments)
                Resolve(argument);
            
            return null;
        }

        public object? VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Object);
            return null;
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            if(_scopes.Count > 0 && _scopes[_scopes.Count - 1].ContainsKey(expr.Name.Lexeme))
            {
                if(_scopes[_scopes.Count - 1][expr.Name.Lexeme] == false)
                    Lox.Error(expr.Name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach(Stmt statement in statements)
                Resolve(statement);
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void BeginScope()
        {
            _scopes.Add(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            _scopes.RemoveAt(_scopes.Count - 1);
        }

        private void Declare(Token name)
        {
            if(_scopes.Count == 0)
                return;
            
            Dictionary<string, bool> scope = _scopes[_scopes.Count - 1];
            if(scope.ContainsKey(name.Lexeme))
                Lox.Error(name, "Variable with this name already declared in this scope.");
            else
                scope.Add(name.Lexeme, false);
        }

        private void Define(Token name)
        {
            if(_scopes.Count == 0)
                return;
            
            _scopes[_scopes.Count - 1][name.Lexeme] = true;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for(int i = _scopes.Count - 1; i >= 0; i--)
            {
                if(_scopes[i].ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                }
            }
        }

        // The parameter type is different from in the book.
        // Cause I changed the definition of Stmt.Function to support anonymous function.
        private void ResolveFunction(Expr.Function expr, FunctionType type)
        {
            FunctionType enclosingFunction = _currentFunction;
            _currentFunction = type;

            BeginScope();
            foreach(Token param in expr.Params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(expr.Body);
            EndScope();

            _currentFunction = enclosingFunction;
        }
    }
}