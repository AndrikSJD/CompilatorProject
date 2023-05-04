using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class ClassType: Type
{
    public readonly string type = "class";

    public ClassType(IToken type, int lvl) : base(type, lvl)
    {
        
    }
}