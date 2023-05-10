using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class ArrayType : Type
{
    public readonly string Type = "array";
    public int size = 0;
    
    public enum ArrTypes
    {
        Int,
        Char,
        Unknown,
    }
    

    private ArrTypes arrType;

    public ArrayType(IToken tok, int lvl, ArrTypes arr) : base(tok, lvl)
    {
        arrType = arr;
    }

    public static ArrTypes showType(string type)
    {
        return type switch
        {
            "int" => ArrTypes.Int,
            "char" => ArrTypes.Char,
            _ => ArrTypes.Unknown,
        };
    }

    public ArrTypes GetSetArrType
    {
        get => arrType;
        set => arrType = value;
    }
    
    public int Size
    {
        get => size;
        set => size = value;
    }

    public override string GetStructureType()
    {
        return this.arrType.ToString();
    }
}