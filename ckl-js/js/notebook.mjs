/*  Copyright (c) 2021 Damian Brunold, Gesundheitsdirektion Kanton Zürich

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

import { StringOutput, ValueOutput } from "./values.mjs";
import { Interpreter } from "./interpreter.mjs";

let interpreter = null;

function createCell(inputContents = "", outputContents = "", editable = true) {
    const cell = document.createElement("div");
    cell.className = "cell";
    const input = document.createElement(editable ? "textarea" : "div");
    input.className = "input";
    if (editable) input.value = inputContents;
    else input.innerText = inputContents;
    const output = document.createElement("div");
    output.className = "output";
    output.innerText = outputContents;
    cell.appendChild(input);
    cell.appendChild(output);
    if (editable) {
        var offset = input.offsetHeight - input.clientHeight;
        input.addEventListener('input', function (event) {
          event.target.style.height = 'auto';
          event.target.style.height = event.target.scrollHeight + offset + 'px';
        });        
        input.style.height = 'auto';
        input.style.height = input.scrollHeight + offset + 'px';
    }
    return cell;
}

function makeCellEditable(cell) {
    const input = cell.querySelector(".input");
    if (input.tagName.toLowerCase() === "textarea") return;
    const contents = input.innerText;
    const editableInput = document.createElement("textarea");
    editableInput.className = "input";
    editableInput.value = contents;
    input.remove();
    cell.insertBefore(editableInput, cell.querySelector(".output"));
    editableInput.focus();
    var offset = editableInput.offsetHeight - editableInput.clientHeight;
    editableInput.addEventListener('input', function (event) {
      event.target.style.height = 'auto';
      event.target.style.height = event.target.scrollHeight + offset + 'px';
    });        
    editableInput.style.height = 'auto';
    editableInput.style.height = editableInput.scrollHeight + offset + 'px';
}

function makeCellReadonly(cell) {
    const input = cell.querySelector(".input");
    if (input.tagName.toLowerCase() === "div") return;
    const contents = input.value;
    const readonlyInput = document.createElement("div");
    readonlyInput.className = "input";
    readonlyInput.innerText = contents;
    const height = input.offsetHeight;
    input.remove();
    cell.insertBefore(readonlyInput, cell.querySelector(".output"));
    readonlyInput.style.height = height + 'px';
}

function getCellState(cell) {
    const input = cell.querySelector(".input");
    const output = cell.querySelector(".output");
    const editable = input.tagName.toLowerCase() === "textarea";
    return {
        input: editable ? input.value : input.innerText,
        output: output.innerText,
        editable: editable
    };
}

function createCellFromState(state) {
    return createCell(state.input, state.output, state.editable);
}

function saveState() {
    const state = [];
    [...document.getElementById("contents").querySelectorAll(".cell")].forEach(function (cell) {
        state.push(getCellState(cell))
    });
    const name = document.getElementById("name").innerText.substr("Script: ".length);
    window.localStorage.setItem("checkerlang_notebook_cells_" + name, JSON.stringify(state));
}

function loadState() {
    const contents = document.getElementById("contents");
    while (contents.firstChild) {
        contents.removeChild(contents.lastChild);
    }    
    const name = document.getElementById("name").innerText.substr("Script: ".length);
    let data = window.localStorage.getItem("checkerlang_notebook_cells_" + name);
    if (data === null) data = window.localStorage.getItem("scriptlang_notebook_cells_" + name);
    if (data === null) {
        contents.appendChild(createCell());
        contents.querySelector(".input").focus();
    } else {
        const state =  JSON.parse(data);
        for (let cell of state) {
            contents.appendChild(createCellFromState(cell));
        }
        clearOutput();
    }
    contents.querySelector("textarea").focus();
    interpreter = new Interpreter(true, getMode() === "Legacy");
}

function clearOutput() {
    [...document.querySelectorAll("div.output")].forEach(d => d.innerText = '');
}

function getMode() {
    return document.getElementById("mode").innerText.substr("Mode: ".length)
}

function setMode(mode) {
    document.getElementById("mode").innerText = "Mode: " + mode;
    window.localStorage.setItem("checkerlang_notebook_mode", mode);
}

function changeMode(mode) {
    if (mode === "Modern" && getMode !== "Modern") {
        setMode(mode);
        interpreter = new Interpreter(true, false);
        clearOutput();
    } else if (mode === "Legacy" && getMode !== "Legacy") {
        setMode(mode);
        interpreter = new Interpreter(true, true);
        clearOutput();
    }
}


const keypress = function(event) {
    if (event.ctrlKey && event.code === "Enter") {
        const input = event.srcElement;
        const cell = input.parentNode;
        const script = input.value;
        const output = cell.querySelector(".output");
        try {
            const stdout = new ValueOutput(new StringOutput());
            interpreter.baseEnvironment.put("stdout", stdout);
            const result = interpreter.interpret(script, "{input}");
            let display = stdout.output.output;
            if (display !== "" && !result.isNull()) display = display.concat("\n", result.toString());
            else if (!result.isNull()) display = result.toString();
            output.innerText = display;
        } catch (e) {
            console.log(e);
            let errortext = e.msg + " " + e.pos.toString();
            if ("stacktrace" in e) {
                errortext += "\n\nStacktrace:\n" + e.stacktrace.join("\n")
            }
            output.innerText = errortext;
        }
        if (cell.nextElementSibling === null) {
            const nextCell = createCell();
            cell.parentNode.appendChild(nextCell);
            makeCellReadonly(cell);
            nextCell.querySelector(".input").focus();
        } else {
            makeCellReadonly(cell);
            makeCellEditable(cell.nextElementSibling);
            cell.nextElementSibling.querySelector(".input").focus();
        }
        saveState();
    }
};

const keydown = function(event) {
    if (event.ctrlKey && event.code === "ArrowUp") {
        const cell = event.srcElement.parentNode;
        if (cell.previousElementSibling !== null) {
            makeCellReadonly(cell);
            makeCellEditable(cell.previousElementSibling);
            cell.previousElementSibling.querySelector(".input").focus();
            saveState();
        }
    } else if (event.ctrlKey && event.code === "ArrowDown") {
        const cell = event.srcElement.parentNode;
        if (cell.nextElementSibling !== null) {
            makeCellReadonly(cell);
            makeCellEditable(cell.nextElementSibling);
            cell.nextElementSibling.querySelector(".input").focus();
            saveState();
        }
    } else if (!event.ctrlKey && event.code === "Tab") {
        let text = event.target.value;
        let pos = event.target.selectionStart;
        event.target.value = text.substring(0, pos) + "  " + text.substring(pos);
        event.target.selectionStart = pos + 2;
        event.target.selectionEnd = pos + 2;
        event.preventDefault();
    }
};

const cellclick = function(event) {
    if (event.srcElement.className === "input") {
        const oldCell = document.getElementById("contents").querySelector("textarea.input").parentNode;
        const newCell = event.srcElement.parentNode;
        if (oldCell === newCell) return;
        makeCellReadonly(oldCell);
        makeCellEditable(newCell);
        newCell.querySelector(".input").focus();
        saveState();
    }
}

const loadclick = function(event) {
    if (event.target.tagName.toLowerCase() === 'p') {
        document.querySelector(".ddname .dropdown-content").style.display = '';
        const name = event.srcElement.innerText;
        document.getElementById("name").innerText = "Script: " + name;
        window.localStorage.setItem("checkerlang_notebook_script", name);
        loadState();
    }
}

const modeclick = function(event) {
    if (event.target.tagName.toLowerCase() === 'p') {
        document.querySelector(".ddmode .dropdown-content").style.display = '';
        const mode = event.srcElement.innerText;
        changeMode(mode);
    }
}

document.addEventListener("keypress", keypress);
document.addEventListener("keydown", keydown);
document.addEventListener("click", cellclick);

document.getElementById("name").addEventListener("click", function(event) {
    if (event.target.id === "name") {
        const list = document.querySelector(".ddname .dropdown-content");
        if (list.style.display === 'none' || list.style.display === '') list.style.display = 'block';
        else list.style.display = 'none';
    }
});

document.querySelector(".ddname .dropdown-content").addEventListener("click", loadclick);

document.getElementById("mode").addEventListener("click", function(event) {
    if (event.target.id === "mode") {
        const list = document.querySelector(".ddmode .dropdown-content");
        if (list.style.display === 'none' || list.style.display === '') list.style.display = 'block';
        else list.style.display = 'none';
    }
});

document.querySelector(".ddmode .dropdown-content").addEventListener("click", modeclick);

document.getElementById("clear").addEventListener("click", function(event) {
    const contents = document.getElementById("contents");
    while (contents.firstChild) {
        contents.removeChild(contents.lastChild);
    }    
    contents.appendChild(createCell());
    contents.querySelector(".input").focus();
    saveState();
});

let mode = window.localStorage.getItem("checkerlang_notebook_mode") || "Legacy";
document.getElementById("mode").innerText = "Mode: " + mode;

let name = window.localStorage.getItem("checkerlang_notebook_script") || "A";
document.getElementById("name").innerText = "Script: " + name;

loadState();

interpreter = new Interpreter(true, mode === "Legacy");

