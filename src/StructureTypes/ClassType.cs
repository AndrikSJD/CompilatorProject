﻿using System.Collections.Generic;
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

/// <summary>
/// Clase que representa un tipo de clase.
/// </summary>
public class ClassType: Type
{
    public readonly string Type = "class";
    public  LinkedList<Type> parametersL = new LinkedList<Type>();
    
    /// <summary>
    /// Constructor de la clase ClassType.
    /// </summary>
    /// <param name="type">Token asociado al tipo de clase.</param>
    /// <param name="lvl">Nivel de anidamiento.</param>
    public ClassType(IToken type, int lvl) : base(type, lvl)// constructor del padre
    {
        
    }

    /// <summary>
    /// Imprime los detalles del tipo de clase en la salida de depuración.
    /// </summary>
    public void PrintClass(IToken s, int level, string type)
    {
        System.Diagnostics.Debug.WriteLine($"---Token: {s.Text}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de dato: {type}");
        System.Diagnostics.Debug.WriteLine($" - Nivel tabla: {level}");
        System.Diagnostics.Debug.WriteLine("\n");
    }
    
    /// <summary>
    /// Busca un atributo dentro de la lista de parámetros de la clase.
    /// </summary>
    /// <param name="name">Nombre del atributo a buscar.</param>
    /// <returns>True si se encuentra el atributo, False en caso contrario.</returns>
    public bool BuscarAtributo(string name)
    {
        foreach (var attribute in parametersL)
        {
            if (attribute.GetToken().Text.Equals(name))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Retorna el tipo de estructura, que en este caso es el nombre de la clase.
    /// </summary>
    public override string GetStructureType()
    {
        return this.GetToken().Text;
    }
}