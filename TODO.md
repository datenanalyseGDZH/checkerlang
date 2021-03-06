# notebook

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
* notebook should have offline and online mode. In offline mode, everything is saved in the browser (as it is currently). In online mode, reached after logging in, the notebooks are saved on the server. It is possible to choose per notebook whether it should be online or offline.

# language

* js: maybe rename isEquals() to equals(), seems to be the canonical name for the equality function
* parse_json should support fileinput values and read only as much as is needed.
* destructuring assign should support more than just identifiers, also a->b, a[b], a(b)[c], a(b)->c and so on
* spreading works in funcalls and lists. What about sets and maps? Can we implement this?

* run unittests for modules, how? in a function, require the module, iterate over the object keys...
* support relative paths (relative to the current module/script!)
* set module import path using a command line argument (or interpreter parameter)

* rework unittests to run per module (without legacy!)
* add much more unit tests and unify them across all plattforms

* Can we prevent loops in toString due to recursive data structures? In objects by using _str_, but in general? This is important, because in stacktraces the objects get printed!
  One possibility would be to add a level into toString and only print up to a certain level (i.e. 3 or 4?) This might also be configurable. If called with initial level -1
  it would be without limit, thus we could still handle string(val) in a general manner, but in e.g. stacktraces, we would limit it.


* Bug in sprintf? sprintf("{0} \{ {1}", 1, 2) should work?!