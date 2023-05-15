
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
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
        return $"[Línea: {token.Line}, Columna: {token.Column}]";
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
            // Abrimos un nuevo ámbito para el programa
            _symbolTable.OpenScope();
        
            // Visitamos el identificador del programa y creamos un objeto ClassType para representarlo
            IToken token = (IToken)Visit(context.ident());
            ClassType classType = new ClassType(token, _symbolTable.currentLevel);
        
            // Insertamos la clase principal en la tabla de símbolos
            _symbolTable.Insert(classType);
        
            // Visitamos todos los hijos del programa
            foreach (var child in context.children)
            {
                Visit(child);
            }
        
            // Cerramos el ámbito del programa
            _symbolTable.CloseScope();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    
        // Imprimimos la tabla de símbolos (opcional, puedes eliminar esta línea si no es necesaria)
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
                consola.SalidaConsola.AppendText($"Error: El tipo de datos del array es incorrecto. Se requiere un tipo válido, como int o char. {ShowToken(currentToken)} \n");
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
                        consola.SalidaConsola.AppendText($"Error: Solo se permiten variables de tipos básicos dentro de la clase. {ShowToken(currentToken)} \n");
                    }
                    else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                    {
                        IToken tok = (IToken)Visit(child);
                        Type typeVariable = _symbolTable.Search(token.Text);
                        if (typeVariable!= null && typeVariable.Level <= _symbolTable.currentLevel)
                        {
                            consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. {ShowToken(currentToken)}\n");
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
                            consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. {ShowToken(currentToken)}\n");
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
                            consola.SalidaConsola.AppendText($"Error: Solo se permiten variables de tipos básicos dentro de una clase. {ShowToken(currentToken)}\n");
                        }
                        else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                        {
                            IToken tok = (IToken)Visit(child);
                            Type typeVariable = _symbolTable.Search(token.Text);
                            if (typeVariable!= null && typeVariable.Level <= _symbolTable.currentLevel)
                            {
                                consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. {ShowToken(currentToken)}\n");
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
                                consola.SalidaConsola.AppendText($"Error: La variable \"{tok.Text}\" ya ha sido declarada previamente. {ShowToken(currentToken)}\n");
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
                                consola.SalidaConsola.AppendText($"Error: La variable \"{token.Text}\" ya ha sido declarada en la clase. {ShowToken(currentToken)}\n");
                            }
                           
                            
                        }
                        else if(_symbolTable.currentMethod!= null) //es una variable local dentro de un metodo
                        {
                            Type variableglobal = _symbolTable.Search(token.Text);
                            if (variableglobal!= null && variableglobal.Level <= _symbolTable.currentLevel)
                            { 
                                consola.SalidaConsola.AppendText($"Error: La variable \"{token.Text}\" ya ha sido declarada como variable local. {ShowToken(currentToken)}\n");
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
                                consola.SalidaConsola.AppendText($"Error: El identificador \"{token.Text}\" ya existe y es de tipo \"{variableglobal.GetStructureType()}\". {ShowToken(currentToken)}\n");
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
            consola.SalidaConsola.AppendText($"Error: El tipo de declaración de la variable no es válido. {ShowToken(currentToken)} \n");
        }

        return null;
    }

    public override object VisitClassDeclAST(MiniCSharpParser.ClassDeclASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;

        // Verificamos si la clase ya ha sido declarada anteriormente
        if (_symbolTable.Search(context.ident().GetText()) != null)
        {
            consola.SalidaConsola.AppendText($"Error: La clase \"{context.ident().GetText()}\" ya ha sido declarada anteriormente. {ShowToken(currentToken)}\n");

            return null;
        }
    
        // Creamos un objeto ClassType para representar la clase actual
        ClassType classDcl = new ClassType((IToken)Visit(context.ident()), _symbolTable.currentLevel);
    
        // Insertamos la clase en la tabla de símbolos
        _symbolTable.Insert(classDcl);
    
        // Establecemos la clase actual en la tabla de símbolos
        _symbolTable.currentClass = classDcl; //saber en la clase actual sobre la que estoy trabajando
    
        // Abrimos un nuevo ámbito para la clase
        _symbolTable.OpenScope();
    
        // Visitamos las declaraciones de variables de la clase, si las hay
        if(context.varDecl()!= null)
        {
            foreach (var child in context.varDecl())
            {
                Visit(child);
            }
        }
   
        // Cerramos el ámbito de la clase
        _symbolTable.CloseScope();
    
        // Volvemos nula la clase actual en la tabla de símbolos
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
            consola.SalidaConsola.AppendText($"Error: La declaración del método \"{methodName}\" ya existe en el contexto actual. {ShowToken(currentToken)}\n");

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
                    consola.SalidaConsola.AppendText($"Error: El tipo del arreglo debe ser \"int\" o \"char\", pero se encontró un tipo no válido. {ShowToken(currentToken)}\n");
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
            consola.SalidaConsola.AppendText($"El tipo del metodo no es valido. {ShowToken(currentToken)} \n");
        }

        _symbolTable.currentMethod = null;
        
        return null;
    }

    public override object VisitFormParsAST(MiniCSharpParser.FormParsASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;  
        
        // Creamos una lista enlazada para almacenar los parámetros
        LinkedList<Type> parameters = new LinkedList<Type>();
        
        // Iteramos sobre los identificadores y tipos de los parámetros
        for (int i = 0; i < context.ident().Length; i++)
        {
            // Obtenemos el token del identificador
            IToken token = (IToken)Visit(context.ident(i));
            
            // Obtenemos el tipo del parámetro como cadena
            string type = context.type(i).GetText();
            
            // Verificamos si es un array
            if (type.Contains("[]"))
            {
                // Obtenemos el tipo del arreglo eliminando los corchetes
                PrimaryType.PrimaryTypes varType = PrimaryType.showType(type.Substring(0, type.Length - 2).Trim());
                
                // Verificamos el tipo del arreglo y creamos un objeto ArrayType correspondiente
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
                    consola.SalidaConsola.AppendText($"Error: El tipo del arreglo debe ser int o char. El tipo actual no es válido. {ShowToken(currentToken)}\n");
                }
            }
            else
            {
                // Obtener el tipo del parámetro como PrimaryType
                PrimaryType.PrimaryTypes varType = PrimaryType.showType(type);
                
                // Verificamos si es un tipo de clase buscándolo en la tabla de símbolos
                Type? paramT = _symbolTable.Search(type);
                
                if (varType is PrimaryType.PrimaryTypes.Unknown && paramT != null)
                {
                    // Si es un tipo desconocido y existe en la tabla de símbolos, es una clase
                    parameters.AddLast(new ClassVarType(token, _symbolTable.currentLevel, type));
                    // También puedes agregarlo a la tabla de símbolos
                    // _symbolTable.Insert(new ClassVarType(token, _symbolTable.currentLevel, type));
                }
                else if (varType is PrimaryType.PrimaryTypes.Unknown && paramT == null)
                {
                    consola.SalidaConsola.AppendText($"Error: El tipo de parámetro \"{token.Text}\" no es válido. {ShowToken(currentToken)}\n");
                }
                else
                {
                    // Es un tipo primario conocido
                    parameters.AddLast(new PrimaryType(token, varType, _symbolTable.currentLevel));
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
                        consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{tipoDesignator}\" no coincide con el tipo de la expresión \"{tipoExpresion.ToLower()}\". {ShowToken(currentToken)}\n");
                    }
                    else
                    {
                        // System.Diagnostics.Debug.WriteLine("Asignacion correcta: " + tipoDesignator + " es el mismo que el tipo de la expresion: " + tipoExpresion);
                    }

                    return null;
                }
            
                if (tipoDesignator != tipoExpresion)
                {
                    consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{tipoDesignator}\" no es compatible con el tipo de la expresión \"{tipoExpresion}\". {ShowToken(currentToken)}\n");

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
                consola.SalidaConsola.AppendText($"Error en la asignación: Los tipos del designador y de la expresión no son válidos. {ShowToken(currentToken)}\n");

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
                            consola.SalidaConsola.AppendText($"Error de parámetros: Los tipos de parámetros no coinciden en el método del. {ShowToken(currentToken)}\n");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        consola.SalidaConsola.AppendText($"Error en los parámetros: Cantidad incorrecta de parámetros para el método del. {ShowToken(currentToken)}\n");
                    }

                }
                else
                {
                    consola.SalidaConsola.AppendText($"Error en el método: Faltan los parámetros para el método del. {ShowToken(currentToken)}\n");
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
                            consola.SalidaConsola.AppendText($"Error de parámetros: Los tipos de parámetros no coinciden en el método len. {ShowToken(currentToken)}\n");
                        }
                    }
                    else
                    {
                        consola.SalidaConsola.AppendText($"Error en los parámetros: Cantidad incorrecta de parámetros para el método len. {ShowToken(currentToken)}\n");
                    }
                    
                }
                else
                { 
                    consola.SalidaConsola.AppendText($"Error en el método: Faltan los parámetros para el método len. {ShowToken(currentToken)}\n");
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
                            consola.SalidaConsola.AppendText($"Error de parámetros: Los tipos de parámetros no coinciden en el método add. {ShowToken(currentToken)}\n");
                        }
                    }
                    else if (parametros.Count != 2)
                    {
                        consola.SalidaConsola.AppendText($"Error en los parámetros: Cantidad incorrecta de parámetros para el método add. {ShowToken(currentToken)}\n");
                    }
                }
                else
                {
                    consola.SalidaConsola.AppendText($"Error en el método: Faltan los parámetros para el método add. {ShowToken(currentToken)}\n");
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
                                consola.SalidaConsola.AppendText($"Error de asignación: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} no coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\". {ShowToken(currentToken)}\n");

                            }
                            else
                            {
                                consola.SalidaConsola.AppendText($"Asignación correcta: El tipo \"{method.parametersL.ElementAt(i).GetStructureType()}\" del parámetro {i} coincide con el tipo de la expresión \"{parametros.ElementAt(i)}\". {ShowToken(currentToken)}\n");
                            }
                        }
                    }
                    else
                    {
                        consola.SalidaConsola.AppendText($"Error de asignación: El número de parámetros ({method.parametersL.Count}) no coincide con el número de expresiones ({parametros.Count}). {ShowToken(currentToken)}\n");
                    }
                }
                else if (method.parametersL.Count > 0)
                {
                    consola.SalidaConsola.AppendText($"Error de parámetros: Faltan parámetros en el método \"{method.GetToken().Text}\". {ShowToken(currentToken)}\n");
                }
            }
            else
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" no es un método. {ShowToken(currentToken)}\n");
            }
            
           
        }
        else if (context.INC()!=null)
        {
            if(tipoDesignator.ToLower() != "int")
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" solo se puede incrementar utilizando el operador ++ en enteros. {ShowToken(currentToken)}\n");
            }
            
        }
        else if (context.DEC()!=null)
        {
            if(tipoDesignator.ToLower() != "int")
            {
                consola.SalidaConsola.AppendText($"Error de asignación: \"{context.designator().GetText()}\" solo se puede decrementar utilizando el operador -- en enteros. {ShowToken(currentToken)}\n");
            }
            
        }
        return null;
    }
    

    public override object VisitIfStatementAST(MiniCSharpParser.IfStatementASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;
    
        // Abrimos un nuevo ámbito en la tabla de símbolos
        _symbolTable.OpenScope();
    
        // Evaluamos la condición del if
        bool conditionValue = (bool)Visit(context.condition());
    
        // Verificamos si la condición es verdadera
        if (conditionValue)
        {
            // Visitamos la primera declaración en el cuerpo del if
            Visit(context.statement(0));
        }
        else
        {
            consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del if es falsa: \"{context.condition().GetText()}\". {ShowToken(currentToken)}\n");
        
            // Verificamos si hay una segunda declaración en el cuerpo del if
            if (context.statement(1) != null)
            {
                // Abrimos un nuevo ámbito en la tabla de símbolos
                _symbolTable.OpenScope();
            
                // Visitamos la segunda declaración en el cuerpo del if
                Visit(context.statement(1));
            
                // Cerramos el ámbito actual en la tabla de símbolos
                _symbolTable.CloseScope();
            }
        }
    
        // Cerramos el ámbito actual en la tabla de símbolos
        _symbolTable.CloseScope();
        return null;
    }


    public override object VisitForStatementAST(MiniCSharpParser.ForStatementASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;
        
        // Abrimos un nuevo ámbito en la tabla de símbolos
        _symbolTable.OpenScope();
        
        // Visitamos la expresión del for
        Visit(context.expr());
        
        // Verificamos si hay una condición en el for
        if (context.condition() != null)
        {
            // Evaluamos la condición del for
            bool conditionValue = (bool)Visit(context.condition());
            
            // Verificamos si la condición es verdadera
            if (conditionValue)
            {
                // Verificamos si hay múltiples declaraciones en el cuerpo del for
                if (context.statement().Length > 1)
                {
                    // Visitamos las dos declaraciones en orden
                    Visit(context.statement(0));
                    Visit(context.statement(1));
                }
                else
                {
                    // Visitamos la única declaración en el cuerpo del for
                    Visit(context.statement(0));
                }
            }
            else
            {
                consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del for es falsa: \"{context.condition().GetText()}\". {ShowToken(currentToken)}\n");
            }
            
            // Cerramos el ámbito actual en la tabla de símbolos
            _symbolTable.CloseScope();
            return null;
        }
        
        // No hay una condición en el for, verificamos si hay múltiples declaraciones en el cuerpo del for
        if (context.statement().Length > 1)
        {
            // Visitamos las dos declaraciones en orden
            Visit(context.statement(0));
            Visit(context.statement(1));
        }
        else
        {
            // Visitamos la única declaración en el cuerpo del for
            Visit(context.statement(0));
        }
        
        // Cerramos el ámbito actual en la tabla de símbolos
        _symbolTable.CloseScope();
        return null;
    }


    public override object VisitWhileStatementAST(MiniCSharpParser.WhileStatementASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;
    
        // Abrimos un nuevo ámbito en la tabla de símbolos
        _symbolTable.OpenScope();
    
        // Evaluamos la condición del while
        bool conditionValue = (bool)Visit(context.condition());
    
        // Verificamos si la condición es verdadera
        if (conditionValue)
        {
            // Visitamos el cuerpo del while
            Visit(context.statement());
        }
        else
        {
            consola.SalidaConsola.AppendText($"ERROR: El tipo en la condición del while es falsa: \"{context.condition().GetText()}\". {ShowToken(currentToken)}\n");
        }
    
        // Cerramos el ámbito actual en la tabla de símbolos
        _symbolTable.CloseScope();
    
        // Retornamos null, ya que no hay un valor específico de retorno
        return null;
    }


    public override object VisitBreakStatementAST(MiniCSharpParser.BreakStatementASTContext context)
    {
        return null;
    }
    
    public override object VisitReturnStatementAST(MiniCSharpParser.ReturnStatementASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;

        // Verificamos si hay una expresión de retorno
        if (context.expr() != null)
        {
            // Obtenemos el tipo de retorno de la expresión
            string returnType = (string)Visit(context.expr());

            // Verificamos si el método actual es de tipo "void"
            if (_symbolTable.currentMethod.ReturnTypeGetSet == "void")
            {
                consola.SalidaConsola.AppendText($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" es de tipo void y no puede tener un valor de retorno. {ShowToken(currentToken)}\n");
            }
            // Verificamos si el tipo de retorno de la expresión es válido
            else if (!IsReturnTypeValid(returnType))
            {
                consola.SalidaConsola.AppendText($"ERROR de Retorno: El método \"{_symbolTable.currentMethod.GetToken().Text}\" no puede retornar un valor de tipo \"{returnType}\". {ShowToken(currentToken)}\n");
            }
        }
        else
        {
            consola.SalidaConsola.AppendText($"ERROR de Retorno: El método no tiene un valor de retorno válido. {ShowToken(currentToken)}\n");
        }

        // Retornamos null, ya que el retorno no tiene un valor específico
        return null;
    }

    private bool IsReturnTypeValid(string returnType)
    {
        // Verificamos si el tipo de retorno es nulo
        if (returnType == null)
        {
            return false;
        }

        // Comparamos el tipo de retorno con el tipo de retorno del método actual (ignorando mayúsculas y minúsculas)
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
        // Obtenemos el token actual asociado al contexto
        IToken currentToken = context.Start;
    
        // Validamos en orden y verificamos el tipo que deben tener
        foreach (var child in context.children)
        {
            // Ignoramos los tokens de llave izquierda y llave derecha
            if (child.Equals(context.LBRACE()) || child.Equals(context.RBRACE()))
                continue;

            if (child is MiniCSharpParser.StatementContext)
            {
                // Visitamos una sentencia
                Visit(child);
            }
            else if (child is MiniCSharpParser.VarDeclASTContext)
            {
                // Visitamos una declaración de variable
                Visit(child);
            }
            else
            {
                // Si no es una declaración de variable ni una sentencia, mostramos un mensaje de error
                consola.SalidaConsola.AppendText($"Error en el bloque: Se esperaba una declaración de variable o una sentencia en VisitBlockAST. {ShowToken(currentToken)}\n");
            }
        }

        // Retornamos null, ya que el bloque no tiene un valor de retorno específico
        return null;
    }


    public override object VisitActParsAST(MiniCSharpParser.ActParsASTContext context)
    {
        // LinkedList para almacenar los tipos de los parámetros
        LinkedList<Type> parametersTypes = new LinkedList<Type>();

        // Iteramos sobre cada expresión en los argumentos
        foreach (var child in context.expr())
        {
            // Obtenemos el tipo de la expresión
            string expressionType = (string)Visit(child);

            // Buscamos el tipo en la tabla de símbolos
            Type? tableType = _symbolTable.Search(child.GetText());

            // Verificamos si el tipo de la expresión es válido
            if (expressionType != null)
            {
                // Creamos un nuevo objeto PrimaryType y lo agregamos a la lista de tipos de parámetros
                parametersTypes.AddLast(new PrimaryType(child.Start, PrimaryType.showType(expressionType.ToLower()),
                    _symbolTable.currentLevel));
            }
            // Verificamos si el tipo se encuentra en la tabla de símbolos
            else if (tableType != null)
            {
                // Agregamos el tipo de la tabla de símbolos a la lista de tipos de parámetros
                parametersTypes.AddLast(tableType);
            }
        }

        // Devolvemos la lista de tipos de parámetros
        return parametersTypes;
    }

    
    public override object VisitConditionAST(MiniCSharpParser.ConditionASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;

        // Variable para indicar si hay una condición válida
        bool hasValidCondition = false;

        // Iteramos sobre cada término de condición
        foreach (var term in context.condTerm())
        {
            // Evaluamos el término de condición
            bool conditionType = (bool)Visit(term);

            // Verificamos si el término de condición es verdadero
            if (conditionType)
            {
                // Hay una condición válida, actualizamos la bandera y salimos del bucle
                hasValidCondition = true;
                break;
            }
        }

        // Verificamos si hay una condición válida
        if (hasValidCondition)
        {
            // Hay una condición válida, retornamos true
            return true;
        }
        else
        {
            // No hay una condición válida, mostramos un mensaje de error
            consola.SalidaConsola.AppendText($"Error: Los tipos que se están comparando no son compatibles. {ShowToken(currentToken)}\n");

            // Retornamos false para indicar que no se cumple ninguna condición válida
            return false;
        }
    }


    public override object VisitCondTermAST(MiniCSharpParser.CondTermASTContext context)
    {
        // Obtenemos el token actual asociado al contexto
        IToken currentToken = context.Start;

        // Variable para almacenar si todas las condiciones son verdaderas
        bool allConditionsTrue = true;

        // Iteramos sobre cada factor de condición
        foreach (var factor in context.condFact())
        {
            // Evaluamos el factor de condición
            bool conditionType = (bool)Visit(factor);

            // Verificamos si la condición es falsa
            if (!conditionType)
            {
                // La condición es falsa, mostramos un mensaje de error
                consola.SalidaConsola.AppendText($"Error: Los tipos no coinciden en VisitConditionTermAST: {conditionType}. {ShowToken(currentToken)}\n");

                // Retornamos false para indicar que no se cumplen todas las condiciones
                return false;
            }
        }

        // Retornamos true para indicar que todas las condiciones se cumplieron
        return allConditionsTrue;
    }


    public override object VisitCondFactAST(MiniCSharpParser.CondFactASTContext context)
    {
        // Obtenemos el token asociado al contexto
        IToken currentToken = context.Start;

        // Evaluamos la expresión del primer operando
        string firstExprType = (string)Visit(context.expr(0));

        // Visitamos el operador relacional
        Visit(context.relop());

        // Evaluamos la expresión del segundo operando
        string secondExprType = (string)Visit(context.expr(1));

        // Verificamos si la comparación es válida
        if (IsComparisonValid(firstExprType, secondExprType))
        {
            // La comparación es válida
            return true;
        }
        else
        {
            // La comparación no es válida, mostramos un mensaje de error
            consola.SalidaConsola.AppendText(GetErrorComparisonMessage(firstExprType, secondExprType) + $"{ShowToken(currentToken)}");

            // Retornamos false para indicar que la comparación no es válida
            return false;
        }
    }

    private bool IsComparisonValid(string firstType, string secondType)
    {
        // Verificamos si alguno de los tipos es nulo
        if (firstType == null || secondType == null)
        {
            // La comparación no es válida si uno de los tipos es nulo
            return false;
        }

        // Comparamos los tipos
        return firstType == secondType;
    }

    private string GetErrorComparisonMessage(string expectedType, string actualType)
    {
        // Verificamos si alguno de los tipos es nulo
        if (expectedType == null || actualType == null)
        {
            // Si alguno de los tipos es nulo, mostramos un mensaje de error específico
            return "Error en el Factor de Condición: No se puede comparar el tipo de condición con null.";
        }

        // Mostramos un mensaje de error indicando los tipos esperado y actual
        return $"Error en el Factor de Condición: Los tipos de condición no coinciden. Se esperaba {expectedType} pero se encontró {actualType}.";
    }


    public override object VisitCastAST(MiniCSharpParser.CastASTContext context)
    {
        // Obtenemos el token actual asociado al contexto
        IToken currentToken = context.Start;

        // Obtenemos el tipo del casting
        string type = (string)Visit(context.type());

        // Verificamos si el tipo es nulo
        if (type == null)
        {
            consola.SalidaConsola.AppendText($"Error en el cast: El valor a castear es nulo. {ShowToken(currentToken)}\n");
        }

        // Retornamos el tipo del casting
        return type;
    }



    public override object VisitExpressionAST(MiniCSharpParser.ExpressionASTContext context)
    {
        // Obtenemos el token actual asociado al contexto
        IToken currentToken = context.Start;

        // Verificamos si hay una operación de casting en la expresión
        if (context.cast() != null)
        {
            // Obtenemos el tipo de casting
            string castType = (string)Visit(context.cast());

            // Retornamos el tipo de casting
            return castType;
        }

        // Obtenemos el tipo del primer término de la expresión
        string termType = (string)Visit(context.term(0));

        // Verificamos si el tipo del primer término es nulo
        if (termType == null)
        {
            consola.SalidaConsola.AppendText($"Error: Tipo no válido de la expresión. Se encontró null. {ShowToken(currentToken)}\n");

            // Retornamos null para indicar un tipo no válido
            return null;
        }
 
        // Verificamos los tipos de los términos restantes
        for (int i = 1; i < context.term().Length; i++)
        {
            // Obtenemos el tipo del término actual
            string currentTerm = (string)Visit(context.term(i));

            // Verificamos si el tipo del término actual es diferente al tipo del primer término
            if (termType != currentTerm)
            {
                consola.SalidaConsola.AppendText($"Error de tipos: Todos los tipos en la expresión deben ser iguales. {ShowToken(currentToken)}\n");

                // Retornamos null para indicar un error de tipos
                return null;
            }
        }
    
        // Retornamos el tipo del primer término
        return termType;
    }

    
    public override object VisitTermAST(MiniCSharpParser.TermASTContext context)
    {
        // Obtenemos el token del inicio del contexto
        IToken currentToken = context.Start;

        // Obtenemos el tipo del primer factor
        string factorType = (string)Visit(context.factor(0));

        // Si hay más de un factor, comprobamos si tienen el mismo tipo
        if (context.factor().Length > 1)
        {
            int i = 1;
            while (i < context.factor().Length && factorType == (string)Visit(context.factor(i)))
            {
                i++;
            }

            // Si hay un factor con un tipo diferente, se muestra un error y se devuelve null
            if (i < context.factor().Length)
            {
                consola.SalidaConsola.AppendText($"Error de tipos: Los tipos son diferentes en term. {ShowToken(currentToken)}\n");

                return null;
            }
        }

        // Devolvemos el tipo del factor
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
                    consola.SalidaConsola.AppendText($"Error: Cantidad de parámetros incorrecta. {ShowToken(currentToken)}\n");

                    return null;
                }
                else
                {
                    for (int i = 0; i < method.parametersL.Count; i++)
                    {
                        if (((MethodType)metodo).parametersL.ElementAt(i).GetStructureType() !=
                            tipos.ElementAt(i).GetStructureType())
                        {
                            consola.SalidaConsola.AppendText($"Error: Tipo de parámetro incorrecto. Se esperaba: {((MethodType)metodo).parametersL.ElementAt(i).GetStructureType()}, se obtuvo: {tipos.ElementAt(i).GetStructureType()} {ShowToken(currentToken)}\n");

                            return null;
                        }
                    }

                    return method.ReturnTypeGetSet;
                }
            }
            else
            {
                consola.SalidaConsola.AppendText($"Error: No se encontró el método. {ShowToken(currentToken)}\n");

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
        // Obtenemos el token actual asociado al contexto
        IToken currentToken = context.Start;

        // Obtenemos el identificador del tipo
        string ident = (string)Visit(context.type());

        // Buscamos en la tabla de símbolos para verificar si es una clase
        Type? classType = _symbolTable.Search(ident);

        if (classType != null)
        {
            // Es una clase, devolvemos el tipo de estructura de la clase
            return classType.GetStructureType();
        }

        // Verificamos si es un arreglo de tipo básico (int o char)
        ArrayType.ArrTypes arrType = ArrayType.showType(ident);

        if (arrType != ArrayType.ArrTypes.Unknown)
        {
            // Es un arreglo de tipo básico, devolvemos el tipo del arreglo
            return arrType.ToString();
        }

        // Si no es una clase ni un arreglo de tipo básico, mostramos un mensaje de error
        consola.SalidaConsola.AppendText($"Error de tipos: El tipo del 'new' no existe en la tabla de símbolos ni es un arreglo de tipo básico. {ShowToken(currentToken)}\n");

        // Retornamos null para indicar un error de tipo
        return null;
    }


    public override object VisitParenFactorAST(MiniCSharpParser.ParenFactorASTContext context)
    {
        // Evaluamos la expresión contenida entre paréntesis
        string expressionType = (string)Visit(context.expr());

        // Verificamos si el tipo de la expresión es válido
        if (expressionType != null)
        {
            // Retornamos el tipo de la expresión
            return expressionType;
        }
    
        // Si el tipo de la expresión es nulo, retornamos también nulo
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
                    
                    consola.SalidaConsola.AppendText($"No se encontró la variable '{context.ident(1).GetText()}' en la clase. {ShowToken(currentToken)}\n");
                    
                    return null;
                
                }
            }
           
            consola.SalidaConsola.AppendText($"No se encontró en dicha clase '{context.ident(context.ident().Length - 2).GetText()}'. {ShowToken(currentToken)}\n");

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
              
                consola.SalidaConsola.AppendText($"Error de tipos: El índice del arreglo no es de tipo Int. {ShowToken(currentToken)}\n");
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

            consola.SalidaConsola.AppendText($"No se encontró en la tabla la variable: {context.ident(0).GetText()}. {ShowToken(currentToken)}\n");

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
