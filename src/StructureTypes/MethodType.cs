﻿using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

/// <summary>
/// Clase que representa un tipo de datos método.
/// </summary>
public class MethodType : Type
{
    private readonly string Type = "method";
    private int ParamsNum;
    private string returnType;
    
    
   
    public LinkedList<Type> parametersL;
    
    /// <summary>
    /// Constructor de la clase MethodType.
    /// </summary>
    /// <param name="tok">Token asociado al tipo de datos método.</param>
    /// <param name="level">Nivel de ámbito.</param>
    /// <param name="parN">Número de parámetros del método.</param>
    /// <param name="rt">Tipo de retorno del método.</param>
    /// <param name="parsList">Lista enlazada de parámetros del método.</param>
    public MethodType(IToken tok, int level, int parN, string rt, LinkedList<Type> parsList) : base(tok, level)
    {
        this.ParamsNum = parN;
        this.returnType = rt;
        this.parametersL = parsList;
    }
    
    /// <summary>
    /// Imprime los detalles del tipo de datos método en la salida de depuración.
    /// </summary>
    public void PrintMethod()
    {
        System.Diagnostics.Debug.WriteLine("---Tipo de metodo "+ this.GetToken().Text + " Nivel: " + Level);
        System.Diagnostics.Debug.WriteLine(" - Nivel: " + Level);
        System.Diagnostics.Debug.WriteLine(" - Tipo de retorno: " + returnType);
        System.Diagnostics.Debug.WriteLine(" - Numero de parametros: " + ParamsNum);
        System.Diagnostics.Debug.WriteLine(" - Parametros: ");
        foreach (var parameter in parametersL)
        {
            if (parameter is ClassVarType classVarType)
            {
                System.Diagnostics.Debug.WriteLine($"Token: {classVarType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {classVarType.Type}");
                System.Diagnostics.Debug.WriteLine($" - Tipo de {classVarType.classType}");
                System.Diagnostics.Debug.WriteLine($" - Nivel: {classVarType.Level}");
            }
            
            if (parameter is PrimaryType primaryType)
            {
                System.Diagnostics.Debug.WriteLine($"Token: {primaryType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {primaryType.TypeGetSet}");
                System.Diagnostics.Debug.WriteLine($" - Nivel: {primaryType.Level}");

            }

            if (parameter is ArrayType arrType)
            {
                System.Diagnostics.Debug.WriteLine($"Token: {arrType.GetToken().Text}");
                System.Diagnostics.Debug.WriteLine($" - Tipo: {arrType.Type}");
                System.Diagnostics.Debug.WriteLine($" - Tipo de array: {arrType.GetSetArrType}");
                System.Diagnostics.Debug.WriteLine($" - Nivel: {arrType.Level}");
            }
            
            
        }
        System.Diagnostics.Debug.WriteLine("\n");
        
    }

    /// <summary>
    /// Propiedad para obtener o establecer el tipo de retorno del método.
    /// </summary>
    public string ReturnTypeGetSet
    {
        get => returnType;
        set => returnType = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Retorna el tipo de estructura, que en este caso es el tipo de retorno del método.
    /// </summary>
    public override string GetStructureType()
    {
        return this.returnType;
    }
}