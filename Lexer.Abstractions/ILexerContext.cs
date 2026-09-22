namespace Lexer.Abstractions;

public interface ILexerContext
{
    int Line { get; }
    int Column { get; }
    // ITokenBuffer TokenBuffer { get; } TODO: Убран, вероятно временно
}