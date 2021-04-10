export const moduledate = `
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


bind_native("parse_date");
bind_native("format_date");


"
is_valid_date(str, fmt='yyyyMMdd')

Returns TRUE if the string represents a valid date. The default format
is yyyyMMdd. It is possible to specify different formats using the fmt
optional parameter.

: is_valid_date('20170304') ==> TRUE
: is_valid_date('2017030412') ==> FALSE
: is_valid_date('20170399') ==> FALSE
"
def is_valid_date(str, fmt="yyyyMMdd") do
  require Date;
  is_string(str) and Date->parse_date(str, fmt) != NULL;
end;


"
is_valid_time(str, fmt='HHmm')

Returns TRUE if the string represents a valid time. The default format
is HHmm. It is possible to specify different formats using the fmt
optional parameter.

: is_valid_time('1245') ==> TRUE
"
def is_valid_time(str, fmt="HHmm") do
  require Date;
  is_string(str) and Date->parse_date(str, fmt) != NULL;
end;


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
iso_date()
iso_date(value)

Formats the date value as an ISO date (i.e. using format yyyy-MM-dd).
If no date value is provided, the current date is used.

: iso_date(date('20200203')) ==> '2020-02-03'
"
def iso_date(value = now()) format_date(value, fmt = 'yyyy-MM-dd');


"
iso_datetime()
iso_datetime(value)

Formats the datetime value as an ISO datetime (i.e. using format yyyy-MM-dd'T'HH:mm:ss).
If no datetime value is provided, the current datetime is used.

: iso_datetime(date('20200203201544')) ==> '2020-02-03T20:15:44'
"
def iso_datetime(value = now()) format_date(value, fmt = 'yyyy-MM-ddTHH:mm:ss');

`;
