using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

/// <summary>
/// Clase que representa un tipo de datos de clase.
/// </summary>
public class ClassVarType : Type
{
    public readonly string Type = "ClassType";
    public readonly string classType;


    /// <summary>
    /// Constructor de la clase ClassVarType.
    /// </summary>
    /// <param name="tk">Token asociado al tipo de datos de clase.</param>
    /// <param name="lvl">Nivel de ámbito.</param>
    /// <param name="tf">Tipo de clase.</param>
    public ClassVarType(IToken tk, int lvl, string tf) : base(tk, lvl)
    {
        classType = tf;
        
    }

    /// <summary>
    /// Imprime los detalles del tipo de datos de clase en la salida de depuración.
    /// </summary>
    public void PrintClassVarType(IToken s, int level, string type, string fatherType)
    {
        System.Diagnostics.Debug.WriteLine($"Nombre: {s.Text}");
        System.Diagnostics.Debug.WriteLine($" - Nivel: {level}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de dato: {type}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de padre: {fatherType}");
        System.Diagnostics.Debug.WriteLine("\n");
    }
    
    /// <summary>
    /// Retorna el tipo de estructura, que en este caso es el tipo de clase.
    /// </summary>
    public override string GetStructureType()
    {
        return this.classType;
    }
}