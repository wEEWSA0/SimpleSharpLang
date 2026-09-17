namespace Interpreter.Lexer.Abstract;

public interface ILexerOptions
{
    int Line { get; }
    int Column { get; }
    IWord Word { get; }
}