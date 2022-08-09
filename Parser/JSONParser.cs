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
        private readonly Dictionary<object, object> JSON = new();
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

        private Dictionary<object, object> container()
        {
            var obj = new Dictionary<object, object>();
            if (match(TokenType.LEFT_BRACE))
            {
                obj = jsonObject();
            }
            return obj;
        }

        private Dictionary<object, object> jsonObject()
        {

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
    }
}
