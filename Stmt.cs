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
            T Visit(Class stmt);
            T Visit(If stmt);
            T Visit(While stmt);
            T Visit(Function stmt);
            T Visit(Return stmt);
            T Visit(Break stmt);
            T Visit(Continue stmt);
            T Visit(Module stmt);
            T Visit(Using stmt);
            T Visit(TryCatch stmt);
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

        public class Class : Stmt
        {
            public readonly Token Name;
            public readonly Expression.Variable Superclass;
            public readonly List<Function> Methods;

            public Class(Token name, List<Function> methods, Expression.Variable superclass)
            {
                Name = name;
                Methods = methods;
                Superclass = superclass;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class If : Stmt
        {
            public readonly Expression Condition;
            public readonly Stmt ThenBranch;
            public readonly Stmt ElseBranch;

            public If(Expression condition, Stmt thenBranch, Stmt elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class While : Stmt
        {
            public readonly Expression Condition;
            public readonly Stmt Body;

            public While(Expression condition, Stmt body)
            {
                Condition = condition;
                Body = body;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Function : Stmt
        {
            public readonly Token Name;
            public readonly List<Token> Parameters;
            public readonly List<Stmt> Body;

            public Function(Token name, List<Token> parameters, List<Stmt> body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Return : Stmt
        {
            public readonly Token Keyword;
            public readonly Expression Value;

            public Return(Token keyword, Expression value)
            {
                Keyword = keyword;
                Value = value;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Break : Stmt
        {
            public readonly Token Keyword;

            public Break(Token keyword)
            {
                Keyword = keyword;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Continue : Stmt
        {
            public Token Keyword;

            public Continue(Token keyword)
            {
                Keyword = keyword;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Module : Stmt
        {
            public Token Name;
            public List<Stmt> Body;

            public Module(Token name, List<Stmt> body)
            {
                Name = name;
                Body = body;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Using : Stmt
        {
            public readonly Token Keyword;
            public readonly string Path;

            public Using(Token keyword, string path)
            {
                Keyword = keyword;
                Path = path;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class TryCatch : Stmt
        {
            public readonly Stmt TryBranch;
            public readonly Stmt CatchBranch;
            public readonly Expression Condition;

            public TryCatch(Stmt tryBranch, Stmt catchBranch, Expression condition)
            {
                TryBranch = tryBranch;
                CatchBranch = catchBranch;
                Condition = condition;
            }

            public override T accept<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
