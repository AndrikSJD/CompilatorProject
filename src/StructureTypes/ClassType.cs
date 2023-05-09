using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class ClassType: Type
{
    public readonly string Type = "class";

    public ClassType(IToken type, int lvl) : base(type, lvl)
    {
        
    }

    public override string GetStructureType()
    {
        return this.Type;
    }
}