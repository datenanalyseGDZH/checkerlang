# checkerlang

This implements the checkerlang programming language.

There are three implementations available:
* for the javascript platform (browser and/or node.js)
* for the java platform
* for the dotnet platform

The three implementations aim to contain the same functionality
and be compatible. Therefore, it should be possible to write
a checkerlang program in any of these and run it on all the
other platforms.

There are two executables available ckl-run and ckl-repl.
The first is used to run a script file without interaction.
The second is a classical REPL for online interaction with
the language.

The dotnet version can be built ("published") as self contained
files using the ckl-dotnet\build.bat file.

The java version can be build as jar files, using the
ckl-java\build.xml ant file.

The javascript version does not need a build step and can be
run using node immediately (e.g. node ckl-js/ckl-repl.mjs).

An online version is available at https://www.checkerlang.ch/.
Here you can interact with the language locally in your
browser (nothing runs on the server) using a simple notebook
type application. Enter your code in a cell and execute it with
Ctrl-Enter. Move between cells with Ctrl-ArrowUp and Ctrl-ArrowDown.

