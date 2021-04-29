# CKL Language Reference

## Data types

### String
### Int
### Decimal
### Boolean
### Pattern
### Date
### List
### Set
### Map
### Object
### Func
### Error
### Input
### Output
### Null
### Node

## Boolean algebra
## Comparison operations
## Arithmetic
## Predicate expressions
## Assignment
## Deref operator
## Invoke or Pipeline operator


## Variables

Variable definitions bind a variable name to a value. Any variable
can be bound to values of any data type. It is thus possible, to
have a variable point to a number and then change it to point to
a string.

The `def` statement defines a variable and assigns it a value:

```
> def num = 12;
12
> def name = 'John';
'John'
```

It is illegal to define a variable without assigning it a value:

```
> def a;

Unexpected end of input {repl}:1:6
```

You can redefine a variable as often as you wish:

```
> def a = 1;
1
> def a = 'hello'
'hello'
```

Be aware that `def` always defines a variable in the current
environment. Should a variable already exist in a parent environment,
then it is not replaced, but shadowed:

```
> def a = 1; # define a in the top environment
1
> def f() do def a = 2; println(a); def a = 3; println(a); end;
<#f>
> f()
2
3
> a
1
```

Once a variable is defined, we can reassign its value at will:

```
> def a = 1;
1
> a = 2;
2
```

This does not create a new variable definition. It only works if the
variable is already defined:

```
> b = 2

Variable b is not defined {repl}:1:1
```

Lets look at the example with the function call from above, but
change it to not redefine the variable `a` but instead assign
it a new value:

```
> def a = 1; # define a in the top environment
1
> def f() a = 2;
<#f>
> f()
2
> a
2
```

We see that in this case, the original variable definition is updated.


## Blocks

### Syntax

```
do
    [ <statement> ; ]
    ...
[ catch <expression> ]
    [ <statement> ; ]
    ...
[ catch all ]
    [ <statement> ; ]
    ...
[ finally ]
    [ <statement> ; ]
    ...
end
```

### Description

A block groups a set of statements into one entity. This is used
for example in the `if` statement when an alternative must contain more
than one statement.

In addition to the basic grouping functionality, a block has two optional
features. One or more `catch` parts and one `finally` part. Both are
entirely optional.

A `catch` part catches a matching exception and thus provides a way
to gracefully handle exceptions. If a `catch` part contains the `all`
marker, then this part catches all exceptions. Otherwise the first
expression after the `catch` keyword specifies the exception value
this part catches.

 A `finally` part runs statements whether or not exceptions occured or a
 return statement was executed. This is useful e.g. to dispose of resources.

Important: a block does not introduce a new environment. Thus any variables
defined within the block are visible even after the block ends.

### Examples

A simple block:

```
do
    println("hello");
    println("world");
end;
```

A block that releases a resource even in case of an exception.

```
def f = acquire_resource();
do
    this_might_throw_an_error();
finally
    close(f);
end;
```

A block that catches and handles an exception:

```
do
    this_might_throw_an_error();
catch e
    handle_the_error_or_rethrow();
end;
```


## If statement

### Syntax

```
if <condition> then <block_or_expression>
[ elif <condition> then <block_or_expression> ]
...
[ else <block_or_expression> ]
```

### Description

The `if` statement executes code if a condition holds.

It can contain one or more `elif` parts and at most one
`else` part.


### Examples

Example:

```
if a % 2 == 0 then println("a is even")
else println("a is odd");
```

The `elif` and `else` branches are optional. Be aware that no semicolon
is required or allowed between the `if`, `elif` and `else` branches.

If you need to execute more than one statement in a branch, use a block:

```
if time_of_day < 12 then do
    println("morning");
    println("let go for a run");
end;
```


## For loop statmeent




## While loop statement

## List, set and map comprehension

## Functions

## Objects

### _init_
### _str_
### new function
### private members

## Modules

