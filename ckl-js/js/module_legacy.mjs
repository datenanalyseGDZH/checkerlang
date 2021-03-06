export const modulelegacy = `
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

require Sys unqualified;
require Bitwise unqualified;
require Core unqualified;
require Date unqualified;
require List unqualified;
require Math unqualified;
require OS unqualified;
require Predicate unqualified;
require Random unqualified;
require IO unqualified;
require Set unqualified;
require Stat unqualified;
require String unqualified;
require Type unqualified;

require String import [
    starts_with as str_starts_with, 
    ends_with as str_ends_with, 
    contains as str_contains, 
    find as str_find,
    find_last as str_find_last,
    matches as str_matches, 
    trim as str_trim];

`;
