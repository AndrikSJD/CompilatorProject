parser grammar MiniCSharpParser;

options {
    tokenVocab = MiniCSharpScanner; 
}

// Reglas sintácticas
program : using* CLASS ID LBRACE (varDecl | classDecl | methodDecl)* RBRACE EOF     #programAST;

using : USING ID SEMICOLON                                                          #usingAST;

varDecl : type ID (COMMA ID)* SEMICOLON                                             #varDeclAST;

classDecl : CLASS ID LBRACE varDecl* RBRACE                                     #classDeclAST;

methodDecl : (type | VOID) ID LPARENT formPars? RPARENT block                   #methodDeclAST;

formPars : type ID (COMMA type ID)*                                             #formParsAST;

type : ID (LBRACK RBRACK)?                                                      #typeAST;

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
    | NEW ID                                        #newFactorAST
    | LPARENT expr RPARENT                      #parenFactorAST
    ;

designator : ID ((DOT ID) | (LBRACK expr RBRACK))*          #designatorAST;

relop : (EQUAL | NOTEQUAL | GT | GE | LT | LE)              #relopAST;