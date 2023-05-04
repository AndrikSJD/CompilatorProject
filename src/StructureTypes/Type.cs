
using System;
using Antlr4.Runtime;


namespace Proyecto.StructureTypes;

public  abstract class Type
{

    private IToken _token;
    private int level;

    protected Type(IToken token, int level)
    {
    this._token = token;
    this.level = level;
    }

    public IToken GetToken()
    {
        return _token;
    }
    

    public int Level
    {
        get => level;
        set => level = value;
    }
}