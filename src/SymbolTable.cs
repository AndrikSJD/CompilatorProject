using System;
using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Proyecto.StructureTypes;
using Type =Proyecto.StructureTypes.Type;

namespace Proyecto;

public class SymbolTable
{
    LinkedList<Object>table;
    public int currentLevel;
    public MethodType currentMethod;
    public ClassType? currentClass;
    

    public int getLevel() {
        return this.currentLevel;
    }
    public SymbolTable()
    {
        table = new LinkedList<Object>();
        currentLevel = -1;
    }

   
    public void Insert(Type typeStruct)
    {

        table.AddLast(typeStruct);
        
    }

    public Type? Search(string name)
    {
        foreach (Type id in table)
        {
            //if id.level <= levelActual
            if (id.GetToken().Text.Equals(name))
            {
                return id;
            }
        }
        return null;
    }
    
    public Type? BuscarCustomVar(string id)
    {
        foreach (Type? i in table)
        {
            //&& i.Level <= nivelActual
            if (i.GetToken().Text.Equals(id) && i is ClassVarType)
                return i;
        }
        return null;
    }

    public int searchIndex(string nombreMetodo)
    {
        // Busco la posicion del metodo en la tabla
        for (int i = 0; i < table.Count; i++)
        {
            if (((Type)table.ElementAt(i)).GetToken().Text.Equals(nombreMetodo))
            {
                return i;
            }
        }

        return 0;
    }

    public void DeleteParametersBody(string nombreMetodo)
    {
        int posMethod = searchIndex(nombreMetodo);
        LinkedList<object> slicedList = new LinkedList<object>();

        if (posMethod != 0)
        {
            // Agrego a la lista los elementos que estan antes del metodo
            for (int j = 0; j <= posMethod; j++)
            {
                slicedList.AddLast(table.ElementAt(j));
            }

            table = slicedList;

            return;
        }
        
        System.Diagnostics.Debug.WriteLine("No se encontro el metodo");
    }

    public void OpenScope()
    {
        currentLevel++;
    }

    public void CloseScope()
    {
        
        table.Remove(new Func<Type, bool>(n => n.Level == currentLevel));
        currentLevel--;
    }

    public void Print()
    {
        System.Diagnostics.Debug.WriteLine("--------------- INICIA TABLA ---------------");
        for (int i = 0; i < table.Count; i++)
        {
            IToken s = ((Type)table.ElementAt(i)).GetToken();
            if(table.ElementAt(i).GetType() == typeof(ClassType))
            {
                ((ClassType)table.ElementAt(i)).PrintClass(s, ((Type)table.ElementAt(i)).Level, ((ClassType)table.ElementAt(i)).Type);
            }
            else if(table.ElementAt(i).GetType() == typeof(ClassVarType))
            {
                ((ClassVarType)table.ElementAt(i)).PrintClassVarType(s, ((Type)table.ElementAt(i)).Level, 
                    ((ClassVarType)table.ElementAt(i)).Type, ((ClassVarType)table.ElementAt(i)).Type);
            }
            else if(table.ElementAt(i).GetType() == typeof(MethodType))
            {
                ((MethodType)table.ElementAt(i)).PrintMethod();
            }
            else if(table.ElementAt(i).GetType() == typeof(PrimaryType))
            {
                ((PrimaryType)table.ElementAt(i)).PrintPrimaryType(s, ((Type)table.ElementAt(i)).Level, ((PrimaryType)table.ElementAt(i)).TypeGetSet);
            }
            else if (table.ElementAt(i).GetType() == typeof(ArrayType))
            {
                ((ArrayType)table.ElementAt(i)).PrintArrayType(s, ((Type)table.ElementAt(i)).Level, 
                    ((ArrayType)table.ElementAt(i)).Type, ((ArrayType)table.ElementAt(i)).GetSetArrType);
            }
        }
        System.Diagnostics.Debug.WriteLine("--------------- TERMINA TABLA ---------------");
    }

        private string GetIndentation(int indentationLevel)
        {
            return new string(' ', indentationLevel * 2);
        }

}