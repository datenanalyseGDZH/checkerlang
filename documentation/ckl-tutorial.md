# CKL Tutorial

## Introduction

CKL (or Checkerlang) is a general purpose scripting language. It
is available for the three platforms Java/JVM, Dotnet and
Nodejs/Browser. Depending on the running platform, some features
may not be available, e.g. in the browser, there are no features
to read or write files.

This tutorial seeks to introduce you to CKL. If you need more
information, you can consult the [CKL Language Reference](ckl-language.md),
look at the various [examples](../examples/) and play with the
online [CKL Notebook](https://www.checkerlang.ch/).

We encourage you to follow along the following examples using the
[CKL Notebook](https://www.checkerlang.ch/).

## Basic data types

CKL supports a wide variety of data types. You can use strings

```
"abc"
```

Strings can be written with double quotes or with single quotes

```
'def'
```

You can use integers values...

```
123
```

...and decimal values

```
123.45
```

Of course there are boolean values

```
TRUE
```

```
FALSE
```

Date values are fully supported, but do not have a literal form. But we can get a date value by converting an ISO-date string (there are also other ways to convert to and from date values)

```
date('20220615')
```

We can include hours, minutes and seconds to get a full datetime value.

```
date('20220615123400')
```

If you need to use regular expressions, you can use a special regexp literal for the pattern

```
//h[ae]llo+//
```

## Simple expressions

The next step is to combine the values using operators. You can use
all the usual operators like +, -, *, /, and, or, not, <, <=, >, >=, !=.

For example, the following is an expression composed of int and decimal
literals

```
(12 + 2 * 3) / 2 + 1.34
```

```
=> 10.34
```

Of course we can compare values

```
10 < 12 < 15
```

```
=> TRUE
```

And we can combine boolean values

```
10 < 12 and 'a' == 'b'
```

```
=> FALSE
```

## Blocks

Multiple expressions must be separated by a semicolon. They
build a block and are executed sequentially. The value of the
block is the value of the last executed expression

```
12;
'x' * 4;
'a' < 'b'
```

```
=> TRUE
```

We can explicitly build blocks by enclosing expressions within
`do` and `end` keywords

```
do
  12;
  'x' * 4;
  'a' < 'b';
end
```

```
=> TRUE
```

## Collections

Besides the simple, atomic values, CKL also supports a range of
compound values, the collections. Supported are lists, sets, maps
and objects.

Lists are written in square brackets and can contain values of
any data type. In most use cases, the lists will contain only
elements of a single data type, but this is not required.

```
[1, 2, 3, 4]
```

```
=> [1, 2, 3, 4]
```

Access list elements by dereferencing the list

```
[5, 6, 7, 8][0]
```

```
=> 5
```

Sets are kind of like lists, but cannot contain duplicate values.
They are written with double angle brackets

```
<<1, 1, 2, 2, 3, 4>>
```

```
<<1, 2, 3, 4>>
```

Check whether a value is in the set using the `in` operator (this also
works for lists)

```
2 in <<1, 2, 3, 4>>
```

```
=> TRUE
```

Maps provide a mapping between keys and values. Both keys and values
can be of any data type. Maps are denoted by using triple angle brackets

```
<<<'a' => 1, 'b' => 2, 'c' => 3>>>
```

```
=> <<<'a' => 1, 'b' => 2, 'c' => 3>>>
```

To get a value of the map, dereference it using the appropriate key

```
<<<'a' => 1, 'b' => 2, 'c' => 3>>>['a']
```

```
=> 1
```

Objects are kind of like maps, but they are some limitations. For
example, the keys used in objects are required to be strings. Also,
objects have no guaranteed, sorted order for the keys. Thanks to
these limitations, objects use less memory and are faster than maps.
Objects are delimited by `<*` and `*>`. The keys do not need to be quoted.

```
<* a = 1, b = 2, c = 3 *>
```

```
=> <* a = 1, b = 2, c = 3 *>
```

You can dereference objects in the same way as maps, but there is
a more idiomatic way using the `->` deref operator.

```
<* a = 1, b = 2, c = 3 *>->a
```

```
=> 1
```

## Variables

When you need more than one line of code, you may want to introduce
names for expressions in order to simplify the code. This is done
using the `def` keyword

```
def a = 3;
a * a
```

```
=> 9
```


```
def obj = <* a = 1, b = 2, c = 3 *>;
(obj->a + obj->b) * obj->c
```

```
=> 9
```

Once a variable is declared, you can set its value using a simple
assignment operation.

```
def x = 2;
def y = 3;
x = 4;
x * y
```

```
=> 12
```

## Functions

CKL contains quite a lot of function, which you can use freely.
See [CKL Functions](ckl-functions.md)).

You can define your own functions and use then in the same way
as built in functions.

Functions consists of a function declaration and a block of code.
when the function is called, the block of code is executed
sequentially. The last executed expression is the return value
of the function.

```
def f(a, b) do
    2 * a + 3 * b
end;
f(1, 2)
```

```
=> 8
```

