
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime;
using SyntacticAnalysisGenerated;
using Proyecto.StructureTypes;
using Type =Proyecto.StructureTypes.Type;

namespace Proyecto;

public class AContextual : MiniCSharpParserBaseVisitor<object> {
    
    private SymbolTable _symbolTable;
    
    public AContextual()
    {
        _symbolTable = new SymbolTable();
    }
    
    
    
 
    
    private string ShowToken(IToken token)
    {
        return token.Text + "Fila, columna: (" + token.Line + "," + token.Column + ")";
    }

    
    private void PrintError(IToken tok, String msg)
    {
        Console.WriteLine("Error en la linea " + tok.Line + ":" + tok.Column + " " + msg);
    }

    private bool IsMultitype(String op)
    {
        switch (op)
        {
            case "==": return true;
            case "!=": return true;
            default: return false;
            
        }
    }

    
    public override object VisitProgramAST(MiniCSharpParser.ProgramASTContext context)
    {
        try
        {
            _symbolTable.OpenScope();
            IToken token = (IToken)Visit(context.ident());
            ClassType classType = new ClassType(token, _symbolTable.currentLevel);
            _symbolTable.Insert(classType);


            foreach (var child in context.children)
            {
                Visit(child);
            }
            _symbolTable.CloseScope();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        _symbolTable.Print();
        return base.VisitProgramAST(context);
        
    }


    public override object VisitUsingAST(MiniCSharpParser.UsingASTContext context)
    {
       
        //TODO : implementar using 
        
       return null;
    }

    public override object VisitVarDeclAST(MiniCSharpParser.VarDeclASTContext context)
    {
        //Para validar que el tipo de la variable sea valido
        PrimaryType.PrimaryTypes varType;
        bool isArray = false;
        bool isClassVarType = false;
        bool isError = false;
        
        LinkedList<Type> lista = new LinkedList<Type>();

        //verificamos si es un array
        if(context.type().GetText().Contains("[]"))
        {
            isArray = true;
            //quitamos los corchetes
            varType = PrimaryType.showType(context.type().GetText().Substring(0, context.type().GetText().Length - 2).Trim());
            if (varType is PrimaryType.PrimaryTypes.Unknown &&
                _symbolTable.Search(context.type().GetText().Substring(0, context.type().GetText().Length - 2)
                    .Trim()) != null)
            {
                isClassVarType = true;
            }
            else if (varType != PrimaryType.PrimaryTypes.Char && varType != PrimaryType.PrimaryTypes.Int)
            {
                System.Diagnostics.Debug.WriteLine("El tipo del array solo puede ser int o char, el tipo actual no es valido ");
                isError = true;
            }
            
        } else
        {
            varType = PrimaryType.showType(context.type().GetText());
            if (varType is PrimaryType.PrimaryTypes.Unknown && 
                _symbolTable.Search(context.type().GetText()) != null)
            {
                isClassVarType = true;
            }
            else if (varType is PrimaryType.PrimaryTypes.Unknown && 
                     _symbolTable.Search(context.type().GetText()) == null)
            {
                isError = true;
            }
            
        }
        if (!isError)
        {
            foreach (var child in context.ident())
            {      
                IToken token = (IToken)Visit(child);
                //verificamos si es un array
                if(isArray) 
                {
                    if (varType is PrimaryType.PrimaryTypes.Int)
                        {
                            ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int);
                            _symbolTable.Insert(array);
                            lista.AddFirst(array);
                            
                        }
                        else if (varType is PrimaryType.PrimaryTypes.Char ) //al validar el tipo de la variable, si no es int, es char anteriormente se valido si no era valido isError = true
                        {
                            ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char);
                            _symbolTable.Insert(array);
                            lista.AddFirst(array);
                        }
                        
                        
                }
                else
                {
                    //si la variable es de un tipo de clase
                    if(isClassVarType)
                    {
                        ClassVarType element = new ClassVarType(token, _symbolTable.currentLevel, context.type().GetText());
                        _symbolTable.Insert(element);
                        lista.AddFirst(element);
                    }
                    else //es de un tipo primario
                    {
     
                        PrimaryType element = new PrimaryType(token,varType, _symbolTable.currentLevel);
                        _symbolTable.Insert(element);
                        lista.AddFirst(element);
                    }

                }
            }  
            
        }

