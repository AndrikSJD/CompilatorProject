﻿
using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using SyntacticAnalysisGenerated;

namespace Proyecto;



using Antlr4.Runtime;




public class ParserErrorListener : BaseErrorListener
{
    private List<string> errorMsgs = new List<string>();

    public ParserErrorListener()
    {
        this.errorMsgs = new List<string>();
    }

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
        int charPositionInLine, string msg, RecognitionException e)
    {

        if (recognizer is MiniCSharpParser)
            errorMsgs.Add($"PARSER ERROR - line {line}:{charPositionInLine} {msg}");
        else
            errorMsgs.Add("Other Error");

    }

    public bool HasErrors()
    {
        return errorMsgs.Count > 0;
    }

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line,
        int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (!HasErrors()) return "0 errores";
        var builder = new System.Text.StringBuilder();
        foreach (string s in errorMsgs)
        {
            builder.AppendLine(s);
        }

        return builder.ToString();
    }
    public override void ReportAmbiguity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, bool exact, BitSet ambigAlts,
        ATNConfigSet configs)
    {
        base.ReportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
    }

    public override void ReportAttemptingFullContext(Parser recognizer, DFA dfa, int startIndex, int stopIndex, BitSet conflictingAlts,
        SimulatorState conflictState)
    {
        base.ReportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, conflictState);
    }

    public override void ReportContextSensitivity(Parser recognizer, DFA dfa, int startIndex, int stopIndex, int prediction,
        SimulatorState acceptState)
    {
        base.ReportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, acceptState);
    }

}