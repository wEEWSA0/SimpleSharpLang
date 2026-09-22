namespace Lexer.Abstractions;

public interface ITransitionResult<TState, TToken>
    where TState : struct, Enum
{
    TToken? Token { get; init; } // TODO
    TState? State { get; init; }
    (int Line, int Column)? Position { get; init; }
    // [ new AppendCurrentToken() ]
}