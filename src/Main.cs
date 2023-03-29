using System;
using Antlr4.Runtime;
using SyntacticAnalysisGenerated;

namespace Proyecto;

public class Main
{

    public static void main(string[] args)
    {
        ICharStream input = null;
        MiniCSharpScanner scanner = null;
        CommonTokenStream tokens = null;
        MiniCSharpParser parser = null;

        try
        {
            input = CharStreams.fromPath("testCode.txt");
            scanner = new MiniCSharpScanner(input);
            tokens = new CommonTokenStream(scanner);
            parser = new MiniCSharpParser(tokens);

            Console.WriteLine("Vamos biensdf1");
            parser.program();
            Console.WriteLine("Vamos bien");
        }
        catch (Exception e)
        {
            Console.WriteLine("Vamos mal" + e);
            throw;
        }
    }
}