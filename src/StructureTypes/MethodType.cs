using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class MethodType : Type
{
    private readonly string Type = "method";
    private int ParamsNum;
    private string returnType;
    
    
    //TODO CAMBIARLO A HASMAP
    public LinkedList<Type> parametersL;
    
    public MethodType(IToken tok, int level, int parN, string rt, LinkedList<Type> parsList) : base(tok, level)
    {
        this.ParamsNum = parN;
        this.returnType = rt;
        this.parametersL = parsList;
    }
    
    public void PrintMethod()
    {
        System.Diagnostics.Debug.WriteLine("-------------------- METHOD -------------------");
        System.Diagnostics.Debug.WriteLine("MethodType "+ this.GetToken().Text + " Level: " + Level);
        System.Diagnostics.Debug.WriteLine("ParamCount: " + ParamsNum);
        System.Diagnostics.Debug.WriteLine("ReturnType: " + returnType);
        System.Diagnostics.Debug.WriteLine("Parameters: ");
        foreach (var parameter in parametersL)
        {
            if (parameter is ClassVarType classVarType)
            {
                System.Diagnostics.Debug.WriteLine("    Name: "+ classVarType.GetToken().Text +" Type: "+ classVarType.Type + " - TypeOf " + classVarType.classType);
                System.Diagnostics.Debug.WriteLine("    Name: "+ classVarType.GetToken().Text+" Level: "+(classVarType.Level));  
            }
            
            if (parameter is PrimaryType primaryType)
            {
                System.Diagnostics.Debug.WriteLine("    Name: "+ primaryType.GetToken().Text+" Type: "+ primaryType.TypeGetSet + " - Level " + primaryType.Level);
            }

            if (parameter is ArrayType arrType)
            {
                System.Diagnostics.Debug.WriteLine("    Name: "+ arrType.GetToken().Text+" Type: "+ arrType.Type + " - Level " + arrType.Level + " - typeArray " + arrType.GetSetArrType);
            }
            
            
        }
        System.Diagnostics.Debug.WriteLine("-------------------- FIN METHOD -------------------"+ "\n");
        
    }

    public int ParamsNumGetSet
    {
        get => ParamsNum;
        set => ParamsNum = value;
    }

    public string ReturnTypeGetSet
    {
        get => returnType;
        set => returnType = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override string GetStructureType()
    {
        return this.returnType;
    }
}