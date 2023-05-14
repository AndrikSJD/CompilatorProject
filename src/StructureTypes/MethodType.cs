﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class MethodType : Type
{
    private readonly string Type = "method";
    private int ParamsNum;
    private string returnType;
    
    
   
    public LinkedList<Type> parametersL;
    
    public MethodType(IToken tok, int level, int parN, string rt, LinkedList<Type> parsList) : base(tok, level)
    {
        this.ParamsNum = parN;
        this.returnType = rt;
        this.parametersL = parsList;
    }
    
    public void PrintMethod()
    {
        System.Diagnostics.Debug.WriteLine("Tipo de metodo "+ this.GetToken().Text + " Nivel: " + Level);
        System.Diagnostics.Debug.WriteLine(" - Nivel: " + Level);
        System.Diagnostics.Debug.WriteLine(" - Tipo de retorno: " + returnType);
        System.Diagnostics.Debug.WriteLine(" - Numero de parametros: " + ParamsNum);
        System.Diagnostics.Debug.WriteLine(" - Parametros: ");
        foreach (var parameter in parametersL)
        {
            if (parameter is ClassVarType classVarType)
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {classVarType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {classVarType.Type}");
                System.Diagnostics.Debug.WriteLine($" - Tipo de {classVarType.classType}");
                System.Diagnostics.Debug.WriteLine($"Nombre: {classVarType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Nivel: {classVarType.Level}");
            }
            
            if (parameter is PrimaryType primaryType)
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {primaryType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {primaryType.TypeGetSet}");
                System.Diagnostics.Debug.WriteLine($" - Nivel {primaryType.Level}");

            }

            if (parameter is ArrayType arrType)
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {arrType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {arrType.Type}");
                System.Diagnostics.Debug.WriteLine($" - Tipo de array {arrType.GetSetArrType}");
                System.Diagnostics.Debug.WriteLine($" - Nivel {arrType.Level}");
            }
            
            
        }
        System.Diagnostics.Debug.WriteLine("\n");
        
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