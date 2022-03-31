using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp
{
    public abstract class Expression
    {
        public interface IVisitor<T>
        {
            T Visit(Assign expression);
            T Visit(Binary expression);
            T Visit(Grouping expression);
            T Visit(Literal expression);
            T Visit(Unary expression);
        }

        public abstract T accept<T>(IVisitor<T> visitor);

        //Member intended to represent variable declaration. EG: var identifierName = value;
        public class Assign : Expression
        {
            public readonly Token Name;
            public readonly Expression value;

            public Assign(Token name, Expression value)
            {
                Name = name;
                this.value = value;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent binary expressions. EG: expression operator expression.
        public class Binary : Expression
        {
            public Binary(Expression left, Expression right, Token operatr)
            {
                Left = left;
                Right = right;
                Operator = operatr;
            }

            public readonly Expression Left;
            public readonly Expression Right;
            public readonly Token Operator;

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        //Member intended to represent grouping expressions. EG: (expression operator expression).
        public class Grouping : Expression
        {
            public readonly Expression expression;

            public Grouping(Expression expression)
            {
                this.expression = expression;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent literal expressions. EG: 1, false, true, nil, "string". 
        public class Literal : Expression
        {
            public readonly Object value;

            public Literal(object value)
            {
                this.value = value;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent unary expressions: EG: !true, -1, -(5+3).
        public class Unary : Expression
        {
            public readonly Token operatr;
            public readonly Expression right;

            public Unary(Token operatr, Expression right)
            {
                this.operatr = operatr;
                this.right = right;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
