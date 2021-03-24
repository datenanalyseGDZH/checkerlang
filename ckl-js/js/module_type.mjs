export const moduletype = `
# Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

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
is_object(obj)

Returns TRUE if the object is of type object.

: is_object(object()) ==> TRUE
: is_object(map()) ==> FALSE
"
def is_object(obj) type(obj) == 'object';


"
is_func(obj) 

Returns TRUE if the object is of type func.

: is_func(fn(x) 2 * x) ==> TRUE
: is_func(sum) ==> TRUE
"
def is_func(obj) type(obj) == 'func';

`;