        else
        {
            System.Diagnostics.Debug.WriteLine("El tipo de declaracion de la variable no es valida");
        }


        return lista;
    }

    public override object VisitClassDeclAST(MiniCSharpParser.ClassDeclASTContext context)
    {
        ClassType classDcl = new ClassType((IToken)Visit(context.ident()), _symbolTable.currentLevel);
        _symbolTable.Insert(classDcl);
        _symbolTable.OpenScope();
        if(context.varDecl().Length > 0 && context.varDecl()!= null)
        {
           
            foreach (var child in context.varDecl())
            {
                LinkedList<Type> list = (LinkedList<Type>)Visit(child);
                    if(list != null)
                    {
                        foreach (var type in list)
                        {
                            if (type is PrimaryType)
                            {
                                classDcl.parametersL.AddFirst(type);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("El tipo de variable"+ child.GetType()  + " no se puede agregar a la clase" + context.ident().GetText() );
                            }
                            
                        }
                    }
            }
            // foreach (var attri in classDcl.parametersL)
            // {
            //     System.Diagnostics.Debug.WriteLine("Atributo de la clase " + classDcl.GetToken().Text + " : " + attri.GetToken().Text + " tipo: " + attri.GetStructureType() + " nivel: " + attri.Level + "");
            // }
            
        }
       
        _symbolTable.CloseScope();
        return null;
    }

    public override object VisitMethodDeclAST(MiniCSharpParser.MethodDeclASTContext context)
    {
        _symbolTable.OpenScope();
       
        //probar con visit(context.ident()) para ver si funciona
        IToken token = (IToken)Visit(context.ident());
        //inicializamos en unknown para validar que el tipo de retorno sea valido y que nos permita accesar cuando
        PrimaryType.PrimaryTypes methodType = PrimaryType.PrimaryTypes.Unknown;
        bool isArray = false;
        bool isClassVarType = false;
        bool isError = false;

        if (context.type()!= null)
        {
            //verificamos si es un array
            if(context.type().GetText().Contains("[]"))
            {
                isArray = true;
                //quitamos los corchetes
                methodType = PrimaryType.showType(context.type().GetText().Substring(0, context.type().GetText().Length - 2).Trim());
                if (methodType is PrimaryType.PrimaryTypes.Unknown && 
                    _symbolTable.Search(context.type().GetText().Substring(0, context.type().GetText().Length - 2 )
                        .Trim()) != null)
                {
                    isClassVarType = true;
                }
                else if (methodType != PrimaryType.PrimaryTypes.Char && methodType != PrimaryType.PrimaryTypes.Int)
                {
                    // TODO revisar
                    //System.Diagnostics.Debug.WriteLine("The type of the array can be only int or char, current type is not valid METHOD DECL ");
                    System.Diagnostics.Debug.WriteLine("El tipo del arreglo solo puede ser int o char, el tipo actual no es valido ");
                    isError = true;
                }
            
            }else
            {
                methodType = PrimaryType.showType(context.type().GetText());
                if (methodType is PrimaryType.PrimaryTypes.Unknown && 
                    _symbolTable.Search(context.type().GetText()) != null)
                {
                    isClassVarType = true;
                }
                else if (methodType is PrimaryType.PrimaryTypes.Unknown && 
                         _symbolTable.Search(context.type().GetText()) == null)
                {
                    isError = true;
                }
            }
        }

        if (!isError)
        {
            LinkedList<Type> parameters = new LinkedList<Type>();
            
            if(context.formPars() != null)
            {
                parameters = (LinkedList<Type>)Visit(context.formPars());
                 
            
            }
            MethodType method;
            if (context.type() != null)
            {
                method = new MethodType(token, _symbolTable.currentLevel, parameters.Count, methodType.ToString() , parameters);
                _symbolTable.Insert(method);
                _symbolTable.currentMethod = method;
            }
            else
            {
                method = new MethodType(token, _symbolTable.currentLevel, parameters.Count, "Void", parameters);
                _symbolTable.Insert(method);
                _symbolTable.currentMethod = method;
            }
        
            foreach (var child in parameters)
        {
            _symbolTable.Insert(child);
        }
 
            //string? methodReturnType = (string)
        Visit(context.block());
        
        

        
    
        System.Diagnostics.Debug.WriteLine("Cerrando scope de metodo " + token.Text);
        
        _symbolTable.Sacar(token.Text);
        
        // _symbolTable.CloseScope();
        // return null;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("El tipo del metodo no es valido");
        }

        
        _symbolTable.CloseScope();
        
        return null;
    }

    public override object VisitFormParsAST(MiniCSharpParser.FormParsASTContext context)
    {
        LinkedList<Type> parameters = new LinkedList<Type>();
        for (int i = 0; i < context.ident().Length; i++)
        {
            IToken token = (IToken)Visit(context.ident(i));
            string type = context.type(i).GetText();
            PrimaryType.PrimaryTypes varType;
            //verificamos si es un array
            if (type.Contains("[]"))
            {
                varType = PrimaryType.showType(context.type(i).GetText().Substring(0, context.type(i).GetText().Length - 2).Trim());
                
                if (varType is PrimaryType.PrimaryTypes.Int)
                {
                    parameters.AddLast(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int));
                    //los agrega tambien a la tabla
                    // _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int));
                }
                else if (varType is PrimaryType.PrimaryTypes.Char)
                {
                    parameters.AddLast(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                    //los agrega a la tabla
                    // _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("El tipo del arreglo solo puede ser int o char, el tipo actual no es valido");
                }
                
            }
            else
            {
                varType = PrimaryType.showType(type);
                //verificamos si es un tipo de clase
                Type? paramT = _symbolTable.Search(type);
                //si es un tipo de desconocido y existe en la tabla de simbolos (es una clase)
                if (varType is PrimaryType.PrimaryTypes.Unknown &&
                    paramT != null)
                {
                    parameters.AddLast(new ClassVarType(token, _symbolTable.currentLevel, type));
                    //los agrega tambien a la tabla
                    // _symbolTable.Insert(new ClassVarType(token, _symbolTable.currentLevel, type));
                    
                }
                else if (varType is PrimaryType.PrimaryTypes.Unknown &&
                    paramT == null)
                {
                    System.Diagnostics.Debug.WriteLine("El tipo de parametro: " + token.Text +" no es valido");
                }
                else
                {
                    parameters.AddLast(new PrimaryType(token, varType, _symbolTable.currentLevel));
                    //los agrega tambien a la tabla
                    // _symbolTable.Insert(new PrimaryType(token, varType, _symbolTable.currentLevel));
                }
            }
            
        }
        return parameters;
    }

    //TODO: Revsar el visit de type
    public override object VisitTypeAST(MiniCSharpParser.TypeASTContext context)
    {
        IToken type = (IToken)Visit(context.ident());

        //se retorna el identificador del tipo
        return type.Text;

    }

    public override object VisitAssignStatementAST(MiniCSharpParser.AssignStatementASTContext context)
    {
       

        
        if(context.expr()!=null)// si es una asignacion 
        {
            string tipoDesignator = (string)Visit(context.designator()); 
            string tipoExpresion = ((string)Visit(context.expr())).ToLower(); //tolower para que no haya problemas con mayusculas y minusculas



            //verificamos si el tipodesignator viene nulo es decir el indice no es null

            if (tipoDesignator != null && tipoExpresion != null)
            {
                tipoDesignator = tipoDesignator.ToLower();//tolower para que no haya problemas con mayusculas y minusculas
                if (tipoDesignator.Contains("[]") && context.expr().GetText().Contains("new"))
                {
          
                    if (!(tipoDesignator.ToLower().Contains(tipoExpresion)))
                    {
                        System.Diagnostics.Debug.WriteLine("Error de asignacion: " + tipoDesignator + " no es el mismo que el tipo de la expresion: " + tipoExpresion);
                    }
                    else
                    {
                        // System.Diagnostics.Debug.WriteLine("Asignacion correcta: " + tipoDesignator + " es el mismo que el tipo de la expresion: " + tipoExpresion);
                    }

                    return null;
                }
            
                if (tipoDesignator != tipoExpresion)
                {
                    System.Diagnostics.Debug.WriteLine("Error de asignacion: " + tipoDesignator + " no es el mismo que el tipo de la expresion: " + tipoExpresion);
                    return null;
                }
                else
                {
                    // System.Diagnostics.Debug.WriteLine("Asignacion correcta: " + tipoDesignator + " es el mismo que el tipo de la expresion: " + tipoExpresion);
                    return null;
                }
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR AssignStatement de asignacion, los tipos del designator y de la expression no son validos ");
                return null;
            }

            
        }
        else if (context.LPARENT() != null) // si es llamada a metodo
        {
            Type? type = _symbolTable.Search(context.designator().GetText());
            
            if (context.designator().GetText() == "del")
            {
                if (context.actPars() != null)
                {
                    //obtener lista de parametros
                    LinkedList<Type> parametros = (LinkedList<Type>)Visit(context.actPars());
                    //Recibe dos parametros, la lista, el indice
                    
                    
                    if (parametros.Count == 2)
                    {
                        if (parametros.ElementAt(0) is ArrayType && parametros.ElementAt(1).GetStructureType().Equals("Int"))
                        {
                            // System.Diagnostics.Debug.WriteLine("Todo bien, todo correcto en el del");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error de parametros, tipos de parametros no coinciden en el del");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        //TODO: verificar orden de los parametros que pueden tener orden diferente
                       
                            System.Diagnostics.Debug.WriteLine("Error en los parametros, cantidad de parametros metodo DEL");
                        
                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR DE METODO: Faltan los parametros para el metodo del");
                }
                
            }
            else if (context.designator().GetText() == "len")
            {
                if (context.actPars() != null)
                {
                    //obtener lista de parametros
                    LinkedList<Type> lenPars = (LinkedList<Type>)Visit(context.actPars());

                    if (lenPars.Count == 1)
                    {
                        if (lenPars.ElementAt(0) is ArrayType)
                        {
                            // System.Diagnostics.Debug.WriteLine("Todo bien, todo correcto en el len");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(
                                "Error de parametros, tipos de parametros no coinciden en el len");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR DE METODO: cantidad de parametros distinta len");
                    }
                    
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR DE METODO: Faltan los parametros para el metodo len");
                }
            }
            else if (context.designator().GetText() == "add")
            {
                if (context.actPars() != null)
                {
                    //obtener lista de parametros
                    LinkedList<Type> parametros = (LinkedList<Type>)Visit(context.actPars());
                    //Recibe dos parametros, la lista, el indice

                    if (parametros.Count == 2)
                    {
                        if (parametros.ElementAt(0).GetStructureType().Equals(parametros.ElementAt(1).GetStructureType()))
                        {
                            // System.Diagnostics.Debug.WriteLine("Todo bien, todo correcto en el add");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error de parametros, tipos de parametros no coinciden en el del");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        
                       
                        System.Diagnostics.Debug.WriteLine("Error en los parametros, cantidad de parametros en el ADD");
                        
                    }

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR METODO ADD: Faltan los parametros para el metodo del");
                }
                
            }
           
            else if (type is MethodType method)
            {
                if (context.actPars() != null)
                {
                    LinkedList<Type> parametros = (LinkedList<Type>)Visit(context.actPars());
                    if (parametros.Count == method.parametersL.Count)
                    {
                        for (int i = 0; i < method.parametersL.Count; i++)
                        {
                            if (method.parametersL.ElementAt(i).GetStructureType().ToString() !=
                                parametros.ElementAt(i).GetStructureType())
                            {
                                System.Diagnostics.Debug.WriteLine("Error de asignacion: " +
                                                                   method.parametersL.ElementAt(i).GetStructureType()
                                                                       .ToString() +
                                                                   " no es el mismo que el tipo de la expresion: " +
                                                                   parametros.ElementAt(i));
                            }
                            else
                            {
                                // System.Diagnostics.Debug.WriteLine("Asignacion correcta: " +
                                //                                    method.parametersL.ElementAt(i).GetStructureType()
                                //                                        .ToString() +
                                //                                    " es el mismo que el tipo de la expresion: " +
                                //                                    parametros.ElementAt(i));
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error de asignacion: " + method.parametersL.Count +
                                                           " no es el mismo que el tipo de la expresion: " +
                                                           parametros.Count);

                    }
                }
                else if (method.parametersL.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Error de parametros, faltan parametros : " + method.GetToken().Text);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error de asignacion: " + context.designator().GetText() + " no es un metodo");
            }
            
           
        }
        else if (context.INC()!=null)
        {
            
        }
        else if (context.DEC()!=null)
        {
            
        }
        return null;
    }
    

    public override object VisitIfStatementAST(MiniCSharpParser.IfStatementASTContext context)
    {
        _symbolTable.OpenScope();
        bool conditionValue = (bool)Visit(context.condition());
        if (conditionValue)
        {
            Visit(context.statement(0));
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("ERROR: Tipo en la condicion if, es falsa: "+'"' + context.condition().GetText()+'"');
            if (context.statement(1) != null)
            {
                _symbolTable.OpenScope();
                Visit(context.statement(1));
                _symbolTable.CloseScope();
            }
        }
        
        _symbolTable.CloseScope();
        return null;
        
    }

    public override object VisitForStatementAST(MiniCSharpParser.ForStatementASTContext context)
    {
        _symbolTable.OpenScope();
        Visit(context.expr());//visitamos la expresion
        if (context.condition()!= null)
        {
            
            bool conditionValue = (bool)Visit(context.condition());
            if (conditionValue)
            {
                if (context.statement().Length > 1)
                {
                    Visit(context.statement(0));
                    Visit(context.statement(1));
                }
                else
                {
                    Visit(context.statement(0));
                }
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Tipo en la condicion for, es falsa" + context.condition().GetText());
            }
            _symbolTable.CloseScope();
            return null;
            
        }
        if(context.statement().Length > 1)
        {
            Visit(context.statement(0));
            Visit(context.statement(1));
        }
        else
        {
            Visit(context.statement(0));
        }
        _symbolTable.CloseScope();
        return null;

    }

    public override object VisitWhileStatementAST(MiniCSharpParser.WhileStatementASTContext context)
    {
        _symbolTable.OpenScope();
        bool conditionValue = (bool)Visit(context.condition());
        if (conditionValue)
        {
            Visit(context.statement());
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("ERROR: Tipo en la condicion while, es falsa" + context.condition().GetText());
        }
        _symbolTable.CloseScope();
        return null;
    }

    public override object VisitBreakStatementAST(MiniCSharpParser.BreakStatementASTContext context)
    {
        return null;
    }

    public override object VisitReturnStatementAST(MiniCSharpParser.ReturnStatementASTContext context)
    {
      
        if (context.expr() != null)
        {

            string typeReturn = (string)Visit(context.expr());
            if (_symbolTable.currentMethod.ReturnTypeGetSet == "void")
            {
                System.Diagnostics.Debug.WriteLine("ERROR de Retorno : El metodo void no puede retornar datos: " + _symbolTable.currentMethod.GetToken().Text);
            }
            //no le pudo identificar el tipo de retorno en caso de que sea null
            if (typeReturn != null)
            {
                if(typeReturn.ToLower() != _symbolTable.currentMethod.ReturnTypeGetSet.ToLower())
                {
                    System.Diagnostics.Debug.WriteLine("ERROR de Retorno : El metodo " + _symbolTable.currentMethod.GetToken().Text + 
                                                       " no puede retornar datos de tipo " + typeReturn);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR de Retorno : El metodo el valor de retorno no coincide con algun tipo valido ");
            }
           
            
        }
        return null;
    }

    public override object VisitReadStatementAST(MiniCSharpParser.ReadStatementASTContext context)
    {
        Visit(context.designator());
        return null;
    }

    public override object VisitWriteStatementAST(MiniCSharpParser.WriteStatementASTContext context)
    {
        Visit(context.expr());
        return null;
    }

    public override object VisitBlockStatementAST(MiniCSharpParser.BlockStatementASTContext context)
    {
        Visit(context.block());
        return null;
    }

    public override object VisitBlockCommentStatementAST(MiniCSharpParser.BlockCommentStatementASTContext context)
    {
        return null;
    }

    

    public override object VisitBlockAST(MiniCSharpParser.BlockASTContext context)
    {

        //validar en orden y que traigan el tipo que deben
        foreach (var child in context.children)
        {
           if(child.Equals(context.LBRACE()) || child.Equals(context.RBRACE())) continue;
           
           
              if (child is MiniCSharpParser.StatementContext )
              {
                Visit(child);
              }
              else if (child is MiniCSharpParser.VarDeclASTContext)
              {
                  Visit(child);
              }
              else
              {
                    System.Diagnostics.Debug.WriteLine("Error en el bloque, se esperaba statement o una vardeclaration en VisitBlockAST");
              }

              
        }
        return null;
        
    }

    public override object VisitActParsAST(MiniCSharpParser.ActParsASTContext context)
    {

        LinkedList<Type> tipos = new LinkedList<Type>();
        foreach (var child in context.expr())
        {
            string tipoExpression = (string)Visit(child);
            ;
            Type? tipoTabla = _symbolTable.Search(child.GetText());
            if (tipoTabla != null)
            {
                tipos.AddLast(tipoTabla);
            }
            else
            {
                if (tipoExpression != null)
                    //TODO: revisar que es ese Start
                    tipos.AddLast(new PrimaryType(child.Start, PrimaryType.showType(tipoExpression.ToLower()),
                        _symbolTable.currentLevel));
            }
        }

        return tipos;
    }

    public override object VisitConditionAST(MiniCSharpParser.ConditionASTContext context)
    {   
        foreach (var term in context.condTerm())
        {
            bool conditionType = (bool) Visit(term);
            if (conditionType)
            {
                
                return true;
            }
        }
        Debug.WriteLine("Error: Comparacion invalida, una de las partes dio false "); 
        return false;
    }

    public override object VisitCondTermAST(MiniCSharpParser.CondTermASTContext context)
    {
        foreach (var factor in context.condFact())
        {
            bool conditionType = (bool) Visit(factor);
            if (conditionType == false)
            {
                Debug.WriteLine("Error: No coinciden los tipos en la VisitConditionTermAST " + conditionType); 
                return false;
            }
        }
        return true;

        
    }

    public override object VisitCondFactAST(MiniCSharpParser.CondFactASTContext context)
    {
        string  typeFirstExpression = (string)Visit(context.expr(0));
        Visit(context.relop());
        string  typeSecondExpression = (string) Visit(context.expr(1));
        
        System.Diagnostics.Debug.WriteLine("TIPO DE LA PRIMERA EXPRESION " + typeFirstExpression);
        System.Diagnostics.Debug.WriteLine("TIPO DE LA SEGUNDA EXPRESION " + typeSecondExpression);
        
        if(typeFirstExpression == null || typeSecondExpression == null)
        {
            System.Diagnostics.Debug.WriteLine("Error Condition Factor en el tipo de la condicion, no se puede comparar con null");
            return false;
        }
        
        if(typeFirstExpression == typeSecondExpression)
        {
            return true;
        }
       
        System.Diagnostics.Debug.WriteLine("Error Condition Factor en el tipo de la condicion, no coinciden " + typeFirstExpression + " y " + typeSecondExpression);
        return false;
        
        
    }

    public override object VisitCastAST(MiniCSharpParser.CastASTContext context)
    {
        string type = (string)Visit(context.type());
        if (type == null)
        {
            System.Diagnostics.Debug.WriteLine("Error en el cast, el valor es nulo");
        }
        return type;
    }

    public override object VisitExpressionAST(MiniCSharpParser.ExpressionASTContext context)
    {
        
        
        if (context.cast() != null)
        {
            string type = (string) Visit(context.cast());
            return type;
           
        }

        string tipo = (string) Visit(context.term(0));
        if (tipo == null)
        {
            Console.WriteLine("Error tipo no valido de la expression se encontro null");
            return null;
        }
     
        for (int i = 1; i < context.term().Length; i++)
        {
            string tipoLista = (string) Visit(context.term(i));
            if (tipo != tipoLista)
            {
                Console.WriteLine("Error de tipos, todos los tipos deben ser iguales en la expression");
                return null;
            }
        }
        
        
        return tipo;
        
    }

    public override object VisitTermAST(MiniCSharpParser.TermASTContext context)
    {
        string tipo = (string)Visit(context.factor(0));

        if (context.factor().Length > 1)
        {
            for (int i = 1; i < context.factor().Length; i++)
            {   
               
                string tipoLista = (string) Visit(context.factor(i));
                if(tipo != tipoLista)
                {
                    System.Diagnostics.Debug.WriteLine("Error de tipos, son diferentes en el term");
                    return null;
                }
                
            }
        }
        
        return tipo;
    }

    public override object VisitFactorAST(MiniCSharpParser.FactorASTContext context)
    {
        //TODO: validar que el metodo exista
        //TODO: verificar que la cantidad de parametros sea correcta
        //TODO: verificar que el orden y tipos de los parametros sean correctos
        string tipo = (string)Visit(context.designator());
        if (context.LPARENT() != null)
        {
            LinkedList<Type> tipos = (LinkedList<Type>)Visit(context.actPars());

            Type? metodo = (Type)_symbolTable.Search(context.designator().GetText());
            if (metodo is MethodType method)
            {
                if (((MethodType)metodo).parametersL.Count == tipos.Count)
                {
                    for (int i = 0; i < ((MethodType)metodo).parametersL.Count; i++)
                    {
                        if (((MethodType)metodo).parametersL.ElementAt(i).GetStructureType() !=
                            tipos.ElementAt(i).GetStructureType())
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: TIPO DE PARAMETRO INCORRECTO, se esperaba :" + 
                                                               ((MethodType)metodo).parametersL.ElementAt(i).GetStructureType() +
                                                               ", se obtuvo: " + tipos.ElementAt(i).GetStructureType());
                            return null;
                        }
                    }

                    return ((MethodType)metodo).ReturnTypeGetSet;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: CANTIDAD DE PARAMETROS INCORRECTA");
                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERROR: NO SE ENCONTRO EL METODO");
                return null;
            }
        }
        else
        {
            return tipo;
        }
    }
    

    public override object VisitNumFactorAST(MiniCSharpParser.NumFactorASTContext context)
    {
        return "Int";
    }

    public override object VisitCharFactorAST(MiniCSharpParser.CharFactorASTContext context)
    {
        return "Char";
    }

    public override object VisitStringFactorAST(MiniCSharpParser.StringFactorASTContext context)
    {
        return "String";
    }

    public override object VisitBooleanFactorAST(MiniCSharpParser.BooleanFactorASTContext context)
    {
        return "Boolean";
    }

    public override object VisitNewFactorAST(MiniCSharpParser.NewFactorASTContext context)
    {
        
        string ident = (string) Visit(context.type());
        //Busca en la tabla para ver si es una clase
        Type? tipo = _symbolTable.Search(ident);

        if (tipo != null)
        {
            return tipo.GetStructureType();
        }
        
        //Verifica si es un arreglo de tipo basico (int o char)
        ArrayType.ArrTypes arrType = ArrayType.showType(ident);
        
        if(arrType != ArrayType.ArrTypes.Unknown)
        {
            return arrType.ToString();
        }
        
        System.Diagnostics.Debug.WriteLine("Error de tipos, el tipo del new no existe en tabla simbolos ni es un arreglo de tipo basico");
        return null;
    }

    public override object VisitParenFactorAST(MiniCSharpParser.ParenFactorASTContext context)
    {
        string tipo = (string)Visit(context.expr());
        if(tipo != null) return tipo;
        return null;
    }

    public override object? VisitDesignatorAST(MiniCSharpParser.DesignatorASTContext context)
    {   
        
        Type? typeIdent= _symbolTable.Search(context.ident(0).GetText());
        //TODO validar el tipo de arreglo que sea correcto y q este en el alcance
        if(context.ident().Length > 1) // mas de un id
        {
            Type? tipo1 =  _symbolTable.Search(context.ident(0).GetText());

            
            if (tipo1 != null)
            {
                ClassVarType tipo = (ClassVarType)tipo1;
                ClassType classType = (ClassType)_symbolTable.Search(tipo.GetStructureType());
                if (classType != null)
                {
                    //TODO: CAMBIAR NOMBRE ENTERITO
                    foreach (var enterito in  classType.parametersL)
                    {
                        if (enterito.GetToken().Text.Equals(context.ident(1).GetText()))
                        {
                            // System.Diagnostics.Debug.WriteLine( "El atributo: "+'"'+enterito.GetToken().Text +'"'+ " se encontro en la clase: " + classType.GetToken().Text);
                            return enterito.GetStructureType();
                        }
                    }
                    System.Diagnostics.Debug.WriteLine(" No se encontro la variable"+ context.ident(1).GetText() + "en la clase");

                    return null;
                
                }
            }
           

            System.Diagnostics.Debug.WriteLine(" No se encontro en dicha clase " +
                                               context.ident(context.ident().Length - 2).GetText());
            return null;
        }
        
        
        if (typeIdent is ArrayType && context.expr().Length == 1) //cuando es arreglo
        {
            string typeExpr = (string) Visit(context.expr(0));
            // System.Diagnostics.Debug.WriteLine("El tipo del arreglo es: " + typeIdent.GetStructureType());
            if (typeExpr != null)
            {
                if(typeExpr.Equals("Int"))
                {
                    return typeIdent.GetStructureType().ToLower();
                }
              
                System.Diagnostics.Debug.WriteLine("Error de tipos, el indice del arreglo no es de tipo Int");
               
            }
            return null;

        }
        
        
        
        if (context.ident().Length == 1) // solo hay un id
        {
            //enterito = 1
            if (context.ident(0).GetText().Equals("del"))
            {
                return "Boolean"; //REVISAR
            }
            if (context.ident(0).GetText().Equals("add"))
            {
                return "Boolean";
            }
            if (context.ident(0).GetText().Equals("len"))
            {
                return "Int";
            }
            if(typeIdent is ArrayType)
            {
                return typeIdent.GetStructureType().ToLower()+"[]"; //int[] o char[]
            }

            if (typeIdent!= null)
            {
                return typeIdent.GetStructureType();
            }
            
            
            System.Diagnostics.Debug.WriteLine( " No se encontro en la tabla la variable: " + context.ident(0).GetText() );
            return null;
        }
        return null;
    }

    public override object VisitRelopAST(MiniCSharpParser.RelopASTContext context)
    {
        return null;
    }

    public override Antlr4.Runtime.IToken VisitIdentAST(MiniCSharpParser.IdentASTContext context)
    {
        // System.Diagnostics.Debug.WriteLine("DENTRO ident :" + context.ID().GetText());
        return context.ID().Symbol;
    }

    public override object VisitDoubleFactorAST(MiniCSharpParser.DoubleFactorASTContext context)
    {
       
        return "Double";
    }

    public override object VisitSemicolonStatementAST(MiniCSharpParser.SemicolonStatementASTContext context)
    {
        return null;
    }
}
