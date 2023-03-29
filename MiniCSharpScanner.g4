lexer grammar MiniCSharpScanner;

COMMENT : '//' ~[\r\n]* -> skip;
WS  :   [ \t\n\r]+ -> skip;

fragment DIGIT : [0-9];
fragment LETTER : [a-z]|[A-Z];
fragment EXPRESION : LETTER+;

// Const values
TYPECHAR : 'char';
TYPEINT : 'int';
TYPEDOUBLE : 'double';
TYPEBOOL : 'bool';
TYPESTRING : 'string';

// Array methods
ARRADD : 'add';
ARRDEL : 'del';
ARRLEN : 'len';

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

NUM : DIGIT+;
ID : LETTER  (LETTER | DIGIT)*;
PLAINTEXT : '"' (LETTER|DIGIT|WS)* '"';
CHAR : '"' ~['\\r\n] '"' ;
BOOLEAN: ('true'|'false');
DOUBLE: DIGIT+ '.' DIGIT*;