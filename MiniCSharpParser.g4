grammar MiniCSharpParser;

// Importamos el lexer generado previamente
import MiniCSharpScanner;

// Reglas sintácticas

program : using* CLASS ID LBRACE (varDecl | classDecl | methodDecl)* RBRACE;

using : USING ID SEMICOLON;

varDecl : type ID (COMMA ID)* SEMICOLON;

classDecl : CLASS ID LBRACE varDecl* RBRACE;

methodDecl : (type | VOID) ID LPARENT formPars? RPARENT block;

formPars : type ID (COMMA type ID)*;

type : ID (LBRACK RBRACK)?;

statement : designator ASSIGN expr SEMICOLON
          | designator (LPARENT (actPars)? RPARENT | INC | DEC) SEMICOLON
          | IF LPARENT condition RPARENT statement (ELSE statement)?
          | FOR LPARENT expr SEMICOLON condition? SEMICOLON statement? RPARENT statement
          | WHILE LPARENT condition RPARENT statement
          | BREAK SEMICOLON
          | RETURN expr? SEMICOLON
          | READ LPARENT designator RPARENT SEMICOLON
          | WRITE LPARENT expr (COMMA NUMBER)? RPARENT SEMICOLON
          | block
          | SEMICOLON;

block : LBRACE (varDecl | statement)* RBRACE;

actPars : expr (COMMA expr)*;

condition : condTerm (OR condTerm)*;

condTerm : condFact (AND condFact)*;

condFact : expr relop expr;

cast : LPARENT type RPARENT;

expr : (SUB | cast)? term ((ADD | SUB) term)*;

term : factor ((MUL | DIV | MOD) factor)*;

factor : designator (LPARENT (actPars)? RPARENT | NUMBER | CHARCONST | STRINGCONST | BOOLCONST | NEW ID | LPARENT expr RPARENT);

designator : ID ((DOT ID) | (LBRACK expr RBRACK))*;

relop : (EQUAL | NOTEQUAL | GT | GE | LT | LE);

// Tokens léxicos

NUMBER : DIGIT+;
BOOLCONST : ('true' | 'false');
CHARCONST : '\'' (~('\n' | '\r' | '\'') | ('\\' .))* '\'';
STRINGCONST : '"' (~('\n' | '\r' | '"') | ('\\' .))* '"';
WS : [ \t\n\r]+ -> skip;

// Reglas para ignorar comentarios y tokens desconocidos

COMMENT : '//' ~[\r\n]* -> skip;
OTHER : . -> skip;
