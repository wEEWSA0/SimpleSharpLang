namespace Interpreter.Lexer.Abstract;

public interface IResponse<TState, TToken>
    where TState : struct, Enum
{
    TToken? Token { get; init; }
    TState? State { get; init; }
    (int Line, int Column)? Position { get; init; }
}