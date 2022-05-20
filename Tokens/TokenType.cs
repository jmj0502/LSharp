using System;
using System.Collections.Generic;
using System.Text;

namespace LSharp.Tokens
{
    public enum TokenType {
        //Single character tokens.
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        COMMA,
        DOT,
        MINNUS,
        PLUS,
        SEMICOLON,
        SLASH,
        STAR,

        //One or two character tokens.
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESS,
        LESS_EQUAL,
        COLON,
        QUESTION,
        PLUS_PLUS,
        MINNUS_MINNUS,

        //Literals
        IDENTIFIER,
        STRING,
        NUMBER,

        //Keywords
        AND,
        CLASS,
        ELSE,
        FALSE,
        FUN,
        FOR,
        IF,
        NIL,
        OR,
        PRINT,
        RETURN,
        SUPER,
        THIS,
        TRUE,
        VAR,
        WHILE,
        CONTINUE,
        BREAK,
        MODULE,

        EOF
    }
}
