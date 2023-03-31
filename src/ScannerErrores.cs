using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using SyntacticAnalysisGenerated;

namespace Proyecto;

public class ScannerErrorListener : IAntlrErrorListener<int>
{
    
    public LinkedList<string> mensajesError;

    public ScannerErrorListener ( )
    {
        mensajesError = new LinkedList<string>();
    }
    
    
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        if (recognizer.GetType() == typeof(MiniCSharpScanner))
        {
            
            mensajesError.AddFirst(new String("Error en el escaner en la linea "+line+":"+charPositionInLine + " " + "Mensaje de error: "+ msg ));
        }
        else
        {
            mensajesError.AddFirst(new String("Otro error"));
        }
    }
    
        
    public bool HasErrors ( )
    {
     
        return mensajesError.Count > 0;
    }

    public override string ToString()
    {
        if ( !HasErrors() ) return "0 errores";
        StringBuilder builder = new StringBuilder();
        foreach (var error in mensajesError)
        {
            Console.WriteLine(error);
            
            builder.Append(error + "\n");
        }
        return builder.ToString();
    }
}