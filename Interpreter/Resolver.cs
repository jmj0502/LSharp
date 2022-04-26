using System;
using System.Collections.Generic;
using LSharp.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Resolver : 
        Stmt.IVisitor<object>, Expression.IVisitor<object>

    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();
        private FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        private enum FunctionType
        {
            NONE,
            FUNCTION
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
                resolve(statement);
        }

        public object Visit(Stmt.Block stmt)
        {
            beginScope();
            Resolve(stmt.Statements);
            endScope();
            return null;
        }

        public object Visit(Stmt.Expr stmt)
        {
            resolve(stmt.Expression);
            return null;
        }

        public object Visit(Stmt.Function stmt)
        {
            declare(stmt.Name);
            define(stmt.Name);

            resolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        public object Visit(Stmt.If stmt)
        {
            resolve(stmt.Condition);
            resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) resolve(stmt.ElseBranch);
            return null;
        }

        public object Visit(Stmt.Print stmt)
        {
            resolve(stmt.Expression);
            return null;
        }

        public object Visit(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword,
                    "Can't return from top-level code.");
            }

            if (stmt.Value != null)
                resolve(stmt.Value);
            return null;
        }

        public object Visit(Stmt.Var stmt)
        {
            declare(stmt.Name);
            if (stmt.Initializer != null)
                resolve(stmt.Initializer);
            define(stmt.Name);
            return null;
        }

        public object Visit(Stmt.While stmt)
        {
            resolve(stmt.Condition);
            resolve(stmt.Body);
            return null;
        }

        public object Visit(Expression.Variable expression)
        {
            if (scopes.Count > 0 &&
                scopes.Peek().ContainsKey(expression.Name.Lexeme) &&
                scopes.Peek()[expression.Name.Lexeme] == false)
                Lox.Error(expression.Name, 
                    "Can't read local variable in its own initializer.");
            resolveLocal(expression, expression.Name);
            return null;
        }

        public object Visit(Expression.Assign expression)
        {
            resolve(expression.Value);
            resolveLocal(expression, expression.Name);
            return null;
        }

        public object Visit(Expression.Binary expression)
        {
            resolve(expression.Left);
            resolve(expression.Right);
            return null;
        }

        public object Visit(Expression.Call expression)
        {
            resolve(expression.Callee);

            foreach (var argument in expression.Arguments)
                resolve(argument);

            return null;
        }

        public object Visit(Expression.Grouping expression)
        {
            resolve(expression.Expression);
            return null;
        }

        public object Visit(Expression.Literal expression)
        {
            return null;
        }

        public object Visit(Expression.Logical expression)
        {
            resolve(expression.Left);
            resolve(expression.Right);
            return null;
        }

        public object Visit(Expression.Unary expression)
        {
            resolve(expression.Right);
            return null;
        }

        private void resolve(Stmt statement)
        {
            statement.accept(this);
        }

        private void resolve(Expression expression)
        {
            expression.Accept(this);
        }

        private void resolveFunction(Stmt.Function function,
            FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;
            beginScope();
            foreach (var parameter in function.Parameters)
            {
                declare(parameter);
                define(parameter);
            }
            Resolve(function.Body);
            endScope();
            currentFunction = enclosingFunction;
        }

        private void beginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void endScope()
        {
            scopes.Pop();
        }

        private void declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, 
                    "There is already a variable with this name in this scope.");
            }
            scope[name.Lexeme] = false;
        }

        private void define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

        private void resolveLocal(Expression expression, Token name)
        {
            var orderedStack = scopes.ToArray().Reverse().ToList();
            for (int i = orderedStack.Count() - 1; i >= 0; i--)
            {
                if (orderedStack[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expression, orderedStack.Count() - 1 - i);
                    return;
                }
            }
        }
    }
}
