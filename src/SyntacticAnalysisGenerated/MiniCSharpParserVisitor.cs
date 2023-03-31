//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.11.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/Admin/Desktop/Compi/CompilatorProject\MiniCSharpParser.g4 by ANTLR 4.11.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace SyntacticAnalysisGenerated {
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="MiniCSharpParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public interface IMiniCSharpParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProgram([NotNull] MiniCSharpParser.ProgramContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.using"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsing([NotNull] MiniCSharpParser.UsingContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitVarDecl([NotNull] MiniCSharpParser.VarDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.classDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitClassDecl([NotNull] MiniCSharpParser.ClassDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.methodDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMethodDecl([NotNull] MiniCSharpParser.MethodDeclContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.formPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFormPars([NotNull] MiniCSharpParser.FormParsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitType([NotNull] MiniCSharpParser.TypeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] MiniCSharpParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBlock([NotNull] MiniCSharpParser.BlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.actPars"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitActPars([NotNull] MiniCSharpParser.ActParsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.condition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondition([NotNull] MiniCSharpParser.ConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.condTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondTerm([NotNull] MiniCSharpParser.CondTermContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.condFact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCondFact([NotNull] MiniCSharpParser.CondFactContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.cast"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCast([NotNull] MiniCSharpParser.CastContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.expr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExpr([NotNull] MiniCSharpParser.ExprContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.term"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTerm([NotNull] MiniCSharpParser.TermContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.factor"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFactor([NotNull] MiniCSharpParser.FactorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.designator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDesignator([NotNull] MiniCSharpParser.DesignatorContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.relop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRelop([NotNull] MiniCSharpParser.RelopContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.arrayMethods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayMethods([NotNull] MiniCSharpParser.ArrayMethodsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="MiniCSharpParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParameters([NotNull] MiniCSharpParser.ParametersContext context);
}
} // namespace SyntacticAnalysisGenerated
