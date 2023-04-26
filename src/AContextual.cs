
using System;
using Antlr4.Runtime;
using SyntacticAnalysisGenerated;

namespace Proyecto;

public class AContextual : MiniCSharpParserBaseVisitor<object> {
    
    private SymbolTable symbolTable;
    
    public AContextual()
    {
        symbolTable = new SymbolTable();
    }
    
    private String GetTypeName(int type)
    {
        switch (type)
        {
            case 0:
                return "int";
            case 1:
                return "float";
            case 2:
                return "string";
            case 3:
                return "bool";
            case 4:
                return "void";
            default:
                return "unknown";
        }
    }
    
    private void PrintError(IToken tok, String msg)
    {
        Console.WriteLine("Error en la linea " + tok.Line + ":" + tok.Column + " " + msg);
    }

    private bool isMultitype(String op)
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
        Visit(context.@using(0));
        for(int i = 0; i < context.@using().Length; i++)
        {
            Visit(context.@using(i));
        }
        return null;
    }

    public override object VisitUsingAST(MiniCSharpParser.UsingASTContext context)
    {
       return null;
    }

    public override object VisitVarDeclAST(MiniCSharpParser.VarDeclASTContext context)
    {
        return null;
    }

    public override object VisitClassDeclAST(MiniCSharpParser.ClassDeclASTContext context)
    {
        return null;
    }

    public override object VisitMethodDeclAST(MiniCSharpParser.MethodDeclASTContext context)
    {
        return null;
    }

    public override object VisitFormParsAST(MiniCSharpParser.FormParsASTContext context)
    {
        return null;
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
    
}
