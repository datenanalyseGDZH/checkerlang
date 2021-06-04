# CKL Language Reference

## Data types

### String

```
def s = "Hello world!";
def t = 'Good morning';
```

Strings are arbitrarily long sequences of characters. They are delimited
by either a pair of double quotes or a pair of single quotes. 

Strings can span multiple lines:

```
def s = "hello
there";
```

Strings can be added:

```
"abc" + "def" == "abcdef"
```

If a string is multiplied by an integer n, the result is a
string containing n times the original string:

```
"abc" * 3 == "abcabcabc"
```

### Int

```
def n = 1234;
```

Integers represent integral numbers. They are signed and have a 64bit
representation.

All the basic arithmetic operations are defined for integers.
Division returns an integer.

```
2 + 3 == 5;
2 - 3 == -1;
2 * 3 == 6;
7 / 3 == 2;
7 % 3 == 1;
```

### Decimal

```
def d = 123.45;
```

Decimals represent floating point numbers. They are signed and have
a 64bit representation.

All the basic arithmetic operations are defined for decimals.

```
2.3 + 3.5 == 5.8
2.5 - 3.0 == -0.5
2.3 * 3.6 == 8.28
6.9 / 2 == 3.45
7.0 % 2.0 == 1.0
```

Some implementations use decimal arithmetic (e.g. the ckl-dotnet one),
others use standard IEEE floating point arithmetic. Thus, you need
to be careful about rounding issues and should never use the ==
operator to compare two decimal values.


### Boolean

```
def a = TRUE;
def b = FALSE;
```

Boolean values are used in logical operations (for example in
condition expressions). A boolean can have either TRUE or FALSE
as value. Other values are not possible.

The boolean operators `and`, `or` and `not` are supported. See
section "Boolean algebra" for details.

### Pattern

### Date
### List

```
def a = [1, 2, 3];
```

### Set

```
def a = <<1, 2, 3>>;
```

### Map

```
def a = <<<a => 1, b => 2, c => 3>>>;
```

### Object

```
def a = <* val = 12, f = fn(x) 2 * x *>;
```

### Func

```
def f = fn(x) 2 * x;
```

### Error
### Input
### Output
### Null

```
def a = NULL;
```

### Node


## Boolean algebra

### or

```
<boolean> or <boolean>
```

Returns TRUE, if either of the booleans is TRUE.

### and

```
<boolean> and <boolean>
```

Returns TRUE, if both booleans are TRUE.

### not

```
not <boolean>
```

Returns TRUE, if the boolean is FALSE.


## Comparison operations

```
<value> < <value>
<value> <= <value>
<value> > <value>
<value> >= <value>
<value> == <value>
<value> <> <value>
<value> != <value>
```

Compares two values and returns either TRUE or FALSE.

It is possible to concatenate multiple comparisons, even with
different operators:

```
1 < x <> y <= 100
```


## Arithmetic
## Predicate expressions
## Assignment
## Deref operator
## Invoke or Pipeline operator


## Variables

### Syntax

```
def <identifier> = <expression>
def [ <identifier>, ... ] = <expression>
```

### Description

Variable definitions bind a variable name to a value. Any variable
can be bound to values of any data type. It is thus possible, to
have a variable point to a number and then change it to point to
a string.

The bindings are stored in an environment. There is a global environment
and each function introduces its own environment. Thus, it is possible
to redefine a global variable in a function without changing the
global binding.

In addition to the simple definition which assigns a value to an identifier,
there exists a more complex definition statement, which assigns values to
multiple identifiers simultaneously. In this case, the expression has to
be of type list or set. The values of the list (or set) are then assigned
to the variables. If there are not enough values, the remaining variables
are assigned NULL.

### Examples

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
then it is shadowed, but not replaced:

```
> def a = 1; # define a in the root environment
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

```
> def [a, b, c] = [1, 2, 3]; println(a + b + c);
6
```

## Blocks

### Syntax

```
do
    { <statement> ; }
    ...
{ catch <expression> }
    { <statement> ; }
    ...
{ catch all }
    { <statement> ; }
    ...
{ finally }
    { <statement> ; }
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
{ elif <condition> then <block_or_expression> }
...
{ else <block_or_expression> }
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

### Syntax

```
for <identifier> in <expression> <block_or_expression>

for [ <identifier>, ... ] in <expression> <block_or_expression>
```

### Description

The expression has to evaluate to one of list, set, map, object, string or input.

The statement iterates over the values of the expression and assigns each value
successively to the identifier and then executes the block or expression.

In case of a string, `for` iterates over the characters.

In case of an input, `for` iterates over text lines.

The more complex variant allows to assign values to multiple variables in each
iteration. In this case, the values of the expression must be of type list or set.

### Examples

```
> for i in interval(1, 5) println(i)
1
2
3
4
5
```

```
> def x = [[1, 2], [3, 4]];
[[1, 2], [3, 4]]
> for [a, b] in x do def y = a * b; println(y - a - b); end
-1
5
```

## While loop statement

### Syntax

```
while <condition> <block>
```

### Description

The `while` statement evaluates a block repeatedly, until the
condition returns FALSE.

The value of the block of the last iteration is the result of
the statement.

### Examples

```
def i = 1;
while i < 100 do
    println(i);
    i *= 2;
end;
```


## List, set and map comprehension

## Functions

## Objects

### _init_
### _str_
### new function
### private members

## Modules

