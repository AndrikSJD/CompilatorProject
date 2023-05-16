using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

/// <summary>
/// Clase que representa un tipo de arreglo.
/// </summary>
public class ArrayType : Type
{
    public readonly string Type = "array";
    public int size = 0;
    
    public enum ArrTypes
    {
        Char,
        Int,
        Unknown,
    }
    

    private ArrTypes arrType;

    /// <summary>
    /// Constructor de la clase ArrayType.
    /// </summary>
    /// <param name="tok">Token asociado al tipo de arreglo.</param>
    /// <param name="lvl">Nivel de anidamiento.</param>
    /// <param name="arr">Tipo base del arreglo.</param>
    public ArrayType(IToken tok, int lvl, ArrTypes arr) : base(tok, lvl)
    {
        arrType = arr;
    }

    /// <summary>
    /// Devuelve el tipo de arreglo correspondiente a partir de una cadena de texto.
    /// </summary>
    /// <param name="type">Cadena de texto que representa el tipo de arreglo.</param>
    /// <returns>Tipo de arreglo correspondiente.</returns>
    public static ArrTypes showType(string type)
    {
        return type switch
        {
            "char" => ArrTypes.Char,
            "int" => ArrTypes.Int,
            _ => ArrTypes.Unknown,
        };
    }

    /// <summary>
    /// Propiedad que permite obtener o establecer el tipo base del arreglo.
    /// </summary>
    public ArrTypes GetSetArrType
    {
        get => arrType;
        set => arrType = value;
    }
    
    /// <summary>
    /// Imprime los detalles del tipo de arreglo en la salida de depuración.
    /// </summary>
    public void PrintArrayType(IToken s, int level, string type, ArrTypes baseType)
    {
        System.Diagnostics.Debug.WriteLine($"---Token: {s.Text}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de dato: {type}");
        System.Diagnostics.Debug.WriteLine($" - Arreglo de: {baseType}");
        System.Diagnostics.Debug.WriteLine($" - Nivel tabla: {level}");

        System.Diagnostics.Debug.WriteLine("\n");
    }

    /// <summary>
    /// Retorna el tipo de estructura, que en este caso es el tipo de arreglo.
    /// </summary>
    public override string GetStructureType()
    {
        return this.arrType.ToString();
    }
}