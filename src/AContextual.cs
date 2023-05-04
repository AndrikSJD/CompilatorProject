
using System;
using System.Collections.Generic;
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
    
    // private String GetTypeName(int type)
    // {
    //     switch (type)
    //     {
    //         case 0:
    //             return "int";
    //         case 1:
    //             return "float";
    //         case 2:
    //             return "char";
    //         case 3:
    //             return "string";
    //         case 4:
    //             return "bool";
    //         case 5:
    //             return "void";
    //         case 6:
    //             return "unknown";
    //         case 7:
    //             return "class";
    //         default:
    //             return "No hay un type";
    //     }
    // }
    
    // private int GetType(String type)
    // {
    //     switch (type)
    //     {
    //         case "int":
    //             return 0;
    //         case "float":
    //             return 1;
    //         case "char":
    //             return 2;
    //         case "string":
    //             return 3;
    //         case "bool":
    //             return 4;
    //         case "void":
    //             return 5;
    //         case "unknown":
    //             return 6;
    //         case "class":
    //             return 7;
    //         default: //no tiene tipo
    //             return -1;
    //     }
    // }
    
    
    
    
 
    
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
            // if (context.@using().Length > 0)
            // {
            //     foreach (var child in context.@using())
            //     {
            //
            //         Visit(child);
            //         System.Diagnostics.Debug.WriteLine("---visita using: " + child.GetText());
            //
            //     }
            //
            // }
            //
            // if (context.varDecl().Length > 0)
            // {
            //     int i = 0;
            //     foreach (var child in context.varDecl())
            //     {
            //
            //         Visit(child);
            //         System.Diagnostics.Debug.WriteLine("---visita varDecl" + i + ": " + child.GetText());
            //         i++;
            //     }
            // }
            //
            // if (context.classDecl().Length > 0)
            // {
            //     int i = 0;
            //     foreach (var child in context.classDecl())
            //     {
            //
            //         Visit(child);
            //         System.Diagnostics.Debug.WriteLine("---visita classDecl" + i + ": " + child.GetText());
            //         i++;
            //     }
            // }
            //
            // if (context.methodDecl().Length > 0)
            // {
            //     int i = 0;
            //     foreach (var child in context.methodDecl())
            //     {
            //
            //         Visit(child);
            //         System.Diagnostics.Debug.WriteLine("---visita methodDecl" + i + ": " + child.GetText());
            //         i++;
            //     }
            //     
            //
            // }
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
      
        //verificamos si es un array
        if(context.type().GetText().Contains('['))
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
                System.Diagnostics.Debug.WriteLine("The type of the array can be only int or char, current type is not valid ");
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
                            _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int));
                        }
                        else if (varType is PrimaryType.PrimaryTypes.Char ) //al validar el tipo de la variable, si no es int, es char anteriormente se valido si no era valido isError = true
                        {
                            _symbolTable.Insert(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                        }
                        
                        
                }
                else
                {
                    //si la variable es de un tipo de clase
                    if(isClassVarType)
                    {
                        _symbolTable.Insert(new ClassVarType(token, _symbolTable.currentLevel, context.type().GetText()));
                    }
                    else //es de un tipo primario
                    {
                        _symbolTable.Insert(new PrimaryType(token,varType, _symbolTable.currentLevel));
                    }

                }
            }  
            
        }

        else
        {
            System.Diagnostics.Debug.WriteLine("The declaration type of the variable is not valid");
        }


        return null;
    }

    public override object VisitClassDeclAST(MiniCSharpParser.ClassDeclASTContext context)
    {
        ClassType classDcl = new ClassType((IToken)Visit(context.ident()), _symbolTable.currentLevel);
        _symbolTable.Insert(classDcl);
        _symbolTable.OpenScope();
        if(context.varDecl().Length > 0)
        {
            foreach (var child in context.varDecl())
            {
                Visit(child);
            }
        }
        _symbolTable.CloseScope();
        return null;
    }

    public override object VisitMethodDeclAST(MiniCSharpParser.MethodDeclASTContext context)
    {
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
            if(context.type().GetText().Contains('['))
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
                    System.Diagnostics.Debug.WriteLine("The type of the array can be only int or char, current type is not valid METHOD DECL ");
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


        }
        else
        {
            System.Diagnostics.Debug.WriteLine("The type of the method is not valid");
        }
        
        
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
            if (type.Contains('['))
            {
                varType = PrimaryType.showType(context.type(i).GetText().Substring(0, context.type(i).GetText().Length - 2).Trim());
                
                if (varType is PrimaryType.PrimaryTypes.Int)
                {
                    parameters.AddFirst(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Int));
                }
                else if (varType is PrimaryType.PrimaryTypes.Char)
                {
                    parameters.AddFirst(new ArrayType(token, _symbolTable.currentLevel, ArrayType.ArrTypes.Char));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("The type of the array can be only int or char, current type is not valid");
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
                    parameters.AddFirst(new ClassVarType(token, _symbolTable.currentLevel, type));
                }
                else if (varType is PrimaryType.PrimaryTypes.Unknown &&
                    paramT == null)
                {
                    System.Diagnostics.Debug.WriteLine("The type of the parameter: " + token.Text +" is not valid");
                }
                else
                {
                    parameters.AddFirst(new PrimaryType(token, varType, _symbolTable.currentLevel));
                }
            }
            
        }
        return parameters;
    }

    public override object VisitTypeAST(MiniCSharpParser.TypeASTContext context)
    {
        
        return null;
    }

    public override object VisitAssignStatementAST(MiniCSharpParser.AssignStatementASTContext context)
    {
        return null;
    }

    public override object VisitMethodCallStatementAST(MiniCSharpParser.MethodCallStatementASTContext context)
    {
        return null;
    } 

    public override object VisitIfStatementAST(MiniCSharpParser.IfStatementASTContext context)
    {
        return null;
        
    }

    public override object VisitForStatementAST(MiniCSharpParser.ForStatementASTContext context)
    {
        return null;
    }

    public override object VisitWhileStatementAST(MiniCSharpParser.WhileStatementASTContext context)
    {
        return null;
    }

    public override object VisitBreakStatementAST(MiniCSharpParser.BreakStatementASTContext context)
    {
        return null;
    }

    public override object VisitReturnStatementAST(MiniCSharpParser.ReturnStatementASTContext context)
    {
        return null;
    }

    public override object VisitReadStatementAST(MiniCSharpParser.ReadStatementASTContext context)
    {
        return null;
    }

    public override object VisitWriteStatementAST(MiniCSharpParser.WriteStatementASTContext context)
    {
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

    public override object VisitEmptyStatementAST(MiniCSharpParser.EmptyStatementASTContext context)
    {
        return null;
    }

    public override object VisitBlockAST(MiniCSharpParser.BlockASTContext context)
    {   
        _symbolTable.OpenScope();
        System.Diagnostics.Debug.WriteLine(context.varDecl().Length);
        System.Diagnostics.Debug.WriteLine(context.children.Count);

        if (context.varDecl().Length > 0)
        {
            foreach (var var in context.varDecl())
            {
                Visit(var);
            }
        }

        if (context.statement().Length > 0)
        {
            foreach (var statement in context.statement())
            {
                Visit(statement);
            }
        }
        _symbolTable.CloseScope();
        return null;
    }

    public override object VisitActParsAST(MiniCSharpParser.ActParsASTContext context)
    {
        return null;
    }

    public override object VisitConditionAST(MiniCSharpParser.ConditionASTContext context)
    {
        return null;
    }

    public override object VisitCondTermAST(MiniCSharpParser.CondTermASTContext context)
    {
        return null;
    }

    public override object VisitCondFactAST(MiniCSharpParser.CondFactASTContext context)
    {
        return null;
    }

    public override object VisitCastAST(MiniCSharpParser.CastASTContext context)
    {
        return null;
    }

    public override object VisitExprAST(MiniCSharpParser.ExprASTContext context)
    {
        return null;
    }

    public override object VisitTermAST(MiniCSharpParser.TermASTContext context)
    {
        return null;
    }

    public override object VisitFactorAST(MiniCSharpParser.FactorASTContext context)
    {
        return null;
    }

    public override object VisitNumFactorAST(MiniCSharpParser.NumFactorASTContext context)
    {
        return null;
    }

    public override object VisitCharFactorAST(MiniCSharpParser.CharFactorASTContext context)
    {
        return null;
    }

    public override object VisitStringFactorAST(MiniCSharpParser.StringFactorASTContext context)
    {
        return null;
    }

    public override object VisitBooleanFactorAST(MiniCSharpParser.BooleanFactorASTContext context)
    {
        return null;
    }

    public override object VisitNewFactorAST(MiniCSharpParser.NewFactorASTContext context)
    {
        return null;
    }

    public override object VisitParenFactorAST(MiniCSharpParser.ParenFactorASTContext context)
    {
        return null;
    }

    public override object VisitDesignatorAST(MiniCSharpParser.DesignatorASTContext context)
    {
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
        return null;
    }
    
    
    
}
