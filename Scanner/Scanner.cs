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
        private readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>{
            ["and"] = TokenType.AND,
            ["class"] = TokenType.CLASS,
            ["else"] = TokenType.ELSE,
            ["false"] = TokenType.FALSE,
            ["for"] = TokenType.FOR,
            ["fun"] = TokenType.FUN,
            ["if"] = TokenType.IF,
            ["nil"] = TokenType.NIL,
            ["or"] = TokenType.OR,
            ["print"] = TokenType.PRINT,
            ["return"] = TokenType.RETURN,
            ["super"] = TokenType.SUPER,
            ["this"] = TokenType.THIS,
            ["true"] = TokenType.TRUE,
            ["var"] = TokenType.VAR,
            ["while"] = TokenType.WHILE,
            ["continue"] = TokenType.CONTINUE,
            ["break"] = TokenType.BREAK,
        };

        public Scanner(string source)
        {
            this.source = source;
        }

        /// <summary>
        /// This method is intended to scan the different token lexemes and keep tract of their position on the source code.
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
                case '?': addToken(TokenType.QUESTION); break;
                case ':': addToken(TokenType.COLON); break;
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
                case '/':
                    if (match('/'))
                    {
                        //this case will allow us to check for comments. In this case we don't
                        //add the commet into our token list (since they all should be ignored at runtime)
                        //so, we call advance once we've reached the end of the line.
                        while (peak() != '\n' && !isAtEnd()) advance();
                    }
                    else if (match('*'))
                    {
                        multilineComment();
                    }
                    else
                    {
                        //if there's no second character behing the /, that means division is being applyed, so 
                        //we added to the list of tokens.
                        addToken(TokenType.SLASH);
                    }
                    break;
                //now we will check for all the whitespace characters in order to ignore them too (since they don't have 
                //any kind of meaning other than visually representing separation between lexemes).
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                    //Here we are deling with string literals.
                case '"': literalString(); break;
                default:
                    if (isDigit(c))
                    {
                        literalNumber();
                    }
                    else if (isAlpha(c))
                    {
                        identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    break;
            }

        }

        private void identifier()
        {
            while (isAlphaNumeric(peak())) advance();
            var keyword = source.Substring(start, current - start);
            TokenType type;
            var exists = keywords.TryGetValue(keyword, out type);
            if (!exists) type = TokenType.IDENTIFIER;
            addToken(type);
        }

        private bool isAlphaNumeric(char v)
        {
            return isAlpha(v) || isDigit(v);
        }

        /// <summary>
        /// Verifies if the provided character is a valid alpha character, based on the language rules.
        /// </summary>
        /// <param name="c">The character to evaluate.</param>
        private bool isAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        /// <summary>
        /// Apply different validations in order to parse numeric literals (in this case lexemes that represents numbers) 
        /// into tokens.
        /// </summary>
        private void literalNumber()
        {
            while (isDigit(peak())) advance();

            //At this point we proceed to check 
            if (peak() == '.' && isDigit(peakNext()))
            {
                //We actually consume the . character.
                advance();

                while (isDigit(peak())) advance();
            }

            addToken(TokenType.NUMBER, 
                double.Parse(source.Substring(start, current - start)));

        }

        /// <summary>
        /// Allow us to look two characters ahead of the current character.
        /// </summary>
        private char peakNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        /// <summary>
        /// Checks if the provided character is a numerical character.
        /// </summary>
        /// <param name="c">Character to analize.</param>
        private bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Walks over a string literal. Is triggered if our scanner matches a " char. It performs different validations
        /// and operations in order to find the end of the string or raise an error, if needed.
        /// </summary>
        private void literalString()
        {
            while(peak() != '"' && !isAtEnd())
            {
                if (peak() == '\n') line++;
                advance();
            }

            if (isAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            //In case we find the second "
            advance();

            var value = source.Substring(start + 1, current - start - 2);
            addToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Walks over a multiline comment, it increments the line number each time a new line break is found and ignores
        /// every sequence of characters contained in the comment. The common execution of the scanner takes place
        /// once the '*/' characters are found. If a multiline comment doesn't have it's corresponding closing characters
        /// an error is thrown.
        /// </summary>
        private void multilineComment()
        {
            while (!isAtEnd())
            {
                if (peak() == '\n') line++;
                if (match('*') && peak() == '/')
                {
                    advance();
                    return;
                }
                advance();
            }

            Lox.Error(line, "Expect '*/' to close multiline comment.");
        }

        /// <summary>
        /// A method intended to help us with the reading of long lexemes. Similar to advance, but it doesn't increment the 
        /// number of analized characters when called.
        /// </summary>
        private char peak()
        {
            if (isAtEnd()) return '\0';
            return source[current];
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
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
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
