
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        System.Diagnostics.Debug.WriteLine("DENTRO using :" + context.ident().GetText());
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
            foreach (var attri in classDcl.parametersL)
            {
                System.Diagnostics.Debug.WriteLine("Atributo de la clase " + classDcl.GetToken().Text + " : " + attri.GetToken().Text + " tipo: " + attri.GetStructureType() + " nivel: " + attri.Level + "");
            }
            
        }
       
        _symbolTable.CloseScope();
        return null;
    }

    public override object VisitMethodDeclAST(MiniCSharpParser.MethodDeclASTContext context)
    {
        _symbolTable.OpenScope();
        System.Diagnostics.Debug.WriteLine("DENTRO methodDecl :" + context.ident().GetText());
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
            }
            else
            {
                method = new MethodType(token, _symbolTable.currentLevel, parameters.Count, "Void", parameters);
                _symbolTable.Insert(method);
            }
        
            if (context.block() != null)
            {
                Visit(context.block());
            }

            //TODO: REVISAR me esta eliminando la variable x que esta dentro de la clase de la tabla porque hay otro parametro que tambien se llama x y seguro el scope es nivel 1 tamb
            foreach (var pars in parameters)
            {
                _symbolTable.Sacar(pars.GetToken().Text);
            }


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
                    _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int));
                }
                else if (varType is PrimaryType.PrimaryTypes.Char)
                {
                    parameters.AddLast(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                    //los agrega a la tabla
                    _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
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
                    _symbolTable.Insert(new ClassVarType(token, _symbolTable.currentLevel, type));
                    
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
                    _symbolTable.Insert(new PrimaryType(token, varType, _symbolTable.currentLevel));
                }
            }
            
        }
        return parameters;
    }

    //TODO: Revsar el visit de type
    public override object VisitTypeAST(MiniCSharpParser.TypeASTContext context)
    {
        System.Diagnostics.Debug.WriteLine("Es un array :" + context.GetText());
        IToken type = (IToken)Visit(context.ident());

        //se retorna el identificador del tipo
        return type.Text;

    }

    public override object VisitAssignStatementAST(MiniCSharpParser.AssignStatementASTContext context)
    {
        string tipoDesignator = (string)Visit(context.designator());
        System.Diagnostics.Debug.WriteLine("visita assignstatement");
        if(context.expr()!=null) // si es una asignacion 
        {
            string tipoExpresion = (string)Visit(context.expr());
            if (tipoDesignator != tipoExpresion)
            {
                System.Diagnostics.Debug.WriteLine("Error de asignacion: " + tipoDesignator + " no es el mismo que el tipo de la expresion: " + tipoExpresion);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Asignacion correcta: " + tipoDesignator + " es el mismo que el tipo de la expresion: " + tipoExpresion);
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
                    

                    foreach (var pars in parametros)
                    {
                        System.Diagnostics.Debug.WriteLine("Parametros DEL: " + pars.GetToken().Text);
                    }
                    if (parametros.Count == 2)
                    {
                        if (parametros.ElementAt(0) is ArrayType && parametros.ElementAt(1).GetStructureType().Equals("Int"))
                        {
                            System.Diagnostics.Debug.WriteLine("Todo bien, todo correcto en el del");
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
                
            }
            else if (context.designator().GetText() == "add")
            {
                if (context.actPars() != null)
                {
                    //obtener lista de parametros
                    LinkedList<Type> parametros = (LinkedList<Type>)Visit(context.actPars());
                    //Recibe dos parametros, la lista, el indice
                    System.Diagnostics.Debug.WriteLine("Cuenta de parametros ADD: " + parametros.Count);
                    foreach (var VARIABLE in parametros)
                    {
                        System.Diagnostics.Debug.WriteLine("Parametros ADD: " + VARIABLE.GetToken().Text);
                    }
                    
                    if (parametros.Count == 2)
                    {
                        if (parametros.ElementAt(0).GetStructureType().Equals(parametros.ElementAt(1).GetStructureType()))
                        {
                            System.Diagnostics.Debug.WriteLine("Todo bien, todo correcto en el add");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Error de parametros, tipos de parametros no coinciden en el del");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        //TODO: verificar orden de los parametros que pueden tener orden diferente
                       
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
                                System.Diagnostics.Debug.WriteLine("Asignacion correcta: " +
                                                                   method.parametersL.ElementAt(i).GetStructureType()
                                                                       .ToString() +
                                                                   " es el mismo que el tipo de la expresion: " +
                                                                   parametros.ElementAt(i));
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
        Visit(context.condition());
        Visit(context.statement(0));
        if (context.statement(1)!= null)
        {
            _symbolTable.OpenScope();
            Visit(context.statement(1));
            _symbolTable.CloseScope();
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
            Visit(context.condition());
        }

        if (context.statement().Length > 1)
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
        Visit(context.condition());
        Visit(context.statement());
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
            Visit(context.expr());
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
        Visit(context.condTerm(0));
        if (context.condTerm().Length > 1)
        {
            for (int i = 1; i < context.condTerm().Length; i++)
            {
                Visit(context.condTerm(i));
            }
        }
        return null;
    }

    public override object VisitCondTermAST(MiniCSharpParser.CondTermASTContext context)
    {
        Visit(context.condFact(0));
        if (context.condFact().Length > 1)
        {
            for (int i = 1; i < context.condFact().Length; i++)
            {
                Visit(context.condFact(i));
            }
        }
        return null;
    }

    public override object VisitCondFactAST(MiniCSharpParser.CondFactASTContext context)
    {
        Visit(context.expr(0));
        Visit(context.relop());
        Visit(context.expr(1));
        return null;
    }

    public override object VisitCastAST(MiniCSharpParser.CastASTContext context)
    {
        Visit(context.type());
        return null;
    }

    public override object VisitExpressionAST(MiniCSharpParser.ExpressionASTContext context)
    {
        
        //verificar mullop porque no se visita esta directo
        
        
        if (context.cast() != null)
        {
            Visit(context.cast());
        }

        
        string tipo = (string) Visit(context.term(0));
        if (tipo == null)
        {
            Console.WriteLine("Error tipo no valido de la expression");
            return null;
        }
        if (context.term().Length > 1)
        {
            for (int i = 1; i < context.term().Length; i++)
            {
                //verificar despues lo del addop porque no se visita
                string tipoLista = (string) Visit(context.term(i));
                if (tipo != tipoLista)
                {
                    Console.WriteLine("Error de tipos, todos los tipos deben ser iguales en la expression");
                    return null;
                }
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
            if (metodo is MethodType)
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
        //TODO: METER VALIDACION DE QUE LA CLASE EXISTA EN LA TABLA DE SIMBOLOS
        string ident = (string) Visit(context.type());
        Type? tipo = _symbolTable.Search(ident);

        if (tipo != null)
        {
            return tipo.GetStructureType();
        }
        System.Diagnostics.Debug.WriteLine("Error de tipos, error clase: "+ '"'+ident +'"'+" no encontrada en la declaracion new de instancia de clase");
        return null;
    }

    public override object VisitParenFactorAST(MiniCSharpParser.ParenFactorASTContext context)
    {
        string tipo = (string)Visit(context.expr());
        if(tipo != null) return tipo;
        return null;
    }

    public override object VisitDesignatorAST(MiniCSharpParser.DesignatorASTContext context)
    {   
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
                    
                
                    foreach (var enterito in  classType.parametersL)
                    {
                        //perro.gato
                        if (enterito.GetToken().Text.Equals(context.ident(1).GetText()))
                        {
                            System.Diagnostics.Debug.WriteLine( "El atributo: "+'"'+enterito.GetToken().Text +'"'+ " se encontro en la clase: " + classType.GetToken().Text);
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
        
        if (context.expr() != null)
        {

            //TODO: validar el tipo de la visita a expression
            // Visit(context.expr(0));
            Type tipo = _symbolTable.Search(context.ident(0).GetText());
            if (tipo != null )
            {
                return tipo.GetStructureType();
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

            Type tipo = _symbolTable.Search(context.ident(0).GetText());
            System.Diagnostics.Debug.WriteLine(context.ident(0).GetText()+ " DESIGNATOR");
            if (tipo != null)
            {
                return tipo.GetStructureType();
            }
            System.Diagnostics.Debug.WriteLine( " No se encontro en la tabla: " + context.ident(0).GetText() );
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
        System.Diagnostics.Debug.WriteLine("DENTRO ident :" + context.ID().GetText());
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
