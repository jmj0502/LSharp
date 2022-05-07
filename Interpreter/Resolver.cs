using System;
using System.Collections.Generic;
using LSharp.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Resolver : 
        Stmt.IVisitor<object>, Expression.IVisitor<object>

    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();
        private FunctionType currentFunction = FunctionType.NONE;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }

        private enum ClassType
        {
            NONE,
            CLASS
        }

        private ClassType currentClass = ClassType.NONE;

        /// <summary>
        /// Performs static analysis over a sequence of statements derived from our parser.
        /// </summary>
        /// <param name="statements">The list of statements to be resolved.</param>
        public void Resolve(List<Stmt> statements)
        {
            foreach (var statement in statements)
                resolve(statement);
        }

        /// <summary>
        /// Performs static analysis on a block of code. It generates an new scope that will hold 
        /// all the information resolved from its inner statements.
        /// </summary>
        /// <param name="stmt">The block statement to be resolved.</param>
        public object Visit(Stmt.Block stmt)
        {
            beginScope();
            Resolve(stmt.Statements);
            endScope();
            return null;
        }

        /// <summary>
        /// Performs static analysis on a class statement. To do so, we first store the current class scope (so we can re-establish it
        /// once we're done parsing this class), then we proceed to declare and define the name of the class and create a new scope
        /// that will contain the definition of the 'this' keyword that belongs to this specific class; once that's done, it proceeds to
        /// resolve each method of the class (including its constructor)  and then resolves back to the outer scope.
        /// </summary>
        /// <param name="stmt">The class stmt to be resolved.</param>
        public object Visit(Stmt.Class stmt)
        {
            var enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            declare(stmt.Name);
            define(stmt.Name);

            beginScope();
            scopes.Peek()["this"] = true;

            foreach (var method in stmt.Methods)
            {
                var declaration = FunctionType.METHOD;
                if (method.Name.Lexeme.Equals("init"))
                {
                    declaration = FunctionType.INITIALIZER;
                }
                resolveFunction(method, declaration);
            }

            endScope();

            currentClass = enclosingClass;
            return null;
        }

        /// <summary>
        /// Performs static analysis over a expression statement.
        /// </summary>
        /// <param name="stmt">The expression statement to be resolved.</param>
        public object Visit(Stmt.Expr stmt)
        {
            resolve(stmt.Expression);
            return null;
        }

        /// <summary>
        /// Performs static analysis on a function statement. Declares, defines and ties the resolved function body and parameters
        /// with the function's name.
        /// </summary>
        /// <param name="stmt">The function statement to be resolved.</param>
        public object Visit(Stmt.Function stmt)
        {
            declare(stmt.Name);
            define(stmt.Name);

            resolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }

        /// <summary>
        /// Performs static analysis on an if statement. In contrast with the logic intended to execute the if statement, 
        /// this method performs resolution for the if condition and each branch of the statement (then and else).
        /// </summary>
        /// <param name="stmt">The if statement to be resolved.</param>
        public object Visit(Stmt.If stmt)
        {
            resolve(stmt.Condition);
            resolve(stmt.ThenBranch);
            if (stmt.ElseBranch != null) resolve(stmt.ElseBranch);
            return null;
        }

        /// <summary>
        /// Performs static analysis on print statements. The resolution process somehow resembles the expresion statements
        /// resolution.
        /// </summary>
        /// <param name="stmt">The print statement to be resolved.</param>
        public object Visit(Stmt.Print stmt)
        {
            resolve(stmt.Expression);
            return null;
        }

        /// <summary>
        /// Performs static analysis on a return statement. It resolves the value tied to the statement (if it isn't null).
        /// Throws an error is resolved statement is not inside the body of a function.
        /// </summary>
        /// <param name="stmt">The return statement to be resolved.</param>
        public object Visit(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.Keyword,
                    "Can't return from top-level code.");
            }

            if (stmt.Value != null)
            {
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(stmt.Keyword, 
                        "Can't return a value from an initializer.");
                }
                resolve(stmt.Value);
            }
            return null;
        }

        /// <summary>
        /// Performs static analysis on var statements. It declares the variable, proceeds to resolve its initalizer value (if any) and 
        /// the defines it.
        /// </summary>
        /// <param name="stmt">The statement variable to be resolved.</param>
        public object Visit(Stmt.Var stmt)
        {
            declare(stmt.Name);
            if (stmt.Initializer != null)
                resolve(stmt.Initializer);
            define(stmt.Name);
            return null;
        }

        /// <summary>
        /// Performs static analysis on a while statement. Similar to what happends with if statements, it resolves the main 
        /// condition of the loop and then proceeds to resolve its body exactly one time.
        /// </summary>
        /// <param name="stmt">The while statement to be resolved.</param>
        public object Visit(Stmt.While stmt)
        {
            resolve(stmt.Condition);
            resolve(stmt.Body);
            return null;
        }

        /// <summary>
        /// Resolves a variable expression (adds its information to the enviroment). First verifies if the variable that belongs
        /// to the scope being assigned to its own initializer; if so it throws an error, else proceeds to resolve it into the scope.
        /// </summary>
        /// <param name="expression">The variable expression to be resolved.</param>
        public object Visit(Expression.Variable expression)
        {
            if (scopes.Count > 0 &&
                scopes.Peek().ContainsKey(expression.Name.Lexeme) &&
                scopes.Peek()[expression.Name.Lexeme] == false)
                Lox.Error(expression.Name, 
                    "Can't read local variable in its own initializer.");
            resolveLocal(expression, expression.Name);
            return null;
        }

        /// <summary>
        /// Resolves an assignment expression. First resolves the value that will be assigned to the variable and then 
        /// proceeds to resolve such variable in the local scope.
        /// </summary>
        /// <param name="expression">The assignment expression to be resolved.</param>
        public object Visit(Expression.Assign expression)
        {
            resolve(expression.Value);
            resolveLocal(expression, expression.Name);
            return null;
        }

        /// <summary>
        /// Resolves each component of a binary expression.
        /// </summary>
        /// <param name="expression">The binary expression to be resolved.</param>
        public object Visit(Expression.Binary expression)
        {
            resolve(expression.Left);
            resolve(expression.Right);
            return null;
        }

        /// <summary>
        /// Resolves the callee and each one argument its arguments out of a call expression.
        /// </summary>
        /// <param name="expression">The call expression to be resolved</param>
        public object Visit(Expression.Call expression)
        {
            resolve(expression.Callee);

            foreach (var argument in expression.Arguments)
                resolve(argument);

            return null;
        }

        /// <summary>
        /// Resolves the value of the provided get expression.
        /// </summary>
        /// <param name="expression">The Get expression to be resolved.</param>
        public object Visit(Expression.Get expression)
        {
            resolve(expression.Object);
            return null;
        }

        /// <summary>
        /// Resolves the inner member of a grouping expression.
        /// </summary>
        /// <param name="expression">the grouping expression to be resolved.</param>
        public object Visit(Expression.Grouping expression)
        {
            resolve(expression.Expression);
            return null;
        }

        /// <summary>
        /// Resolves a literal expression. Literall returns null, since there are no variables to be defined in any scope.
        /// </summary>
        /// <param name="expression">The literal expression to be resolved.</param>
        public object Visit(Expression.Literal expression)
        {
            return null;
        }

        /// <summary>
        /// Resolves the members of a logical expression. Since no logic at all is executed, any logical expression is basically
        /// resolved as if it was a binary expression.
        /// </summary>
        /// <param name="expression">The logical expression to be resolved.</param>
        public object Visit(Expression.Logical expression)
        {
            resolve(expression.Left);
            resolve(expression.Right);
            return null;
        }

        /// <summary>
        /// Performs static analysis on Set expression. In order to do so, the value of that will be assigned to the expression and the
        /// value of the expression itself are resolved.
        /// </summary>
        /// <param name="expression">Any valid set expression.</param>
        public object Visit(Expression.Set expression)
        {
            resolve(expression.Value);
            resolve(expression.Object);
            return null;
        }

        /// <summary>
        /// Performs static analysis on a this expression. Verifies if the this 'expression' is being used on the right enviroment
        /// (inside a class) and then proceeds to resolve it as any other variable.
        /// </summary>
        /// <param name="expression">The This expression to be resolved.</param>
        public object Visit(Expression.This expression)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expression.Keyword, 
                    "Can't use 'this' outside a class.");
                return null;
            }

            resolveLocal(expression, expression.Keyword);
            return null;
        }

        /// <summary>
        /// Resolves the right member of a unary expression.
        /// </summary>
        /// <param name="expression">The unary expression to be resolved.</param>
        public object Visit(Expression.Unary expression)
        {
            resolve(expression.Right);
            return null;
        }

        /// <summary>
        /// Proceeds to call the accept method of the provided statement.
        /// </summary>
        /// <param name="statement">Any statement of our stmt tree.</param>
        private void resolve(Stmt statement)
        {
            statement.accept(this);
        }

        /// <summary>
        /// Proceeds to call the accept method of the provided expression.
        /// </summary>
        /// <param name="expression">Any expression of our expression tree.</param>
        private void resolve(Expression expression)
        {
            expression.Accept(this);
        }

        /// <summary>
        /// Resolves a function parameters and body. It also checks if the function in question 
        /// is contained on another function; if it is, a return keyword wil be deamed valid 
        /// once the body of the function is resolved (it will throw an error otherwise). 
        /// </summary>
        /// <param name="function">The function statement to be resolved.</param>
        /// <param name="type">The type of the element that contains the function.</param>
        private void resolveFunction(Stmt.Function function,
            FunctionType type)
        {
            var enclosingFunction = currentFunction;
            currentFunction = type;
            beginScope();
            foreach (var parameter in function.Parameters)
            {
                declare(parameter);
                define(parameter);
            }
            Resolve(function.Body);
            endScope();
            currentFunction = enclosingFunction;
        }

        /// <summary>
        /// Adds a new element on top of the scopes stack.
        /// </summary>
        private void beginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        /// <summary>
        /// Removes the top element from the scopes stack.
        /// </summary>
        private void endScope()
        {
            scopes.Pop();
        }

        /// <summary>
        /// Initializes a variable on the map at the top level of the stack.
        /// </summary>
        /// <param name="name">The token that contains the identifier associated with a specific varible.</param>
        private void declare(Token name)
        {
            if (scopes.Count == 0) return;

            var scope = scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, 
                    "There is already a variable with this name in this scope.");
            }
            scope[name.Lexeme] = false;
        }

        /// <summary>
        /// Defines a variable on the top level of the stack by setting its initialization value to true.
        /// </summary>
        /// <param name="name">The token the represents the variable to be defined.</param>
        private void define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Peek()[name.Lexeme] = true;
        }

        /// <summary>
        /// Goes through each element of the stack, in order to provide resolution information to the interpreter.
        /// </summary>
        /// <param name="expression">The expression that represents the value to be resolved into the interpreter.</param>
        /// <param name="name">The tokens that carries the information of the variable to be resolved.</param>
        private void resolveLocal(Expression expression, Token name)
        {
            var orderedStack = scopes.ToArray().Reverse().ToList();
            for (int i = orderedStack.Count() - 1; i >= 0; i--)
            {
                if (orderedStack[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expression, orderedStack.Count() - 1 - i);
                    return;
                }
            }
        }
    }
}
