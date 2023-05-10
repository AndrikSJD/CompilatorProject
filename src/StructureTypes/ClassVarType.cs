
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class ClassVarType : Type
{
    public readonly string Type = "ClassType";
    public readonly string classType;


    public ClassVarType(IToken tk, int lvl, string tf) : base(tk, lvl)
    {
        classType = tf;
        
    }

    
    public override string GetStructureType()
    {
        return this.classType;
    }
}