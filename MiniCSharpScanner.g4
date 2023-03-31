lexer grammar MiniCSharpScanner;

COMMENT : '//' ~[\r\n]* -> skip;
BLOCKCOMMENT : '/*' ( BLOCKCOMMENT | ~[*/] | '/' ~'*' )* '*/' -> skip;
WS  :   [ \t\n\r]+ -> skip;

// Reserved words
CLASS : 'class';
USING : 'using';
VOID : 'void';
IF : 'if';
ELSE : 'else';
FOR : 'for';
WHILE : 'while';
BREAK : 'break';
RETURN : 'return';
READ : 'read';
WRITE : 'write';
NEW : 'new';

// Reserved symbols
LBRACE : '{';
RBRACE : '}';
LPARENT : '(';
RPARENT : ')';
LBRACK : '[';
RBRACK : ']';
SEMICOLON : ';';
COMMA : ',';
DOT : '.';

// Reserved operators
ASSIGN : '=';
INC : '++';
DEC : '--';
OR : '||';
AND : '&&';
EQUAL : '==';
NOTEQUAL : '!=';
GT : '>';
GE : '>=';
LT : '<';
LE : '<=';
ADD : '+';
SUB : '-';
MUL : '*';
DIV : '/';
MOD : '%';

fragment DIGIT : [0-9];
fragment LETTER : [a-z]|[A-Z];
fragment EXPRESION : LETTER+;
fragment LCOMMENT : '/*';
fragment RCOMMENT : '*/';

NUM : SUB? (DIGIT+ | DIGIT+ '.' DIGIT*);
ID : LETTER  (LETTER | DIGIT)*;
STRINGCONST : '"' .*? '"';
CHARCONST : '"' ~['\\r\n] '"' ;
BOOLEANCONST: ('true'|'false');