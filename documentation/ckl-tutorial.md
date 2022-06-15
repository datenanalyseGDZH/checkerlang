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


