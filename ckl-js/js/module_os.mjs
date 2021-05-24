export const moduleos = `
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

bind_native("PS");
bind_native("LS");
bind_native("FS");
bind_native("OS_NAME");
bind_native("OS_VERSION");
bind_native("OS_ARCH");

bind_native("file_exists");
bind_native("file_copy");
bind_native("file_delete");
bind_native("file_info");
bind_native("file_move");
bind_native("list_dir");
bind_native("make_dir");
bind_native("get_env");
bind_native("execute");

"
def path(parts...)

Combines the path elements into a path by
joining them with the PS path separator.
"
def path(parts...) do
    PS !> join(parts...)
end;


"
basename(path)

Returns the filename portion of the path.

: basename('dir/file.ext') ==> 'file.ext'
: basename('file.ext') ==> 'file.ext'
: basename('a/b/c/d/e/file.ext') ==> 'file.ext'
"
def basename(path) do
    def pos = path !> replace('\\\\', '/') !> find_last('/');
    if pos == -1 then path
    else path !> substr(pos + 1);
end;


"
dirname(path)

Returns the directory portion of the path.

: dirname('dir/file.ext') ==> 'dir'
: dirname('file.ext') ==> ''
: dirname('a/b/c/d/e/file.ext') ==> 'a/b/c/d/e'
"
def dirname(path) do
    def pos = path !> replace('\\\\', '/') !> find_last('/');
    if pos == -1 then ''
    else path !> substr(0, pos);
end;


"
file_extension(path)

Returns the file extension of the file path.

: file_extension('dir/file.ext') ==> '.ext'
: file_extension('dir/file.a.b.c') ==> '.c'
: file_extension('dir/file') ==> ''
: file_extension('dir/.file') ==> ''
: file_extension('dir/.file.cfg') ==> '.cfg'
"
def file_extension(path) do
    def filename = basename(path);
    def pos = filename !> find_last('.');
    def ext = filename !> substr(pos);
    if pos == -1 or filename == ext then ''
    else ext;
end;

"
strip_extension(path)

Removes the file extension of the file path - if any exists.

: strip_extension('dir/file.ext') ==> 'dir/file'
: strip_extension('dir/file.a.b.c') ==> 'dir/file.a.b'
: strip_extension('dir/file') ==> 'dir/file'
: strip_extension('dir/.file') ==> 'dir/.file'
: strip_extension('dir/.file.cfg') ==> 'dir/.file'
"
def strip_extension(path) do
    def ext = file_extension(path);
    if ext != '' then path !> substr(0, length(path) - length(ext))
    else path
end;


"
which(filename)
which(filename, paths)

Searches filename in either the system PATH (as
defined by the environment variable PATH), or the
specified list of directories. Returns either the
full path or NULL, if not found.
"
def which(filename, paths = NULL) do
    if paths == NULL then paths = get_env('PATH') !> split(FS);
    for p in paths do
        for name in list_dir(p) do
            if name == filename then return path(p, name);
            if OS_NAME == "Windows" and name == filename + ".exe" then return path(p, name);
        end;
    end;
    return NULL;
end;

`;
