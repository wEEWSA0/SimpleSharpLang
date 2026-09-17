using Interpreter.Lexer.Abstract;

namespace Interpreter.Lexer.Realization;

public record struct LexerResponse : IResponse<LexerState, Token>
{
    public Token? Token { get; init; }
    public LexerState? State { get; init; }
    public (int Line, int Column)? Position { get; init; }
}