@echo off
set base=%~dp0
node %base%ckl-js\ckl-run.mjs "-I%base%modules" %*
