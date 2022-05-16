using LSharp.Tokens;
using LSharp.GlobalFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSharp.Interpreter
{
    public class Interpreter : Expression.IVisitor<object>,
        Stmt.IVisitor<object>
    {
        /*
         * In order to represent lox's code (LS in this case) we're going to use the built-in C# types.
         * The following table shows how we can map a Lox value into a C# value.
         * Any Lox value -> object.
         * nil -> null.
         * boolean -> boolean.
         * number -> double.
         * string -> string.
         */
        private Enviroment.Enviroment enviroment = new Enviroment.Enviroment();
        private readonly Dictionary<Expression, int> locals = new();
        public Enviroment.Enviroment Globals { get => enviroment; }

        public Interpreter()
        {
            enviroment.Define("clock", new Clock());
        } 

        /// <summary>
        /// Interprets a expression and produce the expected output to the user. In case the provided expression
        /// is invalid, a runtime error will be araised.
        /// </summary>
        /// <param name="statments">The list of statements intedend to be interpreted.</param>
        public void Interpret(List<Stmt> statments)
        {
            try
            {
                foreach (var statement in statments)
                    execute(statement);
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        /// <summary>
        /// Runs the visitor method on a specific statement.
        /// </summary>
        /// <param name="statement">Any instance of the stamement hierarchy.</param>
        private void execute(Stmt statement)
        {
            statement.accept(this);
        }

        /// <summary>
        /// Stores the resolution information (product of static analysis) in a locals map, so it can be used later
        /// while defining variables or re-assigning them.
        /// </summary>
        /// <param name="expression">Any valid expression.</param>
        /// <param name="depth">The depth of the scope that contains the expression.</param>
        public void Resolve(Expression expression, int depth)
        {
            locals[expression] = depth;
        }

        /// <summary>
        /// Turn the list of statements contained in a block into runtime variables. In order to take advantage of the 
        /// existing execute method, we'll provide each statement as its parameter until every statement of the list
        /// has been addressed.
        /// </summary>
        /// <param name="statements">The list of statement product of a block.</param>
        /// <param name="enviroment">The enviroment that contains the List of statements.</param>
        public void ExecuteBlock(List<Stmt> statements, Enviroment.Enviroment enviroment)
        {
            var previous = this.enviroment;
            try
            {
                this.enviroment = enviroment;
                foreach (var statement in statements)
                {
                    try
                    {
                        execute(statement);
                    }
                    catch(ContinueException ex)
                    {
                        break;
                    }
                }
            }
            finally 
            {
                this.enviroment = previous;
            }
        }

        /// <summary>
        /// Performs the dispatch of the Block statement.
        /// </summary>
        /// <param name="stmt">Any block statement.</param>
        public object Visit(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Enviroment.Enviroment(enviroment));
            return null;
        }

        /// <summary>
        /// Turns a Stmt.Class node into a runtime representation. It proceeds to define the name of the class on the current enviroment
        /// and then proceeds to turns its respective methods into runtime representations too (taking advantage of the existing 
        /// function representation), in order to gather the information of the class into a LSClass instance and then assign it to
        /// the class name.
        /// </summary>
        /// <param name="stmt">Any Stmt.Class node.</param>
        public object Visit(Stmt.Class stmt)
        {
            object superclass = null;
            if (stmt.Superclass != null) 
            {
                superclass = evaluate(stmt.Superclass);
                if (!(superclass is LSClass))
                {
                    throw new RuntimeError(stmt.Superclass.Name, 
                        "Superclass must be a class.");
                }
            }
            enviroment.Define(stmt.Name.Lexeme, null);

            if (stmt.Superclass != null)
            {
                enviroment = new Enviroment.Enviroment(enviroment);
                enviroment.Define("super", superclass);
            }

            var methods = new Dictionary<string, LSFunction>();
            foreach (Stmt.Function method in stmt.Methods)
            {
                var functionExpression = new Expression.Function(
                    method.Parameters, method.Body);
                var function = new LSFunction(functionExpression, method.Name.Lexeme, enviroment,
                    method.Name.Lexeme.Equals("init"));
                methods.Add(method.Name.Lexeme, function);
            }

            var loxClass = new LSClass(stmt.Name.Lexeme, 
                methods, (LSClass)superclass);

            if (stmt.Superclass != null)
            {
                enviroment = enviroment.Enclosing;
            }

            enviroment.Assign(stmt.Name, loxClass);
            return null;
        }


        /// <summary>
        /// Turns C# values into loz values to print them.
        /// </summary>
        /// <param name="value">Any lox literal.</param>
        private string stringify(object value)
        {
            if (value == null) return "nil";
            if (value is double)
            {
                var text = value.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                    return text;
                }
            }

            return value.ToString();
        }

        /// <summary>
        /// Executes an assignment expression. If the name provided in the assignment expression is found, its value is updated.
        /// Otherwise an undefined variable runtime exception is raised.
        /// </summary>
        /// <param name="expression">Any assignment expression.</param>
        public object Visit(Expression.Assign expression)
        {
            object value = evaluate(expression.Value);
            int? distance = null;
            if (locals.ContainsKey(expression))
            {
                distance = locals[expression];
            }
            if (distance != null)
            {
                enviroment.AssignAt(distance.Value, expression.Name, value);
            }
            else
            {
                Globals.Assign(expression.Name, value);
            }
            return value;
        }

        /// <summary>
        /// Converts a binary expression into a runtime value.
        /// </summary>
        /// <param name="expression">Any valid lox expression.</param>
        public object Visit(Expression.Binary expression)
        {
            object left = evaluate(expression.Left);
            object right = evaluate(expression.Right);

            switch (expression.Operator.Type)
            {
                case Tokens.TokenType.EQUAL_EQUAL: return isEqual(left, right);
                case Tokens.TokenType.BANG_EQUAL: return !isEqual(left, right);
                case Tokens.TokenType.GREATER:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left > (double)right;
                case Tokens.TokenType.GREATER_EQUAL:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left >= (double)right;
                case Tokens.TokenType.LESS:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left < (double)right;
                case Tokens.TokenType.LESS_EQUAL:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left <= (double)right;
                case Tokens.TokenType.MINNUS:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left - (double)right;
                case Tokens.TokenType.SLASH:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left / (double)right;
                case Tokens.TokenType.STAR:
                    checkNumberOperands(expression.Operator, left, right);
                    return (double)left * (double)right;
                case Tokens.TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;

                    if (left is string && right is string)
                        return (string)left + (string)right;

                    throw new RuntimeError(expression.Operator, 
                        "Operands must be two numbers or strings.");
            }

            return null;
        }

        /// <summary>
        /// Turns a function call into a runtime value.
        /// </summary>
        /// <param name="expression">Any Call expression.</param>
        public object Visit(Expression.Call expression)
        {
            var callee = evaluate(expression.Callee);

            var arguments = new List<object>();
            foreach (var argument in expression.Arguments)
                arguments.Add(evaluate(argument));

            if (!(callee is ICallable))
                throw new RuntimeError(expression.Paren,
                    "Can only call functions and classes.");

            var function = (ICallable)callee;
            if (arguments.Count != function.Arity())
                throw new RuntimeError(expression.Paren,
                    $"Expected {function.Arity()} parameters but got {arguments.Count}");
            
            return function.Call(this, arguments);
        }

        /// <summary>
        /// Turns a get expression (dot call EJ: test.test) into a runtime representation. It checks to determine if the object
        /// that's been called is an actual instance, if so, it proceeds to resolve the field from its respective list of fields;
        /// it raises a runtime error otherwise.
        /// </summary>
        /// <param name="expression">Any valid Get expression.</param>
        public object Visit(Expression.Get expression)
        {
            object obj = evaluate(expression.Object);
            if (obj is LSInstance)
            {
                return ((LSInstance)obj).Get(expression.Name);
            }

            throw new RuntimeError(expression.Name, 
                "Only instances have properties.");
        }

        /// <summary>
        /// Verifies if the operands to a binary expression are valid numbers.
        /// </summary>
        /// <param name="operatr">The operator that represents the operation type.</param>
        /// <param name="left">left hand side of the operator.</param>
        /// <param name="right">right hand side of the operator.</param>
        private void checkNumberOperands(Token operatr, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(operatr, "Operands must be numbers.");
        }

        /// <summary>
        /// Check if the provided values are "equal".
        /// </summary>
        private bool isEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Converts a grouping expression into a runtime value.
        /// </summary>
        /// <param name="expression">Any grouping expression.</param>
        public object Visit(Expression.Grouping expression)
        {
            return evaluate(expression.Expression);
        }

        /// <summary>
        /// Proceeds to recursively evaluate a expression.
        /// </summary>
        /// <param name="expression">Any valid lox expression.</param>
        private object evaluate(Expression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Converts a literal value into a runtime value.
        /// </summary>
        /// <param name="expression">Any literal expression.</param>
        public object Visit(Expression.Literal expression)
        {
            return expression.Value;
        }

        /// <summary>
        /// Convers a unary expression into a ruitime value.
        /// </summary>
        /// <param name="expression">Any Unary expression.</param>
        public object Visit(Expression.Unary expression)
        {
            object right = evaluate(expression.Right);

            switch (expression.Operatr.Type)
            {
                case Tokens.TokenType.BANG:
                    return !isTruty(right);
                case Tokens.TokenType.MINNUS:
                    checkNumberOperand(expression.Operatr, right);
                    return -(double) right;
            }

            return null;
        }

        public object Visit(Expression.Function expression)
        {
            return new LSFunction(expression, "anonymous", enviroment, false); 
        }

        /// <summary>
        /// Checks if the provided object is a valid number.
        /// </summary>
        /// <param name="operatr">The unary operator that will take effect over the operand.</param>
        /// <param name="operand">The operand which type will be checked.</param>
        private void checkNumberOperand(Token operatr, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(operatr, "Operand must be a number.");
        }

        /// <summary>
        /// Verfiies if a value is truty.
        /// </summary>
        /// <param name="obj">Any object that may represent a lox literal.</param>
        private bool isTruty(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool) obj;
            return true;
        }

        /// <summary>
        /// Interprets a Expression statement.
        /// </summary>
        /// <param name="stmt">The expression statement to be executed.</param>
        public object Visit(Stmt.Expr stmt)
        {
            evaluate(stmt.Expression);
            return null;
        }

        /// <summary>
        /// Turns a function declaration into a set of runtime values that is dinamically allocated on a nested enviroment on each
        /// call.
        /// </summary>
        /// <param name="stmt">Any function statement.</param>
        public object Visit(Stmt.Function stmt)
        {
            var functionExpression = new Expression.Function(stmt.Parameters, stmt.Body);
            var function = new LSFunction(functionExpression, stmt.Name.Lexeme, enviroment, false);
            enviroment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        /// <summary>
        /// Interprets a print statement. The process is pretty similar to the common expression statement
        /// but it prints the value to stdout before discarting it.
        /// </summary>
        /// <param name="stmt">The print statement to be executed.</param>
        public object Visit(Stmt.Print stmt)
        {
            object value = evaluate(stmt.Expression);
            Console.WriteLine(stringify(value));
            return null;
        }

        /// <summary>
        /// Interprets a return statement. It throws an exception to clean the call stack and provided the value that should
        /// return from the function where it is contained within such exception.
        /// </summary>
        /// <param name="stmt">Any return statement.</param>
        public object Visit(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.Value != null) value = evaluate(stmt.Value);

            throw new Return(value);
        }

        /// <summary>
        /// Proceeds to define a variable into the enviroment. If the provided statements contains an initialization,
        /// the provided value is evaluated and the result is tied up to the variable name. Otherwise, nil is stored 
        /// under such variable.
        /// </summary>
        /// <param name="stmt">Variable statement.</param>
        public object Visit(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.Initializer != null)
            {
                value = evaluate(stmt.Initializer);
            }

            enviroment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        /// <summary>
        /// Evaluates a Variable expression. Simply finds a variable on the enviroment and then returns it.
        /// </summary>
        /// <param name="expression">A expression variable.</param>
        public object Visit(Expression.Variable expression)
        {
            return lookUpVariable(expression.Name, expression);
        }

        /// <summary>
        /// Checks if variable is defined in our local scope. If it is, then we proceed to resolve its value from the local
        /// enviroment. If the variable is not part of our local scope, it means that such variable is global, hence we 
        /// resolve it dynamically from the global scope.
        /// </summary>
        /// <param name="name">Any identifier token.</param>
        /// <param name="expression">Any valid expression.</param>
        private object lookUpVariable(Token name, Expression expression)
        {
            int? distance = null;
            int value;
            var result = locals.TryGetValue(expression, out value);
            if (result) distance = value;
            if (locals.ContainsKey(expression)) distance = locals[expression];
            if (distance != null)
            {
                return enviroment.GetAt(distance.Value, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        /// <summary>
        /// Evaluates an if stament using native DotNet if. If the provided condition is truty, we proceed to execute the 
        /// then branch of the statement, otherwise we execute the else part of the statement (in case it was provided).
        /// </summary>
        /// <param name="stmt">The If statement to evaluate.</param>
        public object Visit(Stmt.If stmt)
        {
            if (isTruty(evaluate(stmt.Condition)))
            {
                execute(stmt.ThenBranch);
            }
            else if (stmt.ElseBranch != null)
            {
                execute(stmt.ElseBranch);
            }
            return null;
        }

        /// <summary>
        /// Turns a ternary expression into a runtime representation. This is one of the cases where the underline implementation
        /// mirrors the semantics of the language.
        /// </summary>
        /// <param name="expression">The ternary expression to evaluate.</param>
        public object Visit(Expression.Ternary expression)
        {
            return isTruty(evaluate(expression.Condition)) ? evaluate(expression.Left) : evaluate(expression.Right); 
        }

        /// <summary>
        /// Evaluates the components of a logical expression and applies short circuit if posible. The short circuit evaluation
        /// logic depends on the operator type of the expression.
        /// </summary>
        /// <param name="expression">Any logical expression.</param>
        public object Visit(Expression.Logical expression)
        {
            var left = evaluate(expression.Left);

            if (expression.Operatr.Type == TokenType.OR)
            {
                if (isTruty(left)) return left;
            }
            else
            {
                if (!isTruty(left)) return left;    
            }

            return evaluate(expression.Right);
        }

        /// <summary>
        /// Turns an set expression into a runtime representation. Performs a check similar to the one performed on get expressions,
        /// in order to determine if the object attached to the expression is an instance of a class. If so, it resolves the value
        /// that will be assigned to such instance and adds/updates it on the fields map of said instance.
        /// </summary>
        /// <param name="expression">Any valid set expression.</param>
        public object Visit(Expression.Set expression)
        {
            object obj = evaluate(expression.Object);
            if (!(obj is LSInstance))
            {
                throw new RuntimeError(expression.Name, 
                    "Only instances have fields.");
            }

            object value = evaluate(expression.Value);
            ((LSInstance)obj).Set(expression.Name, value);
            return value;
        }

        /// <summary>
        /// Turns a super expression into a runtime representation. To do so checks if the expression is contained
        /// in the local enviroment, if so proceeds to get its value in order to bind it to the instance of its respective
        /// subclass and resolve methods called on the highest element of the class hierarchy.
        /// </summary>
        /// <param name="expression">The super expression to be resolved.</param>
        public object Visit(Expression.Super expression)
        {
            var distance = locals[expression];
            var superclass = (LSClass)enviroment.GetAt(distance, "super");
            var instance = (LSInstance)enviroment.GetAt(distance - 1, "this");
            var method = superclass.FindMethod(expression.Method.Lexeme);
            if (method == null)
            {
                throw new RuntimeError(expression.Method, 
                    $"Undefined property '{expression.Method.Lexeme}'.");
            }
            return method.Bind(instance);
        }

        /// <summary>
        /// Turns a this expression into a runtime representation. 
        /// </summary>
        /// <param name="expression">Any this expression.</param>
        public object Visit(Expression.This expression)
        {
            return lookUpVariable(expression.Keyword, expression);
        }

        /// <summary>
        /// Executes a while statement using the built-in while. The loop will run until the provided condition is not truty anymore.
        /// </summary>
        /// <param name="stmt">Any while loop.</param>
        public object Visit(Stmt.While stmt)
        {
            while (isTruty(evaluate(stmt.Condition)))
            {
                try
                {
                    execute(stmt.Body);
                }
                catch(BreakException ex)
                {
                    break;
                }
            }
            return null;
        }

        /// <summary>
        /// Turns a break stmt into a runtime representation. Since break is intended to jump out of the context of the loop where it's
        /// called, it must be handled as an exception so it can move one step back on DotNet's execution stack.
        /// </summary>
        /// <param name="stmt">The break statement that should be turn into a runtime value.</param>
        public object Visit(Stmt.Break stmt)
        {
            throw new BreakException();
        }

        /// <summary>
        /// Turns a continue stmt into a runtime representation. It works faily similar to the break statement, so it's also implemented
        /// using exceptions.
        /// </summary>
        /// <param name="stmt">The continue statement intended to be interpreted.</param>
        public object Visit(Stmt.Continue stmt)
        {
            throw new ContinueException();
        }

    }
}
