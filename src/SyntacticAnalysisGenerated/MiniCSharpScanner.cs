//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.12.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/user/Escritorio/Compi/Proyecto/CompilatorProject\MiniCSharpScanner.g4 by ANTLR 4.12.0

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SyntacticAnalysisGenerated {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.12.0")]
[System.CLSCompliant(false)]
public partial class MiniCSharpScanner : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		COMMENT=1, BLOCKCOMMENT=2, WS=3, CLASS=4, USING=5, VOID=6, IF=7, ELSE=8, 
		FOR=9, WHILE=10, BREAK=11, RETURN=12, READ=13, WRITE=14, NEW=15, LBRACE=16, 
		RBRACE=17, LPARENT=18, RPARENT=19, LBRACK=20, RBRACK=21, SEMICOLON=22, 
		COMMA=23, DOT=24, ASSIGN=25, INC=26, DEC=27, OR=28, AND=29, EQUAL=30, 
		NOTEQUAL=31, GT=32, GE=33, LT=34, LE=35, ADD=36, SUB=37, MUL=38, DIV=39, 
		MOD=40, NUM=41, ID=42, STRINGCONST=43, CHARCONST=44, BOOLEANCONST=45;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"COMMENT", "BLOCKCOMMENT", "WS", "CLASS", "USING", "VOID", "IF", "ELSE", 
		"FOR", "WHILE", "BREAK", "RETURN", "READ", "WRITE", "NEW", "LBRACE", "RBRACE", 
		"LPARENT", "RPARENT", "LBRACK", "RBRACK", "SEMICOLON", "COMMA", "DOT", 
		"ASSIGN", "INC", "DEC", "OR", "AND", "EQUAL", "NOTEQUAL", "GT", "GE", 
		"LT", "LE", "ADD", "SUB", "MUL", "DIV", "MOD", "DIGIT", "LETTER", "EXPRESION", 
		"LCOMMENT", "RCOMMENT", "NUM", "ID", "STRINGCONST", "CHARCONST", "BOOLEANCONST"
	};


	    public override void NotifyListeners(LexerNoViableAltException e) {
	        this.ErrorListenerDispatch.SyntaxError(this.ErrorOutput, (IRecognizer) this, 0, TokenStartLine, 
	        this.TokenStartColumn,"token invalido: '" + 
	        this.GetErrorDisplay(this.EmitEOF().InputStream.GetText(Interval.Of(this.TokenStartCharIndex,this.InputStream.Index)))  
	        + "'", (RecognitionException) e);
	    }
	 

	public MiniCSharpScanner(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public MiniCSharpScanner(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, null, null, null, "'class'", "'using'", "'void'", "'if'", "'else'", 
		"'for'", "'while'", "'break'", "'return'", "'read'", "'write'", "'new'", 
		"'{'", "'}'", "'('", "')'", "'['", "']'", "';'", "','", "'.'", "'='", 
		"'++'", "'--'", "'||'", "'&&'", "'=='", "'!='", "'>'", "'>='", "'<'", 
		"'<='", "'+'", "'-'", "'*'", "'/'", "'%'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "COMMENT", "BLOCKCOMMENT", "WS", "CLASS", "USING", "VOID", "IF", 
		"ELSE", "FOR", "WHILE", "BREAK", "RETURN", "READ", "WRITE", "NEW", "LBRACE", 
		"RBRACE", "LPARENT", "RPARENT", "LBRACK", "RBRACK", "SEMICOLON", "COMMA", 
		"DOT", "ASSIGN", "INC", "DEC", "OR", "AND", "EQUAL", "NOTEQUAL", "GT", 
		"GE", "LT", "LE", "ADD", "SUB", "MUL", "DIV", "MOD", "NUM", "ID", "STRINGCONST", 
		"CHARCONST", "BOOLEANCONST"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "MiniCSharpScanner.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static MiniCSharpScanner() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,45,327,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,2,48,7,48,2,49,
		7,49,1,0,1,0,1,0,1,0,5,0,106,8,0,10,0,12,0,109,9,0,1,0,1,0,1,1,1,1,1,1,
		1,1,1,1,1,1,1,1,5,1,120,8,1,10,1,12,1,123,9,1,1,1,1,1,1,1,1,1,1,1,1,2,
		4,2,131,8,2,11,2,12,2,132,1,2,1,2,1,3,1,3,1,3,1,3,1,3,1,3,1,4,1,4,1,4,
		1,4,1,4,1,4,1,5,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,7,1,7,1,7,1,7,1,7,1,8,1,
		8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,1,9,1,10,1,10,1,10,1,10,1,10,1,10,1,11,1,
		11,1,11,1,11,1,11,1,11,1,11,1,12,1,12,1,12,1,12,1,12,1,13,1,13,1,13,1,
		13,1,13,1,13,1,14,1,14,1,14,1,14,1,15,1,15,1,16,1,16,1,17,1,17,1,18,1,
		18,1,19,1,19,1,20,1,20,1,21,1,21,1,22,1,22,1,23,1,23,1,24,1,24,1,25,1,
		25,1,25,1,26,1,26,1,26,1,27,1,27,1,27,1,28,1,28,1,28,1,29,1,29,1,29,1,
		30,1,30,1,30,1,31,1,31,1,32,1,32,1,32,1,33,1,33,1,34,1,34,1,34,1,35,1,
		35,1,36,1,36,1,37,1,37,1,38,1,38,1,39,1,39,1,40,1,40,1,41,3,41,261,8,41,
		1,42,4,42,264,8,42,11,42,12,42,265,1,43,1,43,1,43,1,44,1,44,1,44,1,45,
		3,45,275,8,45,1,45,4,45,278,8,45,11,45,12,45,279,1,45,4,45,283,8,45,11,
		45,12,45,284,1,45,1,45,5,45,289,8,45,10,45,12,45,292,9,45,3,45,294,8,45,
		1,46,1,46,1,46,5,46,299,8,46,10,46,12,46,302,9,46,1,47,1,47,5,47,306,8,
		47,10,47,12,47,309,9,47,1,47,1,47,1,48,1,48,1,48,1,48,1,49,1,49,1,49,1,
		49,1,49,1,49,1,49,1,49,1,49,3,49,326,8,49,1,307,0,50,1,1,3,2,5,3,7,4,9,
		5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,
		35,18,37,19,39,20,41,21,43,22,45,23,47,24,49,25,51,26,53,27,55,28,57,29,
		59,30,61,31,63,32,65,33,67,34,69,35,71,36,73,37,75,38,77,39,79,40,81,0,
		83,0,85,0,87,0,89,0,91,41,93,42,95,43,97,44,99,45,1,0,7,2,0,10,10,13,13,
		2,0,42,42,47,47,1,0,42,42,3,0,9,10,13,13,32,32,1,0,48,57,2,0,65,90,97,
		122,4,0,10,10,39,39,92,92,114,114,336,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,
		0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,
		1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,
		0,0,29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,
		1,0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,
		0,0,51,1,0,0,0,0,53,1,0,0,0,0,55,1,0,0,0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,
		1,0,0,0,0,63,1,0,0,0,0,65,1,0,0,0,0,67,1,0,0,0,0,69,1,0,0,0,0,71,1,0,0,
		0,0,73,1,0,0,0,0,75,1,0,0,0,0,77,1,0,0,0,0,79,1,0,0,0,0,91,1,0,0,0,0,93,
		1,0,0,0,0,95,1,0,0,0,0,97,1,0,0,0,0,99,1,0,0,0,1,101,1,0,0,0,3,112,1,0,
		0,0,5,130,1,0,0,0,7,136,1,0,0,0,9,142,1,0,0,0,11,148,1,0,0,0,13,153,1,
		0,0,0,15,156,1,0,0,0,17,161,1,0,0,0,19,165,1,0,0,0,21,171,1,0,0,0,23,177,
		1,0,0,0,25,184,1,0,0,0,27,189,1,0,0,0,29,195,1,0,0,0,31,199,1,0,0,0,33,
		201,1,0,0,0,35,203,1,0,0,0,37,205,1,0,0,0,39,207,1,0,0,0,41,209,1,0,0,
		0,43,211,1,0,0,0,45,213,1,0,0,0,47,215,1,0,0,0,49,217,1,0,0,0,51,219,1,
		0,0,0,53,222,1,0,0,0,55,225,1,0,0,0,57,228,1,0,0,0,59,231,1,0,0,0,61,234,
		1,0,0,0,63,237,1,0,0,0,65,239,1,0,0,0,67,242,1,0,0,0,69,244,1,0,0,0,71,
		247,1,0,0,0,73,249,1,0,0,0,75,251,1,0,0,0,77,253,1,0,0,0,79,255,1,0,0,
		0,81,257,1,0,0,0,83,260,1,0,0,0,85,263,1,0,0,0,87,267,1,0,0,0,89,270,1,
		0,0,0,91,274,1,0,0,0,93,295,1,0,0,0,95,303,1,0,0,0,97,312,1,0,0,0,99,325,
		1,0,0,0,101,102,5,47,0,0,102,103,5,47,0,0,103,107,1,0,0,0,104,106,8,0,
		0,0,105,104,1,0,0,0,106,109,1,0,0,0,107,105,1,0,0,0,107,108,1,0,0,0,108,
		110,1,0,0,0,109,107,1,0,0,0,110,111,6,0,0,0,111,2,1,0,0,0,112,113,5,47,
		0,0,113,114,5,42,0,0,114,121,1,0,0,0,115,120,3,3,1,0,116,120,8,1,0,0,117,
		118,5,47,0,0,118,120,8,2,0,0,119,115,1,0,0,0,119,116,1,0,0,0,119,117,1,
		0,0,0,120,123,1,0,0,0,121,119,1,0,0,0,121,122,1,0,0,0,122,124,1,0,0,0,
		123,121,1,0,0,0,124,125,5,42,0,0,125,126,5,47,0,0,126,127,1,0,0,0,127,
		128,6,1,0,0,128,4,1,0,0,0,129,131,7,3,0,0,130,129,1,0,0,0,131,132,1,0,
		0,0,132,130,1,0,0,0,132,133,1,0,0,0,133,134,1,0,0,0,134,135,6,2,0,0,135,
		6,1,0,0,0,136,137,5,99,0,0,137,138,5,108,0,0,138,139,5,97,0,0,139,140,
		5,115,0,0,140,141,5,115,0,0,141,8,1,0,0,0,142,143,5,117,0,0,143,144,5,
		115,0,0,144,145,5,105,0,0,145,146,5,110,0,0,146,147,5,103,0,0,147,10,1,
		0,0,0,148,149,5,118,0,0,149,150,5,111,0,0,150,151,5,105,0,0,151,152,5,
		100,0,0,152,12,1,0,0,0,153,154,5,105,0,0,154,155,5,102,0,0,155,14,1,0,
		0,0,156,157,5,101,0,0,157,158,5,108,0,0,158,159,5,115,0,0,159,160,5,101,
		0,0,160,16,1,0,0,0,161,162,5,102,0,0,162,163,5,111,0,0,163,164,5,114,0,
		0,164,18,1,0,0,0,165,166,5,119,0,0,166,167,5,104,0,0,167,168,5,105,0,0,
		168,169,5,108,0,0,169,170,5,101,0,0,170,20,1,0,0,0,171,172,5,98,0,0,172,
		173,5,114,0,0,173,174,5,101,0,0,174,175,5,97,0,0,175,176,5,107,0,0,176,
		22,1,0,0,0,177,178,5,114,0,0,178,179,5,101,0,0,179,180,5,116,0,0,180,181,
		5,117,0,0,181,182,5,114,0,0,182,183,5,110,0,0,183,24,1,0,0,0,184,185,5,
		114,0,0,185,186,5,101,0,0,186,187,5,97,0,0,187,188,5,100,0,0,188,26,1,
		0,0,0,189,190,5,119,0,0,190,191,5,114,0,0,191,192,5,105,0,0,192,193,5,
		116,0,0,193,194,5,101,0,0,194,28,1,0,0,0,195,196,5,110,0,0,196,197,5,101,
		0,0,197,198,5,119,0,0,198,30,1,0,0,0,199,200,5,123,0,0,200,32,1,0,0,0,
		201,202,5,125,0,0,202,34,1,0,0,0,203,204,5,40,0,0,204,36,1,0,0,0,205,206,
		5,41,0,0,206,38,1,0,0,0,207,208,5,91,0,0,208,40,1,0,0,0,209,210,5,93,0,
		0,210,42,1,0,0,0,211,212,5,59,0,0,212,44,1,0,0,0,213,214,5,44,0,0,214,
		46,1,0,0,0,215,216,5,46,0,0,216,48,1,0,0,0,217,218,5,61,0,0,218,50,1,0,
		0,0,219,220,5,43,0,0,220,221,5,43,0,0,221,52,1,0,0,0,222,223,5,45,0,0,
		223,224,5,45,0,0,224,54,1,0,0,0,225,226,5,124,0,0,226,227,5,124,0,0,227,
		56,1,0,0,0,228,229,5,38,0,0,229,230,5,38,0,0,230,58,1,0,0,0,231,232,5,
		61,0,0,232,233,5,61,0,0,233,60,1,0,0,0,234,235,5,33,0,0,235,236,5,61,0,
		0,236,62,1,0,0,0,237,238,5,62,0,0,238,64,1,0,0,0,239,240,5,62,0,0,240,
		241,5,61,0,0,241,66,1,0,0,0,242,243,5,60,0,0,243,68,1,0,0,0,244,245,5,
		60,0,0,245,246,5,61,0,0,246,70,1,0,0,0,247,248,5,43,0,0,248,72,1,0,0,0,
		249,250,5,45,0,0,250,74,1,0,0,0,251,252,5,42,0,0,252,76,1,0,0,0,253,254,
		5,47,0,0,254,78,1,0,0,0,255,256,5,37,0,0,256,80,1,0,0,0,257,258,7,4,0,
		0,258,82,1,0,0,0,259,261,7,5,0,0,260,259,1,0,0,0,261,84,1,0,0,0,262,264,
		3,83,41,0,263,262,1,0,0,0,264,265,1,0,0,0,265,263,1,0,0,0,265,266,1,0,
		0,0,266,86,1,0,0,0,267,268,5,47,0,0,268,269,5,42,0,0,269,88,1,0,0,0,270,
		271,5,42,0,0,271,272,5,47,0,0,272,90,1,0,0,0,273,275,3,73,36,0,274,273,
		1,0,0,0,274,275,1,0,0,0,275,293,1,0,0,0,276,278,3,81,40,0,277,276,1,0,
		0,0,278,279,1,0,0,0,279,277,1,0,0,0,279,280,1,0,0,0,280,294,1,0,0,0,281,
		283,3,81,40,0,282,281,1,0,0,0,283,284,1,0,0,0,284,282,1,0,0,0,284,285,
		1,0,0,0,285,286,1,0,0,0,286,290,5,46,0,0,287,289,3,81,40,0,288,287,1,0,
		0,0,289,292,1,0,0,0,290,288,1,0,0,0,290,291,1,0,0,0,291,294,1,0,0,0,292,
		290,1,0,0,0,293,277,1,0,0,0,293,282,1,0,0,0,294,92,1,0,0,0,295,300,3,83,
		41,0,296,299,3,83,41,0,297,299,3,81,40,0,298,296,1,0,0,0,298,297,1,0,0,
		0,299,302,1,0,0,0,300,298,1,0,0,0,300,301,1,0,0,0,301,94,1,0,0,0,302,300,
		1,0,0,0,303,307,5,34,0,0,304,306,9,0,0,0,305,304,1,0,0,0,306,309,1,0,0,
		0,307,308,1,0,0,0,307,305,1,0,0,0,308,310,1,0,0,0,309,307,1,0,0,0,310,
		311,5,34,0,0,311,96,1,0,0,0,312,313,5,39,0,0,313,314,8,6,0,0,314,315,5,
		39,0,0,315,98,1,0,0,0,316,317,5,116,0,0,317,318,5,114,0,0,318,319,5,117,
		0,0,319,326,5,101,0,0,320,321,5,102,0,0,321,322,5,97,0,0,322,323,5,108,
		0,0,323,324,5,115,0,0,324,326,5,101,0,0,325,316,1,0,0,0,325,320,1,0,0,
		0,326,100,1,0,0,0,16,0,107,119,121,132,260,265,274,279,284,290,293,298,
		300,307,325,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace SyntacticAnalysisGenerated
