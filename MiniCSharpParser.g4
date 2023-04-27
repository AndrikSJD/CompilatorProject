parser grammar MiniCSharpParser;

options {
    tokenVocab = MiniCSharpScanner; 
}

// Reglas sintácticas
program : using* CLASS ID LBRACE (varDecl | classDecl | methodDecl)* RBRACE EOF     #programAST;

using : USING ident SEMICOLON                                                          #usingAST;

varDecl : type ident (COMMA ident)* SEMICOLON                                             #varDeclAST;

classDecl : CLASS ident LBRACE varDecl* RBRACE                                     #classDeclAST;

methodDecl : (type | VOID) ident LPARENT formPars? RPARENT block                   #methodDeclAST;

formPars : type ID (COMMA type ident)*                                             #formParsAST;

type : ident (LBRACK RBRACK)?                                                      #typeAST;

statement : designator ASSIGN expr SEMICOLON                                    #assignStatementAST
          | designator (LPARENT (actPars)? RPARENT | INC | DEC) SEMICOLON       #methodCallStatementAST            
          | IF LPARENT condition RPARENT statement (ELSE statement)?            #ifStatementAST
          | FOR LPARENT expr SEMICOLON condition? SEMICOLON statement? RPARENT statement #forStatementAST
          | WHILE LPARENT condition RPARENT statement                           #whileStatementAST
          | BREAK SEMICOLON                                                     #breakStatementAST   
          | RETURN expr? SEMICOLON                                              #returnStatementAST                    
          | READ LPARENT designator RPARENT SEMICOLON                           #readStatementAST
          | WRITE LPARENT expr (COMMA NUM)? RPARENT SEMICOLON                   #writeStatementAST
          | block                                                               #blockStatementAST
          | BLOCKCOMMENT                                                        #blockCommentStatementAST                        
          | SEMICOLON                                                           #emptyStatementAST
          ;

block : LBRACE (varDecl | statement)* RBRACE                        #blockAST;      

actPars : expr (COMMA expr)*                                        #actParsAST;

condition : condTerm (OR condTerm)*                               #conditionAST;

condTerm : condFact (AND condFact)*                               #condTermAST;

condFact : expr relop expr                                          #condFactAST;

cast : LPARENT type RPARENT                                         #castAST;         

expr : (SUB | cast)? term ((ADD | SUB) term)*                       #exprAST;

term : factor ((MUL | DIV | MOD) factor)*                            #termAST;

factor : designator (LPARENT actPars? RPARENT)?                #factorAST
    | NUM                                                       #numFactorAST
    | CHARCONST                                         #charFactorAST
    | STRINGCONST                                    #stringFactorAST
    | BOOLEANCONST                                  #booleanFactorAST
    | NEW ident                                       #newFactorAST
    | LPARENT expr RPARENT                      #parenFactorAST
    ;

                                              

designator : ident ((DOT ident) | (LBRACK expr RBRACK))*          #designatorAST;

ident: ID                                                    #identAST;

relop : (EQUAL | NOTEQUAL | GT | GE | LT | LE)              #relopAST;