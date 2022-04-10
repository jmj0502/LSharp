using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp
{
    public abstract class Stmt
    {
        public interface IVisitor<T>
        {
            T Visit(Expr stmt);
            T Visit(Print stmt);
            T Visit(Var stmt);
            T Visit(Block stmt);
        }

        public abstract T accept<T>(IVisitor<T> visitor);

        public class Expr : Stmt
        {
            public readonly Expression Expression;

            public Expr(Expression expression)
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Print : Stmt
        {
            public readonly Expression Expression;

            public Print(Expression expression)
            {
                Expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Var : Stmt
        {
            public readonly Token Name;
            public readonly Expression Initializer;

            public Var(Token name, Expression initializer)
            {
                Name = name;
                Initializer = initializer;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Block : Stmt
        {
            public readonly List<Stmt> Statements;

            public Block(List<Stmt> statements)
            {
                Statements = statements;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
