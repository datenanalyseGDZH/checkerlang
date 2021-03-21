export const modulelists = `
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
first(lst) 

Returns the first element of a list.

: first([1, 2, 3]) ==> 1
: first(NULL) ==> NULL
"
def first(lst) do
  if is_null(lst) then NULL
  if not is_list(lst) then error("argument is not a list (" + type(lst) + ")")
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
  if not is_list(lst) then error("argument is not a list (" + type(lst) + ")")
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
reverse(obj)

Returns a reversed copy of a string or a list.

: reverse([1, 2, 3]) ==> [3, 2, 1]
: reverse('abc') ==> 'cba'
"
def reverse(obj) do
  if is_string(obj) then do
    def result = "";
    for ch in obj do
        result = ch + result
    end;
    result
  end elif is_list(obj) then do
    def result = [];
    for element in obj do
        insert_at(result, 0, element);
    end;
    result 
  end else error("cannot reverse " + type(obj))
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


"
permutations(lst)

Returns a list containing all permutations of the input list.

: permutations([1, 2, 3]) ==> [[1, 2, 3], [2, 1, 3], [3, 1, 2], [1, 3, 2], [2, 3, 1], [3, 2, 1]]
"
def permutations(lst) do
    def result = [];
    def perms(lst, size) do
        if size == 1 then do
            result !> append([...lst]);
        end else do
            for i in range(size) do
                perms(lst, size - 1);
                if size % 2 == 1 then do
                    def temp = lst[0];
                    lst[0] = lst[size - 1];
                    lst[size - 1] = temp;
                end else do
                    def temp = lst[i];
                    lst[i] = lst[size - 1];
                    lst[size - 1] = temp;
                end;
            end;
        end;
    end;
    perms(lst, length(lst));
    result;
end;


"
flatten(lst)

Flattens a list, by replacing child lists with their contents.
This does only work at the top level and does not recurse.

: flatten([[1, 2], 3, 4, [5, 6]]) ==> [1, 2, 3, 4, 5, 6]
: flatten([1, 2, 3]) ==> [1, 2, 3]
: flatten([1, [2], [3, 4], [5, [6, 7]]]) ==> [1, 2, 3, 4, 5, [6, 7]]
"
def flatten(lst) do
    def result = [];
    for item in lst do
        if is_list(item) then result !> append_all(item)
        else result !> append(item);
    end;
    result;
end;

`;
