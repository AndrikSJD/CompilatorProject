
using System;
using System.Collections.Generic;
using System.IO;
using SyntacticAnalysisGenerated;

namespace Proyecto;



using Antlr4.Runtime;




public class MyParserErrorListener : BaseErrorListener, IAntlrErrorListener<int>
{
    private List<string> errorMsgs = new List<string>();

    public MyParserErrorListener()
    {
        this.errorMsgs = new List<string>();
    }
    
    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        
            if (recognizer is MiniCSharpParser)
                errorMsgs.Add($"PARSER ERROR - line {line}:{charPositionInLine} {msg}");
            else if (recognizer is MiniCSharpScanner)
                errorMsgs.Add($"SCANNER ERROR - line {line}:{charPositionInLine} {msg}");
            else
                errorMsgs.Add("Other Error");
        
    }

    public bool HasErrors()
    {
        return errorMsgs.Count > 0;
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (!HasErrors()) return "0 errors";
        var builder = new System.Text.StringBuilder();
        foreach (string s in errorMsgs)
        {
            builder.AppendLine(s);
        }
        return builder.ToString();
    }

    
}