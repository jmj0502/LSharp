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
        * program -> declaration* EOF; //Added on chapter 8.
        * declaration -> funDecl | varDecl | statement; //Added on chapter 8.
        * statement -> classDecl | exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | block; //Added on chapter 8.
        * classDecl -> "class" IDENTIFIER "{" function* "}";
        * returnStmt -> "return" expression? ";" ;
        * forStmt -> "for" "(" (varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement; //Added on chapter 9. 
        * whileStmt -> "while" "(" expression ")" statement; // Added on chapter 9.
        * ifStmt -> "if" "(" expression ")" statement ( "else" statement)?; //Added on chapter 9.
        * block -> "{" declaration* "}";
        * varDecl -> "var" IDENTIFIER ("=" expression)? ";"; //Added on chapter 8.
        * funDecl -> "fun" function;
        * function -> IDENTIFIER "(" parameters? ")" block;
        * parameters -> IDENTIFIER ( "," IDENTIFIER )*;
        * exprStmt -> expression ";"; //Added on chapter 8.
        * printStmt -> "print" expression ";"; //Added on chapter 8.
        * expression -> assignment; //Added on chapter 8.
        * assignment -> IDENTIFIER "=" assignment | equality; //Added on chapter 8.
        * logical_or -> logical_and ("or" logical_and)*; //Added on chapter 9.
        * logical_and -> equality ("and" equality)*; //Added on chapter 9.
        * equality -> comparison (("!=" | "==") comparison)*;
        * comparison -> term ((">"|">="|"<"|"<=") term )*;
        * term -> factor (("-"|"+") factor )*;
        * factor -> unary (("/"|"*") unary)*;
        * unary -> ("!","-") unary | call;
        * call -> primary ( "(" arguments? ")" )*;
        * arguments -> expression ("," expression )*;
        * primary -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | "IDENTIFIER";
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
        public List<Stmt> Parse()
        {
            var statements = new List<Stmt>();
            while(!isAtEnd())
            {
                statements.Add(declaration());
            }
            return statements;
        }

        /// <summary>
        /// Rule handler for print statement production (Non-terminal).
        /// </summary>
        private Stmt printStatement()
        {
            var value = expression();
            consume(TokenType.SEMICOLON, "Expect ; after value");
            return new Stmt.Print(value);
        }

        /// <summary>
        /// Rule handler for the return statement production (Non-terminal).
        /// </summary>
        private Stmt returnStatement()
        {
            var token = previous();
            Expression value = null;
            if (!check(TokenType.SEMICOLON))
            {
                value = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(token, value);
        }

        /// <summary>
        /// Execute the varDeclaration rule. It consumes an identifier for a variable (raises an exception if it can 
        /// find it) and then checks if the variable if defined by matching a EQUAL token to proceed and evaluate such expression.
        /// If an EQUAL token wasn't provided, it defines a variable with no initialization (hence a variable with a nil value).
        /// </summary>
        private Stmt varDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expression initializer = null;
            if (match(TokenType.EQUAL))
            {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        /// <summary>
        /// Executes the parsing rule for while statements.
        /// </summary>
        public Stmt whileStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after while.");
            var condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");
            var body = statement();

            return new Stmt.While(condition, body);
        }

        /// <summary>
        /// Rule handler for expression statament production (Non-terminal).
        /// </summary>
        private Stmt expressionStatement()
        {
            var value = expression();
            consume(TokenType.SEMICOLON, "Expect ; after expression");
            return new Stmt.Expr(value);
        }

        /// <summary>
        /// Rule handler to deal with the parsing of the function production (Non-terminal).
        /// </summary>
        /// <param name="kind">The kind of function to be parsed.</param>
        private Stmt function(string kind)
        {
            var name = consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
            consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
            var parameters = new List<Token>();
            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(
                        consume(TokenType.IDENTIFIER, "Expect parameter name."));
                }
                while (match(TokenType.COMMA));
            }

            consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");
            var body = block();
            return new Stmt.Function(name, parameters, body);
        }

        /// <summary>
        /// Turn the components of a block statement into RunTime values.
        /// </summary>
        private List<Stmt> block()
        {
            var statements = new List<Stmt>();
            
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                statements.Add(declaration());
            }

            consume(TokenType.RIGHT_BRACE, "Expect } after block.");
            return statements;
        }

        /// <summary>
        /// Rule handler for assignment expressions. It treats the right-hand side of the expression as if it were
        /// a binary expression.
        /// </summary>
        private Expression assignment()
        {
            var expression = or();
            if (match(TokenType.EQUAL))
            {
                var equals = previous();
                var value = assignment();

                if (expression is Expression.Variable)
                {
                    var name = ((Expression.Variable)expression).Name;
                    return new Expression.Assign(name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expression;
        }

        /// <summary>
        /// Executes the parsing rule for the "or" operator. This code somehow mirrors the binary expression rule.
        /// </summary>
        private Expression or()
        {
            var expression = and();

            while(match(TokenType.OR))
            {
                var operatr = previous();
                var right = and();
                return new Expression.Logical(expression, right, operatr);
            }

            return expression;
        }


        /// <summary>
        /// Executes the parsing rule for the "or" operator. This code somehow mirrors the binary expression rule.
        /// </summary>
        private Expression and()
        {
            var expression = equality();

            while(match(TokenType.AND))
            {
                var operatr = previous();
                var right = equality();
                return new Expression.Logical(expression, right, operatr);
            }

            return expression;
        }

        /// <summary>
        ///  Rule handler for the expression production (Non-terminal).
        /// </summary>
        private Expression expression()
        {
            return assignment();
        }


        /// <summary>
        /// Executes the parsing process on a declaration statment. It matches the start of a statement, if it contains
        /// the var keyword at the beginning, it consumes it and executes the varDeclaration rules.
        /// </summary>
        private Stmt declaration()
        {
            try
            {
                if (match(TokenType.CLASS)) return classDeclaration();
                if (match(TokenType.FUN)) return function("function");
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError error)
            {
                synchronize();
                return null;
            }
        }

        private Stmt classDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect class name before body.");
            consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
            var methods = new List<Stmt.Function>();
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                methods.Add((Stmt.Function)function("method"));
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
            return new Stmt.Class(name, methods);
        }

        /// <summary>
        /// Executes the statement rules.
        /// </summary>
        private Stmt statement()
        {
            if (match(TokenType.FOR)) return forStatement();
            if (match(TokenType.IF)) return ifStatement();
            if (match(TokenType.PRINT)) return printStatement();
            if (match(TokenType.RETURN)) return returnStatement();
            if (match(TokenType.WHILE)) return whileStatement();
            if (match(TokenType.LEFT_BRACE)) return new Stmt.Block(block());
            return expressionStatement();
        }


        /// <summary>
        /// Method intended to parse the for statement rule. It takes advantage of different contructs of the langague to
        /// represent a for loop (that's why it doesn't have a corresponding visit method on the statements hierarchy).
        /// </summary>
        private Stmt forStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
            Stmt initializer = null;
            if (match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (match(TokenType.VAR))
            {
                initializer = varDeclaration();
            }
            else
            {
                initializer = expressionStatement();
            }

            Expression condition = null;
            if (!check(TokenType.SEMICOLON))
            {
                condition = expression();
            }
            consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");

            Expression increment = null;
            if (!check(TokenType.RIGHT_PAREN))
            {
                increment = expression();
            }
            consume(TokenType.RIGHT_PAREN, "Expect ')' after 'for' clauses");

            var body = statement();

            if (increment != null)
            {
                body = new Stmt.Block(
                    new List<Stmt> { 
                        body, 
                        new Stmt.Expr(increment) 
                    });
            }

            if (condition == null)
                condition = new Expression.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new List<Stmt> { 
                    initializer, 
                    body
                });
            }

            return body;
        }

        /// <summary>
        /// Executes the if statement rules.
        /// </summary>
        private Stmt ifStatement()
        {
            consume(TokenType.LEFT_PAREN, "Expect '(' after if.");
            var condition = expression();
            consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            var thenBranch = statement();
            Stmt elseBranch = null;
            if (match(TokenType.ELSE))
            {
                elseBranch = statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
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
            return call();
        }

        /// <summary>
        /// Helper function intended to detect the end of a function call.
        /// </summary>
        private Expression finishCall(Expression callee)
        {
            var arguments = new List<Expression>();

            if (!check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (arguments.Count >= 255)
                    {
                        error(peek(), "Can't have more than 255 arguments.");
                    }
                    arguments.Add(expression());
                } 
                while (match(TokenType.COMMA));
            }

            var closingParen = consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
            return new Expression.Call(callee, closingParen, arguments);
        }

        /// <summary>
        /// Rule handler for function calls (Non-Terminal).
        /// </summary>
        private Expression call()
        {
            var expression = primary();

            while(true)
            {
                if (match(TokenType.LEFT_PAREN))
                {
                    expression = finishCall(expression);
                }
                else
                {
                    break;
                }
            }

            return expression;
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

            if (match(TokenType.IDENTIFIER))
            {
                return new Expression.Variable(previous());
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
