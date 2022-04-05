using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Interpreter : Expression.IVisitor<object>
    {
        /*
         * In order to represent lox's code (LS in this case) we're going to use the built-in C# types.
         * The following table shows how we can map a Lox value into a C# value.
         * Any Lox value -> object.
         * nil -> null.
         * boolean -> boolean.
         * number -> double.
         * string -> string.
         */

        public object Visit(Expression.Assign expression)
        {
            throw new NotImplementedException();
        }

        public object Visit(Expression.Binary expression)
        {
            object left = evaluate(expression.Left);
            object right = evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case Tokens.TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case Tokens.TokenType.BANG_EQUAL: return !isEqual(left, right);
                case Tokens.TokenType.GREATER:
                    return (double)left > (double)right;
                case Tokens.TokenType.GREATER_EQUAL:
                    return (double)left >= (double)right;
                case Tokens.TokenType.LESS:
                    return (double)left < (double)right;
                case Tokens.TokenType.LESS_EQUAL:
                    return (double)left <= (double)right;
                case Tokens.TokenType.MINNUS:
                    return (double)left - (double)right;
                case Tokens.TokenType.SLASH:
                    return (double)left / (double)right;
                case Tokens.TokenType.STAR:
                    return (double)left * (double)right;
                case Tokens.TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;

                    if (left is string && right is string)
                        return (string)left + (string)right;
                    break;
            }

            return null;
        }

        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Converts a grouping expression into a runtime value.
        /// </summary>
        /// <param name="expression">Any grouping expression.</param>
        public object Visit(Expression.Grouping expression)
        {
            return evaluate(expression.Expression);
        }

        private object evaluate(Expression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Converts a literal value into a runtime value.
        /// </summary>
        /// <param name="expression">Any literal expression.</param>
        public object Visit(Expression.Literal expression)
        {
            return expression.Value;
        }

        /// <summary>
        /// Convers a unary expression into a ruitime value.
        /// </summary>
        /// <param name="expression">Any Unary expression.</param>
        public object Visit(Expression.Unary expression)
        {
            object right = evaluate(expression.Right);

            switch (expression.Operatr.Type)
            {
                case Tokens.TokenType.BANG:
                    return !isTruty(right);
                case Tokens.TokenType.MINNUS:
                    return -(double) right;
            }

            return null;
        }

        private bool isTruty(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool) obj;
            return true;
        }
    }
}
