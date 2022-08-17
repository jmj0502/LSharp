using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSharp.Tokens;

namespace LSharp.Scanner
{
    class JSONScanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new();
        private int start;
        private int current;
        private int line = 1;
        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>{
            ["true"] = TokenType.TRUE,
            ["false"] = TokenType.FALSE,
            ["null"] = TokenType.NIL,
        };

        public JSONScanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            addToken(TokenType.EOF);
            return tokens;
        }

        private void scanToken()
        {
            var c = advance();
            switch (c)
            {
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case '[': addToken(TokenType.LEFT_BRACKET); break;
                case ']': addToken(TokenType.RIGHT_BRACKET); break;
                case ',': addToken(TokenType.COMMA); break;
                case ':': addToken(TokenType.COLON); break;
                case '"': literalString(); break;
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                default:
                    if (isDigit(c))
                    {
                        literalNumber();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
                    }
                    break;
            }
        }

        private void addToken(TokenType token) 
        {
            addToken(token, null);
        }

        private void addToken(TokenType token, object literal = null)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(token, text, literal, line));
        }

        private char advance()
        {
            return source[current++];
        }

        private bool isAtEnd()
        {
            return current >= source.Length;
        }

        private char peak()
        {
            if (isAtEnd()) return '\0';
            return source[current];
        }

        private char peakNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || (c == '_');
        }

        private bool isAlphaNumeric(char c)
        {
            return isAlpha(c) || isDigit(c);
        }

        private void literalNumber()
        {
            while (isDigit(peak())) advance();

            if (peak() == '.' && isDigit(peakNext()))
            {
                advance();

                while (isDigit(peak())) advance();
            }

            addToken(TokenType.NUMBER, 
                double.Parse(source.Substring(start, current - start)));
        }

        private void literalString()
        {
            while (peak() != '"' && !isAtEnd())
            {
                if (peak() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                throw new JSONScanError("Unterminated JSON string.");
            }

            advance();
            var text = source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, text);
        }

        private void identifier()
        {
            while (isAlphaNumeric(peak())) advance();
            var keyword = source.Substring(start, current - start);
            TokenType token;
            var isKeyword = keywords.TryGetValue(keyword, out token);
            if (!isKeyword)
            {
                throw new JSONScanError("Invalid JSON value.");
            }
            addToken(token);
        }
    }

    class JSONScanError : ApplicationException
    {
        public JSONScanError(string message) : base(message)
        {
        }
    }
}
