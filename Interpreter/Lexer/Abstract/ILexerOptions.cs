namespace Interpreter.Lexer.Abstract;

public interface ILexerOptions
{
    int Line { get; }
    int Column { get; }
    ITokenBuffer TokenBuffer { get; }
}