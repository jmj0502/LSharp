# The L# Programming Language

The `L#` programming language is a superset of the (Lox Programming Language)[https://github.com/munificent/craftinginterpreters]. The project started as
as a Lox implementation written in `C#`. However, after finishing the first part of (Crafting Interpreters And Compilers)[http://www.craftinginterpreters.com/]
(the tree-walk interpreter project) I decided to give (@Munificent)[https://github.com/munificent]'s challenge a shot and try to come up with features that I
thought `Lox` is missing.

Despite the fact that this project is still a work in progress, it feels like a huge achivement. I've always been interested in language programming, mainly because I 
always wondered how the programing languages that I use on a daily basis interact with the different components of my machine (I know that this implementation
only scratch the surface of what can be done but still). On the other hand, language design results particularly interesting to me; on my spare time I like to write
multiple implementations of a program using different programming languages since I really like to contrast the approach taken in each one of them to achieve the
same goal.

When I discovered **Crafting Interpreters And Compilers** I was truly amazed! The book had everything I needed to learn in order to get started on this
field and I'm trully grateful for that, in fact the book is so good that I was able to understand the key concepts behind scanners, grammars, parsers and even some 
design decision that led to interesting features in popular languages. Before discovering **CIAC** the only compiler design book I knew about was the Dragon Book and that 
wasn't a good place to start my journey in the field of language programming (I'm still planning to tackle it in the future tho'), mainly because I didn't even had a clear
idea of the different components involved in this kind of project.

## Scope

L# aims to become a multi-paradigm scripting language that can be used to generate scripts that interact with the `File System`, gather
data from a resource hosted on the cloud (through HTTP) or both. Right now I don't think the language can be used for extremely complex tasks 
(That may change as the language improves), but I would like to see if its use cases actually go beyond the fields of application I initially ambitioned.

## Features

`L#` supports all the features supported by `Lox`, so let's start by reviewing those:

### Lox supports variables and functions
```lox

```
