# notebook

* number the cells?
*  store the last value in a variable (LAST)
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
* add module system
* categorize functions into modules
* provide legacy module that corresponds to the features of ckl 2
* parse_json should support fileinput values and read only as much as is needed.
* map comprehension
* destructuring assign should support more than just identifiers, also a->b, a[b], a(b)[c], a(b)->c and so on
