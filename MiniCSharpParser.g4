parser grammar MiniCSharpParser;

options {
    tokenVocab = MiniCSharpScanner; 
}

// Reglas sintácticas
program : using* CLASS ID LBRACE (varDecl | classDecl | methodDecl)* RBRACE EOF;

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
          | WRITE LPARENT expr (COMMA NUM)? RPARENT SEMICOLON
          | block
          | BLOCKCOMMENT
          | SEMICOLON;

block : LBRACE (varDecl | statement)* RBRACE;

actPars : expr (COMMA expr)*;

condition : condTerm (OR condTerm)*;

condTerm : condFact (AND condFact)*;

condFact : expr relop expr;

cast : LPARENT type RPARENT;

expr : (SUB | cast)? term ((ADD | SUB) term)*;

term : factor ((MUL | DIV | MOD) factor)*;

factor : designator (LPARENT actPars? RPARENT)?
    | NUM
    | CHARCONST
    | STRINGCONST
    | BOOLEANCONST
    | NEW ID 
    | LPARENT expr RPARENT;

designator : ID ((DOT ID) | (LBRACK expr RBRACK))*;

relop : (EQUAL | NOTEQUAL | GT | GE | LT | LE);