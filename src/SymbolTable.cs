using System;
using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Proyecto.StructureTypes;
using Type =Proyecto.StructureTypes.Type;

namespace Proyecto;

public class SymbolTable
{
    LinkedList<Object>table;
    public int currentLevel;
    

    public int getLevel() {
        return this.currentLevel;
    }
    public SymbolTable()
    {
        table = new LinkedList<Object>();
        currentLevel = -1;
    }

    //TODO CAMBIARLO A HASHMAP
    public void Insert(Type typeStruct)
    {

        table.AddLast(typeStruct);
        
    }

    public Type? Search(string name)
    {
        foreach (Type id in table)
        {
            if (id.GetToken().Text.Equals(name))
            {
                return id;
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
        
        table.Remove(new Func<Type, bool>(n => n.Level == currentLevel));
        currentLevel--;
    }

    public void Print()
    {
        //TODO CAMBIAR IMPRESION
        System.Diagnostics.Debug.WriteLine("----- INICIO TABLA ------");
        for (int i = 0; i < table.Count; i++)
        {
            IToken s = ((Type)table.ElementAt(i)).GetToken();
            string printMessage = "Nombre " + s.Text + " - Nivel global: " + ((Type)table.ElementAt(i)).Level + "\n";
            if(table.ElementAt(i).GetType() == typeof(MethodType))
            {
                printMessage += "Tipo dato: " + ((MethodType)table.ElementAt(i)).ReturnTypeGetSet ;
                printMessage += " - Cuenta de parametros: " + ((MethodType)table.ElementAt(i)).ParamsNumGetSet + "\n";
                if (((MethodType)table.ElementAt(i)).ParamsNumGetSet > 0)
                {
                    ((MethodType)table.ElementAt(i)).PrintMethod();
                }
            }
            if(table.ElementAt(i).GetType() == typeof(ClassVarType))
            {
                printMessage += "Tipo dato: " + (( ClassVarType)table.ElementAt(i)).classType ;
                printMessage += " - Tipo de padre: " + (( ClassVarType)table.ElementAt(i)).Type + "\n";
            }
            else if(table.ElementAt(i).GetType() == typeof(PrimaryType))
            {
                printMessage += " - Tipo dato: " + ((PrimaryType)table.ElementAt(i)).TypeGetSet + "\n";

            }
            else if(table.ElementAt(i).GetType() == typeof(ClassType))
            {
                printMessage += " - Tipo dato: " + ((ClassType)table.ElementAt(i)).type + "\n";
            }
            else if (table.ElementAt(i).GetType() == typeof(ArrayType))
            {
                printMessage += " - Tipo dato: " + ((ArrayType)table.ElementAt(i)).Type;
                printMessage += " - Tipo base: " + ((ArrayType)table.ElementAt(i)).GetSetArrType;
            }
            System.Diagnostics.Debug.WriteLine(printMessage);
        }
        System.Diagnostics.Debug.WriteLine("----- FIN TABLA ------");
    }
        
        
    
}
