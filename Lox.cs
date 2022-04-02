using System;
using static System.Console;

namespace LSharp
{
    class Lox
    {
        private static bool HadErrors = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                WriteLine("Usage: ls [script]");
                Environment.Exit(1);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        ///<summary>
        /// Method intended to parse a file if LS is executed with a path to a lox file as a parameter.
        ///</summary>
        private static void RunFile(string path) 
        {
            var source = System.IO.File.ReadAllText(path);
            Run(source);
            if (HadErrors) Environment.Exit(1);
        }

        /// <summary>
        /// Method intended to be executed if the interpreter is used via REPL.
        /// </summary>
        private static void RunPrompt()
        {
            for (; ; )
            {
                Write(">");
                var line = ReadLine();
                if (line == "exit") break;
                Run(line);
                HadErrors = false;
            }
        }

        /// <summary>
        /// Method intended to begin the execution of the code, by providing the received text input (from both a file or REPL) 
        /// to the Scanner.
        /// </summary>
        /// <param name="source">The string that represents the source code of our app. Not nullable.</param>
        private static void Run(string source)
        {
            var scaner = new LSharp.Scanner.Scanner(source);
            var tokens = scaner.ScanTokens();

            foreach (var token in tokens)
                WriteLine(token);
        }

        /// <summary>
        /// Method intended to provide error information to the Report method. It can be percived as a decorator around the 
        /// report method.
        /// </summary>
        /// <param name="line">Line number where the error was found.</param>
        /// <param name="message">Message intended to notify a user about its errors.</param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        /// <summary>
        /// Method intended to write to the console error messages.
        /// </summary>
        /// <param name="line">Line number where the error was found.</param>
        /// <param name="where">Scope where the error took place.</param>
        /// <param name="message">Message intented to nofity the user about its errors.</param>
        private static void Report(int line, string where, string message)
        {
            WriteLine($"[line {line}] Error {where}: {message}");
            HadErrors = true;
        }
    }

    class Tester
    {
        static void Main(string[] args)
        {
            var expression = new Expression.Binary(
                    new Expression.Unary(
                        new Tokens.Token(Tokens.TokenType.MINNUS, "-", null, 1),
                        new Expression.Literal(123)
                ),
                    new Expression.Grouping(new Expression.Literal(48.67)),
                    new Tokens.Token(Tokens.TokenType.STAR, "*", null, 1));
            var expressionTwo = new Expression.Binary(
                    new Expression.Grouping(
                            new Expression.Binary(
                                new Expression.Literal(1),
                                new Expression.Literal(2),
                                new Tokens.Token(Tokens.TokenType.PLUS, "+", null, 1)
                                )
                        ),
                    new Expression.Grouping(
                        new Expression.Binary(
                            new Expression.Literal(4),
                            new Expression.Literal(5),
                            new Tokens.Token(Tokens.TokenType.MINNUS, "-", null, 1)
                            )
                        ),
                    new Tokens.Token(Tokens.TokenType.STAR, "*", null, 1)
                    );
            WriteLine(new AstPrinter().Visit(expression));
            WriteLine(new RPNPrinter().Visit(expressionTwo));
        }
    }
}
