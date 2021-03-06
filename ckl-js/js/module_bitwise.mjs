export const modulebitwise = `
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

bind_native("bit_and", "bit_and_32");
bind_native("bit_or", "bit_or_32");
bind_native("bit_not", "bit_not_32");
bind_native("bit_xor", "bit_xor_32");
bind_native("bit_rotate_left", "bit_rotate_left_32");
bind_native("bit_rotate_right", "bit_rotate_right_32");
bind_native("bit_shift_left", "bit_shift_left");
bind_native("bit_shift_right", "bit_shift_right");

`;
