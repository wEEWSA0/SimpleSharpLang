namespace Lexer.Abstractions;

public interface IStateTransitionRule<TState, TToken>
    where TState : struct, Enum
{
    Func<char, bool> Condition { get; } 
    Func<ILexerContext, ITransitionResult<TState, TToken>> Transition { get; }
}