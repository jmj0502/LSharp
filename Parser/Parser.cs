using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;
using LSharp;

namespace LSharp.Parser
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        /*
        * From this point and on, we'll start translating every rule of Lox into C#.
        * The rules we're about to parse are defined as a variation of the EBNF (Extended Backus-Naur Form) 
        * expressed in Crafting Interpreter And Compilers. Those are:
        * expression -> equality;
        * equality -> comparison (("!=" | "==") comparison)*;
        * comparison -> term ((">"|">="|"<"|"<=") term )*;
        * term -> factor (("-"|"+") factor )*;
        * factor -> unary (("/"|"*") unary)*;
        * unary -> ("!","-") unary | primary;
        * primary -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")";
        * This sintax will allow us to represent our productions in code as follows:
        * Terminal -> Code to match and consume a token.
        * Non-Terminal -> Call to that rule's function.
        * | -> if or switch statement.
        * *, + -> while or for loop.
        * ? -> if statement.
        */

        /// <summary>
        /// Base point, will start the top-to-down (recursive) parsing process invoking the rule for the expression production.
        /// </summary>
        /// <returns></returns>
        public Expression Parse()
        {
            try
            {
                return expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        /// <summary>
        ///  Rule handler for the expression production (Non-terminal).
        /// </summary>
        private Expression expression()
        {
            return equality();
        }

        /// <summary>
        /// Rule handler for the equality production (Non-terminal).
        /// </summary>
        private Expression equality()
        {
            var expression = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                var operatr = previous();
                var right = comparison();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule handler for the comparison production (Non-terminal).
        /// </summary>
        private Expression comparison()
        {
            var expression = term();
            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var operatr = previous();
                var right = term();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule handler for the term production (Non-Terminal).
        /// </summary>
        private Expression term()
        {
            var expression = factor();
            while (match(TokenType.PLUS, TokenType.MINNUS))
            {
                var operatr = previous();
                var right = factor();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule hadler for the factor term production (Non-terminal).
        /// </summary>
        /// <returns></returns>
        private Expression factor()
        {
            var expression = unary(); 
            while(match(TokenType.SLASH, TokenType.STAR))
            {
                var operatr = previous();
                var right = unary();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule handler for the unary production (Non-terminal).
        /// </summary>
        private Expression unary()
        {
            if (match(TokenType.BANG, TokenType.MINNUS))
            {
                var operatr  = previous();
                var right = unary();
                return new Expression.Unary(operatr, right);
            }
            return primary();
        }

        /// <summary>
        /// Rule handler for the primary production. It takes care of the terminal characters, an applies
        /// recurssive parsing to group expressions.
        /// </summary>
        private Expression primary()
        {
            if (match(TokenType.TRUE)) return new Expression.Literal(true);
            if (match(TokenType.FALSE)) return new Expression.Literal(false);
            if (match(TokenType.NIL)) return new Expression.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING))
            {
                var value = previous();
                return new Expression.Literal(value.Literal);
            }

            if (match(TokenType.LEFT_PAREN))
            {
                var expression = this.expression();
                consume(TokenType.RIGHT_PAREN, "Expected ')' after expression.");
                return new Expression.Grouping(expression);
            }

            throw error(peek(), "Expected expression.");
        }

        /// <summary>
        /// Verifies if at least one of the provided Token Types is present on the current position of the tokens array.
        /// If so, it consumes it, and returns true. Returns false otherwise.
        /// </summary>
        /// <param name="types">The array of Tokens to match againts.</param>
        private bool match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (check(type))
                {
                    advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Verifies if the provided Token is present on the current position of the tokens array.
        /// If so, consumes it and then returns it. Throws and error that restarts the execution stack 
        /// (turns the compiler into panic mode) otherwise.
        /// </summary>
        /// <param name="type">Type to match against.</param>
        /// <param name="message">Error message in case the current position of the tokens array doesn't match
        /// the provided token.
        /// </param>
        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }

        /// <summary>
        /// Checks if the provided type is at the current position of the tokens array. Returns true if so. Returns false if the
        /// end of the file is reached or if the current position of the tokens array doesn't match the provided type.
        /// </summary>
        /// <param name="type">Type to check on the tokens array.</param>
        private bool check(TokenType type)
        {
            if (isAtEnd()) return false;
            return peek().Type == type;
        }

        /// <summary>
        /// Moves increments the current pointer on the tokes array by one position in case it hasn't reached the end of the file. 
        /// Returns the character on the position of the previous current pointer.
        /// </summary>
        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        /// <summary>
        /// Checks if the end of the line's been reached. Returns true if so and flase otherwise.
        /// </summary>
        private bool isAtEnd()
        {
            return peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Returns the token at the current position on the tokens array.
        /// </summary>
        private Token peek()
        {
            return tokens[current];
        }

        /// <summary>
        /// Returns the current located one position before to the current position of the tokens array.
        /// </summary>
        private Token previous()
        {
            return tokens[current - 1];
        }

        /// <summary>
        /// Invokes the error reporting method and returns a parse error.
        /// </summary>
        /// <param name="token">Token that caused a sintax error.</param>
        /// <param name="message">Error message.</param>
        private ParseError error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        /// <summary>
        /// Synchronizes the parser by jumping right to the 'next' statement after a parse error has taken place.
        /// </summary>
        private void synchronize()
        {
            advance();

            while(!isAtEnd())
            {
                if (previous().Type == TokenType.SEMICOLON) return;

                switch (peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                advance();
            }
        }

        public class ParseError : SystemException { }
           
    }
}
