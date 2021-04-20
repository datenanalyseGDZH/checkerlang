syn keyword basicLanguageKeywords def in do end fn if then elif else
syn keyword basicLanguageKeywords return break continue
syn keyword basicLanguageKeywords for while try catch finally is
syn keyword basicLanguageKeywords require as unqualified import

syn match operators '+\|-\|*\|/\|%\|<=\|<\|>=\|>\|<>\|!=\|==\|+=\|-=\|\*=\|/=\|%=\|!>\|=>\|<<<\|<<\|>>>\|>>\|<\*\|\*>\|\[\|\]\|(\|)\|='

syn match cklNumber '\d\+' contained display
syn match cklNumber '[-+]\d\+' contained display
syn match cklNumber '\d\+\.\d*' contained display
syn match cklNumber '[-+]\d\+\.\d*' contained display

syn region cklString start='"' end='"' contained
syn region cklString start='\'' end='\'' contained
syn region cklDesc start='"' end='"'
syn region cklDesc start='\'' end='\''

syn keyword cklTodo contained TODO FIXME XXX NOTE
syn match cklComment "#.*$" contains=cklTodo

let b:current_syntax = "ckl"

hi def link cklTodo                 Todo
hi def link cklComment              Comment
hi def link cklString               Constant
hi def link cklDesc                 PreProc
hi def link cklNumber               Constant
hi def link basicLanguageKeywords   Keyword
hi def link operators               Operator

