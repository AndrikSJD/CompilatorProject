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
        System.Diagnostics.Debug.WriteLine("-------------------- Metodo -------------------");
        System.Diagnostics.Debug.WriteLine("Tipo de metodo "+ this.GetToken().Text + " Nivel: " + Level);
        System.Diagnostics.Debug.WriteLine("Contador de parametros: " + ParamsNum);
        System.Diagnostics.Debug.WriteLine("Tipo de retorno: " + returnType);
        System.Diagnostics.Debug.WriteLine("Parametros: ");
        foreach (var parameter in parametersL)
        {
            if (parameter is ClassVarType classVarType)
            {
                System.Diagnostics.Debug.WriteLine("    Nombre: "+ classVarType.GetToken().Text +" Tipo: "+ classVarType.Type + " - Tipo de " + classVarType.classType);
                System.Diagnostics.Debug.WriteLine("    Nombre: "+ classVarType.GetToken().Text+" Nivel: "+(classVarType.Level));  
            }
            
            if (parameter is PrimaryType primaryType)
            {
                System.Diagnostics.Debug.WriteLine("    Nombre: "+ primaryType.GetToken().Text+" Tipo: "+ primaryType.TypeGetSet + " - Nivel " + primaryType.Level);
            }

            if (parameter is ArrayType arrType)
            {
                System.Diagnostics.Debug.WriteLine("    Nombre: "+ arrType.GetToken().Text+" Tipo: "+ arrType.Type + " - Nivel " + arrType.Level + " - Tipo de array " + arrType.GetSetArrType);
            }
            
            
        }
        System.Diagnostics.Debug.WriteLine("-------------------- Fin del Metodo -------------------"+ "\n");
        
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