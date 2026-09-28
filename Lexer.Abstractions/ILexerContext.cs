namespace Lexer.Abstractions;

public interface ILexerContext
{
    int Line { get; }
    int Column { get; }
}