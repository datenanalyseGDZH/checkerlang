export const modulestring = `
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

bind_native("starts_with");
bind_native("ends_with");
bind_native("chr");
bind_native("contains");
bind_native("find");
bind_native("find_last");
bind_native("lower");
bind_native("matches");
bind_native("ord");
bind_native("s");
bind_native("split");
bind_native("split2");
bind_native("substr");
bind_native("trim");
bind_native("upper");


"
reverse(str)

Returns a reversed copy of a string.

: reverse('abc') ==> 'cba'
: reverse(NULL) ==> NULL
: reverse(12) ==> NULL
"
def reverse(str) do
  if not is_string(str) then return NULL;
  def result = "";
  for ch in str do
    result = ch + result
  end;
  result
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
  def pos = find(s, a, start = start);
  if pos == -1 then return s;
  return replace(substr(s, 0, pos) + b + substr(s, pos + length(a)), a, b, start = pos + length(b));
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
`;
