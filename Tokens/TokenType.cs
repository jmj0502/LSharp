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
        LEFT_BRACKET,
        RIGHT_BRACKET,
        COMMA,
        DOT,
        MINNUS,
        PLUS,
        SEMICOLON,
        SLASH,
        PERCENT,
        STAR,

        //One or two character tokens.
        BANG,
        BANG_EQUAL,
        EQUAL,
        EQUAL_EQUAL,
        PLUS_EQUAL,
        MINNUS_EQUAL,
        STAR_EQUAL,
        SLASH_EQUAL,
        R_SHIFT_EQUAL,
        L_SHIFT_EQUAL,
        AND_EQUAL,
        XOR_EQUAL,
        OR_EQUAL,
        GREATER,
        GREATER_EQUAL,
        LESS,
        LESS_EQUAL,
        COLON,
        QUESTION,
        PLUS_PLUS,
        MINNUS_MINNUS,
        BITWISE_AND,
        BITWISE_OR,
        BITWISE_XOR,
        L_SHIFT,
        R_SHIFT,

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
        USING,

        EOF
    }
}
