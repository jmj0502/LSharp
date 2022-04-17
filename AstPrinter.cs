using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp
{
    public class AstPrinter : Expression.IVisitor<string>
    {
        public string Print(Expression expression)
        {
            return expression.Accept(this);
        }

        public string Visit(Expression.Assign expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Binary expression)
        {
            return parenthesize(expression.Operator.Lexeme, 
                expression.Left, expression.Right);
        }

        public string Visit(Expression.Grouping expression)
        {
            return parenthesize("group", expression.Expression);
        }

        public string Visit(Expression.Literal expression)
        {
            if (expression.Value == null) return "nil";
            return expression.Value.ToString();
        }

        public string Visit(Expression.Unary expression)
        {
            return parenthesize(expression.Operatr.Lexeme, expression.Right);
        }

        public string Visit(Expression.Variable expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Logical expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Call expression)
        {
            throw new NotImplementedException();
        }

        private string parenthesize(string name, params Expression[] expressions)
        {
            var sb = new StringBuilder();

            sb.Append("(").Append(name);
            foreach (var expr in expressions)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }
            sb.Append(")");

            return sb.ToString();
        }
    }

    public class RPNPrinter : Expression.IVisitor<string>
    {
        public string Visit(Expression.Assign expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Binary expression)
        {
            return $"{expression.Left.Accept(this)} {expression.Right.Accept(this)} {expression.Operator.Lexeme} ";
        }

        public string Visit(Expression.Grouping expression)
        {
            return $"{expression.Expression.Accept(this)}";
        }

        public string Visit(Expression.Literal expression)
        {
            return $"{expression.Value}";
        }

        public string Visit(Expression.Unary expression)
        {
            return $"{expression.Right.Accept(this)}{expression.Operatr.Lexeme} ";
        }

        public string Visit(Expression.Variable expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Logical expression)
        {
            throw new NotImplementedException();
        }

        public string Visit(Expression.Call expression)
        {
            throw new NotImplementedException();
        }
    }
}
