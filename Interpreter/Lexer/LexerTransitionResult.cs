using Lexer.Abstractions;

namespace Interpreter.Lexer;

public record struct LexerTransitionResult : ITransitionResult<LexerState, Token>
{
    public Token? Token { get; init; }
    public LexerState? State { get; init; }
    public (int Line, int Column)? Position { get; init; }
}