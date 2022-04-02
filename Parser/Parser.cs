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

        private Expression expression()
        {
            return equality();
        }

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

        private Token consume(TokenType type, string message)
        {
            if (check(type)) return advance();

            throw error(peek(), message);
        }


        private bool check(TokenType type)
        {
            if (isAtEnd()) return false;
            return peek().Type == type;
        }

        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        private bool isAtEnd()
        {
            return peek().Type == TokenType.EOF;
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

        private ParseError error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

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
