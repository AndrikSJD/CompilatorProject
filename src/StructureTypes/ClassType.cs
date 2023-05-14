using System.Collections.Generic;
using Antlr4.Runtime;

namespace Proyecto.StructureTypes;

public class ClassType: Type
{
    public readonly string Type = "class";
    public  LinkedList<Type> parametersL = new LinkedList<Type>();
    public ClassType(IToken type, int lvl) : base(type, lvl)
    {
        
    }

    public void PrintClass(IToken s, int level, string type)
    {
        System.Diagnostics.Debug.WriteLine($"Nombre: {s.Text}");
        System.Diagnostics.Debug.WriteLine($" - Nivel: {level}");
        System.Diagnostics.Debug.WriteLine($" - Tipo de dato: {type}");
        System.Diagnostics.Debug.WriteLine("\n");
    }

    public override string GetStructureType()
    {
        return this.GetToken().Text;
    }
}