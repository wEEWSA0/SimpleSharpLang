namespace Lexer.Abstractions;

public interface ILexerCommandContext
{
    int Line { get; set; }
    int Column { get; set; }
    char CurrentChar { get; }
    ITokenBuffer TokenBuffer { get; }
}

public interface ILexerCommandContext<in TTokenType> : ILexerCommandContext
    where TTokenType : struct, Enum
{
    void AddToken(TTokenType tokenType, string? value = null);
}