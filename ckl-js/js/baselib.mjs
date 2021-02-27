/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
export const baselib = `
# this is the base library of the checkerlang language

"
is_list(obj) 

Returns TRUE if the object is of type list.

: is_list([1, 2, 3]) ==> TRUE
"
def is_list(obj) type(obj) == 'list';


"
is_string(obj) 

Returns TRUE if the object is of type string.

: is_string('abc') ==> TRUE
"
def is_string(obj) type(obj) == 'string';


"
is_int(obj) 

Returns TRUE if the object is of type int.

: is_int(123) ==> TRUE
"
def is_int(obj) type(obj) == 'int';


"
is_decimal(obj) 

Returns TRUE if the object is of type decimal.

: is_decimal(123.45) ==> TRUE
"
def is_decimal(obj) type(obj) == 'decimal';


"
is_numeric(obj) 

Returns TRUE if the object is of type numeric. Numeric
types are int and decimal.

: is_numeric(123) ==> TRUE
: is_numeric(123.45) ==> TRUE
"
def is_numeric(obj) is_int(obj) or is_decimal(obj);


"
is_boolean(obj) 

Returns TRUE if the object is of type boolean.

: is_boolean(1 == 2) ==> TRUE
"
def is_boolean(obj) type(obj) == 'boolean';


"
is_set(obj) 

Returns TRUE if the object is of type set.

: is_set(set([1, 2, 3])) ==> TRUE
"
def is_set(obj) type(obj) == 'set';


"
is_map(obj) 

Returns TRUE if the object is of type map.

: is_map(map([['a', 1], ['b', 2]])) ==> TRUE
"
def is_map(obj) type(obj) == 'map';


"
is_func(obj) 

Returns TRUE if the object is of type func.

: is_func(fn(x) 2 * x) ==> TRUE
: is_func(sum) ==> TRUE
"
def is_func(obj) type(obj) == 'func';


"
non_zero(a, b) 

Returns the value a, if a is a non-zero integer, otherwise returns b.

: non_zero(1, 2) ==> 1
: non_zero(0, 2) ==> 2
"
def non_zero(a, b) do
  if int(a) == 0 then b
  else a
end;


"
non_empty(a, b) 

Returns the value a, if a is a non-empty string, otherwise returns b.

: non_empty('a', 'b') ==> 'a'
: non_empty('', 'b') ==> 'b'
"
def non_empty(a, b) do
  if a == '' then b
  else a
end;


"
const(val) 

Returns a function that returns a constant value, regardless of the argument used.

: def f = const(2); f(1) ==> 2
: def f = const(2); f('x') ==> 2
"
def const(val) fn(a) val;


"
min(a, b, key = identity) 
min(a, key = identity)

Returns the minimum of the values a, b. 

Returns the mininmum value of the list a.

The optional key parameter takes a function with one parameter, which 
is used to get the value from a and b that is used for the comparison. 
Default key is the identity function.

: min(1, 2) ==> 1
: min([1, 'z'], [2, 'a'], key = fn(x) x[1]) ==> [2, 'a']
: min([1, 3, 2, 4, 2]) ==> 1
"
def min(a, b = NULL, key = identity) do
  if is_list(a) and is_null(b) then do
    def m = a[0];
    def k = key(m);
    for item in a do
      if key(item) < m then do
        k = key(item);
        m = item;
      end;
    end;
    return m;
  end;
  if key(a) < key(b) then a
  else b;
end;


"
max(a, b, key = identity) 
max(a, key = identity)

Returns the maximum of the values a, b. 

Returns the maximum value of the list a.

The optional key parameter takes a function with one parameter, which 
is used to get the value from a and b that is used for the comparison. 
Default key is the identity function.

: max(1, 2) ==> 2
: max([1, 'z'], [2, 'a'], key = fn(x) x[1]) ==> [1, 'z']
: max([1, 3, 2, 4, 2]) ==> 4
"
def max(a, b = NULL, key = identity) do
  if is_list(a) and is_null(b)then do
    def m = a[0];
    def k = key(m);
    for item in a do
      if key(item) > m then do
        k = key(item);
        m = item;
      end;
    end;
    return m;
  end;
  if key(a) > key(b) then a
  else b;
end;


"
abs(n) 

Returns the absolute value of n.

: abs(2) ==> 2
: abs(-3) ==> 3
"
def abs(n) do
  if is_null(n) then NULL
  if not is_numeric(n) then error(string(n) + " is not numerical")
  if n < 0 then - n
  else n;
end;

"
sign(n) 

Returns the signum of n

: sign(2) ==> 1
: sign(-3) ==> -1
"
def sign(n) do
  if is_null(n) then NULL
  if not is_numeric(n) then error(string(n) + " is not numerical")
  if n < 0 then -1
  if n > 0 then 1
  else 0;
end;


"
first(lst) 

Returns the first element of a list.

: first([1, 2, 3]) ==> 1
: first(NULL) ==> NULL
"
def first(lst) do
  if is_null(lst) then NULL
  if not is_list(lst) then error(string(lst) + " is not a list")
  else lst[0];
end;


"
first_n(lst, n)

Returns the first n elements of the list.

: range(100) !> first_n(5) ==> [0, 1, 2, 3, 4]
"
def first_n(lst, n) do
  lst !> sublist(0, n)
end;


"
last(lst) 

Returns the last element of a list.

: last([1, 2, 3]) ==> 3
: last(NULL) ==> NULL
"
def last(lst) do
  if is_null(lst) then NULL
  if not is_list(lst) then error(string(lst) + " is not a list")
  else lst[-1];
end;


"
last_n(lst, n)

Returns the last n elements of the list.

: range(100) !> last_n(5) ==> [95, 96, 97, 98, 99]
"
def last_n(lst, n) do
  lst !> sublist(-n)
end;


"
rest(lst) 

Returns the rest of a list, i.e. everything but the first element.

: rest([1, 2, 3]) ==> [2, 3]
"
def rest(lst) sublist(lst, 1);


"
is_zero(obj)

Returns TRUE if the obj is zero.

: is_zero(0) ==> TRUE
"
def is_zero(obj) is_numeric(obj) and obj == 0;


"
is_negative(obj)

Returns TRUE if the obj is negative.

: is_negative(-1) ==> TRUE
"
def is_negative(obj) is_numeric(obj) and obj < 0;


"
is_positive(obj)

Returns TRUE if the obj is positive.

: is_positive(1) ==> TRUE
"
def is_positive(obj) is_numeric(obj) and obj > 0;


"
is_alphanumerical(str, min = 0, max = 99999)

Returns TRUE if the string is alphanumerical, i.e. contains only a-z, A-Z and 0-9.
It is possible to specify minimal and maximal length using the min and max optional
parameters.

: is_alphanumerical('Ab12') ==> TRUE
"
def is_alphanumerical(str, min=0, max=99999) is_string(str) and str_matches(str, pattern('^[a-zA-Z0-9]{' + min + ',' + max + '}$'));


"
is_numerical(str, min = 0, max = 99999)

Returns TRUE if the string is numerical, i.e. contains only 0-9. It is possible to
specify minimal and maximal length using the min and max optional parameters.

: is_numerical('123') ==> TRUE
" 
def is_numerical(str, min=0, max=99999) is_string(str) and str_matches(str, pattern('^[0-9]{' + min + ',' + max + '}$'));


"
is_valid_date(str, fmt='yyyyMMdd')

Returns TRUE if the string represents a valid date. The default format
is yyyyMMdd. It is possible to specify different formats using the fmt
optional parameter.

: is_valid_date('20170304') ==> TRUE
: is_valid_date('2017030412') ==> FALSE
: is_valid_date('20170399') ==> FALSE
"
def is_valid_date(str, fmt="yyyyMMdd") is_string(str) and parse_date(str, fmt) != NULL;


"
is_valid_time(str, fmt='HHmm')

Returns TRUE if the string represents a valid time. The default format
is HHmm. It is possible to specify different formats using the fmt
optional parameter.

: is_valid_time('1245') ==> TRUE
"
def is_valid_time(str, fmt="HHmm") is_string(str) and parse_date(str, fmt) != NULL;


"
reverse_list(list)

Returns a reversed copy of a list.

: reverse_list([1, 2, 3]) ==> [3, 2, 1]
: reverse_list(NULL) ==> NULL
: reverse_list('abc') ==> NULL
"
def reverse_list(list) do
  if not is_list(list) then return NULL;
  def result = [];
  for element in list do
    insert_at(result, 0, element);
  end;
  result 
end;


"
reverse_string(str)

Returns a reversed copy of a string.

: reverse_string('abc') ==> 'cba'
: reverse_string(NULL) ==> NULL
: reverse_string(12) ==> NULL
"
def reverse_string(str) do
  if not is_string(str) then return NULL;
  def result = "";
  for ch in str do
    result = ch + result
  end;
  result
end;


"
reverse(obj)

Returns a reversed copy of a string or a list.

: reverse([1, 2, 3]) ==> [3, 2, 1]
: reverse('abc') ==> 'cba'
"
def reverse(obj) do
  if is_string(obj) then reverse_string(obj)
  if is_list(obj) then reverse_list(obj)
  else error("cannot reverse " + string(obj))
end;


"
reduce(list, f)

Reduces a list by successively applying the binary function f to
partial results and list elements.

: reduce([1, 2, 3, 4], add) ==> 10 
"
def reduce(list, f) do
  if is_null(list) then return NULL
  if length(list) == 0 then error("Cannot reduce empty list")
  if length(list) == 1 then return list[0]
  else do
    def result = list[0];
    for element in sublist(list, 1) do
      result = f(result, element)
    end;
    result;
  end;
end;


"
prod(list) 

Returns the product of a list of numbers.

: prod([1, 2, 3]) ==> 6
: prod(range(1, 10)) ==> 362880
"
def prod(list) reduce(list, mul);


"
log2(x)

Returns the logarithm of x to base 2.

: int(log2(1024)) ==> 10
"
def log2(x) round(log(x) / log(2), 12);


"
log10(x)

Returns the logarithm of x to base 10.

: int(log10(1000)) ==> 3
"
def log10(x) round(log(x) / log(10), 12);


"
substitute(obj, idx, value)

If obj is a list or string, returns a list or string with the element
at index idx replaced by value.

The original string or list remain untouched.

: substitute('abcd', 2, 'x') ==> 'abxd'
: substitute([1, 2, 3, 4], 2, 'x') ==> [1, 2, 'x', 4] 
"
def substitute(obj, idx, value) do
  if is_string(obj) then substr(obj, 0, idx) + value + substr(obj, idx + 1)
  if is_list(obj) then sublist(obj, 0, idx) + value + sublist(obj, idx + 1)
  else error('Cannot substitute in ' + string(obj))
end;


"
grep(lst, pat, key = identity)

Returns the sublist of list lst, that contains only entries,
that match the regular expression pattern pat.

If pat does not contain ^ and $, then the prefix ^.* and the postfix .*$
are added to the pattern, so that the pattern matches partially.

: grep(['one', 'two', 'three'], //e//) ==> ['one', 'three']
: grep(['one', 'two', 'three'], //^one$//) ==> ['one']
: grep(['1:2', '12:2', '123:3'], //2//, key = fn(x) split(x, ':')[1]) ==> ['1:2', '12:2']
"
def grep(lst, pat, key = identity) do
  def pat_ = string(pat);
  if not str_contains(pat_, '^') and not str_contains(pat_, '$') then pat_ = '^.*' + pat_ + '.*$';
  return [element for element in lst if str_matches(key(element), pattern(pat_))]
end;


"
interval(a)
interval(a, b)

Returns the interval of integers between a and b, inclusive.
If only a is provided, the interval is taken to be [1, ... a]

: interval(1, 10) ==> [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
: interval(1, 1) ==> [1]
: interval(10) ==> [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
"
def interval(a, b = NULL) do
    if is_null(a) then return NULL
    if is_null(b) then range(1, a + 1)
    else range(a, b + 1)
end;


"
replace(s, a, b, start = 0)

Replaces all occurences of a in the string s with b.
The optional parameter start specifies the start index.

: replace('abc', 'b', 'x') ==> 'axc'
: replace('abc', 'b', 'xy') ==> 'axyc'
: replace('abcdef', 'bcd', 'xy') ==> 'axyef'
: replace('abcabcabc', 'abc', 'xy', start = 3) ==> 'abcxyxy'
"
def replace(s, a, b, start = 0) do
  if is_null(s) then return NULL;
  def pos = str_find(s, a, start = start);
  if pos == -1 then return s;
  return replace(substr(s, 0, pos) + b + substr(s, pos + length(a)), a, b, start = pos + length(b));
end;


"
label_data(labels, data)

Creates a map that labels the data with the
given labels.

Labels and data must be two lists of equal
length. Labels must be unique.

: label_data(['a', 'b', 'c'], [1, 2, 3]) ==> <<<'a' => 1, 'b' => 2, 'c' => 3>>>
"
def label_data(labels, data) do
    zip_map(labels, data);
end;


"
any(lst, predicate)

Returns TRUE, if the predicate function returns
TRUE for any element of the list.

: any([1, 2, 3], fn(n) n == 3) ==> TRUE
: any([1, 2, 3], fn(n) n == 4) ==> FALSE
"
def any(lst, pred) do
    for element in lst do
        if pred(element) then return TRUE;
    end;
    return FALSE;
end;


"
all(lst, predicate)

Returns TRUE, if the predicate function returns
TRUE for all elements of the list.

: all([1, 2, 3], fn(n) n <= 3) ==> TRUE
: all([1, 2, 3], fn(n) n <  3) ==> FALSE
"
def all(lst, pred) do
    for element in lst do
        if not pred(element) then return FALSE;
    end;
    return TRUE;
end;


"
pairs(lst)

Returns a list where each entry consists of a pair
of elements of lst.

: pairs([1, 2, 3]) ==> [[1, 2], [2, 3]]
: pairs([1, 2, 3, 4]) ==> [[1, 2], [2, 3], [3, 4]]
"
def pairs(lst) do
    def result = [];
    for index in range(length(lst)-1) do
        append(result, [lst[index], lst[index + 1]]);
    end;
    return result;
end;


"
join(lst, sep = ' ')

Returns a string containing all elements of the list lst
separated by the string sep.

: join([1, 2, 3], '|') ==> '1|2|3'
: join(['one', 'world'], '--') ==> 'one--world'
: join([], '|') ==> ''
: join([1], '|') ==> '1'
: join('|', [1, 2, 3]) ==> '1|2|3'
"
def join(lst, sep) do
    if type(lst) == "string" then [lst, sep] = [sep, lst];
    def result = "";
    for element in lst do
        result = result + sep + string(element);
    end;
    return substr(result, length(sep));
end;


"
q(lst)

Returns a string containing all elements of the list lst
separated by a pipe character.

: q([1, 2, 3]) ==> '1|2|3'
: q([]) ==> ''
"
def q(lst) do
    return join("|", lst);
end;


"
esc(str)

Escapes the characters <, > and & by their HTML entities.

: esc('a<b') ==> 'a&lt;b'
: esc('<code>') ==> '&lt;code&gt;'
"
def esc(str) do
    return replace(replace(replace(str, '&', '&amp;'), '<', '&lt;'), '>', '&gt;');
end;


"
map_list(lst, f)

Returns a list where each element is the corresponding
element of lst with func applied. Thus, the elements of
the list are mapped using the function func to new values.

: map_list([1, 2, 3], fn(x) 2 * x) ==> [2, 4, 6]
: ['one', 'two', 'three'] !> map_list(fn(x) '*' + x + '*') ==> ['*one*', '*two*', '*three*']
: map_list(identity, [1, 2, 3]) ==> [1, 2, 3]
"
def map_list(lst, f) do
    if type(lst) == 'func' then [f, lst] = [lst, f];
    return [f(element) for element in lst];
end;


"
now()

Returns the current date.
"
def now() date();


"
date_year(value)

Extracts the year part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.

: date_year('20190102') ==> 2019
"
def date_year(value) do
    return int(format_date(date(value), fmt = 'yyyy'));
end;


"
date_month(value)

Extracts the month part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.

: date_month('20190102') ==> 01
"
def date_month(value) do
    return int(format_date(date(value), fmt = 'MM'));
end;


"
date_day(value)

Extracts the day part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.

: date_day('20190102') ==> 02
"
def date_day(value) do
    return int(format_date(date(value), fmt = 'dd'));
end;


"
date_hour(value)

Extracts the hour part from the given date value and
returns it as an integer. The value will be converted
to a date value using the date function.

: date_hour('2019010212') ==> 12
"
def date_hour(value) do
    return int(format_date(date(value), fmt = 'HH'));
end;


"
date_minute(value)

Extracts the hour part from the given date value and
returns it as an integer.

: date_minute(parse_date('201901021223', fmt='yyyyMMddHHmm')) ==> 23
"
def date_minute(value) do
    return int(format_date(date(value), fmt = 'mm'));
end;


"
date_second(value)

Extracts the second part from the given date value and
returns it as an integer.

: date_second(parse_date('20190102122345', fmt='yyyyMMddHHmmss')) ==> 45
"
def date_second(value) do
    return int(format_date(date(value), fmt = 'ss'));
end;


"
div0(a, b, div_0_value = MAXINT)

If b is not zero, the result of a / b is returned.
If b is zero, the value div_0_value is returned.

: div0(12, 3) ==> 4
: div0(12, 5) ==> 2
: div0(12.0, 5) ==> 2.4
: div0(12.5, 2) ==> 6.25
: div0(12, 0) ==> MAXINT
: div0(12, 0, 0) ==> 0
: div0(12, 0.0, 0) ==> 0
"
def div0(a, b, div_0_value = MAXINT) do
    if b == 0 then div_0_value
    else div(a, b);
end;


"
map_get(m, k, default_value=NULL)

If the map m contains the key k, then the corresponding
value is returned. Otherwise, the default_value is
returned.

: map_get(<<<a => 1, b => 2>>>, 'a') ==> 1
: map_get(<<<a => 1, b => 2>>>, 'b') ==> 2
: map_get(<<<a => 1, b => 2>>>, 'c') ==> NULL
: map_get(<<<a => 1, b => 2>>>, 'c', default_value = 9) ==> 9
"
def map_get(m, k, default_value=NULL) do
    if k in m then m[k] else default_value;
end;


"
map_get_pattern(m, k, default_value=NULL)

The map m is assumed to contain regex patterns as keys.
If the key k matches one of the regex patterns, then
the corresponding value is returned. Otherwise, the
default_value is returned.

If more than one pattern matches the key k, then it is
undefined, which pattern is selected for retrieving its
value.

: map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'a') ==> 1
: map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'b') ==> 1
: map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'c') ==> 2
: map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'd') ==> 2
: map_get_pattern(<<<//[ab]// => 1, //[cd]// => 2>>>, 'e') ==> NULL
"
def map_get_pattern(m, k, default_value=NULL) do
    for pattern in set(m) if str_matches(k, pattern) then return m[pattern];
    return default_value;
end;


"
enumerate(obj)

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

: enumerate(['a', 'b', 'c']) ==> [[0, 'a'], [1, 'b'], [2, 'c']]
: enumerate(<<<a => 5, b => 6, c => 7>>>) ==> [['a', 5], ['b', 6], ['c', 7]]
"
def enumerate(obj) do
    if is_list(obj) then [[i, obj[i]] for i in range(length(obj))]
    if is_map(obj) then [[key, obj[key]] for key in set(obj)]
    else error("can only enumerate list, set and map objects")
end;


"
curry(f, arg)

Partially applies the function f with the argument arg.
This returns another function which takes the remaining
args of the original function f.

: def f(a, b, c) [a, b, c]; def g = curry(f, 1); g(2, 3) ==> [1, 2, 3]
"
def curry(f, arg) fn(args...) f(arg, ...args...);


"
apply(f, args)

Applies the function with the arguments in the list args.

: apply(fn(a, b, c) a + b + c, [1, 2, 3]) ==> 6
"
def apply(f, args) f(...args);


"
unique(lst, key = identity)

Makes the elements of the list unique, by discarding duplicates,
while retaining the original ordering. The first occurence of each
duplicate is retained.

: unique([1, 4, 2, 3, 3, 4, 5]) ==> [1, 4, 2, 3, 5]
: ['a1', 'b2', 'c2', 'd3'] !> unique(key = fn(x) x[1]) ==> ['a1', 'b2', 'd3']
"
def unique(lst, key = identity) do
  def result = [];
  def s = <<>>;
  for item in lst do
    def val = key(item);
    if val in s then continue;
    s !> append(val);
    result !> append(item);
  end;
  result;
end;


"
filter(lst, predicate, key = identity)

Returns a filtered copy of the list by discarding
all elements for which the predicate returns FALSE.

: [1, 2, 3, 4, 5, 6] !> filter(fn(x) x % 2 == 0) ==> [2, 4, 6]
: [1, 'one', 2.2, TRUE, sum] !> filter(is_numeric) ==> [1, 2.2]
: [['abc', 1], ['bbc', 2], ['acc', 3]] !> filter(fn(x) x !> starts_with('a'), key = fn(x) x[0]) ==> [['abc', 1], ['acc', 3]]
"
def filter(lst, predicate, key = identity) do
  def result = [];
  for element in lst do
    def val = key(element);
    if predicate(val) then result !> append(element);
  end;
  return result;
end;


"
is_even(n)

Returns TRUE if the number is even.

: is_even(2) ==> TRUE
: is_even(3) ==> FALSE
"
def is_even(n) do
  if not is_numeric(n) then return FALSE;
  return n % 2 == 0;
end;


"
is_odd(n)

Returns TRUE if the number is odd.

: is_odd(2) ==> FALSE
: is_odd(3) ==> TRUE
"
def is_odd(n) do
  if not is_numeric(n) then return FALSE;
  return n % 2 == 1;
end;


"
append_all(lst, items)

Appends all items to the list or set. The items
can be in a list or a set.

: def a = [1, 2, 3]; append_all(a, [4, 5, 6]); a ==> [1, 2, 3, 4, 5, 6]
: def a = <<1, 2, 3>>; append_all(a, [2, 3, 4]); a ==> <<1, 2, 3, 4>>
: def a = [1, 2, 3]; append_all(a, <<4, 5, 6>>); a ==> [1, 2, 3, 4, 5, 6]
: def a = [1, 2, 3]; append_all(a, [2, 3, 4]); a ==> [1, 2, 3, 2, 3, 4]
"
def append_all(lst, items) do
  for item in list(items) do 
    lst !> append(item);
  end;
  lst;
end;


"
lines(str)

Splits the string str into lines and returns them as a list.

: lines('a\\\\nb c\\\\r\\\\nd') ==> ['a', 'b c', 'd']
"
def lines(str) do
  split(str, '\\r?\\n');
end;


"
words(str)

Splits the string str into words and returns them as a list.

: words('one  two\\\\tthree four') ==> ['one', 'two', 'three', 'four']
"
def words(str) do
  split(str, '[ \\t\\r\\n]+');
end;


"
unlines(lst)

Joins a list of lines into one string.

: unlines(['a', 'b', 'c']) ==> 'a\\\\nb\\\\nc'
"
def unlines(lst) do
  lst !> join(sep = '\n');
end;


"
unwords(lst)

Joins a list of words into one string.

: unwords(['a', 'b', 'c']) ==> 'a b c'
"
def unwords(lst) do
  lst !> join(sep = ' ');
end;


"
read_file(filename, encoding = 'utf-8')

Opens a file, reads the contents as a single
string, closes the file and returns the string.
"
def read_file(filename, encoding = 'utf-8') do
  def infile = file_input(filename, encoding);
  do
      read_all(infile);
  finally
      close(infile);
  end;
end;


"
grouped(lst, cmp = compare, key = identity)

Creates a list of groups, where all equal adjacent elements
of a list are put together in one group.

Typically, you would use the sorted function first to gather
equal elements next to each other.

: [1, 1, 2, 2, 2, 3, 4, 5, 2] !> grouped() ==> [[1, 1], [2, 2, 2], [3], [4], [5], [2]]
: [1, 1, 2, 2, 2] !> grouped() ==> [[1, 1], [2, 2, 2]]
: [1, 1] !> grouped() ==> [[1, 1]]
: [1] !> grouped() ==> [[1]]
: [] !> grouped() ==> []
: [[1, 'a'], [1, 'b'], [2, 'c']] !> grouped(key = fn(x) x[0]) ==> [[[1, 'a'], [1, 'b']], [[2, 'c']]]
"
def grouped(lst, cmp = compare, key = identity) do
  if length(lst) == 0 then return [];
  def result = [];
  def current_group = [];
  def current_key = key(lst[0]);
  for element in lst do
    def next_key = key(element);
    
    if cmp(current_key, next_key) <> 0 then do
      result !> append(current_group);
      current_group = [];
      current_key = next_key;
    end;
    
    current_group !> append(element);
    end;
  if length(current_group) > 0 then result !> append(current_group);
  return result;
end;


"
for_each(lst, func)

Calls the function func once for each successive element
of the list lst.
"
def for_each(lst, func) do
  for item in lst do
    func(item);
  end;
  return NULL;
end;

`;
