﻿using LSharp.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Interpreter : Expression.IVisitor<object>,
        Stmt.IVisitor<object>
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

        private Enviroment.Enviroment enviroment = new Enviroment.Enviroment();

        /// <summary>
        /// Interprets a expression and produce the expected output to the user. In case the provided expression
        /// is invalid, a runtime error will be araised.
        /// </summary>
        /// <param name="statments">The list of statements intedend to be interpreted.</param>
        public void Interpret(List<Stmt> statments)
        {
            try
            {
                foreach (var statement in statments)
                    execute(statement);
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        /// <summary>
        /// Runs the visitor method on a specific statement.
        /// </summary>
        /// <param name="statement">Any instance of the stamement hierarchy.</param>
        private void execute(Stmt statement)
        {
            statement.accept(this);
        }

        /// <summary>
        /// Turns C# values into loz values to print them.
        /// </summary>
        /// <param name="value">Any lox literal.</param>
        private string stringify(object value)
        {
            if (value == null) return "nil";
            if (value is double)
            {
                var text = value.ToString(); 
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                    return text;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// Executes an assignment expression. If the name provided in the assignment expression is found, its value is updated.
        /// Otherwise an undefined variable runtime exception is raised.
        /// </summary>
        /// <param name="expression">Any assignment expression.</param>
        public object Visit(Expression.Assign expression)
        {
            object value = evaluate(expression.Value);
            enviroment.Assign(expression.Name, value);
            return value;
        }

        /// <summary>
        /// Converts a binary expression into a runtime value.
        /// </summary>
        /// <param name="expression">Any valid lox expression.</param>
        public object Visit(Expression.Binary expression)
        {
            object left = evaluate(expression.Left);
            object right = evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case Tokens.TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case Tokens.TokenType.BANG_EQUAL: return !isEqual(left, right);
                case Tokens.TokenType.GREATER:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left > (double)right;
                case Tokens.TokenType.GREATER_EQUAL:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left >= (double)right;
                case Tokens.TokenType.LESS:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left < (double)right;
                case Tokens.TokenType.LESS_EQUAL:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left <= (double)right;
                case Tokens.TokenType.MINNUS:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case Tokens.TokenType.SLASH:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left / (double)right;
                case Tokens.TokenType.STAR:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left * (double)right;
                case Tokens.TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;

                    if (left is string && right is string)
                        return (string)left + (string)right;

                    throw new RuntimeError(expression.Operator, 
                        "Operands must be two numbers or strings.");
            }

            return null;
        }

        /// <summary>
        /// Verifies if the operands to a binary expression are valid numbers.
        /// </summary>
        /// <param name="operatr">The operator that represents the operation type.</param>
        /// <param name="left">left hand side of the operator.</param>
        /// <param name="right">right hand side of the operator.</param>
        private void checkNumberOperands(Token operatr, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(operatr, "Operands must be numbers.");
        }

        /// <summary>
        /// Check if the provided values are "equal".
        /// </summary>
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

        /// <summary>
        /// Proceeds to recursively evaluate a expression.
        /// </summary>
        /// <param name="expression">Any valid lox expression.</param>
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
                    checkNumberOperand(expression.Operatr, right);
                    return -(double) right;
            }

            return null;
        }

        /// <summary>
        /// Checks if the provided object is a valid number.
        /// </summary>
        /// <param name="operatr">The unary operator that will take effect over the operand.</param>
        /// <param name="operand">The operand which type will be checked.</param>
        private void checkNumberOperand(Token operatr, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(operatr, "Operand must be a number.");
        }

        /// <summary>
        /// Verfiies if a value is truty.
        /// </summary>
        /// <param name="obj">Any object that may represent a lox literal.</param>
        private bool isTruty(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool) obj;
            return true;
        }

        /// <summary>
        /// Interprets a Expression statement.
        /// </summary>
        /// <param name="stmt">The expression statement to be executed.</param>
        public object Visit(Stmt.Expr stmt)
        {
            evaluate(stmt.Expression);
            return null;
        }

        /// <summary>
        /// Interprets a print statement. The process is pretty similar to the common expression statement
        /// but it prints the value to stdout before discarting it.
        /// </summary>
        /// <param name="stmt">The print statement to be executed.</param>
        public object Visit(Stmt.Print stmt)
        {
            object value = evaluate(stmt.Expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        /// <summary>
        /// Proceeds to define a variable into the enviroment. If the provided statements contains an initialization,
        /// the provided value is evaluated and the result is tied up to the variable name. Otherwise, nil is stored 
        /// under such variable.
        /// </summary>
        /// <param name="stmt">Variable statement.</param>
        public object Visit(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = evaluate(stmt.Initializer);
            }

            enviroment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        /// <summary>
        /// Evaluates a Variable expression. Simply finds a variable on the enviroment and then returns it.
        /// </summary>
        /// <param name="expression">A expression variable.</param>
        public object Visit(Expression.Variable expression)
        {
            return enviroment.Get(expression.Name);
        }
    }
}