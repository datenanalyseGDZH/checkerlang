export const modulestat = `
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

require math;

"
mean(lst)

Returns the mean of lst.

: mean([1, 2, 3, 4, 4]) ==> 2.8
: mean([-1.0, 2.5, 3.25, 5.75]) ==> 2.625
"
def mean(lst) do
	decimal(sum(lst)) / length(lst);
end;


"
median(lst)

Returns the median of lst, using the 'mean of middle two'
method.

: median([1, 3, 5]) ==> 3
: median([1, 3, 5, 7]) ==> 4.0
"
def median(lst) do
	def lst = sorted(list(lst));
	def len = length(lst);
	def idx = len / 2;
	if len % 2 == 0 then (lst[idx - 1] + lst[idx]) / 2.0
	else lst[idx];
end;


"
median_low(lst)

Returns the low median of lst.

: median_low([1, 3, 5]) ==> 3
: median_low([1, 3, 5, 7]) ==> 3
"
def median_low(lst) do
	def lst_ = sorted(list(lst));
	def len = length(lst_);
	def idx = len / 2;
	if len % 2 == 0 then lst[idx - 1]
	else lst[idx];
end;


"
median_high(lst)

Returns the high median of lst.

: median_high([1, 3, 5]) ==> 3
: median_high([1, 3, 5, 7]) ==> 5
"
def median_high(lst) do
	def lst_ = sorted(list(lst));
	def len = length(lst_);
	lst_[len / 2];
end;


"
geometric_mean(lst)

Returns the geometric mean of lst.

: round(geometric_mean([54, 24, 36]), 1) ==> 36.0
"
def geometric_mean(lst) do
	math->pow(prod(lst), 1.0 / length(lst));
end;


"
harmonic_mean(lst)

Returns the harmonic mean of lst.

: round(harmonic_mean([40, 60]), 1) ==> 48.0
: round(harmonic_mean([2.5, 3, 10]), 1) ==> 3.6
"
def harmonic_mean(lst) do
	decimal(length(lst)) / sum([1.0 / x for x in lst]);
end;

`;
