@echo off
set base=%~dp0
node %base%ckl-js\\ckl-repl.mjs "-I%base%modules" %*
