# CKL 3.6.2-js library
## Table of contents
* [Module Bitwise](#module_Bitwise)
* [Module Date](#module_Date)
* [Module IO](#module_IO)
* [Module List](#module_List)
* [Module Math](#module_Math)
* [Module OS](#module_OS)
* [Module Predicate](#module_Predicate)
* [Module Random](#module_Random)
* [Module Set](#module_Set)
* [Module Stat](#module_Stat)
* [Module String](#module_String)
* [Module Sys](#module_Sys)
* [Module Type](#module_Type)

<a name="module_Bitwise"></a>
## Bitwise
* [bit_and](#bit_and)
* [bit_and_32](#bit_and_32)
* [bit_not](#bit_not)
* [bit_not_32](#bit_not_32)
* [bit_or](#bit_or)
* [bit_or_32](#bit_or_32)
* [bit_rotate_left](#bit_rotate_left)
* [bit_rotate_left_32](#bit_rotate_left_32)
* [bit_rotate_right](#bit_rotate_right)
* [bit_rotate_right_32](#bit_rotate_right_32)
* [bit_shift_left](#bit_shift_left)
* [bit_shift_right](#bit_shift_right)
* [bit_xor](#bit_xor)
* [bit_xor_32](#bit_xor_32)

<a name="module_Date"></a>
## Date
* [date_day](#date_day)
* [date_hour](#date_hour)
* [date_minute](#date_minute)
* [date_month](#date_month)
* [date_second](#date_second)
* [date_year](#date_year)
* [format_date](#format_date)
* [is_valid_date](#is_valid_date)
* [is_valid_time](#is_valid_time)
* [iso_date](#iso_date)
* [iso_datetime](#iso_datetime)
* [parse_date](#parse_date)

<a name="module_IO"></a>
## IO
* [close](#close)
* [file_input](#file_input)
* [file_output](#file_output)
* [get_output_string](#get_output_string)
* [print](#print)
* [printf](#printf)
* [println](#println)
* [process_lines](#process_lines)
* [read](#read)
* [read_all](#read_all)
* [read_file](#read_file)
* [readln](#readln)
* [str_input](#str_input)
* [str_output](#str_output)

<a name="module_List"></a>
## List
* [append_all](#append_all)
* [contains](#contains)
* [filter](#filter)
* [find](#find)
* [find_last](#find_last)
* [first](#first)
* [first_n](#first_n)
* [flatten](#flatten)
* [for_each](#for_each)
* [grep](#grep)
* [grouped](#grouped)
* [last](#last)
* [last_n](#last_n)
* [map_list](#map_list)
* [permutations](#permutations)
* [prod](#prod)
* [reduce](#reduce)
* [rest](#rest)
* [reverse](#reverse)
* [reverse_list](#reverse_list)
* [unique](#unique)

<a name="module_Math"></a>
## Math
* [E](#E)
* [PI](#PI)
* [abs](#abs)
* [acos](#acos)
* [asin](#asin)
* [atan](#atan)
* [atan2](#atan2)
* [cos](#cos)
* [exp](#exp)
* [gcd](#gcd)
* [is_even](#is_even)
* [is_odd](#is_odd)
* [lcm](#lcm)
* [log](#log)
* [log10](#log10)
* [log2](#log2)
* [pow](#pow)
* [sign](#sign)
* [sin](#sin)
* [sqrt](#sqrt)
* [tan](#tan)

<a name="module_OS"></a>
## OS
* [FS](#FS)
* [LS](#LS)
* [OS_ARCH](#OS_ARCH)
* [OS_NAME](#OS_NAME)
* [OS_VERSION](#OS_VERSION)
* [PS](#PS)
* [basename](#basename)
* [dirname](#dirname)
* [execute](#execute)
* [file_copy](#file_copy)
* [file_delete](#file_delete)
* [file_exists](#file_exists)
* [file_extension](#file_extension)
* [file_info](#file_info)
* [file_move](#file_move)
* [get_env](#get_env)
* [list_dir](#list_dir)
* [make_dir](#make_dir)
* [path](#path)
* [strip_extension](#strip_extension)
* [which](#which)

<a name="module_Predicate"></a>
## Predicate
* [is_alphanumerical](#is_alphanumerical)
* [is_empty](#is_empty)
* [is_negative](#is_negative)
* [is_not_empty](#is_not_empty)
* [is_not_null](#is_not_null)
* [is_null](#is_null)
* [is_numerical](#is_numerical)
* [is_positive](#is_positive)
* [is_zero](#is_zero)

<a name="module_Random"></a>
## Random
* [choice](#choice)
* [choices](#choices)
* [random](#random)
* [sample](#sample)
* [set_seed](#set_seed)

<a name="module_Set"></a>
## Set
* [diff](#diff)
* [intersection](#intersection)
* [symmetric_diff](#symmetric_diff)
* [union](#union)

<a name="module_Stat"></a>
## Stat
* [geometric_mean](#geometric_mean)
* [harmonic_mean](#harmonic_mean)
* [mean](#mean)
* [median](#median)
* [median_high](#median_high)
* [median_low](#median_low)

<a name="module_String"></a>
## String
* [chr](#chr)
* [contains](#contains)
* [ends_with](#ends_with)
* [esc](#esc)
* [find](#find)
* [find_last](#find_last)
* [join](#join)
* [lower](#lower)
* [matches](#matches)
* [ord](#ord)
* [q](#q)
* [replace](#replace)
* [reverse](#reverse)
* [s](#s)
* [split](#split)
* [split2](#split2)
* [starts_with](#starts_with)
* [substr](#substr)
* [trim](#trim)
* [upper](#upper)

<a name="module_Sys"></a>
## Sys

<a name="module_Type"></a>
## Type
* [is_boolean](#is_boolean)
* [is_decimal](#is_decimal)
* [is_func](#is_func)
* [is_int](#is_int)
* [is_list](#is_list)
* [is_map](#is_map)
* [is_node](#is_node)
* [is_numeric](#is_numeric)
* [is_object](#is_object)
* [is_set](#is_set)
* [is_string](#is_string)

<a name="E"></a>
## E
### Syntax
```
E
```

### Description
The mathematical constant E (Eulers number)

### Modules
Math

<a name="FS"></a>
## FS
### Syntax
```
FS
```

### Description
The OS field separator (posix: :, windows: ;).

### Modules
OS

<a name="LS"></a>
## LS
### Syntax
```
PS
```

### Description
The OS line separator (posix: \n, windows: \r\n).

### Modules
OS

<a name="MAXINT"></a>
## MAXINT
### Syntax
```
MAXINT
```

### Description
The maximal int value

### Modules
Core

<a name="MININT"></a>
## MININT
### Syntax
```
MININT
```

### Description
The minimal int value

### Modules
Core

<a name="OS_ARCH"></a>
## OS_ARCH
### Syntax
```
OS_ARCH
```

### Description
The architecture of the operating system, one of x86, amd64.

### Modules
OS

<a name="OS_NAME"></a>
## OS_NAME
### Syntax
```
OS_NAME
```

### Description
The name of the operating system, one of Windows, Linux, macOS

### Modules
OS

<a name="OS_VERSION"></a>
## OS_VERSION
### Syntax
```
OS_VERSION
```

### Description
The version of the operating system.

### Modules
OS

<a name="PI"></a>
## PI
### Syntax
```
PI
```

### Description
The mathematical constant PI

### Modules
Math

<a name="PS"></a>
## PS
### Syntax
```
PS
```

### Description
The OS path separator (posix: /, windows: \).

### Modules
OS

<a name="abs"></a>
## abs
### Syntax
```
abs(n) 
```

### Description
Returns the absolute value of n.



### Modules
Core, Math

### Examples
```
abs(2) ==> 2
abs(-3) ==> 3
```

<a name="acos"></a>
## acos
### Syntax
```
acos(x)
```

### Description
Returns the arcus cosinus of x.



### Modules
Math

### Examples
```
acos(1) ==> 0.0
```

<a name="add"></a>
## add
### Syntax
```
add(a, b)
```

### Description
Returns the sum of a and b. For numerical values this uses usual arithmetic.
For lists and strings it concatenates. For sets it uses union.



### Modules
Core

### Examples
```
add(1, 2) ==> 3
add(date('20100201'), 3) ==> '20100204000000'
```

<a name="all"></a>
## all
### Syntax
```
all(lst, predicate)
all(lst)
```

### Description
Returns TRUE, if the predicate function returns
TRUE for all elements of the list.

If no predicate function is passed, the list must
contain boolean values.



### Modules
Core

### Examples
```
all([1, 2, 3], fn(n) n <= 3) ==> TRUE
all([1, 2, 3], fn(n) n <  3) ==> FALSE
all([TRUE, TRUE, TRUE]) ==> TRUE
all([TRUE, FALSE, TRUE]) ==> FALSE
all([e >= 2 for e in [2, 3, 4]]) ==> TRUE
all([e >= 2 for e in [1, 3, 4]]) ==> FALSE
```

<a name="any"></a>
## any
### Syntax
```
any(lst, predicate)
any(lst)
```

### Description
Returns TRUE, if the predicate function returns
TRUE for any element of the list.

If no predicate function is passed, the list must
contain boolean values.



### Modules
Core

### Examples
```
any([1, 2, 3], fn(n) n == 3) ==> TRUE
any([1, 2, 3], fn(n) n == 4) ==> FALSE
any([TRUE, TRUE, TRUE]) ==> TRUE
any([TRUE, FALSE, TRUE]) ==> TRUE
any([e >= 2 for e in [2, 3, 4]]) ==> TRUE
any([e >= 2 for e in [1, 3, 4]]) ==> TRUE
```

<a name="append"></a>
## append
### Syntax
```
append(lst, element)
```

### Description
Appends the element to the list lst. The lst may also be a set.
Returns the changed list.



### Modules
Core

### Examples
```
append([1, 2], 3) ==> [1, 2, 3]
append(set([1, 2]), 3) ==> set([1, 2, 3])
```

<a name="append_all"></a>
## append_all
### Syntax
```
append_all(lst, items)
```

### Description
Appends all items to the list or set. The items
can be in a list or a set.



### Modules
List

### Examples
```
def a = [1, 2, 3]; append_all(a, [4, 5, 6]); a ==> [1, 2, 3, 4, 5, 6]
def a = <<1, 2, 3>>; append_all(a, [2, 3, 4]); a ==> <<1, 2, 3, 4>>
def a = [1, 2, 3]; append_all(a, <<4, 5, 6>>); a ==> [1, 2, 3, 4, 5, 6]
def a = [1, 2, 3]; append_all(a, [2, 3, 4]); a ==> [1, 2, 3, 2, 3, 4]
```

<a name="apply"></a>
## apply
### Syntax
```
apply(f, args)
```

### Description
Applies the function with the arguments in the list args.



### Modules
Core

### Examples
```
apply(fn(a, b, c) a + b + c, [1, 2, 3]) ==> 6
```

<a name="asin"></a>
## asin
### Syntax
```
asin(x)
```

### Description
Returns the arcus sinus of x.



### Modules
Math

### Examples
```
asin(0) ==> 0.0
```

<a name="atan"></a>
## atan
### Syntax
```
atan(x)
```

### Description
Returns the arcus tangens of x.



### Modules
Math

### Examples
```
atan(0) ==> 0.0
```

<a name="atan2"></a>
## atan2
### Syntax
```
atan2(y, x)
```

### Description
Returns the arcus tangens of y / x.



### Modules
Math

### Examples
```
atan2(0, 1) ==> 0.0
```

<a name="basename"></a>
## basename
### Syntax
```
basename(path)
```

### Description
Returns the filename portion of the path.



### Modules
OS

### Examples
```
basename('dir/file.ext') ==> 'file.ext'
basename('file.ext') ==> 'file.ext'
basename('a/b/c/d/e/file.ext') ==> 'file.ext'
```

<a name="bind_native"></a>
## bind_native
### Syntax
```
bind_native(native)
bind_native(native, alias)
```

### Description
Binds a native function in the current environment.


### Modules
Core

<a name="bit_and"></a>
## bit_and
### Syntax
```
bit_and(a, b)
```

### Description
Performs bitwise and for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_and(5, 6) ==> 4
bit_and(4, 4) ==> 4
```

<a name="bit_and_32"></a>
## bit_and_32
### Syntax
```
bit_and(a, b)
```

### Description
Performs bitwise and for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_and(5, 6) ==> 4
bit_and(4, 4) ==> 4
```

<a name="bit_not"></a>
## bit_not
### Syntax
```
bit_not(a)
```

### Description
Performs bitwise not for the 32bit value a.


### Modules
Bitwise

### Examples
```
bit_not(1) ==> 4294967294
bit_not(0) ==> 4294967295
```

<a name="bit_not_32"></a>
## bit_not_32
### Syntax
```
bit_not(a)
```

### Description
Performs bitwise not for the 32bit value a.


### Modules
Bitwise

### Examples
```
bit_not(1) ==> 4294967294
bit_not(0) ==> 4294967295
```

<a name="bit_or"></a>
## bit_or
### Syntax
```
bit_or(a, b)
```

### Description
Performs bitwise or for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_or(1, 2) ==> 3
bit_or(3, 4) ==> 7
bit_or(4, 4) ==> 4
```

<a name="bit_or_32"></a>
## bit_or_32
### Syntax
```
bit_or(a, b)
```

### Description
Performs bitwise or for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_or(1, 2) ==> 3
bit_or(3, 4) ==> 7
bit_or(4, 4) ==> 4
```

<a name="bit_rotate_left"></a>
## bit_rotate_left
### Syntax
```
bit_rotate_left(a, n)
```

### Description
Performs bitwise rotate of 32bit value a by n bits to the left.


### Modules
Bitwise

### Examples
```
bit_rotate_left(1, 2) ==> 4
bit_rotate_left(1, 3) ==> 8
bit_rotate_left(4, 4) ==> 64
```

<a name="bit_rotate_left_32"></a>
## bit_rotate_left_32
### Syntax
```
bit_rotate_left(a, n)
```

### Description
Performs bitwise rotate of 32bit value a by n bits to the left.


### Modules
Bitwise

### Examples
```
bit_rotate_left(1, 2) ==> 4
bit_rotate_left(1, 3) ==> 8
bit_rotate_left(4, 4) ==> 64
```

<a name="bit_rotate_right"></a>
## bit_rotate_right
### Syntax
```
bit_rotate_right(a, n)
```

### Description
Performs bitwise rotate of 32bit value a by n bits to the right.


### Modules
Bitwise

### Examples
```
bit_rotate_right(1, 2) ==> 1073741824
bit_rotate_right(1, 3) ==> 536870912
bit_rotate_right(4, 4) ==> 1073741824
```

<a name="bit_rotate_right_32"></a>
## bit_rotate_right_32
### Syntax
```
bit_rotate_right(a, n)
```

### Description
Performs bitwise rotate of 32bit value a by n bits to the right.


### Modules
Bitwise

### Examples
```
bit_rotate_right(1, 2) ==> 1073741824
bit_rotate_right(1, 3) ==> 536870912
bit_rotate_right(4, 4) ==> 1073741824
```

<a name="bit_shift_left"></a>
## bit_shift_left
### Syntax
```
bit_shift_left(a, n)
```

### Description
Performs bitwise shift of 32bit value a by n bits to the left.


### Modules
Bitwise

### Examples
```
bit_shift_left(1, 2) ==> 4
bit_shift_left(1, 3) ==> 8
bit_shift_left(1, 4) ==> 16
```

<a name="bit_shift_right"></a>
## bit_shift_right
### Syntax
```
bit_shift_right(a, n)
```

### Description
Performs bitwise shift of 32bit value a by n bits to the right.


### Modules
Bitwise

### Examples
```
bit_shift_right(4, 1) ==> 2
bit_shift_right(4, 3) ==> 0
bit_shift_right(4, 2) ==> 1
```

<a name="bit_xor"></a>
## bit_xor
### Syntax
```
bit_xor(a, b)
```

### Description
Performs bitwise xor for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_xor(1, 2) ==> 3
bit_xor(1, 3) ==> 2
bit_xor(4, 4) ==> 0
```

<a name="bit_xor_32"></a>
## bit_xor_32
### Syntax
```
bit_xor(a, b)
```

### Description
Performs bitwise xor for the two 32bit values a and b.


### Modules
Bitwise

### Examples
```
bit_xor(1, 2) ==> 3
bit_xor(1, 3) ==> 2
bit_xor(4, 4) ==> 0
```

<a name="body"></a>
## body
### Syntax
```
body(f)
```

### Description
Returns the body of the lambda f.



### Modules
Core

### Examples
```
body(fn(x) 2 * x) ==> '(mul 2, x)'
```

<a name="boolean"></a>
## boolean
### Syntax
```
boolean(obj)
```

### Description
Converts the obj to a boolean, if possible.



### Modules
Core

### Examples
```
boolean(1) ==> TRUE
```

<a name="ceiling"></a>
## ceiling
### Syntax
```
ceiling(x)
```

### Description
Returns the integral decimal value that is equal to or next higher than x.



### Modules
Core

### Examples
```
ceiling(1.3) ==> 2.0
```

<a name="choice"></a>
## choice
### Syntax
```
choice(lst)
```

### Description
Returns a random element from the list or set lst.



### Modules
Random

### Examples
```
choice([1, 1, 1, 1]) ==> 1
```

<a name="choices"></a>
## choices
### Syntax
```
choices(lst, n)
```

### Description
Returns a list with n random elements from the list or set lst.


### Modules
Random

<a name="chr"></a>
## chr
### Syntax
```
chr(n)
```

### Description
Returns a single character string for the code point integer n.



### Modules
String

### Examples
```
chr(97) ==> 'a'
chr(32) ==> ' '
```

<a name="chunks"></a>
## chunks
### Syntax
```
chunks(obj, chunk_size)
```

### Description
Splits the obj into a list where each item is of size chunk_size,
except perhaps the last, which may be smaller. Obj can be a string
or a list.



### Modules
Core

### Examples
```
range(9) !> chunks(3) ==> [[0, 1, 2], [3, 4, 5], [6, 7, 8]]
range(10) !> chunks(3) ==> [[0, 1, 2], [3, 4, 5], [6, 7, 8], [9]]
'abcdefghi' !> chunks(3) ==> ['abc', 'def', 'ghi']
```

<a name="close"></a>
## close
### Syntax
```
close(conn)
```

### Description
Closes the input or output connection and releases system resources.


### Modules
IO

<a name="compare"></a>
## compare
### Syntax
```
compare(a, b)
```

### Description
Returns an int less than 0 if a is less than b,
0 if a is equal to b, and an int greater than 0
if a is greater than b.



### Modules
Core

### Examples
```
compare(1, 2) < 0 ==> TRUE
compare(3, 1) > 0 ==> TRUE
compare(1, 1) == 0 ==> TRUE
compare('1', 2) < 0 ==> TRUE
compare('2', 1) < 0 ==> TRUE
compare(100, '100') > 0 ==> TRUE
compare(NULL, 1) > 0 ==> TRUE
compare(NULL, NULL) == 0 ==> TRUE
```

<a name="const"></a>
## const
### Syntax
```
const(val) 
```

### Description
Returns a function that returns a constant value, regardless of the argument used.



### Modules
Core

### Examples
```
def f = const(2); f(1) ==> 2
def f = const(2); f('x') ==> 2
```

<a name="contains"></a>
## contains
### Syntax
```
contains(obj, part)
```

### Description
Returns TRUE if the string obj contains part.
If obj is a list, set or map, TRUE is returned,
if part is contained.



### Modules
Core, List, String

### Examples
```
contains('abcdef', 'abc') ==> TRUE
contains('abcdef', 'cde') ==> TRUE
contains('abcdef', 'def') ==> TRUE
contains('abcdef', 'efg') ==> FALSE
contains(NULL, 'abc') ==> FALSE
contains([1, 2, 3], 2) ==> TRUE
<<1, 2, 3>> !> contains(3) ==> TRUE
<<<a => 1, b => 2>>> !> contains('b') ==> TRUE
```

<a name="cos"></a>
## cos
### Syntax
```
cos(x)
```

### Description
Returns the cosinus of x.



### Modules
Math

### Examples
```
cos(PI) ==> -1.0
```

<a name="count"></a>
## count
### Syntax
```
count(obj, elem)
```

### Description
Returns the number of times, elem is contained in obj.
Obj can be a string, list or map. If it is a map,
then the values of the map, not the keys are checked.



### Modules
Core

### Examples
```
count([1, 2, 2, 2, 3, 4], 2) ==> 3
count(<<<1 => 1, 1 => 2, 2 => 2, 3 => 3>>>, 2) ==> 2
count('122234', '2') ==> 3
```

<a name="curry"></a>
## curry
### Syntax
```
curry(f, arg)
```

### Description
Partially applies the function f with the argument arg.
This returns another function which takes the remaining
args of the original function f.



### Modules
Core

### Examples
```
def f(a, b, c) [a, b, c]; def g = curry(f, 1); g(2, 3) ==> [1, 2, 3]
```

<a name="date"></a>
## date
### Syntax
```
date(obj)
```

### Description
Converts the obj to a date, if possible.
If obj is a string, the format yyyyMMdd is assumed.
If this fails, the fallback yyyyMMddHH is tried.
If this fails, the fallback yyyyMMddHHmmss is tried.

See parse_date for handling other formats.



### Modules
Core

### Examples
```
string(date('20170102')) ==> '20170102000000'
string(date('2017010212')) ==> '20170102120000'
string(date('20170102123456')) ==> '20170102123456'
```

<a name="date_day"></a>
## date_day
### Syntax
```
date_day(value)
```

### Description
Extracts the day part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.



### Modules
Date

### Examples
```
date_day('20190102') ==> 02
```

<a name="date_hour"></a>
## date_hour
### Syntax
```
date_hour(value)
```

### Description
Extracts the hour part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.



### Modules
Date

### Examples
```
date_hour('2019010212') ==> 12
```

<a name="date_minute"></a>
## date_minute
### Syntax
```
date_minute(value)
```

### Description
Extracts the hour part from the given date value and
returns it as an integer.



### Modules
Date

### Examples
```
date_minute(parse_date('201901021223', fmt='yyyyMMddHHmm')) ==> 23
```

<a name="date_month"></a>
## date_month
### Syntax
```
date_month(value)
```

### Description
Extracts the month part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.



### Modules
Date

### Examples
```
date_month('20190102') ==> 01
```

<a name="date_second"></a>
## date_second
### Syntax
```
date_second(value)
```

### Description
Extracts the second part from the given date value and
returns it as an integer.



### Modules
Date

### Examples
```
date_second(parse_date('20190102122345', fmt='yyyyMMddHHmmss')) ==> 45
```

<a name="date_year"></a>
## date_year
### Syntax
```
date_year(value)
```

### Description
Extracts the year part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.



### Modules
Date

### Examples
```
date_year('20190102') ==> 2019
```

<a name="decimal"></a>
## decimal
### Syntax
```
decimal(obj)
```

### Description
Converts the obj to a decimal, if possible.



### Modules
Core

### Examples
```
decimal('1.2') ==> 1.2
```

<a name="delete_at"></a>
## delete_at
### Syntax
```
delete_at(lst, index)
```

### Description
Removes the element at the given index from the list lst.
The list is changed in place. Returns the removed element or
NULL, if no element was removed



### Modules
Core

### Examples
```
delete_at(['a', 'b', 'c', 'd'], 2) ==> 'c'
delete_at(['a', 'b', 'c', 'd'], -3) ==> 'b'
def lst = ['a', 'b', 'c', 'd']; delete_at(lst, 2); lst ==> ['a', 'b', 'd']
delete_at(['a', 'b', 'c', 'd'], 4) ==> NULL
```

<a name="diff"></a>
## diff
### Syntax
```
diff(seta, setb)
```

### Description
Returns the difference between seta and setb, i.e. a set
containing all elements in seta, which are not in setb.
Also works for lists.



### Modules
Set

### Examples
```
diff(<<1, 2, 3, 4>>, <<3, 4>>) ==> <<1, 2>>
diff([1, 2, 3, 4], [3, 4]) ==> <<1, 2>>
diff(<<1, 2, 3, 4>>, <<>>) ==> <<1, 2, 3, 4>>
diff(<<1, 2, 3, 4>>, <<1, 2, 3, 4>>) ==> <<>>
```

<a name="dirname"></a>
## dirname
### Syntax
```
dirname(path)
```

### Description
Returns the directory portion of the path.



### Modules
OS

### Examples
```
dirname('dir/file.ext') ==> 'dir'
dirname('file.ext') ==> ''
dirname('a/b/c/d/e/file.ext') ==> 'a/b/c/d/e'
```

<a name="div"></a>
## div
### Syntax
```
div(a, b)
```

### Description
Returns the value of a divided by b. If both values are ints,
then the result is also an int. Otherwise, it is a decimal.



### Modules
Core

### Examples
```
div(6, 2) ==> 3
```

<a name="div0"></a>
## div0
### Syntax
```
div0(a, b, div_0_value = MAXINT)
```

### Description
If b is not zero, the result of a / b is returned.
If b is zero, the value div_0_value is returned.



### Modules
Core

### Examples
```
div0(12, 3) ==> 4
div0(12, 5) ==> 2
div0(12.0, 5) ==> 2.4
div0(12.5, 2) ==> 6.25
div0(12, 0) ==> MAXINT
div0(12, 0, 0) ==> 0
div0(12, 0.0, 0) ==> 0
```

<a name="ends_with"></a>
## ends_with
### Syntax
```
ends_with(str, part)
```

### Description
Returns TRUE if the string str ends with part.



### Modules
Core, String

### Examples
```
ends_with('abcdef', 'def') ==> TRUE
ends_with('abcdef', 'abc') ==> FALSE
ends_with(NULL, 'abc') ==> FALSE
```

<a name="enumerate"></a>
## enumerate
### Syntax
```
enumerate(obj)
```

### Description
Enumerates the contents of a list, set or map.

The result for a list is a list of pairs (index, value).

The result for a map is a list of pairs (key, value).

A set cannot be enumerated, since it does not have a
well defined ordering of its elements. If you wish to
enumerate a set, then convert it into a list and sort it
first, e.g. enumerate(sorted(list(some_set))). But be aware
that this involves a costly sort operation, maybe you
should think about using a list instead of the set in
the first place.

For other data types, an error is thrown.

Typically, you would use this in a for loop, e.g.
  for entry in enumerate(some_list) do
    println('index = ' + entry[0] + ', value = ' + entry[1]);
  end;



### Modules
Core

### Examples
```
enumerate(['a', 'b', 'c']) ==> [[0, 'a'], [1, 'b'], [2, 'c']]
enumerate(<<<a => 5, b => 6, c => 7>>>) ==> [['a', 5], ['b', 6], ['c', 7]]
```

<a name="equals"></a>
## equals
### Syntax
```
equals(a, b)
```

### Description
Returns TRUE if a is equals to b.

Integer values are propagated to decimal values, if required.



### Modules
Core

### Examples
```
equals(1, 2) ==> FALSE
equals(1, 1) ==> TRUE
equals(1, 1.0) ==> TRUE
equals('a', 'b') ==> FALSE
```

<a name="esc"></a>
## esc
### Syntax
```
esc(str)
```

### Description
Escapes the characters <, > and & by their HTML entities.



### Modules
Core, String

### Examples
```
esc('a<b') ==> 'a&lt;b'
esc('<code>') ==> '&lt;code&gt;'
```

<a name="escape_pattern"></a>
## escape_pattern
### Syntax
```
escape_pattern(s)
```

### Description
Escapes special characters in the string s, so that
the result can be used in pattern matching to match
the literal string.

Currently, the | and . characters are escaped.



### Modules
Core

### Examples
```
escape_pattern('|') ==> '\\|'
escape_pattern('|.|') ==> '\\|\\.\\|'
```

<a name="eval"></a>
## eval
### Syntax
```
eval(s)
```

### Description
Evaluates the string or node s.



### Modules
Core

### Examples
```
eval('1+1') ==> 2
```

<a name="execute"></a>
## execute
### Syntax
```
execute(program, args, work_dir = NULL, echo = FALSE, output_file = NULL)
```

### Description
Executes the program and provides the specified arguments in the list args.


### Modules
OS

<a name="exp"></a>
## exp
### Syntax
```
exp(x)
```

### Description
Returns the power e ^ x.



### Modules
Math

### Examples
```
exp(0) ==> 1
```

<a name="file_copy"></a>
## file_copy
### Syntax
```
file_copy(src, dest)
```

### Description
Copies the specified file.


### Modules
OS

<a name="file_delete"></a>
## file_delete
### Syntax
```
file_delete(filename)
```

### Description
Deletes the specified file.


### Modules
OS

<a name="file_exists"></a>
## file_exists
### Syntax
```
file_exists(filename)
```

### Description
Returns TRUE if the specified file exists.


### Modules
OS

<a name="file_extension"></a>
## file_extension
### Syntax
```
file_extension(path)
```

### Description
Returns the file extension of the file path.



### Modules
OS

### Examples
```
file_extension('dir/file.ext') ==> '.ext'
file_extension('dir/file.a.b.c') ==> '.c'
file_extension('dir/file') ==> ''
file_extension('dir/.file') ==> ''
file_extension('dir/.file.cfg') ==> '.cfg'
```

<a name="file_info"></a>
## file_info
### Syntax
```
file_info(filename)
```

### Description
Returns information about the specified file (e.g. modification date, size).


### Modules
OS

<a name="file_input"></a>
## file_input
### Syntax
```
file_input(filename, encoding = 'UTF-8')
```

### Description
Returns an input object, that reads the characters from the given file.


### Modules
IO

<a name="file_move"></a>
## file_move
### Syntax
```
file_move(src, dest)
```

### Description
Moves the specified file.


### Modules
OS

<a name="file_output"></a>
## file_output
### Syntax
```
file_output(filename, encoding = 'UTF-8', append = FALSE)
```

### Description
Returns an output object, that writes to the given file. If
the file exists it is overwritten.


### Modules
IO

<a name="filter"></a>
## filter
### Syntax
```
filter(lst, predicate, key = identity)
```

### Description
Returns a filtered copy of the list by discarding
all elements for which the predicate returns FALSE.



### Modules
List

### Examples
```
[1, 2, 3, 4, 5, 6] !> filter(fn(x) x % 2 == 0) ==> [2, 4, 6]
[1, 'one', 2.2, TRUE, sum] !> filter(is_numeric) ==> [1, 2.2]
[['abc', 1], ['bbc', 2], ['acc', 3]] !> filter(fn(x) x !> starts_with('a'), key = fn(x) x[0]) ==> [['abc', 1], ['acc', 3]]
```

<a name="find"></a>
## find
### Syntax
```
find(obj, part, key = identity, start = 0)
```

### Description
Returns the index of the first occurence of part in obj.
If part is not contained in obj, then -1 is returned. Start specifies
the search start index. It defaults to 0.
Obj can be a string or a list. In case of a string, part can be any
substring, in case of a list, a single element.
In case of lists, the elements can be accessed using the
key function.



### Modules
Core, List, String

### Examples
```
find('abcdefg', 'cde') ==> 2
find('abc|def|ghi', '|', start = 4) ==> 7
find('abcxyabc', 'abc', start = 5) ==> 5
find([1, 2, 3, 4], 3) ==> 2
find(['abc', 'def'], 'e', key = fn(x) x[1]) ==> 1
```

<a name="find_last"></a>
## find_last
### Syntax
```
find_last(obj, part, key = identity, start = length(obj) - 1)
```

### Description
Returns the index of the last  occurence of part in obj.
If part is not contained in obj, then -1 is returned. Start specifies
the search start index. It defaults to length(obj) - 1.
Obj can be a string or a list. In case of a string, part can be any
substring, in case of a list, a single element.
In case of lists, the elements can be accessed using the
key function.



### Modules
Core, List, String

### Examples
```
find_last('abcdefgcdexy', 'cde') ==> 7
find_last('abc|def|ghi|jkl', '|', start = 10) ==> 7
find_last('abcxyabc', 'abc', start = 4) ==> 0
find_last([1, 2, 3, 4, 3], 3) ==> 4
find_last(['abc', 'def'], 'e', key = fn(x) x[1]) ==> 1
```

<a name="first"></a>
## first
### Syntax
```
first(lst)
```

### Description
Returns the first element of a list.



### Modules
List

### Examples
```
first([1, 2, 3]) ==> 1
first(NULL) ==> NULL
```

<a name="first_n"></a>
## first_n
### Syntax
```
first_n(lst, n)
```

### Description
Returns the first n elements of the list.



### Modules
List

### Examples
```
range(100) !> first_n(5) ==> [0, 1, 2, 3, 4]
```

<a name="flatten"></a>
## flatten
### Syntax
```
flatten(lst)
```

### Description
Flattens a list, by replacing child lists with their contents.
This does only work at the top level and does not recurse.



### Modules
List

### Examples
```
flatten([[1, 2], 3, 4, [5, 6]]) ==> [1, 2, 3, 4, 5, 6]
flatten([1, 2, 3]) ==> [1, 2, 3]
flatten([1, [2], [3, 4], [5, [6, 7]]]) ==> [1, 2, 3, 4, 5, [6, 7]]
```

<a name="floor"></a>
## floor
### Syntax
```
floor(x)
```

### Description
Returns the integral decimal value that is equal to or next lower than x.



### Modules
Core

### Examples
```
floor(1.3) ==> 1.0
```

<a name="for_each"></a>
## for_each
### Syntax
```
for_each(lst, func)
```

### Description
Calls the function func once for each successive element
of the list lst.


### Modules
List

<a name="format_date"></a>
## format_date
### Syntax
```
format_date(date, fmt = 'yyyy-MM-dd HH:mm:ss')
```

### Description
Formats the date value according to fmt and returns a string value.



### Modules
Date

### Examples
```
format_date(date('20170102')) ==> '2017-01-02 00:00:00'
format_date(NULL) ==> NULL
format_date(date('2017010212'), fmt = 'HH') ==> '12'
```

<a name="gcd"></a>
## gcd
### Syntax
```
gcd(a, b)
```

### Description
Calculates the greatest common divisor of two
integers a and b.

Use reduce([a, b, ...], gcd) to calculate the gcd
of more than two values.



### Modules
Math

### Examples
```
gcd(2 * 3, 2 * 2) ==> 2
gcd(2 * 2  * 3 * 5, 2  * 3 * 5) ==> 30
reduce([2 * 2 * 3 * 5, 2 * 3 * 3, 2 * 3 * 5 * 7], gcd) ==> 6
```

<a name="geometric_mean"></a>
## geometric_mean
### Syntax
```
geometric_mean(lst)
```

### Description
Returns the geometric mean of lst.



### Modules
Stat

### Examples
```
round(geometric_mean([54, 24, 36]), 1) ==> 36.0
```

<a name="get_env"></a>
## get_env
### Syntax
```
get_env(var)
```

### Description
Returns the value of the environment variable var.


### Modules
OS

<a name="get_output_string"></a>
## get_output_string
### Syntax
```
get_output_string(output)
```

### Description
Returns the value of a string output object.



### Modules
IO

### Examples
```
do def o = str_output(); print('abc', out = o); get_output_string(o); end ==> 'abc'
```

<a name="greater"></a>
## greater
### Syntax
```
greater(a, b)
```

### Description
Returns TRUE if a is greater than b.



### Modules
Core

### Examples
```
greater(1, 2) ==> FALSE
greater(1, 1) ==> FALSE
greater(2, 1) ==> TRUE
```

<a name="greater_equals"></a>
## greater_equals
### Syntax
```
greater_equals(a, b)
```

### Description
Returns TRUE if a is greater than or equals to b.



### Modules
Core

### Examples
```
greater_equals(1, 2) ==> FALSE
greater_equals(1, 1) ==> TRUE
greater_equals(2, 1) ==> TRUE
```

<a name="grep"></a>
## grep
### Syntax
```
grep(lst, pat, key = identity)
```

### Description
Returns the sublist of list lst, that contains only entries,
that match the regular expression pattern pat.

If pat does not contain ^ and $, then the prefix ^.* and the postfix .*$
are added to the pattern, so that the pattern matches partially.



### Modules
List

### Examples
```
grep(['one', 'two', 'three'], //e//) ==> ['one', 'three']
grep(['one', 'two', 'three'], //^one$//) ==> ['one']
grep(['1:2', '12:2', '123:3'], //2//, key = fn(x) split(x, ':')[1]) ==> ['1:2', '12:2']
```

<a name="grouped"></a>
## grouped
### Syntax
```
grouped(lst, cmp = compare, key = identity)
```

### Description
Creates a list of groups, where all equal adjacent elements
of a list are put together in one group.

Typically, you would use the sorted function first to gather
equal elements next to each other.



### Modules
List

### Examples
```
[1, 1, 2, 2, 2, 3, 4, 5, 2] !> grouped() ==> [[1, 1], [2, 2, 2], [3], [4], [5], [2]]
[1, 1, 2, 2, 2] !> grouped() ==> [[1, 1], [2, 2, 2]]
[1, 1] !> grouped() ==> [[1, 1]]
[1] !> grouped() ==> [[1]]
[] !> grouped() ==> []
[[1, 'a'], [1, 'b'], [2, 'c']] !> grouped(key = fn(x) x[0]) ==> [[[1, 'a'], [1, 'b']], [[2, 'c']]]
```

<a name="harmonic_mean"></a>
## harmonic_mean
### Syntax
```
harmonic_mean(lst)
```

### Description
Returns the harmonic mean of lst.



### Modules
Stat

### Examples
```
round(harmonic_mean([40, 60]), 1) ==> 48.0
round(harmonic_mean([2.5, 3, 10]), 1) ==> 3.6
```

<a name="identity"></a>
## identity
### Syntax
```
identity(obj)
```

### Description
Returns obj.



### Modules
Core

### Examples
```
identity(1) ==> 1
identity('a') ==> 'a'
```

<a name="if_empty"></a>
## if_empty
### Syntax
```
if_empty(a, b)
```

### Description
Returns b if a is an empty string otherwise returns a.



### Modules
Core

### Examples
```
if_empty(1, 2) ==> 1
if_empty('', 2) ==> 2
```

<a name="if_null"></a>
## if_null
### Syntax
```
if_null(a, b)
```

### Description
Returns b if a is NULL otherwise returns a.



### Modules
Core

### Examples
```
if_null(1, 2) ==> 1
if_null(NULL, 2) ==> 2
```

<a name="if_null_or_empty"></a>
## if_null_or_empty
### Syntax
```
if_null_or_empty(a, b)
```

### Description
Returns b if a is null or an empty string otherwise returns a.



### Modules
Core

### Examples
```
if_null_or_empty(1, 2) ==> 1
if_null_or_empty(NULL, 2) ==> 2
if_null_or_empty('', 2) ==> 2
```

<a name="info"></a>
## info
### Syntax
```
info(obj)
```

### Description
Returns the info associated with an object.


### Modules
Core

<a name="insert_at"></a>
## insert_at
### Syntax
```
insert_at(lst, index, value)
```

### Description
Inserts the element at the given index of the list lst.
The list is changed in place. Returns the changed list.
If index is out of bounds, the list is not changed at all.



### Modules
Core

### Examples
```
insert_at([1, 2, 3], 0, 9) ==> [9, 1, 2, 3]
insert_at([1, 2, 3], 2, 9) ==> [1, 2, 9, 3]
insert_at([1, 2, 3], 3, 9) ==> [1, 2, 3, 9]
insert_at([1, 2, 3], -1, 9) ==> [1, 2, 3, 9]
insert_at([1, 2, 3], -2, 9) ==> [1, 2, 9, 3]
insert_at([1, 2, 3], 4, 9) ==> [1, 2, 3]
```

<a name="int"></a>
## int
### Syntax
```
int(obj)
```

### Description
Converts the obj to an int, if possible.



### Modules
Core

### Examples
```
int('1') ==> 1
```

<a name="intersection"></a>
## intersection
### Syntax
```
intersection(seta, setb)
```

### Description
Returns the intersection of the two sets. Also works for lists.



### Modules
Set

### Examples
```
intersection(<<1, 2, 3>>, <<2, 3, 4>>) ==> <<2, 3>>
intersection([1, 2, 3], [2, 3, 4]) ==> <<2, 3>>
intersection(<<1, 2>>, <<3, 4>>) ==> <<>>
intersection(<<1, 2>>, <<>>) ==> <<>>
```

<a name="interval"></a>
## interval
### Syntax
```
interval(a)
interval(a, b)
```

### Description
Returns the interval of integers between a and b, inclusive.
If only a is provided, the interval is taken to be [1, ... a]



### Modules
Core

### Examples
```
interval(1, 10) ==> [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
interval(1, 1) ==> [1]
interval(10) ==> [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
```

<a name="is_alphanumerical"></a>
## is_alphanumerical
### Syntax
```
is_alphanumerical(str, min = 0, max = 99999)
```

### Description
Returns TRUE if the string is alphanumerical, i.e. contains only a-z, A-Z and 0-9.
It is possible to specify minimal and maximal length using the min and max optional
parameters.



### Modules
Core, Predicate

### Examples
```
is_alphanumerical('Ab12') ==> TRUE
```

<a name="is_boolean"></a>
## is_boolean
### Syntax
```
is_boolean(obj) 
```

### Description
Returns TRUE if the object is of type boolean.



### Modules
Type

### Examples
```
is_boolean(1 == 2) ==> TRUE
```

<a name="is_decimal"></a>
## is_decimal
### Syntax
```
is_decimal(obj) 
```

### Description
Returns TRUE if the object is of type decimal.



### Modules
Type

### Examples
```
is_decimal(123.45) ==> TRUE
```

<a name="is_empty"></a>
## is_empty
### Syntax
```
is_empty(obj)
```

### Description
Returns TRUE, if the obj is empty.
Lists, sets and maps are empty, if they do not contain elements.
Strings are empty, if the contain no characters. NULL is always empty.



### Modules
Core, Predicate

### Examples
```
is_empty(NULL) ==> TRUE
is_empty(1) ==> FALSE
is_empty([]) ==> TRUE
is_empty(<<>>) ==> TRUE
is_empty(set([1, 2])) ==> FALSE
is_empty('') ==> TRUE
```

<a name="is_even"></a>
## is_even
### Syntax
```
is_even(n)
```

### Description
Returns TRUE if the number is even.



### Modules
Math

### Examples
```
is_even(2) ==> TRUE
is_even(3) ==> FALSE
```

<a name="is_func"></a>
## is_func
### Syntax
```
is_func(obj) 
```

### Description
Returns TRUE if the object is of type func.



### Modules
Type

### Examples
```
is_func(fn(x) 2 * x) ==> TRUE
is_func(sum) ==> TRUE
```

<a name="is_int"></a>
## is_int
### Syntax
```
is_int(obj) 
```

### Description
Returns TRUE if the object is of type int.



### Modules
Type

### Examples
```
is_int(123) ==> TRUE
```

<a name="is_list"></a>
## is_list
### Syntax
```
is_list(obj) 
```

### Description
Returns TRUE if the object is of type list.



### Modules
Core, Type

### Examples
```
is_list([1, 2, 3]) ==> TRUE
```

<a name="is_map"></a>
## is_map
### Syntax
```
is_map(obj) 
```

### Description
Returns TRUE if the object is of type map.



### Modules
Core, Type

### Examples
```
is_map(map([['a', 1], ['b', 2]])) ==> TRUE
```

<a name="is_negative"></a>
## is_negative
### Syntax
```
is_negative(obj)
```

### Description
Returns TRUE if the obj is negative.



### Modules
Core, Predicate

### Examples
```
is_negative(-1) ==> TRUE
```

<a name="is_node"></a>
## is_node
### Syntax
```
is_node(obj) 
```

### Description
Returns TRUE if the object is of type node.


### Modules
Type

<a name="is_not_empty"></a>
## is_not_empty
### Syntax
```
is_not_empty(obj)
```

### Description
Returns TRUE, if the obj is not empty.
Lists, sets and maps are empty, if they do not contain elements.
Strings are empty, if the contain no characters. NULL is always empty.



### Modules
Core, Predicate

### Examples
```
is_not_empty([]) ==> FALSE
is_not_empty(set([1, 2])) ==> TRUE
is_not_empty('a') ==> TRUE
```

<a name="is_not_null"></a>
## is_not_null
### Syntax
```
is_not_null(obj)
```

### Description
Returns TRUE, if the obj is not NULL.



### Modules
Core, Predicate

### Examples
```
is_not_null('') ==> TRUE
is_not_null(1) ==> TRUE
is_not_null(NULL) ==> FALSE
```

<a name="is_null"></a>
## is_null
### Syntax
```
is_null(obj)
```

### Description
Returns TRUE, if the obj is NULL.



### Modules
Core, Predicate

### Examples
```
is_null('') ==> FALSE
is_null(1) ==> FALSE
is_null(NULL) ==> TRUE
```

<a name="is_numeric"></a>
## is_numeric
### Syntax
```
is_numeric(obj) 
```

### Description
Returns TRUE if the object is of type numeric. Numeric
types are int and decimal.



### Modules
Core, Type

### Examples
```
is_numeric(123) ==> TRUE
is_numeric(123.45) ==> TRUE
```

<a name="is_numerical"></a>
## is_numerical
### Syntax
```
is_numerical(str, min = 0, max = 99999)
```

### Description
Returns TRUE if the string is numerical, i.e. contains only 0-9. It is possible to
specify minimal and maximal length using the min and max optional parameters.



### Modules
Core, Predicate

### Examples
```
is_numerical('123') ==> TRUE
```

<a name="is_object"></a>
## is_object
### Syntax
```
is_object(obj)
```

### Description
Returns TRUE if the object is of type object.



### Modules
Core, Type

### Examples
```
is_object(object()) ==> TRUE
is_object(map()) ==> FALSE
```

<a name="is_odd"></a>
## is_odd
### Syntax
```
is_odd(n)
```

### Description
Returns TRUE if the number is odd.



### Modules
Math

### Examples
```
is_odd(2) ==> FALSE
is_odd(3) ==> TRUE
```

<a name="is_positive"></a>
## is_positive
### Syntax
```
is_positive(obj)
```

### Description
Returns TRUE if the obj is positive.



### Modules
Predicate

### Examples
```
is_positive(1) ==> TRUE
```

<a name="is_set"></a>
## is_set
### Syntax
```
is_set(obj) 
```

### Description
Returns TRUE if the object is of type set.



### Modules
Core, Type

### Examples
```
is_set(set([1, 2, 3])) ==> TRUE
```

<a name="is_string"></a>
## is_string
### Syntax
```
is_string(obj) 
```

### Description
Returns TRUE if the object is of type string.



### Modules
Core, Type

### Examples
```
is_string('abc') ==> TRUE
```

<a name="is_valid_date"></a>
## is_valid_date
### Syntax
```
is_valid_date(str, fmt='yyyyMMdd')
```

### Description
Returns TRUE if the string represents a valid date. The default format
is yyyyMMdd. It is possible to specify different formats using the fmt
optional parameter.



### Modules
Core, Date

### Examples
```
is_valid_date('20170304') ==> TRUE
is_valid_date('2017030412') ==> FALSE
is_valid_date('20170399') ==> FALSE
```

<a name="is_valid_time"></a>
## is_valid_time
### Syntax
```
is_valid_time(str, fmt='HHmm')
```

### Description
Returns TRUE if the string represents a valid time. The default format
is HHmm. It is possible to specify different formats using the fmt
optional parameter.



### Modules
Core, Date

### Examples
```
is_valid_time('1245') ==> TRUE
```

<a name="is_zero"></a>
## is_zero
### Syntax
```
is_zero(obj)
```

### Description
Returns TRUE if the obj is zero.



### Modules
Core, Predicate

### Examples
```
is_zero(0) ==> TRUE
```

<a name="iso_date"></a>
## iso_date
### Syntax
```
iso_date()
iso_date(value)
```

### Description
Formats the date value as an ISO date (i.e. using format yyyy-MM-dd).
If no date value is provided, the current date is used.



### Modules
Date

### Examples
```
iso_date(date('20200203')) ==> '2020-02-03'
```

<a name="iso_datetime"></a>
## iso_datetime
### Syntax
```
iso_datetime()
iso_datetime(value)
```

### Description
Formats the datetime value as an ISO datetime (i.e. using format yyyy-MM-dd'T'HH:mm:ss).
If no datetime value is provided, the current datetime is used.



### Modules
Date

### Examples
```
iso_datetime(date('20200203201544')) ==> '2020-02-03T20:15:44'
```

<a name="join"></a>
## join
### Syntax
```
join(lst, sep = ' ')
```

### Description
Returns a string containing all elements of the list lst
separated by the string sep.



### Modules
Core, String

### Examples
```
join([1, 2, 3], '|') ==> '1|2|3'
join(['one', 'world'], '--') ==> 'one--world'
join([], '|') ==> ''
join([1], '|') ==> '1'
join('|', [1, 2, 3]) ==> '1|2|3'
```

<a name="label_data"></a>
## label_data
### Syntax
```
label_data(labels, data)
```

### Description
Creates a map that labels the data with the
given labels.

Labels and data must be two lists of equal
length. Labels must be unique.



### Modules
Core

### Examples
```
label_data(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>
```

<a name="last"></a>
## last
### Syntax
```
last(lst)
```

### Description
Returns the last element of a list.



### Modules
List

### Examples
```
last([1, 2, 3]) ==> 3
last(NULL) ==> NULL
```

<a name="last_n"></a>
## last_n
### Syntax
```
last_n(lst, n)
```

### Description
Returns the last n elements of the list.



### Modules
List

### Examples
```
range(100) !> last_n(5) ==> [95, 96, 97, 98, 99]
```

<a name="lcm"></a>
## lcm
### Syntax
```
lcm(a, b)
```

### Description
Calculates the least common multiple of two
integers a and b.

Use reduce([a, b, ...], lcm) to calculate the lcm
of more than two values.



### Modules
Math

### Examples
```
lcm(2 * 2 * 2 * 3, 2 * 2 * 3 * 3) ==> 72
```

<a name="length"></a>
## length
### Syntax
```
length(obj)
```

### Description
Returns the length of obj. This only works for strings, lists, sets and maps.



### Modules
Core

### Examples
```
length('123') ==> 3
length([1, 2, 3]) ==> 3
length(<<1, 2, 3>>) ==> 3
<<<'a' => 1, 'b' => 2, 'c' =>3>>> !> length() ==> 3
length(object()) ==> 0
```

<a name="less"></a>
## less
### Syntax
```
less(obj)
```

### Description
Returns TRUE if a is less than b.



### Modules
Core

### Examples
```
less(1, 2) ==> TRUE
```

<a name="less_equals"></a>
## less_equals
### Syntax
```
less_equals(a, b)
```

### Description
Returns TRUE if a is less than or equals to b.



### Modules
Core

### Examples
```
less_equals(1, 2) ==> TRUE
less_equals(2, 1) ==> FALSE
less_equals(1, 1) ==> TRUE
```

<a name="lines"></a>
## lines
### Syntax
```
lines(str)
```

### Description
Splits the string str into lines and returns them as a list.



### Modules
Core

### Examples
```
lines('a\nb c\r\nd') ==> ['a', 'b c', 'd']
```

<a name="list"></a>
## list
### Syntax
```
list(obj)
```

### Description
Converts the obj to a list.



### Modules
Core

### Examples
```
list(1) ==> [1]
```

<a name="list_dir"></a>
## list_dir
### Syntax
```
list_dir(dir, recursive = FALSE, include_path = FALSE, include_dirs = FALSE)
```

### Description
Enumerates the files and directories in the specified directory and
returns a list of filename or paths.


### Modules
OS

<a name="log"></a>
## log
### Syntax
```
log(x)
```

### Description
Returns the natural logarithm of x.



### Modules
Math

### Examples
```
int(log(E)) ==> 1
```

<a name="log10"></a>
## log10
### Syntax
```
log10(x)
```

### Description
Returns the logarithm of x to base 10.



### Modules
Math

### Examples
```
int(log10(1000)) ==> 3
```

<a name="log2"></a>
## log2
### Syntax
```
log2(x)
```

### Description
Returns the logarithm of x to base 2.



### Modules
Math

### Examples
```
int(log2(1024)) ==> 10
```

<a name="lower"></a>
## lower
### Syntax
```
lower(str)
```

### Description
Converts str to lower case letters.



### Modules
String

### Examples
```
lower('Hello') ==> 'hello'
```

<a name="ls"></a>
## ls
### Syntax
```
ls()
ls(module)
```

### Description
Returns a list of all defined symbols (functions and constants) in
the current environment or in the specified module.

### Modules
Core

<a name="make_dir"></a>
## make_dir
### Syntax
```
make_dir(dir, with_parents = FALSE)
```

### Description
Creates a new directory.


### Modules
OS

<a name="map"></a>
## map
### Syntax
```
map(obj)
map()
```

### Description
Converts the obj to a map, if possible. If obj is omitted,
an empty map is returned.



### Modules
Core

### Examples
```
map([[1, 2], [3, 4]]) ==> <<<1 => 2, 3 => 4>>>
map() ==> <<<>>>
```

<a name="map_get"></a>
## map_get
### Syntax
```
map_get(m, k, default_value=NULL)
```

### Description
If the map m contains the key k, then the corresponding
value is returned. Otherwise, the default_value is
returned.



### Modules
Core

### Examples
```
map_get(<<<a => 1, b => 2>>>, 'a') ==> 1
map_get(<<<a => 1, b => 2>>>, 'b') ==> 2
map_get(<<<a => 1, b => 2>>>, 'c') ==> NULL
map_get(<<<a => 1, b => 2>>>, 'c', default_value = 9) ==> 9
```

<a name="map_get_pattern"></a>
## map_get_pattern
### Syntax
```
map_get_pattern(m, k, default_value=NULL)
```

### Description
The map m is assumed to contain regex patterns as keys.
If the key k matches one of the regex patterns, then
the corresponding value is returned. Otherwise, the
default_value is returned.

If more than one pattern matches the key k, then it is
undefined, which pattern is selected for retrieving its
value.



### Modules
Core

### Examples
```
map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'a') ==> 1
map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'b') ==> 1
map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'c') ==> 2
map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'd') ==> 2
map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'e') ==> NULL
```

<a name="map_list"></a>
## map_list
### Syntax
```
map_list(lst, f)
```

### Description
Returns a list where each element is the corresponding
element of lst with func applied. Thus, the elements of
the list are mapped using the function func to new values.



### Modules
List

### Examples
```
map_list([1, 2, 3], fn(x) 2 * x) ==> [2, 4, 6]
['one', 'two', 'three'] !> map_list(fn(x) '*' + x + '*') ==> ['*one*', '*two*', '*three*']
map_list(identity, [1, 2, 3]) ==> [1, 2, 3]
```

<a name="matches"></a>
## matches
### Syntax
```
matches(str, pattern)
```

### Description
Returns TRUE, if str matches the regular expression pattern.



### Modules
Core, String

### Examples
```
matches('abc12', //[a-c]+[1-9]+//) ==> TRUE
matches(NULL, //[a-c]+[1-9]+//) ==> FALSE
```

<a name="max"></a>
## max
### Syntax
```
max(a, b, key = identity) 
max(a, key = identity)
```

### Description
Returns the maximum of the values a, b. 

Returns the maximum value of the list a.

The optional key parameter takes a function with one parameter, which 
is used to get the value from a and b that is used for the comparison. 
Default key is the identity function.



### Modules
Core

### Examples
```
max(1, 2) ==> 2
max([1, 'z'], [2, 'a'], key = fn(x) x[1]) ==> [1, 'z']
max([1, 3, 2, 4, 2]) ==> 4
```

<a name="mean"></a>
## mean
### Syntax
```
mean(lst)
```

### Description
Returns the mean of lst.



### Modules
Stat

### Examples
```
mean([1, 2, 3, 4, 4]) ==> 2.8
mean([-1.0, 2.5, 3.25, 5.75]) ==> 2.625
```

<a name="median"></a>
## median
### Syntax
```
median(lst)
```

### Description
Returns the median of lst, using the 'mean of middle two'
method.



### Modules
Stat

### Examples
```
median([1, 3, 5]) ==> 3
median([1, 3, 5, 7]) ==> 4.0
```

<a name="median_high"></a>
## median_high
### Syntax
```
median_high(lst)
```

### Description
Returns the high median of lst.



### Modules
Stat

### Examples
```
median_high([1, 3, 5]) ==> 3
median_high([1, 3, 5, 7]) ==> 5
```

<a name="median_low"></a>
## median_low
### Syntax
```
median_low(lst)
```

### Description
Returns the low median of lst.



### Modules
Stat

### Examples
```
median_low([1, 3, 5]) ==> 3
median_low([1, 3, 5, 7]) ==> 3
```

<a name="min"></a>
## min
### Syntax
```
min(a, b, key = identity) 
min(a, key = identity)
```

### Description
Returns the minimum of the values a, b. 

Returns the mininmum value of the list a.

The optional key parameter takes a function with one parameter, which 
is used to get the value from a and b that is used for the comparison. 
Default key is the identity function.



### Modules
Core

### Examples
```
min(1, 2) ==> 1
min([1, 'z'], [2, 'a'], key = fn(x) x[1]) ==> [2, 'a']
min([1, 3, 2, 4, 2]) ==> 1
```

<a name="mod"></a>
## mod
### Syntax
```
mod(a, b)
```

### Description
Returns the modulus of a modulo b.



### Modules
Core

### Examples
```
mod(7, 2) ==> 1
```

<a name="mul"></a>
## mul
### Syntax
```
mul(a, b)
```

### Description
Returns the product of a and b. For numerical values this uses the usual arithmetic.
If a is a string and b is an int, then the string a is repeated b times. If a is a
list and b is an int, then the list is repeated b times.



### Modules
Core

### Examples
```
mul(2, 3) ==> 6
mul('2', 3) ==> '222'
mul([1, 2], 3) ==> [1, 2, 1, 2, 1, 2]
```

<a name="new"></a>
## new
### Syntax
```
new(cls, args...)
```

### Description
Creates an instance of the class cls.


### Modules
Core

<a name="non_empty"></a>
## non_empty
### Syntax
```
non_empty(a, b) 
```

### Description
Returns the value a, if a is a non-empty string, otherwise returns b.



### Modules
Core

### Examples
```
non_empty('a', 'b') ==> 'a'
non_empty('', 'b') ==> 'b'
```

<a name="non_zero"></a>
## non_zero
### Syntax
```
non_zero(a, b) 
```

### Description
Returns the value a, if a is a non-zero integer, otherwise returns b.



### Modules
Core

### Examples
```
non_zero(1, 2) ==> 1
non_zero(0, 2) ==> 2
```

<a name="not_equals"></a>
## not_equals
### Syntax
```
not_equals(a, b)
```

### Description
Returns TRUE if a is not equals to b.

Integer values are propagated to decimal values, if required.



### Modules
Core

### Examples
```
not_equals(1, 2) ==> TRUE
not_equals(1, 1) ==> FALSE
not_equals(1, 1.0) ==> FALSE
not_equals('a', 'b') ==> TRUE
```

<a name="now"></a>
## now
### Syntax
```
now()
```

### Description
Returns the current date.


### Modules
Core

<a name="object"></a>
## object
### Syntax
```
object()
object(obj)
```

### Description
Creates an empty object value or converts a list of pairs or a map to an object.



### Modules
Core

### Examples
```
object() ==> <**>
object(<<<a => 1>>>) ==> <*a=1*>
object([['a', 1]]) ==> <*a=1*>
```

<a name="ord"></a>
## ord
### Syntax
```
ord(ch)
```

### Description
Returns the code point integer of the character ch.



### Modules
String

### Examples
```
ord('a') ==> 97
ord(' ') ==> 32
```

<a name="pairs"></a>
## pairs
### Syntax
```
pairs(lst)
```

### Description
Returns a list where each entry consists of a pair
of elements of lst.



### Modules
Core

### Examples
```
pairs([1, 2, 3]) ==> [[1, 2], [2, 3]]
pairs([1, 2, 3, 4]) ==> [[1, 2], [2, 3], [3, 4]]
```

<a name="parse"></a>
## parse
### Syntax
```
parse(s)
```

### Description
Parses the string s.



### Modules
Core

### Examples
```
parse('2+3') ==> '(add 2, 3)'
```

<a name="parse_date"></a>
## parse_date
### Syntax
```
parse_date(str, fmt = 'yyyyMMdd')
```

### Description
Parses the string str according to fmt and returns a datetime value.
If the format does not match or if the date is invalid, the NULL
value is returned.

It is possible to pass a list of formats to the fmt parameter.
The function sequentially tries to convert the str and if it
works, returns the value.



### Modules
Date

### Examples
```
parse_date('20170102') ==> '20170102000000'
parse_date('20170102', fmt = 'yyyyMMdd') ==> '20170102000000'
parse_date('2017010222', fmt = 'yyyyMMdd') ==> NULL
parse_date('20170102', fmt = 'yyyyMMddHH') ==> NULL
parse_date('20170102', fmt = ['yyyyMMdd']) ==> '20170102000000'
parse_date('201701022015', fmt = ['yyyyMMddHHmm', 'yyyyMMddHH', 'yyyyMMdd']) ==> '20170102201500'
parse_date('20170112', fmt = ['yyyyMM', 'yyyy']) ==> NULL
parse_date('20170144') ==> NULL
```

<a name="parse_json"></a>
## parse_json
### Syntax
```
parse_json(s)
```

### Description
Parses the JSON string s and returns a map or list.



### Modules
Core

### Examples
```
parse_json('{"a": 12, "b": [1, 2, 3, 4]}') ==> '<<<\'a\' => 12, \'b\' => [1, 2, 3, 4]>>>'
parse_json('[1, 2.5, 3, 4]') ==> '[1, 2.5, 3, 4]'
```

<a name="path"></a>
## path
### Syntax
```
def path(parts...)
```

### Description
Combines the path elements into a path by
joining them with the PS path separator.


### Modules
OS

<a name="pattern"></a>
## pattern
### Syntax
```
pattern(obj)
```

### Description
Converts the obj to a regexp pattern, if possible.



### Modules
Core

### Examples
```
pattern('xy[1-9]{3}') ==> //xy[1-9]{3}//
```

<a name="permutations"></a>
## permutations
### Syntax
```
permutations(lst)
```

### Description
Returns a list containing all permutations of the input list.



### Modules
List

### Examples
```
permutations([1, 2, 3]) ==> [[1, 2, 3], [2, 1, 3], [3, 1, 2], [1, 3, 2], [2, 3, 1], [3, 2, 1]]
```

<a name="pow"></a>
## pow
### Syntax
```
pow(x, y)
```

### Description
Returns the power x ^ y.



### Modules
Math

### Examples
```
pow(2, 3) ==> 8
pow(2.5, 2) ==> 6.25
pow(4, 2) ==> 16
pow(4.0, 2.0) ==> 16.0
round(pow(2, 1.5), digits = 3) ==> 2.828
```

<a name="print"></a>
## print
### Syntax
```
print(obj, out = stdout)
```

### Description
Prints the obj to the output out. Default output is stdout which
may be connected to the console (e.g. in case of REPL) or a file
or be silently ignored.



### Modules
Core, IO

### Examples
```
print('hello') ==> NULL
```

<a name="printf"></a>
## printf
### Syntax
```
printf(fmt, args...)
```

### Description
Formats and prints a string format using the provided args.
The string is printed to standard output.

This is basically the combination of print and sprintf.


### Modules
IO

<a name="println"></a>
## println
### Syntax
```
println(obj = '', out = stdout)
```

### Description
Prints the obj to the output out and terminates the line. Default
output is stdout which may be connected to the console (e.g. in
case of REPL) or a file or be silently ignored.



### Modules
Core, IO

### Examples
```
println('hello') ==> NULL
```

<a name="process_lines"></a>
## process_lines
### Syntax
```
process_lines(input, callback)
```

### Description
Reads lines from the input and calls the callback function
once for each line. The line string is the single argument
of the callback function.

If input is a list, then each list element is converted to
a string and processed as a line

The function returns the number of processed lines.


### Modules
IO

### Examples
```
def result = []; str_input('one\ntwo\nthree') !> process_lines(fn(line) result !> append(line)); result ==> ['one', 'two', 'three']
str_input('one\ntwo\nthree') !> process_lines(fn(line) line) ==> 3
def result = ''; process_lines(['a', 'b', 'c'], fn(line) result += line); result ==> 'abc'
```

<a name="prod"></a>
## prod
### Syntax
```
prod(list)
```

### Description
Returns the product of a list of numbers.



### Modules
List

### Examples
```
prod([1, 2, 3]) ==> 6
prod(range(1, 10)) ==> 362880
```

<a name="put"></a>
## put
### Syntax
```
put(m, key, value)
```

### Description
Puts the value into the map m at the given key.



### Modules
Core

### Examples
```
def m = map([[1, 2], [3, 4]]); put(m, 1, 9) ==> <<<1 => 9, 3 => 4>>>
```

<a name="q"></a>
## q
### Syntax
```
q(lst)
```

### Description
Returns a string containing all elements of the list lst
separated by a pipe character.



### Modules
Core, String

### Examples
```
q([1, 2, 3]) ==> '1|2|3'
q([]) ==> ''
```

<a name="random"></a>
## random
### Syntax
```
random()
random(a)
random(a, b)
```

### Description
Returns a random number. If no argument is provided, a decimal
value in the range [0, 1) is returned. If only a is provided, then 
an int value in the range [0, a) is returned. If both a and b are
provided, then an int value in the range [a, b) is returned.



### Modules
Random

### Examples
```
set_seed(1); random(5) ==> 1
```

<a name="range"></a>
## range
### Syntax
```
range(a)
range(a, b)
range(a, b, step)
```

### Description
Returns a list containing int values in the range. If only a is
provided, the range is [0, a). If both a and b are provided, the
range is [a, b). If step is given, then only every step element
is included in the list.



### Modules
Core

### Examples
```
range(4) ==> [0, 1, 2, 3]
range(3, 6) ==> [3, 4, 5]
range(10, step = 3) ==> [0, 3, 6, 9]
range(10, 0, step = -2) ==> [10, 8, 6, 4, 2]
```

<a name="read"></a>
## read
### Syntax
```
read(input = stdin)
```

### Description
Read a character from the input. If end of input is reached, an empty string is returned.



### Modules
IO

### Examples
```
def s = str_input('hello'); read(s) ==> 'h'
```

<a name="read_all"></a>
## read_all
### Syntax
```
read_all(input = stdin)
```

### Description
Read the whole input. If end of input is reached, NULL is returned.



### Modules
IO

### Examples
```
def s = str_input('hello'); read_all(s) ==> 'hello'
```

<a name="read_file"></a>
## read_file
### Syntax
```
read_file(filename, encoding = 'utf-8')
```

### Description
Opens a file, reads the contents as a single
string, closes the file and returns the string.


### Modules
IO

<a name="readln"></a>
## readln
### Syntax
```
readln(input = stdin)
```

### Description
Read one line from the input. If end of input is reached, NULL is returned.



### Modules
IO

### Examples
```
def s = str_input('hello'); readln(s) ==> 'hello'
```

<a name="reduce"></a>
## reduce
### Syntax
```
reduce(list, f)
```

### Description
Reduces a list by successively applying the binary function f to
partial results and list elements.



### Modules
List

### Examples
```
reduce([1, 2, 3, 4], add) ==> 10
```

<a name="remove"></a>
## remove
### Syntax
```
remove(lst, element)
```

### Description
Removes the element from the list lst. The lst may also be a set or a map.
Returns the changed list, but the list is changed in place.



### Modules
Core

### Examples
```
remove([1, 2, 3, 4], 3) ==> [1, 2, 4]
remove(<<1, 2, 3, 4>>, 3) ==> <<1, 2, 4>>
remove(<<<a => 1, b => 2, c => 3, d => 4>>>, 'c') ==> <<<'a' => 1, 'b' => 2, 'd' => 4>>>
remove(<*a=1, b=2*>, 'b') ==> <*a=1*>
```

<a name="replace"></a>
## replace
### Syntax
```
replace(s, a, b, start = 0)
```

### Description
Replaces all occurences of a in the string s with b.
The optional parameter start specifies the start index.



### Modules
Core, String

### Examples
```
replace('abc', 'b', 'x') ==> 'axc'
replace('abc', 'b', 'xy') ==> 'axyc'
replace('abcdef', 'bcd', 'xy') ==> 'axyef'
replace('abcabcabc', 'abc', 'xy', start = 3) ==> 'abcxyxy'
```

<a name="rest"></a>
## rest
### Syntax
```
rest(lst)
```

### Description
Returns the rest of a list, i.e. everything but the first element.



### Modules
List

### Examples
```
rest([1, 2, 3]) ==> [2, 3]
```

<a name="reverse"></a>
## reverse
### Syntax
```
reverse(obj)
```

### Description
Returns a reversed copy of a string or a list.



### Modules
List

### Examples
```
reverse([1, 2, 3]) ==> [3, 2, 1]
reverse('abc') ==> 'cba'
```

<a name="reverse"></a>
## reverse
### Syntax
```
reverse(str)
```

### Description
Returns a reversed copy of a string.



### Modules
String

### Examples
```
reverse('abc') ==> 'cba'
reverse(NULL) ==> NULL
reverse(12) ==> NULL
```

<a name="reverse_list"></a>
## reverse_list
### Syntax
```
reverse_list(list)
```

### Description
Returns a reversed copy of a list.



### Modules
List

### Examples
```
reverse_list([1, 2, 3]) ==> [3, 2, 1]
reverse_list(NULL) ==> NULL
reverse_list('abc') ==> NULL
```

<a name="reverse_string"></a>
## reverse_string
### Syntax
```
reverse(str)
```

### Description
Returns a reversed copy of a string.



### Modules
Core

### Examples
```
reverse('abc') ==> 'cba'
reverse(NULL) ==> NULL
reverse(12) ==> NULL
```

<a name="round"></a>
## round
### Syntax
```
round(x, digits = 0)
```

### Description
Returns the decimal value x rounded to the specified number of digits.
Default for digits is 0.



### Modules
Core

### Examples
```
round(1.345, digits = 1) ==> 1.3
```

<a name="run"></a>
## run
### Syntax
```
run(file)
```

### Description
Loads and interprets the file.


### Modules
Core

<a name="s"></a>
## s
### Syntax
```
s(str, start = 0)
```

### Description
Returns a string, where all placeholders are replaced with their
appropriate values. Placeholder have the form '{var}' and result
in the value of the variable var inserted at this location.

The placeholder can also be expressions and their result will
be inserted instead of the placeholder.

There are formatting suffixes to the placeholder, which allow
some control over the formatting. They formatting spec starts after
a # character and consists of align/fill, width and precision fields.
For example #06.2 will format the decimal to a width of six characters
and uses two digits after the decimal point. If the number is less than
six characters wide, then it is prefixed with zeroes until the width
is reached, e.g. '001.23'. Please refer to the examples below.



### Modules
Core, String

### Examples
```
def name = 'damian'; s('hello {name}') ==> 'hello damian'
def foo = '{bar}'; def bar = 'baz'; s('{foo}{bar}') ==> '{bar}baz'
def a = 'abc'; s('"{a#-8}"') ==> '"abc     "'
def a = 'abc'; s('"{a#8}"') ==> '"     abc"'
def a = 'abc'; s('a = {a#5}') ==> 'a =   abc'
def a = 'abc'; s('a = {a#-5}') ==> 'a = abc  '
def n = 12; s('n = {n#5}') ==> 'n =    12'
def n = 12; s('n = {n#-5}') ==> 'n = 12   '
def n = 12; s('n = {n#05}') ==> 'n = 00012'
def n = 1.2345678; s('n = {n#.2}') ==> 'n = 1.23'
def n = 1.2345678; s('n = {n#06.2}') ==> 'n = 001.23'
s('2x3 = {2*3}') ==> '2x3 = 6'
def n = 123; s('n = {n#x}') ==> 'n = 7b'
def n = 255; s('n = {n#04x}') ==> 'n = 00ff'
require Math; s('{Math->PI} is cool') ==> '3.141592653589793 is cool'
```

<a name="sample"></a>
## sample
### Syntax
```
sample(lst, n)
```

### Description
Returns a set with n random elements from the list or set lst,
without repetitions. Also works with a string.



### Modules
Random

### Examples
```
sample([1, 2, 3], 3) ==> <<1, 2, 3>>
sample([1, 1, 1, 2, 2, 3], 3) ==> <<1, 2, 3>>
sample('abc', 3) ==> <<'a', 'b', 'c'>>
```

<a name="set"></a>
## set
### Syntax
```
set(obj)
```

### Description
Converts the obj to a set, if possible.



### Modules
Core

### Examples
```
set([1, 2, 3]) ==> <<1, 2, 3>>
```

<a name="set_seed"></a>
## set_seed
### Syntax
```
set_seed(n)
```

### Description
Sets the seed of the random number generator to n.



### Modules
Random

### Examples
```
set_seed(1) ==> 1
```

<a name="sign"></a>
## sign
### Syntax
```
sign(n) 
```

### Description
Returns the signum of n



### Modules
Core, Math

### Examples
```
sign(2) ==> 1
sign(-3) ==> -1
```

<a name="sin"></a>
## sin
### Syntax
```
sin(x)
```

### Description
Returns the sinus of x.



### Modules
Math

### Examples
```
sin(0) ==> 0.0
```

<a name="sorted"></a>
## sorted
### Syntax
```
sorted(lst, cmp=compare, key=identity)
```

### Description
Returns a sorted copy of the list. This is sorted according to the
value returned by the key function for each element of the list.
The values are compared using the compare function cmp.



### Modules
Core

### Examples
```
sorted([3, 2, 1]) ==> [1, 2, 3]
sorted([6, 2, 5, 3, 1, 4]) ==> [1, 2, 3, 4, 5, 6]
```

<a name="split"></a>
## split
### Syntax
```
split(str, delim = '[ \t]+')
```

### Description
Splits the string str into parts and returns a list of strings.
The delim is a regular expression. Default is spaces or tabs.



### Modules
Core, String

### Examples
```
split('a,b,c', //,//) ==> ['a', 'b', 'c']
```

<a name="split2"></a>
## split2
### Syntax
```
split2(str, sep1, sep2)
```

### Description
Performs a two-stage split of the string data.
This results in a list of list of strings.



### Modules
Core, String

### Examples
```
split2('a:b:c|d:e:f', escape_pattern('|'), escape_pattern(':')) ==> [['a', 'b', 'c'], ['d', 'e', 'f']]
split2('', //\|//, //://) ==> []
```

<a name="sprintf"></a>
## sprintf
### Syntax
```
sprintf(fmt, args...)
```

### Description
Formats a string format using the provided args. Each
value can be referred to in the fmt string using the
{0} syntax, where 0 means the first argument passed.

This uses internally the s function. See there for
an explanation of available formatting suffixes.



### Modules
Core

### Examples
```
sprintf('{0} {1}', 1, 2) ==> '1 2'
sprintf('{0} {1}', 'a', 'b') ==> 'a b'
sprintf('{0#5} {1#5}', 1, 2) ==> '    1     2'
sprintf('{0#-5} {1#-5}', 1, 2) ==> '1     2    '
sprintf('{0#05} {1#05}', 1, 2) ==> '00001 00002'
require Math; sprintf('{0#.4}', Math->PI) ==> '3.1416'
```

<a name="sqrt"></a>
## sqrt
### Syntax
```
sqrt(x)
```

### Description
Returns the square root of num as a decimal value.



### Modules
Math

### Examples
```
sqrt(4) ==> 2.0
```

<a name="starts_with"></a>
## starts_with
### Syntax
```
starts_with(str, part)
```

### Description
Returns TRUE if the string str starts with part.



### Modules
Core, String

### Examples
```
starts_with('abcdef', 'abc') ==> TRUE
starts_with(NULL, 'abc') ==> FALSE
```

<a name="str_input"></a>
## str_input
### Syntax
```
str_input(str)
```

### Description
Returns an input object, that reads the characters of the given string str.



### Modules
IO

### Examples
```
str_input('abc') ==> <!input-stream>
```

<a name="str_output"></a>
## str_output
### Syntax
```
str_output()
```

### Description
Returns an output object. Things written to this output object can be retrieved using the function get_output_string.



### Modules
IO

### Examples
```
do def o = str_output(); print('abc', out = o); get_output_string(o); end ==> 'abc'
```

<a name="string"></a>
## string
### Syntax
```
string(obj)
```

### Description
Converts the obj to a string, if possible.



### Modules
Core

### Examples
```
string(123) ==> '123'
```

<a name="strip_extension"></a>
## strip_extension
### Syntax
```
strip_extension(path)
```

### Description
Removes the file extension of the file path - if any exists.



### Modules
OS

### Examples
```
strip_extension('dir/file.ext') ==> 'dir/file'
strip_extension('dir/file.a.b.c') ==> 'dir/file.a.b'
strip_extension('dir/file') ==> 'dir/file'
strip_extension('dir/.file') ==> 'dir/.file'
strip_extension('dir/.file.cfg') ==> 'dir/.file'
```

<a name="sub"></a>
## sub
### Syntax
```
sub(a, b)
```

### Description
Returns the subtraction of b from a. For numerical values this uses usual arithmetic.
For lists and sets, this returns lists and sets minus the element b. If a is a datetime
value and b is datetime value, then the date difference is returned. If a is a datetime
value and b is a numeric value, then b is interpreted as number of days and the corresponding
datetime after subtracting these number of days is returned.


### Modules
Core

### Examples
```
sub(1, 2) ==> -1
sub([1, 2, 3], 2) ==> [1, 3]
sub(date('20170405'), date('20170402')) ==> 3
sub(date('20170405'), 3) ==> '20170402000000'
sub(<<3, 1, 2>>, 2) ==> <<1, 3>>
```

<a name="sublist"></a>
## sublist
### Syntax
```
sublist(lst, startidx)
sublist(lst, startidx, endidx)
```

### Description
Returns the sublist starting with startidx. If endidx is provided,
this marks the end of the sublist. Endidx is not included.



### Modules
Core

### Examples
```
sublist([1, 2, 3, 4], 2) ==> [3, 4]
```

<a name="substitute"></a>
## substitute
### Syntax
```
substitute(obj, idx, value)
```

### Description
If obj is a list or string, returns a list or string with the element
at index idx replaced by value.

The original string or list remain untouched.



### Modules
Core

### Examples
```
substitute('abcd', 2, 'x') ==> 'abxd'
substitute([1, 2, 3, 4], 2, 'x') ==> [1, 2, 'x', 4]
```

<a name="substr"></a>
## substr
### Syntax
```
substr(str, startidx)
substr(str, startidx, endidx)
```

### Description
Returns the substring starting with startidx. If endidx is provided,
this marks the end of the substring. Endidx is not included.



### Modules
Core, String

### Examples
```
substr('abcd', 2) ==> 'cd'
```

<a name="sum"></a>
## sum
### Syntax
```
sum(list, ignore = [])
```

### Description
Returns the sum of a list of numbers. Values contained in the optional list ignore
are counted as 0.



### Modules
Core

### Examples
```
sum([1, 2, 3]) ==> 6
sum([1, 2.5, 3]) ==> 6.5
sum([1, 2.5, 1.5, 3]) ==> 8.0
sum([1.0, 2.0, 3.0]) ==> 6.0
sum([1.0, 2, -3.0]) ==> 0.0
sum([1, 2, -3]) ==> 0
sum([1, '1', 1], ignore = ['1']) ==> 2
sum(range(101)) ==> 5050
sum([]) ==> 0
sum([NULL], ignore = [NULL]) ==> 0
sum([1, NULL, 3], ignore = [NULL]) ==> 4
sum([1, NULL, '', 3], ignore = [NULL, '']) ==> 4
```

<a name="symmetric_diff"></a>
## symmetric_diff
### Syntax
```
symmetric_diff(seta, setb)
```

### Description
Returns a set containing all elements of seta and setb,
which are either only in seta, or only in setb contained.
Also works for lists.



### Modules
Set

### Examples
```
symmetric_diff(<<1, 2, 3, 4>>, <<3, 4, 5, 6>>) ==> <<1, 2, 5, 6>>
```

<a name="tan"></a>
## tan
### Syntax
```
tan(x)
```

### Description
Returns the tangens of x.



### Modules
Math

### Examples
```
tan(0) ==> 0
```

<a name="timestamp"></a>
## timestamp
### Syntax
```
timestamp(x)
```

### Description
Returns current system timestamp.


### Modules
Core

<a name="trim"></a>
## trim
### Syntax
```
trim(str)
```

### Description
Trims any leading or trailing whitespace from the string str.



### Modules
Core, String

### Examples
```
trim(' a  ') ==> 'a'
```

<a name="type"></a>
## type
### Syntax
```
type(obj)
```

### Description
Returns the name of the type of obj as a string.



### Modules
Core

### Examples
```
type('Hello') ==> 'string'
```

<a name="union"></a>
## union
### Syntax
```
union(seta, setb)
```

### Description
Returns the union of the two sets. Also works for lists.



### Modules
Set

### Examples
```
union(<<1, 2, 3>>, <<2, 3, 4>>) ==> <<1, 2, 3, 4>>
union([1, 2, 3], [2, 3, 4]) ==> <<1, 2, 3, 4>>
union(<<1, 2>>, <<3, 4>>) ==> <<1, 2, 3, 4>>
union(<<1, 2>>, <<>>) ==> <<1, 2>>
```

<a name="unique"></a>
## unique
### Syntax
```
unique(lst, key = identity)
```

### Description
Makes the elements of the list unique, by discarding duplicates,
while retaining the original ordering. The first occurence of each
duplicate is retained.



### Modules
List

### Examples
```
unique([1, 4, 2, 3, 3, 4, 5]) ==> [1, 4, 2, 3, 5]
['a1', 'b2', 'c2', 'd3'] !> unique(key = fn(x) x[1]) ==> ['a1', 'b2', 'd3']
```

<a name="unlines"></a>
## unlines
### Syntax
```
unlines(lst)
```

### Description
Joins a list of lines into one string.



### Modules
Core

### Examples
```
unlines(['a', 'b', 'c']) ==> 'a\nb\nc'
```

<a name="unwords"></a>
## unwords
### Syntax
```
unwords(lst)
```

### Description
Joins a list of words into one string.



### Modules
Core

### Examples
```
unwords(['a', 'b', 'c']) ==> 'a b c'
```

<a name="upper"></a>
## upper
### Syntax
```
upper(str)
```

### Description
Converts str to upper case letters.



### Modules
String

### Examples
```
upper('Hello') ==> 'HELLO'
```

<a name="which"></a>
## which
### Syntax
```
which(filename)
which(filename, paths)
```

### Description
Searches filename in either the system PATH (as
defined by the environment variable PATH), or the
specified list of directories. Returns either the
full path or NULL, if not found.


### Modules
OS

<a name="words"></a>
## words
### Syntax
```
words(str)
```

### Description
Splits the string str into words and returns them as a list.



### Modules
Core

### Examples
```
words('one  two\tthree four') ==> ['one', 'two', 'three', 'four']
```

<a name="zip"></a>
## zip
### Syntax
```
zip(a, b)
```

### Description
Returns a list where each element is a list of two items.
The first of the two items is taken from the first list,
the second from the second list. The resulting list has
the same length as the shorter of the two input lists.



### Modules
Core

### Examples
```
zip([1, 2, 3], [4, 5, 6, 7]) ==> [[1, 4], [2, 5], [3, 6]]
```

<a name="zip_map"></a>
## zip_map
### Syntax
```
zip_map(a, b)
```

### Description
Returns a map where the key of each entry is taken from a,
and where the value of each entry is taken from b, where
a and b are lists of identical length.



### Modules
Core

### Examples
```
zip_map(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>
```
