# The L# Programming Language

The `L#` programming language is a superset of the [Lox Programming Language](https://github.com/munificent/craftinginterpreters). The project started as
as a Lox implementation written in `C#`. However, after finishing the first part of [Crafting Interpreters And Compilers](http://www.craftinginterpreters.com/)
(the tree-walk interpreter project) I decided to give [@Munificent](https://github.com/munificent)'s challenge a shot and try to come up with features that I
thought `Lox` is missing.

Despite the fact that this project is still a work in progress, it feels like a huge achievement. I've always been interested in language programming, mainly because I 
always wondered how the programing languages that I use on a daily basis interact with the different components of my machine (I know that this implementation
only scratch the surface of what can be done but still). On the other hand, language design results particularly interesting to me; on my spare time I like to write
multiple implementations of a program using different programming languages since I really like to contrast the approach taken in each one of them to achieve the
same goal.

When I discovered **Crafting Interpreters And Compilers** I was truly amazed! The book had everything I needed to learn in order to get started on this
field and I'm truly grateful for that, in fact the book is so good that I was able to understand the key concepts behind scanners, grammars, parsers and even some 
design decision that led to interesting features in popular languages. Before discovering **CIAC** the only compiler design book I knew about was the Dragon Book and that 
wasn't a good place to start my journey in the field of language programming (I'm still planning to tackle it in the future tho'), mainly because I didn't even have a clear
idea of the different components involved in this kind of project.

## How to build from source

The only requirement to build this language is `.NET 6.0.x`. Right now I'm not using any dependencies so the process is really easy (it's
just building a common CLI with `.NET`).

## Scope

L# aims to become a multi-paradigm scripting language that can be used to generate scripts that interact with the `File System`, gather
data from a resource hosted on the cloud (through HTTP) or both. Right now, I don't think the language can be used for extremely complex tasks 
(That may change as the language improves), but I would like to see if its use cases actually go beyond the fields of application I initially ambitioned.

## Features

`L#` supports all the features supported by `Lox`, so let's start by reviewing those:

### Lox supports variables, functions and closures.
```lox
var someString = "something";
print someString; // something.
var nilVar;

fun add(a, b) {
  return a + b;
}

print add(2, 1); // 3.

fun findMiddlePoint(x1, x2) {
  fun add(a, b) {
    return a + b;
  }

  return add(x1, x2) / 2;
}
```

### Lox has lexical blocks.
```lox
var a = "global a";
{
  var a  = "inner a";
  print a; // inner a.
}
print a; // global a.
```

### Lox supports logical operators.
```lox
var a = true;
var b = false;

if (a and b) {
  print "Truee.";
} else {
  print "Falsee.";
}

if (a or b) {
  print "Partially true.";
} else {
  print "Again, false.";
}

if (a and !b) {
  print "True.";
} else {
  print "False.";
}
```

### Lox has different control flow mechanisms.
```lox
var boolean = true;

if (boolean) {
  print "true!";
} else {
  print "false";
}

var counter = 0;
while (counter < 5) {
  print counter;
  counter = counter + 1;
}

for (var i = 0; i < 10; i = i + 1) {
  print i;
}
```

### Lox supports OOP.
```lox
// Basic class.
class Breakfast {
  cook() {
    print "Eggs a-fryin'!";
  }

  serve(who) {
    print "Enjoy your breakfast, " + who + ".";
  }
}

var breakfast = Breakfast();
breakfast.cook(); // Eggs a fryin'!
breakfast.serve("Bob"); // Enjoy your breakfast, Bob.

// Class with a constructor.
class Dog {
  init(name, age) {
    this.name = name;
    this.age = age;
  }

  pet() {
    print this.name + " is happy!";
  }

  howOld() {
    if (age < 14) {
      print this.name + " is quite young.";
    } else {
      print this.name + " is getting old.";
    }
  }
}

var fiddo = Dog("fiddo", 5);
fiddo.name(); // fiddo.
fiddo.pet(); // fiddo is happy.
fiddo.howOld(); // fiddo is quite young.

// Inheritance.
class Bread {
  init(kind) {
    this.kind = kind;
  }

  choosenBread() {
    print "You choose " + kind + " as your desired kind of bread!";
  }
}

class Bagget < Bread {
  init(kind, crunchy) {
    super(kind);
    this.cruncry = crunchy;
  }

  isCrunchy() {
    if (crunchy) {
      print "This is a crunchy " + this.kind + "!";
    } else {
      print "This a not-so crunchy " + this.kind + ".";
    }
  }
}
```

## Now let's take a look at `L#` features.

### L# supports single-line and multi-line comments.
```
// single line comment.
fun testFunction() {
  /*
   Multi-line
   comment.
  */
  print "test";
}
```
**NOTE**: multi-line comments can't be nested (C, Java, C#, JS, TS and Rust doesn't allow such behaviour, so I decided to do the same).

### L# support postfix and prefix increment/decrement.
```javascript
var number = 0;
number++; // 1.
++number; // 2.
number--; // 1.
--number; // 0.
```

### L# has ternary expressions.
```
var boolean = true;
var test = boolean ? "true" : "false";
var something = boolean ? "true" : 4 < 5 ? "false" : "something";
```
Ternary expressions are right-associative (as they are in most programming languages).

### L# has list and dictionaries.
```
var list = [1,2,3,4];
print list[0]; // 1 
list[1] = 3;
print list[1]; // 3

var dictionary = %{"test": "string", 5: "five", true: false};
print dictionary["test"]; // string
dictionary["some"] = "thing";
print dictionary["some"]; // thing
```

### L# supports F#/Elixir's pipe operator.
```
fun sum(a, b) {
  return a + b;
}

fun multiply(a, b) {
  return a * b;
}

fun divide(a, b) {
  return a / b;
}

print sum(2,2) |> multiply(2) |> divide(2); // 4
```

The pipe operator is just syntactic sugar for function composition. The following expression `f(g(r(x)))` can be turned into
`r(x) |> g() |> f()`. In simple terms, the pipe operator will turn a value into the first positional parameter
of the next function on the pipe chain. If a function takes more than one parameter, each one of them should be provided in the
appropriate order.

### L# supports hex notation for numbers and bitwise operations.
```javascript
var hex = 0x10; // 16.
print hex >> 1; // 8.
print hex << 1; // 32.
print 0x11 & hex; // 16. 
print 0x11 | hex; // 17.
print 0x11 ^ hex; // 1.
```

### L# has function expressions.
```
var testFun = fun (test) {
  print test;
};

testFun("Hello, yo'"); // Hello, yo'.

fun increment(number, callback) {
  return callback(number);
}

var result = increment(5, fun (number) {
  return number + 1;
});
print result; // 6. 
```

### L# has support for modules (inspired by Ruby, ML modules and C# namespaces).
```
module Example {
  var moduleConstant = "constant";

  fun exampleFunction() {
    print "module function";
  }

  class ModuleClass {
    module() {
      print "example";
    }
  }
}
```

Modules are basically an environment, so variables, classes, and functions can be defined inside them.
Modules allow you to avoid namespace collisions and to efficiently encapsulate the behavior related to a 
certain functionality.

### L# supports multi-file imports.
```
// test.ls
module Example {
  var moduleConstant = "constant";

  fun exampleFunction() {
    print "module function";
  }

  class ModuleClass {
    module() {
      print "example";
    }
  }
}

// usingTest.ls
using "./test.ls";

print Example.moduleConstant; // constant.
```

The symbols of a `Lox` file can also be imported from `L#`. However, you should always try to keep your code
organized in modules if you want to do so. `Lox` doesn't have a way to group code, so all the symbols imported from a `Lox` file
will be defined on the global scope.

```
// test.lox
fun testFunction() {
  print "test function";
}

// usingTest.ls
using "./test.lox";

testFunction(); // test function.
```

### L# has support for `break` and `continue` statements.
```
fun printNumbersFromOneToFive() {
    for (var i = 1; i < 10; i++) {
      print i;
      if (i == 5) {
        break;
      }
    }
}

printNumbersFromOneToFive()
// 1.
// 2.
// 3.
// 4.
// 5.

fun skipNumbers() {
  for (var i = 0; i < 10; i++) {
     if (i == 5 or i == 7) {
       continue;
     }
     print i;
  }
}

skipNumbers();
// 1.
// 2.
// 3.
// 4.
// 6.
// 8.
// 9.
```

### L# supports error propagation using result values and the error propagation operator `??`.
```
fun div(a, b) {
  if (b == 0) return Err("Can't divide by 0");
  return Ok(a/b);
}

fun sum(a, b) {
  return a + b;
}

fun divideAndSum(a, b) {
  // If an error is returned the function will return such error, else it will extract the value from ok. 
  return div(a, b)??
  |> sum(b); 
}

// The ?? operator can also be used at top-level. If an error is 
// returned, a runtime error will take place.
divideAndSum(2,2)??; 
```

The error propagation approach is inspired by Rust's `Result` enum. There are other useful ways to deal with `Result`s 
on the standard library, keep reading in order to find out about all of them.

### L# also has a standard library composed by native modules.
`L#`'s standard library is composed of different modules that expose functions that can be used to perform multiple operations
on the different data types available, and the file system. This approach was inspired by languages like F# and Elixir.
However, I'm planning to add native classes as well (for Strings, FS, etc.), since that will allow users to take the approach
they like/need the most (I would really like `L#` to be truly multiparadigm).

Let's explore some of the native modules!
#### String module.
Allow users to perform string manipulation.
```
String.reverse("hello") // "olleh".
String.substring("something else", 1); // "omething else".
String.trim(" test ") // "test".
String.len("hello") // 5.
String.toUpper("Something") // "SOMETHING".	
String.toLower("SCREEAM") // "screeam".
String.startsWith("test string", "te") // true.
String.slice("test", 0, 2) //  "tes".
String.endsWith("test string", "string") // true.
String.contains("something in the way", "in") // true.
String.indexOf("hello", "o") // 4.
String.at("hello", 0) // "h".

// String.match: The `match` returns a list of matches based on the provided pattern. It takes the following parameters:
// 1. The string to match against.
// 2. The regex pattern we'll use to perform the match operation (expressed as a string).
// 3. A dictionary that will contain additional options (`insensitive` is the only supported option right now).
String.match("test 123", "[0-9]+", %{}); // ["123", "123"]
String.match("Xd123456789", "(XD)([0-9]{8,})", %{"insensitive": true}); // ["Xd123456789", "Xd", "123456789"].

// String.replace: `replace` and `match` take similar parameters.
// `replace` takes:
// 1. The string that will be modified.
// 2. The pattern that will be replaced.
// 3. The string that will replace the specified pattern.
// 4. A dictionary containing addittional options (`insensitive` is the only valid option right now).
String.replace("The fox jumped over the bridge", "(fox)", "dog", %{}); // The dog jumped over the bridge.
String.replace("The Rabbit dug a hole.", "(rabbit)", "mole", %{"insensitive": true}); // The mole dug a hole.
```

#### Result module.
Allow users to perform better error handling on `Result` types.
```
fun divide(a, b) {
  if (b == 0) return Err("Can't divide by 0");
  return Ok(a/b);
}

// Result.unwrapOrElse: Extracts the value contained on Ok() and returns it, allow the user the perform error handling otherwise.
// The function takes two parameters.
// 1. A Result (could be either Ok or Err).
// 2. A function expression that will be used to perform error handling.
var number = divide(1,0) 
|> Result.unwrapOrElse(fun (e) {
  print e;  // Will print the error message. In this case: "Can't divide by 0". 
  return 0; // Will return 0; hence number will equal 0. NOTE: If no value is returned, number will be equal to nil.
});

// Result.unwrap: Extracts the value contained on OK() an returns it, throws a runtime error otherwise.
// Takes one parameter:
// 1. A Result(could be either Ok or Err).
var number = divide(2,0) |> Result.unwrap(); // Runtime error. Can't divide by 0.

// Result.isOk: Checks if the provided Result is an OK Result.
// Takes one parameter:
// 1. A Result(could be either Ok or Err).
var number = divide(2,2) |> Result.isOk(); // true.

// Result.isErr: Checks if the provided Result is an Err Result.
// Takes one parameter:
// 1. A Result(could be either Ok or Err).
var number = divide(4,0) |> Result.isErr(); // true.
```

#### List module.
Allow users to perform list manipulation.
```
List.len([1,2,4]); // 3.
List.reverse([4,3,2]); // [2,3,4].
List.contains([1,2,4], 4); // true.
List.indexOf([1,2,3]); // 1.
List.join([1,2,3], "*"); // "1*2*3".
List.add([1,2,3], 4); // Adds an element to the end on the list. Hence, the list provided as parameter will now contain [1,2,3,4].
List.prepend([1,2,3,4], 0); // Adds an element to the beginning of the list. Hence, the list provided as parameter will now contain [0,1,2,3,4].
List.concat([1,2,3], [4,5,6]); // [1,2,3,4,5,6].
List.removeFirst([1,2,3]); // Mutates the original list and removes its first element. The list provided as parameter will now contain [2,3].
List.removeLast([1,2,3]); // Mutates the original list and removes its last element. The list provided as parameter will now contain [1,2].
List.removeAt([1,4,2,3], 1); // Mutates the original list and removes the element at the specified index from the list.The list provided as parameter will now contain [1,2,3].
List.getFirst([1,2,3]); // Returns the first element of a list. In this case 1.
List.getLast([1,2,3]); // Returns the last element of the list. In this case 3.
List.at([1,2,3], 1); // Returns the element located on the specied index. In this case 2.

// List.slice: Creates a sublist based on the provided list.
// Takes the following parameters:
// 1. The list that will be used to create the slice.
// 2. The beginning index of the slice.
// 3. The ending index of the slice.
List.slice([1,2,3], 1, 2); // [2,3].

// List.insert: Inserts an element at the specified index.
// Takes the following parameters:
// 1. The list that will be modified.
// 2. The index where the new element will be placed.
// 3. The element that will be added to the list.
// NOTE: Mutates the original list.
List.insert([1,3,4], 1, 2); // [1,2,3,4].

// List.each: Iterates over each element of a list and perform the specied action on each one of them.
// Takes the following parameters:
// 1. The list that will be mutated.
// 2. A function expression that represents the operation that will be performed on each list element.
var exampleList = [1,2,3]
|> List.each(fun (el) {
   print el; // 1, 2 and 3 respectively.
   return el + 1;
});
print exampleList; // [2,3,4].

// List.map: Iterates over each element of a list and generates a new list containing the modified elements.
// Takes the following parameters:
// 1. The list that will be used to generate the updated list.
// 2. A fuction expression that will operate on each provided element.
var baseList = [1,2,3,4];
var updatedList = baseList 
|> List.map(fun (el) {
  return el + 1;
});
print baseList; // [1,2,3,4].
print updatedList; // [2,3,4,5].

// List.filter: Iterates over each element of a list and returns a new list containing the elements that meet the specified
// condition.
// Takes two parameters:
// 1. The list that will be used to generate the updated list.
// 2. A function expression that will check for a certain condition.
var numbers = [1,2,3,4];
var numbersGreaterThanOne = numbers 
|> List.filter(fun (el) {
  return el > 1;
});
print numbersGreaterThanOne; // [2,3,4].

// List.reduce: Reduce the elements of a list into a single element based on the provided callback function.
// Takes three parameters:
// 1. The list that will be used to perform the reduce operation (won't be mutated).
// 2. A function expression that takes two positional parameters (a reference to the accumulator and a reference to the current element).
// 3. The base value of the accumulator.
var numbers = [1,2,3,4];
var result = numbers 
|> List.reduce(fun (acc, curr) { return acc + curr; }, 0);
print result; // 10.

// List.sort: Sorts the elements of the provided list (mutates the original list).
// Takes two parameters:
// 1. The list that will be sorted.
// 2. An optional function expression that should be provided only if the list is composed by non-primitive members.
// NOTE: Lists containing elements of different types can't be sorted.
var unsortedList = [7,6,8,5,9,4,2,3,1];
unsortedList |> List.sort(nil); // A comparator function is not required in this scenario.
print unsortedList; // [1,2,3,4,5,6,7,8].

// For this example a comparator function must be provided. If a user tries to sort a list of objects, or maps without
// a comparator function a runtime error while be raised.
var people = [%{"name": "Jesse"}, %{"name": "Zorg"}, %{"name": "Alexa"}];
people 
|> List.sort(fun (lhs, rhs) {
    if (lhs["name"] > rhs["name"]) return 1;
    if (lhs["name"] < rhs["name"]) return -1;
    return 0;
});
print people; // [%{"name": "Alexa"}, %{"name": "Jesse"}, %{"name": "Zorg"}].

var unsortableList = [1,"tree",4];
unsortableList 
|> List.sort(nil); //--> Runtime error: Can't compare elements of different types.
```

#### Dictionary module.
The dictionary module allow users to perform different operations on dictionaries.
```
Dictionary.keys(%{"test": true, 5: false}); // ["test",5].
Dictionary.values(%{"test": "indeed", "something": false}); // ["something",false].
Dictionary.containsKey(%{"test": "something"}, "test"); // true.
Dictionary.at(%{"test": 5}, "test"); // 5.

// Dictionary.ToList: Turns a dictionary into a list composed of sublist that contain each key-value pair.
// Takes 1 argument:
// 1. The dictionary that will be turned into a list.
Dictionary.toList(%{"test": "value", 1: 2, "false": false, "true": true}); //[["test","value"],[1,2],["false",false],["true",true]]. 

// Dictionary.delete: Removes the specified key from the provided dictionary (mutates the original dictionary).
// Takes two parameters:
// 1. The dictionary that will be modified.
// 2. The key that will be removed from the dictionary.
Dictionary.delete(%{"test": 4, "fake": true}, "fake") // %{"test": 4}.

// Dictionary.clear: Removes all the keys from the provided dictionary (mutates the original dictionary).
// Takes one parameter:
// 1. The dictionary that will be modified.
Dictionary.clear(%{"some": "value", "another": 3, true: false}); // %{}.
```

#### JSON module.
`L#` has a built-in module that allow users to serialize and deserialize JSON.
```
// JSON.parse: Turns a valid JSON string into a L# dictionary.
// Takes one parameter:
// 1. A string that represents a valid JSON.
// Returns a Result Ok containing the parsed JSON as a dictionary or a Result Err containing an error message.
var parsedJSON = '{
  "test": 3, 
  "test2": "xd", 
  "boolT": true, 
  "boolF": false, 
  "null": null,
  "recursive": {
        "test": 4, 
        "test2": "xd"}
}' |> JSON.parse();
print parsedJSON??;
/*
%{"test": 3, "test2": "xd", "boolT": true, "boolF": false, "null": nil, "recursive": %{"test": 4, "test2": "xd"}}
*/

// JSON.into: Turns a L# dictionary into a JSON string.
// Takes one parameter:
// 1. A non-empty dictionary.
// Returns a Result OK containing the expected JSON string or a Result Err containing an error message.
var dict = %{
  "test": [
    %{
       "something": true,
       "else": false,
       "test": "yup"
    },
    %{
        "something": true,
        "else": false,
        "test": "yup"
    }
  ]
};

var json = (dict |> JSON.into())??;
print json; // '{"test":[{"something":true,"else":false,"test":"yup"},{"something":true,"else":false,"test":"yup"}]}'.
```

#### IO module.
The `IO` module allow users to get user input and deal with the file system.
```
// IO.readFileAsText: Reads the content of a file as text.
// Takes 1 parameter:
// 1. A string, that represents the path of the file.
// Returns a Result.OK containing a string that represents the content of a file
// or a Result.Err is something failed while trying to read the file.

// text.txt
Some string.

IO.readFileAsText("./test.txt")??; // text.txt

// IO.copyFile: Copy a file into a specific path.
// Takes two parameters:
// 1. The path where the original file is located.
// 2. The path where the copy of the file will be placed.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.copyFile("./test.txt", "../another-folder/test.txt") 
|> Result.isOk(); // true.

// IO.moveFile: Moves a file to a specific path.
// Takes two parameters:
// 1. The path where the file is located.
// 2. The path where the file will be placed.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.moveFile("./test.txt", "../another-file/test.txt")
|> Result.isOk(); // true.

// IO.deleteFile: Deletes a file from the file system.
// Takes one parameter:
// 1. The path where the file is located.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.deleteFile("./test.txt")
|> Result.isOK(); // true.

// IO.createFile: Creates an empty file on the specified path.
// Takes two parameters:
// 1. The path where the file will be created.
// 2. The content that will be written on the file.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.createFile("./another-test.txt", '{"test": true}')
|> Result.isOK();

// IO.writeFile: Writes the provided string to the specified file.
// Takes two parameters:
// 1. The path of the file that will be modified.
// 2. The content that will be written to the file.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.writeFile("./test.txt", "Hello!")
|> Result.isOK(); // true.

// test.txt
Hello!

// IO.fileExists: Checks if there's file located on the specified path.
// Takes one parameter:
// 1. The path of the file we want to check.
// Returns a Result.OK containing a bool (true, or false) depending
// if the file exists and Result.Err containing an error message if
// something goes wrong.
IO.fileExists("./test.txt")
|> Result.isOk(); // true.

// IO.directoryExists: Checks if there's directory located on the specified path.
// Takes one parameter:
// 1. The path of the directory we want to check.
// Returns a Result.OK containing a bool (true, or false) depending
// if the directory exists and Result.Err containing an error message if
// something goes wrong.
IO.directoryExists("../test-folder")
|> Result.isOK();


// IO.createDictory: Creates an empty directory on the specified path.
// Takes one parameter:
// 1. The path where the new directory will be created.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.createDirectory("./another-folder")
|> Result.isOK(); // true.

// IO.removeFile: Moves a directory to a specific path.
// Takes two parameters:
// 1. The path where the directory is located.
// 2. The path where the directory will be placed.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.moveDirectory("../test-dir, "./another-directory")
|> Result.isOk(); // true.

// IO.removeDirectory: Removes a directory located at the specified path.
// Takes one parameter:
// 1. The path where the new directory will be created.
// Returns an empty Result.OK if the operation was successful and Result.Err otherwise.
IO.deleteDirectory("./another-folder")
|> Result.isOK(); // true.

// IO.getDirectoryFiles: Get all the information of the files placed under the specified directory.
// Takes one parameter:
// 1. The path of the directory which information will be returned.
// Returns a Result.OK containing a list with the paths of the files located on the specified directory
// and Result.Err containing an Result.Err in case something went wrong.
IO.getDirectoryFiles("./another-folder")??; // [./another-folder/test-file.txt].

// IO.getParentDirectory: Gets the infomation of the parent directory of the directory located
// under the specified path.
// 1. The path of directory contained on the directory which info will be returned.
// Returns a Result.OK that contains a list of dictionaries that hold the
// information on every file, if something goes wrong a Result.Err holding an error message
// is returned.
IO.getParentDirectory("./")??;
/*
%{
    "directory":"C:\Users\jmj_0\source\repos\LSharp\bin\Debug\net5.0\test-dir", 
    "exists":True, 
    "creationDate":9/22/2022 12:05:08 PM, 
    "files":[
        %{
            "extension":".json", 
            "name":"json-copy.json", 
            "path":"C:\Users\jmj_0\source\repos\LSharp\bin\Debug\net5.0\test-dir", 
            "creationDate":9/22/2022 12:08:31 PM, 
            "exists":True, 
            "lastUpdate":9/22/2022 12:08:31 PM
        }, 
        %{
            "extension":".json", 
            "name":"json.json", 
            "path":"C:\Users\jmj_0\source\repos\LSharp\bin\Debug\net5.0\test-dir", 
            "creationDate":9/22/2022 12:08:31 PM,
            "exists":True, 
            "lastUpdate":9/22/2022 12:08:31 PM
        }
   ], 
   "subdirectories":[
        %{
            "directory":"C:\Users\jmj_0\source\repos\LSharp\bin\Debug\net5.0\test-dir\inner-dir", 
            "exists":True, 
            "creationDate":9/22/2022 12:05:23 PM, 
            "files":[], 
            "subdirectories":[]
        }
   ]
} 
*/

// IO.readLine: Prompts the provided message to the console and takes the provided
// input.
// Takes one parameter:
// 1. The message that will printed before taking input.
// Returns the string provided as a response by the user.
IO.readLine("one.."); // Input: two -> returns "two".
```

### Contributing
Right now there is not an official guide on how to contribute to the project (i'll be working on that as soon as I can). However,
you can help a lot by creating issues, since there's probably a lot of bugs that I haven't discovered. By the time being, I could
provide a detailed explanation on how to solve certain problems. So, if you happen to feel adventurous and want to give a bugfix
a shot, feel free to do so.

### Pending points
There are some features (and fixes) I would like to add to the language. Here are some of them:
- [ ] Add an HTTP Client.
- [ ] Add native classes (as an alternative to modules).
- [ ] Add basic IDE support (this would be part of a different project).
- [ ] Identify bugs an proceed to fix them.
- [ ] Add other useful functionalites through modules (like a Math module, other useful functions on the string module, etc.).
- [ ] Different tests cases for the language.
- [X] Add CI/CD to generate automated releases.
- [ ] Add Unit and Integration Tests.

This is the first programming language I've ever written so I don't expect it to be perfect. I'm still planning to tackle the 
Byte Code Interpreter project of **CI**, so I may even re-write this language using a non-garbage collected language.
