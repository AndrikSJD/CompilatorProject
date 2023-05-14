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

    public void PrintClassVarType(IToken s, int level, string type, string fatherType)
    {
        System.Diagnostics.Debug.WriteLine($"Nombre: {s.Text}");
        System.Diagnostics.Debug.WriteLine($" - Nivel: {level}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de dato: {type}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de padre: {fatherType}");
        System.Diagnostics.Debug.WriteLine("\n");
    }
    
    public override string GetStructureType()
    {
        return this.classType;
    }
}