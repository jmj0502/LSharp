using System;
using System.Collections.Generic;
using System.Text;
using LSharp.Tokens;

namespace LSharp.Scanner
{
    public class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        /// <summary>
        /// This method is intended to scan the different tokens lexemes and keep tract of their position on the source code.
        /// It will allow us to produce Tokens aware of their value and location within the source code.
        /// </summary>
        public List<Token> ScanTokens()
        {
            while(!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        /// <summary>
        /// Method on charge of reading the lexemes available in the source code and produce its respective token.
        /// It will walk through the lexemes, produce the required tokens, and keep moving forward.
        /// </summary>
        private void scanToken()
        {
            var c = advance();
            switch (c)
            {
                case '(': addToken(TokenType.LEFT_PAREN);  break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINNUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                default:
                    Lox.Error(line, "Unexpected character.");
                    break;
            }

        }

        /// <summary>
        /// Verifies if the provided lexeme is the next in the chain. If that's the case, the string pointer is moved
        /// forward and we proceed with the evaluation of other lexeme.
        /// </summary>
        /// <param name="expected">The lexeme we're expecting to encounter in the next position.</param>
        private bool match(char expected)
        {
            if (isAtEnd()) return false;
            if (source[current] != expected) return false;
            current++;
            return true;
        }

        /// <summary>
        /// Adds a token to the token list based on an analysed lexeme.
        /// </summary>
        /// <param name="type">The type of the token that'll be added to the list of tokens.</param>
        private void addToken(TokenType type)
        {
            addToken(type, null);
        }


        /// <summary>
        /// Adds a token to the token list based on an analysed lexeme.
        /// </summary>
        /// <param name="type">The type of the token that'll be added to the list of tokens.</param>
        /// <param name="literal">The literal represented by the received token type.</param>
        private void addToken(TokenType type, object literal)
        {
            var text = source.Substring(start, current);
            tokens.Add(new Token(type, text, null, line));
        }

        /// <summary>
        /// Moves the pointer to the next lexeme and returns it, in order to proceed with its evaluation.
        /// </summary>
        private char advance()
        {
            return source[current++];
        }

        /// <summary>
        /// Method intended to determine if the current pointer of the scanner is a valid location within the file.
        /// </summary>
        private bool isAtEnd()
        {
            return current >= source.Length;
        }
    }
}
