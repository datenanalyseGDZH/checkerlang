# notebook

* provide switch between default and legacy mode (where default means, modules must be used)
* number the cells?
* store the last value in a variable (LAST)
* delete, insert, copy cells
* join adjacent cells
* split cell
* after reload, show the results in gray, only when they are freshly computed in the current session change color to black
* toggle between dark/light mode
* toggle right hand pane containing list of functions and documentation
* toggle right or bottom pane with standard input/output (i.e. a textarea...)
* export cells as one consolidated program text
* run calculation in async
* syntax highlighting in div and textarea! at least keywords.
* implement save as
* provide means to remove a script
* maybe add daily script, i.e. each day a default script is created with the current date as name this results in a track of all interactions with scriptlang. of course, it must be possible to remove such scripts!
* notebook should have offline and online mode. In offline mode, everything is saved in the browser (as it is currently). In online mode, reached after logging in, the notebooks are saved on the server. It is possible to choose per notebook whether it should be online or offline.

# language

* js: maybe rename isEquals() to equals(), seems to be the canonical name for the equality function
* parse_json should support fileinput values and read only as much as is needed.
* destructuring assign should support more than just identifiers, also a->b, a[b], a(b)[c], a(b)->c and so on
* optimize for loop in java/dotnet so that sets and maps are not first converted to lists and only then traversed
* unfortunately, iterating over a map traverses the values, not the keys. This cannot be changed due to backwards compatibility, but maybe a more efficient way than for x in set(m) could be devised.
* spreading works in funcalls and lists. What about sets and maps? Can we implement this?

* run unittests for modules, how? in a function, require the module, iterate over the object keys...
* revisit all function names, make them more consistent, provide legacy module for older names
* make valueobject parseable (useful as a more efficient map with string keys)
* support relative paths (relative to the current module/script!)
* set module import path using a command line argument (or interpreter parameter)

* expand list comprehensions to include more than one for-loop, e.g. [[x, y] for x in xs for y in ys if x < y]

* port modules string, type, predicate to java and dotnet including base/core/legacy implementation and fixes to tests
