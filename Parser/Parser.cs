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
        * statement -> classDecl | exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | block
        * | continueStmt | breakStmt; //Added on chapter 8.
        * usingStmt -> "using" path;
        * path -> STRING;
        * moduleDecl -> "module" IDENTIFIER "{" statement* "}";
        * classDecl -> "class" IDENTIFIER ("<" IDENTIFIER)? "{" function* "}";
        * returnStmt -> "return" expression? ";" ;
        * breakStmt -> "break" ";";
        * continueStmt -> "continue" ";";
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
        * assignment -> (call ".")? IDENTIFIER (access)? "=" assignment | ternaryEx; //Added on chapter 8.
        * ternaryEx -> comparison "?" logical_or ":" ( logical_or | ternary_expression ) | logical_or;
        * logical_or -> logical_and ("or" logical_and)*; //Added on chapter 9.
        * logical_and -> bitwise_or ("and" bitwise_or)*; //Added on chapter 9.
        * bitwise_or -> bitwise_xor ("|" bitwise_xor)*;
        * bitwise_xor -> bitwise_and ("^" bitwise_and)*;
        * bitwise_and -> equality ("&" equality)*;
        * equality -> comparison (("!=" | "==") comparison)*;
        * comparison -> shift ((">"|">="|"<"|"<=") shift )*;
        * shift -> term ((">>" | "<<") term)*;
        * term -> factor (("-"|"+") factor )*;
        * factor -> prefix (("/"|"*") prefix)*;
        * prefix -> ("++" | "--") unary | unary;
        * unary -> ("!","-") unary | call;
        * postfix -> call ("++" | "--") | call;
        * call -> primary ( "(" arguments? ")" | "." IDENTIFIER )*;
        * access -> primary "[" primary "]";
        * arguments -> expression ("," expression )*;
        * primary -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | "IDENTIFIER"
        * | "super" "." IDENTIFIER | funExpression | list | dict;
        * funExpression -> "fun" "(" parameters? ")" block;
        * list -> "[" primary ("," primary | IDENTIFIER)* "]";
        * dict -> "%" "{" ((NUMBER | STRING | BOOLEAN) ":" primary)* "}";
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
            if (kind != "method")
                advance();
            var name = consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
            consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
            var parameters = getFuntionParameters();
            consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body.");
            var body = block();
            return new Stmt.Function(name, parameters, body);
        }

        /// <summary>
        /// Helper function intended to parse the parameters of a function statement/expression.
        /// </summary>
        private List<Token> getFuntionParameters()
        {
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
            return parameters;
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
        /// Run handler for break statment. It basically gathers the statement keyword and checks for terminal semicolon.
        /// </summary>
        private Stmt breakStatement()
        {
            var token = previous();
            consume(TokenType.SEMICOLON, "Expect ';' after 'break'.");
            return new Stmt.Break(token);
        }

        /// <summary>
        /// Run handler for continue statment. It basically gathers the statement keyword and checks for terminal semicolon.
        /// </summary>
        private Stmt continueStatement()
        {
            var token = previous();
            consume(TokenType.SEMICOLON, "Expect ';' after 'continue'.");
            return new Stmt.Continue(token);
        }

        /// <summary>
        /// Rule handler for assignment expressions. It treats the right-hand side of the expression as if it were
        /// a binary expression.
        /// </summary>
        private Expression assignment()
        {
            var expression = ternary();
            if (match(
                TokenType.EQUAL, 
                TokenType.PLUS_EQUAL,
                TokenType.MINNUS_EQUAL, 
                TokenType.SLASH_EQUAL, 
                TokenType.STAR_EQUAL,
                TokenType.AND_EQUAL,
                TokenType.OR_EQUAL,
                TokenType.XOR_EQUAL,
                TokenType.L_SHIFT_EQUAL,
                TokenType.R_SHIFT_EQUAL))
            {
                var equals = previous();
                var value = assignment();

                if (expression is Expression.Variable)
                {
                    var name = ((Expression.Variable)expression).Name;
                    return new Expression.Assign(name, value, equals);
                }
                else if (expression is Expression.Get)
                {
                    var get = (Expression.Get)expression;
                    return new Expression.Set(get.Object, get.Name, value, equals);
                }
                else if (expression is Expression.Access)
                {
                    var access = (Expression.Access)expression;
                    return new Expression.Set(access.Member, access.Index, access.Accessor, value, equals);
                }

                error(equals, "Invalid assignment target.");
            }

            return expression;
        }

        /// <summary>
        /// Executes the parsing rule for the "ternary" expression. This is the only expression composed by three subexpressions
        /// (as in most programming languages); also, this is one of the few right associative parsing rules.
        /// </summary>
        private Expression ternary()
        {
            var expression = or();

            while(match(TokenType.QUESTION))
            {
                var left = or();
                consume(TokenType.COLON, "Expect ':' after 'then' result in ternary expression.");
                var right = ternary();
                expression = new Expression.Ternary(expression, left, right);
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
        /// Executes the parsing rule for the "and" operator. This code somehow mirrors the binary expression rule.
        /// </summary>
        private Expression and()
        {
            var expression = bitwiseOr();

            while(match(TokenType.AND))
            {
                var operatr = previous();
                var right = bitwiseOr();
                return new Expression.Logical(expression, right, operatr);
            }

            return expression;
        }

        /// <summary>
        /// Executes the parsing rules for bitwise "| (OR)" operations.
        /// </summary>
        private Expression bitwiseOr()
        {
            var expression = bitwiseXOR();

            while(match(TokenType.BITWISE_OR))
            {
                var operatr = previous();
                var right = bitwiseXOR();
                return new Expression.Binary(expression, right, operatr);
            }

            return expression;
        }

        /// <summary>
        /// Executes the parsing rules for bitwise "^ (XOR)" operations.
        /// </summary>
        private Expression bitwiseXOR()
        {
            var expression = bitwiseAnd();

            while(match(TokenType.BITWISE_XOR))
            {
                var operatr = previous();
                var right = bitwiseAnd();
                return new Expression.Binary(expression, right, operatr);
            }

            return expression;
        }

        /// <summary>
        /// Executes the parsing rules for bitwise "& (AND)" operations.
        /// </summary>
        private Expression bitwiseAnd()
        {
            var expression = equality();

            while(match(TokenType.BITWISE_AND))
            {
                var operatr = previous();
                var right = equality();
                return new Expression.Binary(expression, right, operatr);
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
                if (match(TokenType.MODULE)) return moduleDeclaration();
                if (match(TokenType.CLASS)) return classDeclaration();
                if (check(TokenType.FUN, TokenType.IDENTIFIER)) return function("function");
                if (match(TokenType.VAR)) return varDeclaration();
                return statement();
            }
            catch (ParseError error)
            {
                synchronize();
                return null;
            }
        }

        /// <summary>
        /// Performs the parsing of a class statement. To do so, it checks the structure of the class and
        /// the methods that compose it.
        /// </summary>
        private Stmt classDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect class name before body.");
            Expression.Variable superclass = null;
            if (match(TokenType.LESS))
            {
                consume(TokenType.IDENTIFIER, "Expect superclass name.");
                superclass = new Expression.Variable(previous());
            }
            consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");
            var methods = new List<Stmt.Function>();
            while (!check(TokenType.RIGHT_BRACE) && !isAtEnd())
            {
                methods.Add((Stmt.Function)function("method"));
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");
            return new Stmt.Class(name, methods, superclass);
        }

        /// <summary>
        /// Handles the module stmt production. A module is represented by a name (that will be used to add it into the 
        /// enviroment) and a body (literally a list of statements, since any element of the lang can be part of a module).
        /// </summary>
        private Stmt moduleDeclaration()
        {
            var name = consume(TokenType.IDENTIFIER, "Expect module name before body.");
            consume(TokenType.LEFT_BRACE, "Expect '{' before module body.");
            var body = block();
            return new Stmt.Module(name, body);
        }

        /// <summary>
        /// Handles the using stmt production. A using statement can be represented by the using keyword an a string
        /// that represents the path to another ls file.
        /// </summary>
        private Stmt usingStatement()
        {
            var keyword = previous();
            var path = consume(TokenType.STRING, "Expect 'path' after 'using' statement.");
            consume(TokenType.SEMICOLON, "Expect ';' to close 'using' statement.");
            return new Stmt.Using(keyword, (string)path.Literal);
        }

        /// <summary>
        /// Executes the statement rules.
        /// </summary>
        private Stmt statement()
        {
            if (match(TokenType.USING)) return usingStatement();
            if (match(TokenType.FOR)) return forStatement();
            if (match(TokenType.BREAK)) return breakStatement();
            if (match(TokenType.CONTINUE)) return continueStatement();
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
            var expression = shift();
            while (match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                var operatr = previous();
                var right = shift();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule handler for the shift operations (Non-terminal).
        /// </summary>
        private Expression shift()
        {
            var expression = term();
            while (match(TokenType.R_SHIFT, TokenType.L_SHIFT))
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
        private Expression factor()
        {
            var expression = prefix(); 
            while(match(TokenType.SLASH, TokenType.STAR))
            {
                var operatr = previous();
                var right = prefix();
                expression = new Expression.Binary(expression, right, operatr);
            }
            return expression;
        }

        /// <summary>
        /// Rule handler for the prefix production (Non-terminal).
        /// </summary>
        private Expression prefix()
        {
            if (match(TokenType.PLUS_PLUS, TokenType.MINNUS_MINNUS))
            {
                var operatr = previous();
                var right = primary();
                return new Expression.Unary(operatr, right); 
            }

            return unary();
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
            return postfix();
        }

        /// <summary>
        /// Rule handle for the postfix production (Non-terminal).
        /// </summary>
        private Expression postfix()
        {
            if (matchNext(TokenType.PLUS_PLUS, TokenType.MINNUS_MINNUS))
            {
                var value = primary();
                var operatr = advance();
                return new Expression.Unary(operatr, value, true);
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
        /// Rule handling for access expressions. 
        /// </summary>
        /// <param name="collection">Any collection type (Lists, Dicts).</param>
        private Expression access(Expression collection)
        {
            var index = peek();
            var accessor = or();
            consume(TokenType.RIGHT_BRACKET, "Expect ']' after accessing a collection.");
            return new Expression.Access(collection, accessor, index);
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
                else if (match(TokenType.DOT))
                {
                    var name = consume(TokenType.IDENTIFIER,
                        "Expect property name after '.'.");
                    expression = new Expression.Get(expression, name);
                }
                else if (match(TokenType.LEFT_BRACKET))
                {
                    expression = access(expression);
                }
                else
                {
                    break;
                }
            }

            return expression;
        }

        /// <summary>
        /// Rule handler for list expressions. Once the "[" character is found, the parser begins to parse a list, the rule
        /// keeps on until it can find any commas or until a "]" is reached.
        /// </summary>
        private Expression list()
        {
            var elements = new List<Expression>();

            if (!check(TokenType.RIGHT_BRACKET))
            {
                do
                {
                    if (check(TokenType.RIGHT_BRACKET)) break;
                    elements.Add(or());
                } while (match(TokenType.COMMA));
            }

            consume(TokenType.RIGHT_BRACKET, "Expect ']' to close a list.");
            return new Expression.List(elements);
        }
        
        /// <summary>
        /// Rule handler for dict expressions. Once the % character is found, the parser begins to parse a dict, the rule
        /// keeps on until it reaches the "}" character.
        /// </summary>
        private Expression dictionary()
        {
            consume(TokenType.LEFT_BRACE, "Expected '{' after dictionary declaration.");
            var keys = new List<Expression>();
            var vals = new List<Expression>();
            if (!check(TokenType.RIGHT_BRACE))
            {
                do
                {
                    if (check(TokenType.RIGHT_BRACE)) break;
                    keys.Add(or());
                    consume(TokenType.COLON, "Expect ':' after dictionary key.");
                    vals.Add(or());
                } while (match(TokenType.COMMA));
            }

            consume(TokenType.RIGHT_BRACE, "Expect '}' to close dictionary.");
            return new Expression.Dict(keys, vals);
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

            if (match(TokenType.FUN))
            {
                consume(TokenType.LEFT_PAREN, "Expect '(' after 'fun' expression.");
                var parameters = getFuntionParameters();
                consume(TokenType.LEFT_BRACE, "Expect '{' after 'parameter list.");
                var body = block();
                return new Expression.Function(parameters, body);
            }

            if (match(TokenType.SUPER))
            {
                var keyword = previous();
                consume(TokenType.DOT, "Expect '.' after super.");
                var method = consume(TokenType.IDENTIFIER, 
                    "Expect superclass method name.");
                return new Expression.Super(keyword, method);
            }

            if (match(TokenType.THIS)) return new Expression.This(previous());

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

            if (match(TokenType.LEFT_BRACKET))
            {
                return list();
            }

            if (match(TokenType.PERCENT))
            {
                return dictionary(); 
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
        /// Checks if the token located on the next position matches at least one of the provided types. 
        /// </summary>
        /// <param name="types">A sequence of types to evaluate.</param>
        private bool matchNext(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (checkNext(type))
                {
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
        /// Check overload. Checks if the provided types are in the specified order in the tokens array.
        /// </summary>
        /// <param name="types">Token types intended to check.</param>
        private bool check(params TokenType[] types)
        {
            if (isAtEnd()) return false;
            int lookAheadChars = 0;
            foreach (var type in types)
            {
                if (lookAhead(lookAheadChars).Type == type)
                {
                    lookAheadChars++;
                    continue;
                }

                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the token on the next position has the same type as the provided one.
        /// </summary>
        /// <param name="type">The type that will use as a validation.</param>
        private bool checkNext(TokenType type)
        {
            if (isAtEnd()) return false;
            return lookAhead(1).Type == type;
        }

        /// <summary>
        /// Returns the token located a fixed number of positions away from the current one.
        /// </summary>
        /// <param name="numberOfChars">The number of positions to advance.</param>
        private Token lookAhead(int numberOfChars)
        {
            return tokens[current + numberOfChars];
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
