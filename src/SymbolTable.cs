
using Antlr4.Runtime;
using System.Collections.Generic;
namespace Proyecto;

public class SymbolTable
{
    LinkedList<Identifier> table;
    private static int currentLevel;

    public class Identifier
    {
        public IToken token;
        public int type; // This might change to a more structured type
        public int level;
        public int value;
        public bool isMethod;

        public Identifier(IToken t, int tp, bool ism)
        {
            token = t;
            type = tp;
            level = currentLevel;
            value = 0;
            isMethod = ism;
        }

        public void SetValue(int v)
        {
            value = v;
        }
    }

    public SymbolTable()
    {
        table = new LinkedList<Identifier>();
        currentLevel = -1;
    }

    public void Insert(IToken id, int type, bool ism)
    {
        Identifier i = new Identifier(id, type, ism);
        table.AddFirst(i);
    }

    public Identifier Search(IToken name)
    {
        foreach (var id in table)
        {
            if (((Identifier)id).token.Text.Equals(name.Text))
            {
                return ((Identifier)id);
            }
        }
        return null;
    }

    public void OpenScope()
    {
        currentLevel++;
    }

    public void CloseScope()
    {
        
        LinkedListNode<Identifier> current = table.First;
        while (current != null)
        {
            if (current.Value.level == currentLevel)
            {
                LinkedListNode<Identifier> next = current.Next;
                table.Remove(current);
                current = next;
            }
            else
            { 
                current = current.Next;
            }
        }
        currentLevel--;
    }

    public void Print()
    {
        System.Diagnostics.Debug.WriteLine("----- START TABLE ------");
        foreach (var item in table)
        {
            var s = ((Identifier)item).token;
            System.Diagnostics.Debug.WriteLine("Name: " + s.Text + " - " + ((Identifier)item).level + " - " + ((Identifier)item).type);
        }
        System.Diagnostics.Debug.WriteLine("----- END TABLE ------");
        
        
    }
}
