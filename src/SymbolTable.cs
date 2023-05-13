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
            //if id.level <= levelActual
            if (id.GetToken().Text.Equals(name))
            {
                return id;
            }
        }
        return null;
    }
    public bool Sacar(string id)
    {
        foreach (Type? i in table)
        {
            if (i.GetToken().Text.Equals(id) && i.Level == currentLevel)
            {
                table.Remove(i);
                return true;
            }
        }
        return false;
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
        System.Diagnostics.Debug.WriteLine("*************** INICIO TABLA  *****************");
        for (int i = 0; i < table.Count; i++)
        {
            IToken s = ((Type)table.ElementAt(i)).GetToken();
            if(table.ElementAt(i).GetType() == typeof(MethodType))
            {
          
                    ((MethodType)table.ElementAt(i)).PrintMethod();

               
            }
            if(table.ElementAt(i).GetType() == typeof(ClassVarType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre " + s.Text + " - Nivel global: " + ((Type)table.ElementAt(i)).Level);
                System.Diagnostics.Debug.WriteLine("Tipo dato: " + (( ClassVarType)table.ElementAt(i)).classType);
                System.Diagnostics.Debug.WriteLine(" - Tipo de padre: " + (( ClassVarType)table.ElementAt(i)).Type );
                System.Diagnostics.Debug.WriteLine("\n");

            }
            else if(table.ElementAt(i).GetType() == typeof(PrimaryType))
            {

                System.Diagnostics.Debug.WriteLine("Nombre " + s.Text + " - Nivel global: " + ((Type)table.ElementAt(i)).Level);
                System.Diagnostics.Debug.WriteLine(" - Tipo dato: " + ((PrimaryType)table.ElementAt(i)).TypeGetSet );
                System.Diagnostics.Debug.WriteLine("\n");


            }
            else if(table.ElementAt(i).GetType() == typeof(ClassType))
            {

                System.Diagnostics.Debug.WriteLine("\n"+"-------------------- CLASE ------------------");
                System.Diagnostics.Debug.WriteLine("Nombre " + s.Text + " - Nivel global: " + ((Type)table.ElementAt(i)).Level);
                System.Diagnostics.Debug.WriteLine(" - Tipo dato: " + ((ClassType)table.ElementAt(i)).Type);
                System.Diagnostics.Debug.WriteLine("-------------------- FIN CLASE -----------------" + "\n");

            }
            else if (table.ElementAt(i).GetType() == typeof(ArrayType))
            {
                System.Diagnostics.Debug.WriteLine("Nombre " + s.Text + " - Nivel global: " + ((Type)table.ElementAt(i)).Level);
                System.Diagnostics.Debug.WriteLine(" - Tipo dato: " + ((ArrayType)table.ElementAt(i)).Type);
                System.Diagnostics.Debug.WriteLine(" - Tipo base: " + ((ArrayType)table.ElementAt(i)).GetSetArrType);
                System.Diagnostics.Debug.WriteLine("\n");

            }
        }
        System.Diagnostics.Debug.WriteLine("**************** FIN TABLA ****************");
    }
        
        
    
}
