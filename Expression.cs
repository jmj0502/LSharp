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
            T Visit(Variable expression);
            T Visit(Logical expression);
            T Visit(Call expression);
            T Visit(Get expression);
            T Visit(Set expression);
            T Visit(This expression);
            T Visit(Super expression);
            T Visit(Ternary expression);
            T Visit(Function expression);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        //Member intended to represent variable declaration. EG: var identifierName = value;
        public class Assign : Expression
        {
            public readonly Token Name;
            public readonly Expression Value;

            public Assign(Token name, Expression value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
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

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }

        }

        //Member intended to represent grouping expressions. EG: (expression operator expression).
        public class Grouping : Expression
        {
            public readonly Expression Expression;

            public Grouping(Expression expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent literal expressions. EG: 1, false, true, nil, "string". 
        public class Literal : Expression
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent unary expressions: EG: !true, -1, -(5+3).
        public class Unary : Expression
        {
            public readonly Token Operatr;
            public readonly Expression Right;

            public Unary(Token operatr, Expression right)
            {
                Operatr = operatr;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to present variable expressions (access).
        public class Variable : Expression
        {
            public readonly Token Name;

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent the structure of logical expressions (or, and).
        public class Logical : Expression
        {
            public readonly Expression Left;
            public readonly Expression Right;
            public readonly Token Operatr;

            public Logical(Expression left, Expression right, Token operatr)
            {
                Left = left;
                Right = right;
                Operatr = operatr;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        //Member intended to represent function calls.
        public class Call : Expression
        {
            public readonly Expression Callee;
            public readonly Token Paren;
            public readonly List<Expression> Arguments;

            public Call(Expression callee, Token paren, List<Expression> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Get : Expression
        {
            public readonly Expression Object;
            public readonly Token Name;

            public Get(Expression obj, Token name)
            {
                Object = obj;
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Set : Expression
        {
            public readonly Expression Object;
            public readonly Token Name;
            public readonly Expression Value;

            public Set(Expression obj, Token name, Expression value)
            {
                Object = obj;
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Super : Expression
        {
            public readonly Token Keyword;
            public readonly Token Method;

            public Super(Token keyword, Token method)
            {
                Keyword = keyword;
                Method = method;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class This : Expression
        {
            public readonly Token Keyword;

            public This(Token keyword)
            {
                Keyword = keyword;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Ternary : Expression
        {
            public readonly Expression Condition;
            public readonly Expression Left;
            public readonly Expression Right;

            public Ternary(Expression condition, Expression left, Expression right)
            {
                Condition = condition;
                Left = left;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Function : Expression
        {
            public readonly List<Token> Parameters;
            public readonly List<Stmt> Body;

            public Function(List<Token> parameters, List<Stmt> body)
            {
                Parameters = parameters;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
