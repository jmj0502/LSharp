using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Parser
{
    class JSONParser
    {
        private int current;
        private List<Token> tokens;

        public JSONParser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        /**
         * JSON BNF:
         * <json> ::= <primitive> | <container>
         *  <primitive> ::= <number> | <string> | <boolean>
         *  ; Where:
         *  ; <number> is a valid real number expressed in one of a number of given formats
         *  ; <string> is a string of valid characters enclosed in quotes
         *  ; <boolean> is one of the literal strings 'true', 'false', or 'null' (unquoted)
         *  <container> ::= <object> | <array>
         *  <array> ::= '[' [ <json> *(', ' <json>) ] ']' ; A sequence of JSON values separated by commas
         *  <object> ::= '{' [ <member> *(', ' <member>) ] '}' ; A sequence of 'members'
         *  <member> ::= <string> ': ' <json> ; A pair consisting of a name, and a JSON value
         *
         */

        public object Parse()
        {
            return container();
        } 

        private object container()
        {
            object obj = null;
            if (match(TokenType.LEFT_BRACE))
            {
                obj = jsonObject();
            }
            if (match(TokenType.LEFT_BRACKET))
            {
                obj = jsonArray();
            }
            return obj;
        }

        private Dictionary<object, object> jsonObject()
        {
            var obj = new Dictionary<object, object>();
            do
            {
                consume(TokenType.STRING, "Only strings can be used as keys!");
                var key = previous().Literal;
                consume(TokenType.COLON, ": must be provided after a key definition");
                obj[key] = primitive();
                if (peek().Type == TokenType.RIGHT_BRACE) break;
            }
            while (match(TokenType.COMMA));
            return obj;
        }

        private List<object> jsonArray()
        {
            var arr = new List<object>();
            return arr;
        }

        private object primitive()
        {
            if (check(TokenType.STRING))
            {
                var token = advance();
                return token.Literal;
            }
            if (check(TokenType.NUMBER))
            {
                var token = advance();
                return token.Literal;
            }
            if (check(TokenType.TRUE))
            {
                advance();
                return true;
            }
            if (check(TokenType.FALSE))
            {
                advance();
                return false;
            }
            if (check(TokenType.NIL))
            {
                advance();
                return null;
            }
            if (check(TokenType.LEFT_BRACE))
            {
                advance();
                return jsonObject();
            }
            if (check(TokenType.LEFT_BRACKET))
            {
                // TODO.
            }
            throw new JSONError();
        }

        private Token peek()
        {
            return tokens[current];
        }

        private Token previous()
        {
            return tokens[current - 1];
        }

        private bool isAtEnd()
        {
            return peek().Type == TokenType.EOF;
        }

        private Token advance()
        {
            if (!isAtEnd()) current++;
            return previous();
        }

        private bool check(TokenType type)
        {
            return tokens[current].Type == type;
        }

        private bool match(params TokenType[] tokenTypes)
        {
            foreach (var type in tokenTypes)
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
            throw new JSONError();
        }
    }

    public class JSONError : ApplicationException { }
}
