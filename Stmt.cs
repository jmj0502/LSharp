using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp
{
    public abstract class Stmt
    {
        public interface IVisitor<T>
        {
            T Visit(Expr stmt);
            T Visit(Print stmt);
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
    }
}
