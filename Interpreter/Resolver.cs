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
        public readonly Interpreter Interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();

        public Resolver(Interpreter interpreter)
        {
            Interpreter = interpreter;
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

        public object Visit(Stmt.Var stmt)
        {
            declare(stmt.Name);
            if (stmt.Initializer != null)
                resolve(stmt.Initializer);
            define(stmt.Name);
            return null;
        }

        public object Visit(Expression.Variable expression)
        {
            if ((scopes.Count > 0) &&
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

        private void resolve(Stmt statement)
        {
            statement.accept(this);
        }

        private void resolve(Expression expression)
        {
            expression.Accept(this);
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
            scope[name.Lexeme] = false;
        }

        private void define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

        private void resolveLocal(Expression expression, Token name)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    Interpreter.Resolve(expression, scopes.Count - 1 - i);
                    return;
                }
            }
        }
    }
}
