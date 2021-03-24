export const modulecore = `
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

bind_native("add");
bind_native("append");
bind_native("body");
bind_native("boolean");
bind_native("ceiling");
bind_native("compare");
bind_native("contains", "str_contains");
bind_native("date");
bind_native("decimal");
bind_native("delete_at");
bind_native("div");
bind_native("ends_with", "str_ends_with");
bind_native("equals");
bind_native("escape_pattern");
bind_native("eval");
bind_native("find", "str_find");
bind_native("floor");
bind_native("greater");
bind_native("greater_equals");
bind_native("identity");
bind_native("if_empty");
bind_native("if_null");
bind_native("if_null_or_empty");
bind_native("info");
bind_native("insert_at");
bind_native("int");
bind_native("is_empty");
bind_native("is_not_empty");
bind_native("is_not_null");
bind_native("is_null");
bind_native("length");
bind_native("less");
bind_native("less_equals");
bind_native("list");
bind_native("lower");
bind_native("ls");
bind_native("map");
bind_native("matches", "str_matches");
bind_native("mod");
bind_native("mul");
bind_native("not_equals");
bind_native("object");
bind_native("parse");
bind_native("parse_json");
bind_native("pattern");
bind_native("put");
bind_native("range");
bind_native("remove");
bind_native("round");
bind_native("s");
bind_native("set");
bind_native("sorted");
bind_native("split");
bind_native("split2");
bind_native("starts_with", "str_starts_with");
bind_native("string");
bind_native("sub");
bind_native("sublist");
bind_native("substr");
bind_native("sum");
bind_native("timestamp");
bind_native("trim", "str_trim");
bind_native("type");
bind_native("upper");
bind_native("zip");
bind_native("zip_map");

`;
