
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
    private Consola consola;
    
    public AContextual(Consola consola)
    {
        this.consola = consola;
        _symbolTable = new SymbolTable();
    }
    
    private string ShowToken(IToken token)
    {
        return $" Fila, columna: ({token.Line},{token.Column})";
    }

    
    private void PrintError(IToken tok, String msg)
    {
        Console.WriteLine($"Error en la línea {tok.Line}:{tok.Column} {msg}");
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

        return null;
    }


    public override object VisitUsingAST(MiniCSharpParser.UsingASTContext context)
    {
        return null;
    }

    public override object VisitVarDeclAST(MiniCSharpParser.VarDeclASTContext context)
    {
        IToken currentToken = context.Start;
        //Para validar que el tipo de la variable sea valido
        PrimaryType.PrimaryTypes varType;
        bool isArray = false;
        bool isClassVarType = false;
        bool isError = false;
        

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
                consola.SalidaConsola.AppendText("Error: El tipo de datos del array es incorrecto. Se requiere un tipo válido, como int o char. \n");
                System.Diagnostics.Debug.WriteLine("Error: El tipo de datos del array es incorrecto. Se requiere un tipo válido, como int o char.");
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

                    if (_symbolTable.currentClass != null)
                    {
                        consola.SalidaConsola.AppendText("Error: Solo se permiten variables de tipos básicos dentro de la clase. \n");
                        System.Diagnostics.Debug.WriteLine("Error: Solo se permiten variables de tipos básicos dentro de la clase.");
                    }
                    else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                    {
                        IToken tok = (IToken)Visit(child);
                        Type typeVariable = _symbolTable.Search(token.Text);
                        if (typeVariable!= null && typeVariable.Level <= _symbolTable.currentLevel)
                        {
                            consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. \n");
                            System.Diagnostics.Debug.WriteLine($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente.");
                        }
                        else
                        {
                            if (varType is PrimaryType.PrimaryTypes.Int)
                            {
                                ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int);
                                _symbolTable.Insert(array);
                                // lista.AddFirst(array);
                                
                            }
                            else if (varType is PrimaryType.PrimaryTypes.Char ) //al validar el tipo de la variable, si no es int, es char anteriormente se valido si no era valido isError = true
                            {
                                ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char);
                                _symbolTable.Insert(array);
                                // lista.AddFirst(array);
                            }
                        }
                    }
                    else if (_symbolTable.currentClass == null && _symbolTable.currentMethod == null) //es una variable global
                    {
                        IToken tok = (IToken)Visit(child);
                        Type typeVariable = _symbolTable.Search(token.Text);
                        if (typeVariable!= null )
                        {
                            consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. \n");
                            System.Diagnostics.Debug.WriteLine($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente.");
                        }
                        else
                        {
                            if (varType is PrimaryType.PrimaryTypes.Int)
                            {
                                ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int);
                                _symbolTable.Insert(array);    
                            }
                            else if (varType is PrimaryType.PrimaryTypes.Char ) //al validar el tipo de la variable, si no es int, es char anteriormente se valido si no era valido isError = true
                            {
                                ArrayType array = new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char);
                                _symbolTable.Insert(array);
                                
                            }
                        }
                    }
                }
                else
                {
                    //si la variable es de un tipo de clase
                    if(isClassVarType)
                    {
                        
                        if(_symbolTable.currentClass != null) //si esta dentro en una clase
                        {
                            consola.SalidaConsola.AppendText("Error: Solo se permiten variables de tipos básicos dentro de una clase. \n");
                            System.Diagnostics.Debug.WriteLine("Error: Solo se permiten variables de tipos básicos dentro de una clase.");
                        }
                        else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                        {
                            IToken tok = (IToken)Visit(child);
                            Type typeVariable = _symbolTable.Search(token.Text);
                            if (typeVariable!= null && typeVariable.Level <= _symbolTable.currentLevel)
                            {
                                consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. \n");
                                System.Diagnostics.Debug.WriteLine($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente.");
                            }
                            else
                            {
                                ClassVarType element = new ClassVarType(token, _symbolTable.currentLevel, context.type().GetText());
                                _symbolTable.Insert(element);
                            }
                        }
                        else if (_symbolTable.currentClass == null && _symbolTable.currentMethod == null) //es una variable global
                        {
                            IToken tok = (IToken)Visit(child);
                            Type typeVariable = _symbolTable.Search(token.Text);
                            if (typeVariable!= null )
                            {
                                consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. \n");
                                System.Diagnostics.Debug.WriteLine($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente.");
                            }
                            else
                            {
                                ClassVarType element = new ClassVarType(token, _symbolTable.currentLevel, context.type().GetText());
                                _symbolTable.Insert(element);
                            }
                        }
                        
      
                    }
                    else //es de un tipo primario
                    {
                        PrimaryType element = new PrimaryType(token,varType, _symbolTable.currentLevel);
                        if(_symbolTable.currentClass != null) //si estamos dentro de una clase
                        {

                            if (!_symbolTable.currentClass.BuscarAtributo(element.GetToken().Text))
                            {
                                _symbolTable.Insert(element);
                                _symbolTable.currentClass.parametersL.AddLast(element);
                            }
                            else
                            { 
                                consola.SalidaConsola.AppendText($"Error: La variable \"{token.Text}\" ya ha sido declarada en la clase. \n");
                                System.Diagnostics.Debug.WriteLine($"Error: La variable \"{token.Text}\" ya ha sido declarada en la clase.");
                            }
                           
                            
                        }
                        else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                        {
                            Type variableglobal = _symbolTable.Search(token.Text);
                            if (variableglobal!= null && variableglobal.Level <= _symbolTable.currentLevel)
                            { 
                                consola.SalidaConsola.AppendText($"Error: La variable \"{token.Text}\" ya ha sido declarada como variable local. \n");
                                System.Diagnostics.Debug.WriteLine($"Error: La variable \"{token.Text}\" ya ha sido declarada como variable global.");
                            }
                            else
                            {
                                _symbolTable.Insert(element);
                            }
                        }
                        else if (_symbolTable.currentClass == null && _symbolTable.currentMethod == null) //es una variable global
                        {
                            
                            Type variableglobal = _symbolTable.Search(token.Text);
                            if (variableglobal!= null )
                            {
                                consola.SalidaConsola.AppendText($"Error: La variable \"{token.Text}\" ya ha sido declarada como variable global. \n");
                                System.Diagnostics.Debug.WriteLine($"Error: El identificador \"{token.Text}\" ya existe y es de tipo \"{variableglobal.GetStructureType()}\".");
                            }
                            else
                            {
                                _symbolTable.Insert(element);
                            }
                        }

                    }

                }
            }  
            
        }

        else
        {
            System.Diagnostics.Debug.WriteLine("Error: El tipo de declaración de la variable no es válido.");
        }

        return null;
    }

    public override object VisitClassDeclAST(MiniCSharpParser.ClassDeclASTContext context)
    {
        IToken currentToken = context.Start;

        if (_symbolTable.Search(context.ident().GetText()) != null)
        {
            consola.SalidaConsola.AppendText($"Error: La clase \"{context.ident().GetText()}\" ya ha sido declarada anteriormente. \n");
            System.Diagnostics.Debug.WriteLine($"Error: La clase \"{context.ident().GetText()}\" ya ha sido declarada anteriormente.");

            return null;
        }
        
        ClassType classDcl = new ClassType((IToken)Visit(context.ident()), _symbolTable.currentLevel);
        _symbolTable.Insert(classDcl);
        _symbolTable.currentClass = classDcl; //saber en la clase actual sobre la que estoy trabajando
        _symbolTable.OpenScope();
        if(context.varDecl()!= null)
        {
           
            foreach (var child in context.varDecl())
            {
                Visit(child);
            }
        }
       
        _symbolTable.CloseScope();
        _symbolTable.currentClass = null; //volvemos null la clase actual
        
        return null;
    }

    public override object VisitMethodDeclAST(MiniCSharpParser.MethodDeclASTContext context)
    {
        IToken currentToken = context.Start;  
        
        IToken token = (IToken)Visit(context.ident());
        string methodName = token.Text;
        Type? existingType = _symbolTable.Search(methodName);

        if (existingType != null && existingType is MethodType)
        {
            consola.SalidaConsola.AppendText($"Error: La declaración del método \"{methodName}\" ya existe en el contexto actual\n");
            System.Diagnostics.Debug.WriteLine($"Error: La declaración del método \"{methodName}\" ya existe en el contexto actual.");

            return null;
        }

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
                    consola.SalidaConsola.AppendText("Error: El tipo del arreglo debe ser \"int\" o \"char\", pero se encontró un tipo no válido. \n");
                    System.Diagnostics.Debug.WriteLine("Error: El tipo del arreglo debe ser \"int\" o \"char\", pero se encontró un tipo no válido.");
                    isError = true;
                }
            
            }
            else
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
                _symbolTable.OpenScope();
                parameters = (LinkedList<Type>)Visit(context.formPars());
                _symbolTable.CloseScope();
            
            }
            MethodType method;
            if (context.type() != null)
            {
                if(isArray)
                {
                    method = new MethodType(token, _symbolTable.currentLevel, parameters.Count, methodType.ToString() + "[]", parameters);
                    _symbolTable.Insert(method);
                    _symbolTable.currentMethod = method;
                }
                else
                {
                    method = new MethodType(token, _symbolTable.currentLevel, parameters.Count, methodType.ToString() , parameters);
                    _symbolTable.Insert(method);
                    _symbolTable.currentMethod = method;
                    
                }
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
            _symbolTable.OpenScope();
            Visit(context.block());
            _symbolTable.CloseScope();
        
            _symbolTable.DeleteParametersBody(token.Text);

        }
        else
        {
            consola.SalidaConsola.AppendText("El tipo del metodo no es valido");
            System.Diagnostics.Debug.WriteLine("El tipo del metodo no es valido");
        }

        _symbolTable.currentMethod = null;
        
        return null;
    }

    public override object VisitFormParsAST(MiniCSharpParser.FormParsASTContext context)
    {
        IToken currentToken = context.Start;  
        
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
                }
                else if (varType is PrimaryType.PrimaryTypes.Char)
                {
                    parameters.AddLast(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                }
                else
                {
                    consola.SalidaConsola.AppendText("Error: El tipo del arreglo debe ser int o char. El tipo actual no es válido. \n");
                    System.Diagnostics.Debug.WriteLine("Error: El tipo del arreglo debe ser int o char. El tipo actual no es válido.");
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
                    consola.SalidaConsola.AppendText($"Error: El tipo de parámetro \"{token.Text}\" no es válido. \n");
                    System.Diagnostics.Debug.WriteLine($"Error: El tipo de parámetro \"{token.Text}\" no es válido.");
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
        IToken currentToken = context.Start;   
    
        string tipoDesignator = (string)Visit(context.designator());
        if(context.expr()!=null)// si es una asignacion 
        {
             
            string tipoExpresion = ((string)Visit(context.expr())); //tolower para que no haya problemas con mayusculas y minusculas
            
            //verificamos si el tipodesignator viene nulo es decir el indice no es null

            if (tipoDesignator != null && tipoExpresion != null)
            {
                tipoDesignator = tipoDesignator.ToLower();//tolower para que no haya problemas con mayusculas y minusculas
                if (tipoDesignator.Contains("[]") && context.expr().GetText().Contains("new"))
                {
          
                    if (!(tipoDesignator.ToLower().Contains(tipoExpresion.ToLower())))
                    {
                        consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{tipoDesignator}\" no coincide con el tipo de la expresión \"{tipoExpresion.ToLower()}\". \n");
                        System.Diagnostics.Debug.WriteLine($"Error de asignación: El tipo \"{tipoDesignator}\" no coincide con el tipo de la expresión \"{tipoExpresion.ToLower()}\".");
                    }
                    else
                    {
                        // System.Diagnostics.Debug.WriteLine("Asignacion correcta: " + tipoDesignator + " es el mismo que el tipo de la expresion: " + tipoExpresion);
                    }

                    return null;
                }
            
                if (tipoDesignator != tipoExpresion)
                {
                    consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{tipoDesignator}\" no es compatible con el tipo de la expresión \"{tipoExpresion}\". \n");
                    System.Diagnostics.Debug.WriteLine($"Error de asignación: El tipo \"{tipoDesignator}\" no es compatible con el tipo de la expresión \"{tipoExpresion}\".");

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
                consola.SalidaConsola.AppendText("Error en la asignación: Los tipos del designador y de la expresión no son válidos. \n");
                System.Diagnostics.Debug.WriteLine("Error en la asignación: Los tipos del designador y de la expresión no son válidos.");

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
                            consola.SalidaConsola.AppendText("Error de parámetros: Los tipos de parámetros no coinciden en el método del. \n");
                            System.Diagnostics.Debug.WriteLine("Error de parámetros: Los tipos de parámetros no coinciden en el método del.");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        consola.SalidaConsola.AppendText("Error en los parámetros: Cantidad incorrecta de parámetros para el método del. \n");
                        System.Diagnostics.Debug.WriteLine("Error en los parámetros: Cantidad incorrecta de parámetros para el método del.");
                    }

                }
                else
                {
                    consola.SalidaConsola.AppendText("Error en el método: Faltan los parámetros para el método del. \n");
                    System.Diagnostics.Debug.WriteLine("Error en el método: Faltan los parámetros para el método del.");
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
                            consola.SalidaConsola.AppendText("Error de parámetros: Los tipos de parámetros no coinciden en el método len. \n");
                            System.Diagnostics.Debug.WriteLine("Error de parámetros: Los tipos de parámetros no coinciden en el método len.");
                        }
                    }
                    else
                    {
                        consola.SalidaConsola.AppendText("Error en los parámetros: Cantidad incorrecta de parámetros para el método len. \n");
                        System.Diagnostics.Debug.WriteLine("Error en los parámetros: Cantidad incorrecta de parámetros para el método len.");
                    }
                    
                }
                else
                { 
                    consola.SalidaConsola.AppendText("Error en el método: Faltan los parámetros para el método len. \n");
                    System.Diagnostics.Debug.WriteLine("Error en el método: Faltan los parámetros para el método len.");
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
                            consola.SalidaConsola.AppendText("Error de parámetros: Los tipos de parámetros no coinciden en el método add. \n");
                            System.Diagnostics.Debug.WriteLine("Error de parámetros: Los tipos de parámetros no coinciden en el método add.");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        consola.SalidaConsola.AppendText("Error en los parámetros: Cantidad incorrecta de parámetros para el método add. \n");
                        System.Diagnostics.Debug.WriteLine("Error en los parámetros: Cantidad incorrecta de parámetros para el método add.");
                    }
                }
                else
                {
                    consola.SalidaConsola.AppendText("Error en el método: Faltan los parámetros para el método add. \n");
                    System.Diagnostics.Debug.WriteLine("Error en el método: Faltan los parámetros para el método add.");
                }
                
            }
           
            else if (type is MethodType method)
            {
                
                //System.Diagnostics.Debug.WriteLine("Es un metodo" + method.GetToken().Text);
                
                
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
                                consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} no coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\". \n");
                                System.Diagnostics.Debug.WriteLine($"Error de asignación: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} no coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\".");

                            }
                            else
                            {
                                consola.SalidaConsola.AppendText($"Asignación correcta: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\". \n");
                                System.Diagnostics.Debug.WriteLine($"Asignación correcta: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\".");
                            }
                        }
                    }
                    else
                    {
                        consola.SalidaConsola.AppendText($"Error de asignación: El número de parámetros ({method.parametersL.Count}) no coincide con el número de expresiones ({parametros.Count}). \n");
                        System.Diagnostics.Debug.WriteLine($"Error de asignación: El número de parámetros ({method.parametersL.Count}) no coincide con el número de expresiones ({parametros.Count}).");
                    }
                }
                else if (method.parametersL.Count > 0)
                {
                    consola.SalidaConsola.AppendText($"Error de parámetros: Faltan parámetros en el método \"{method.GetToken().Text}\". \n");
                    System.Diagnostics.Debug.WriteLine($"Error de parámetros: Faltan parámetros en el método \"{method.GetToken().Text}\".");
                }
            }
            else
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" no es un método. \n");
                System.Diagnostics.Debug.WriteLine($"Error de asignación: \"{context.designator().GetText()}\" no es un método.");
            }
            
           
        }
        else if (context.INC()!=null)
        {
            if(tipoDesignator.ToLower() != "int")
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" solo se puede incrementar utilizando el operador ++ en enteros. \n");
                System.Diagnostics.Debug.WriteLine($"Error de asignación: \"{context.designator().GetText()}\" solo se puede incrementar utilizando el operador ++ en enteros.");
            }
            
        }
        else if (context.DEC()!=null)
        {
            if(tipoDesignator.ToLower() != "int")
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" solo se puede decrementar utilizando el operador -- en enteros. \n");
                System.Diagnostics.Debug.WriteLine($"Error de asignación: \"{context.designator().GetText()}\" solo se puede decrementar utilizando el operador -- en enteros.");
            }
            
        }
        return null;
    }
    

    public override object VisitIfStatementAST(MiniCSharpParser.IfStatementASTContext context)
    {
        IToken currentToken = context.Start;
        
        _symbolTable.OpenScope();
        bool conditionValue = (bool)Visit(context.condition());
        if (conditionValue)
        {
            Visit(context.statement(0));
        }
        else
        {
            consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del if es falsa: \"{context.condition().GetText()}\". \n");
            System.Diagnostics.Debug.WriteLine($"ERROR: El tipo en la condición del if es falsa: \"{context.condition().GetText()}\".");
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
        IToken currentToken = context.Start;
        
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
                consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del for es falsa: \"{context.condition().GetText()}\". \n");
                System.Diagnostics.Debug.WriteLine($"ERROR: El tipo en la condición del for es falsa: \"{context.condition().GetText()}\".");

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
        IToken currentToken = context.Start;
        
        _symbolTable.OpenScope();
        bool conditionValue = (bool)Visit(context.condition());
        if (conditionValue)
        {
            Visit(context.statement());
        }
        else
        {
            consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del while es falsa: \"{context.condition().GetText()}\". \n");
            System.Diagnostics.Debug.WriteLine($"ERROR: El tipo en la condición del while es falsa: \"{context.condition().GetText()}\".");
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
        IToken token = context.Start;

        if (context.expr() != null)
        {
            string returnType = (string)Visit(context.expr());

            if (_symbolTable.currentMethod.ReturnTypeGetSet == "void")
            {
                consola.SalidaConsola.AppendText($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" es de tipo void y no puede tener un valor de retorno. \n");
                System.Diagnostics.Debug.WriteLine($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" es de tipo void y no puede tener un valor de retorno.");
            }
            else if (!IsReturnTypeValid(returnType))
            {
                consola.SalidaConsola.AppendText($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" no puede retornar un valor de tipo \"{returnType}\". \n");
                System.Diagnostics.Debug.WriteLine($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" no puede retornar un valor de tipo \"{returnType}\".");
            }
        }
        else
        {
            consola.SalidaConsola.AppendText("ERROR de Retorno: El método no tiene un valor de retorno válido. \n");
            System.Diagnostics.Debug.WriteLine("ERROR de Retorno: El método no tiene un valor de retorno válido.");
        }

        return null;
    }

    private bool IsReturnTypeValid(string returnType)
    {
        if (returnType == null)
        {
            return false;
        }

        return string.Equals(returnType, _symbolTable.currentMethod.ReturnTypeGetSet, StringComparison.OrdinalIgnoreCase);
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
        IToken currentToken = context.Start;
        
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
                consola.SalidaConsola.AppendText("Error en el bloque: Se esperaba una declaración de variable o una sentencia en VisitBlockAST. \n");
                System.Diagnostics.Debug.WriteLine("Error en el bloque: Se esperaba una declaración de variable o una sentencia en VisitBlockAST.");
              }

              
        }
        
        return null;
    }

    public override object VisitActParsAST(MiniCSharpParser.ActParsASTContext context)
    {
        LinkedList<Type> parametersTypes = new LinkedList<Type>();
        
        foreach (var child in context.expr())
        {
            string expressionType = (string)Visit(child);
            Type? tableType = _symbolTable.Search(child.GetText());
            
            if (expressionType != null)
            {
                parametersTypes.AddLast(new PrimaryType(child.Start, PrimaryType.showType(expressionType.ToLower()),
                    _symbolTable.currentLevel));
            }
            else if (tableType != null)
            {
                parametersTypes.AddLast(tableType);
            }
        }

        return parametersTypes;
    }
    
    public override object VisitConditionAST(MiniCSharpParser.ConditionASTContext context)
    {
        IToken token = context.Start;
        bool hasValidCondition = false;

        foreach (var term in context.condTerm())
        {
            bool conditionType = (bool)Visit(term);
            if (conditionType)
            {
                hasValidCondition = true;
                break;
            }
        }

        if (hasValidCondition)
        {
            return true;
        }
        else
        {
            consola.SalidaConsola.AppendText("Error: Los tipos que se están comparando no son compatibles. \n");
            System.Diagnostics.Debug.WriteLine("Error: Los tipos que se están comparando no son compatibles.");
            
            return false;
        }
    }

    public override object VisitCondTermAST(MiniCSharpParser.CondTermASTContext context)
    {
        IToken currentToken = context.Start;
        bool allConditionsTrue = true;

        foreach (var factor in context.condFact())
        {
            bool conditionType = (bool)Visit(factor);
            if (!conditionType)
            {
                consola.SalidaConsola.AppendText($"Error: Los tipos no coinciden en VisitConditionTermAST: {conditionType} \n");
                System.Diagnostics.Debug.WriteLine($"Error: Los tipos no coinciden en VisitConditionTermAST: {conditionType}");
                return false;
            }
        }

        return allConditionsTrue;
    }

    public override object VisitCondFactAST(MiniCSharpParser.CondFactASTContext context)
    {
        IToken token = context.Start;
        string firstExprType = (string)Visit(context.expr(0));
        Visit(context.relop());
        string secondExprType = (string)Visit(context.expr(1));

        if (IsComparisonValid(firstExprType, secondExprType))
        {
            return true;
        }
        else
        {
            consola.SalidaConsola.AppendText(GetErrorComparisonMessage(firstExprType, secondExprType));
            System.Diagnostics.Debug.WriteLine(GetErrorComparisonMessage(firstExprType, secondExprType));
            
            return false;
        }
    }

    private bool IsComparisonValid(string firstType, string secondType)
    {
        if (firstType == null || secondType == null)
        {
            return false;
        }

        return firstType == secondType;
    }

    private string GetErrorComparisonMessage(string expectedType, string actualType)
    {
        if (expectedType == null || actualType == null)
        {
            return "Error en el Factor de Condición: No se puede comparar el tipo de condición con null.";
        }

        return $"Error en el Factor de Condición: Los tipos de condición no coinciden. Se esperaba {expectedType} pero se encontró {actualType}.";
    }

    public override object VisitCastAST(MiniCSharpParser.CastASTContext context)
    {
        IToken currentToken = context.Start;
        
        string type = (string)Visit(context.type());
        if (type == null)
        {
            consola.SalidaConsola.AppendText("Error en el cast: El valor a castear es nulo. \n");
            System.Diagnostics.Debug.WriteLine("Error en el cast: El valor a castear es nulo.");
        }
        return type;
    }

    public override object VisitExpressionAST(MiniCSharpParser.ExpressionASTContext context)
    {
        IToken currentToken = context.Start;
        
        if (context.cast() != null)
        {
            string castType = (string) Visit(context.cast());
            return castType;
        }

        string termType = (string) Visit(context.term(0));
        if (termType == null)
        {
            consola.SalidaConsola.AppendText("Error: Tipo no válido de la expresión. Se encontró null. \n");
            System.Diagnostics.Debug.WriteLine("Error: Tipo no válido de la expresión. Se encontró null.");

            return null;
        }
     
        for (int i = 1; i < context.term().Length; i++)
        {
            string currentTerm = (string) Visit(context.term(i));
            if (termType != currentTerm)
            {
                consola.SalidaConsola.AppendText("Error de tipos: Todos los tipos en la expresión deben ser iguales. \n");
                System.Diagnostics.Debug.WriteLine("Error de tipos: Todos los tipos en la expresión deben ser iguales.");

                return null;
            }
        }
        
        return termType;
    }
    
    public override object VisitTermAST(MiniCSharpParser.TermASTContext context)
    {
        IToken currentToken = context.Start;
    
        string factorType = (string)Visit(context.factor(0));
    
        if (context.factor().Length > 1)
        {
            int i = 1;
            while (i < context.factor().Length && factorType == (string)Visit(context.factor(i)))
            {
                i++;
            }
    
            if (i < context.factor().Length)
            {
                consola.SalidaConsola.AppendText("Error de tipos: Los tipos son diferentes en term. \n");
                System.Diagnostics.Debug.WriteLine("Error de tipos: Los tipos son diferentes en term.");
    
                return null;
            }
        }
    
        return factorType;
    }

    public override object VisitFactorAST(MiniCSharpParser.FactorASTContext context)
    {
        IToken currentToken = context.Start;
        string designatorType = (string)Visit(context.designator());
        
        if (context.LPARENT() != null)
        {
            LinkedList<Type> tipos = (LinkedList<Type>)Visit(context.actPars());

            Type? metodo = (Type)_symbolTable.Search(context.designator().GetText());
            if (metodo is MethodType method)
            {

                if (!(method.parametersL.Count == tipos.Count))
                {
                    consola.SalidaConsola.AppendText("Error: Cantidad de parámetros incorrecta");
                    System.Diagnostics.Debug.WriteLine("Error: Cantidad de parámetros incorrecta");

                    return null;
                }
                else
                {
                    for (int i = 0; i < method.parametersL.Count; i++)
                    {
                        if (((MethodType)metodo).parametersL.ElementAt(i).GetStructureType() !=
                            tipos.ElementAt(i).GetStructureType())
                        {
                            consola.SalidaConsola.AppendText($"Error: Tipo de parámetro incorrecto. Se esperaba: {((MethodType)metodo).parametersL.ElementAt(i).GetStructureType()}, se obtuvo: {tipos.ElementAt(i).GetStructureType()} \n");
                            System.Diagnostics.Debug.WriteLine($"Error: Tipo de parámetro incorrecto. Se esperaba: {((MethodType)metodo).parametersL.ElementAt(i).GetStructureType()}, se obtuvo: {tipos.ElementAt(i).GetStructureType()}");

                            return null;
                        }
                    }

                    return method.ReturnTypeGetSet;
                }
            }
            else
            {
                consola.SalidaConsola.AppendText("Error: No se encontró el método\n");
                System.Diagnostics.Debug.WriteLine("Error: No se encontró el método");

                return null;
            }
        }
        else
        {
            return designatorType;
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
        IToken currentToken = context.Start;
        
        string ident = (string) Visit(context.type());
        //Busca en la tabla para ver si es una clase
        Type? classType = _symbolTable.Search(ident);

        if (classType != null)
        {
            return classType.GetStructureType();
        }
        
        //Verifica si es un arreglo de tipo basico (int o char)
        ArrayType.ArrTypes arrType = ArrayType.showType(ident);
        
        if(arrType != ArrayType.ArrTypes.Unknown)
        {
            return arrType.ToString();
        }
        
        consola.SalidaConsola.AppendText($"Error de tipos: El tipo del 'new' no existe en la tabla de símbolos ni es un arreglo de tipo básico\n");
        System.Diagnostics.Debug.WriteLine("Error de tipos: El tipo del 'new' no existe en la tabla de símbolos ni es un arreglo de tipo básico");

        return null;
    }

    public override object VisitParenFactorAST(MiniCSharpParser.ParenFactorASTContext context)
    {
        string expressionType = (string)Visit(context.expr());
        if(expressionType != null)
        {
            return expressionType;
        }
        
        return null;
    }

    public override object? VisitDesignatorAST(MiniCSharpParser.DesignatorASTContext context)
    {   
        IToken currentToken = context.Start;
        
        Type? typeIdent= _symbolTable.Search(context.ident(0).GetText());
        //TODO validar el tipo de arreglo que sea correcto y q este en el alcance
        if(context.ident().Length > 1) // mas de un id
        {

            Type? tipo1 = _symbolTable.BuscarCustomVar(context.ident(0).GetText());       
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
                    
                    consola.SalidaConsola.AppendText($"No se encontró la variable '{context.ident(1).GetText()}' en la clase");
                    System.Diagnostics.Debug.WriteLine($"No se encontró la variable '{context.ident(1).GetText()}' en la clase");
                    
                    return null;
                
                }
            }
           
            consola.SalidaConsola.AppendText($"No se encontró en dicha clase '{context.ident(context.ident().Length - 2).GetText()}'");
            System.Diagnostics.Debug.WriteLine($"No se encontró en dicha clase '{context.ident(context.ident().Length - 2).GetText()}'");

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
              
                consola.SalidaConsola.AppendText($"Error de tipos: El índice del arreglo no es de tipo Int");
                System.Diagnostics.Debug.WriteLine("Error de tipos: El índice del arreglo no es de tipo Int");
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

            consola.SalidaConsola.AppendText($"No se encontró en la tabla la variable: {context.ident(0).GetText()}\n");
            System.Diagnostics.Debug.WriteLine($"No se encontró en la tabla la variable: {context.ident(0).GetText()}");

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
