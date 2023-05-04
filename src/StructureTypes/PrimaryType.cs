using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class PrimaryType: Type
{
    public enum PrimaryTypes
    {
        Int,
        Char,
        Boolean,
        Double,
        String,
        Unknown,
    }

    private PrimaryTypes Type;

    public PrimaryType(IToken t, PrimaryTypes pt, int lvl) : base(t, lvl)
    {
        Type = pt;
    }

    public PrimaryType() : base(null, -1)
    {
        
    }
    
    public static PrimaryTypes showType(string type)
    {
        return type switch
        {
            "int" => PrimaryTypes.Int,
            "string" => PrimaryTypes.String,
            "boolean" => PrimaryTypes.Boolean,
            "double" => PrimaryTypes.Double,
            "char" => PrimaryTypes.Char,
            _ => PrimaryTypes.Unknown,
        };
    }

    public PrimaryTypes TypeGetSet
    {
        get => Type;
        set => Type = value;
    }
}