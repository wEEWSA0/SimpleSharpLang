using System.Diagnostics.CodeAnalysis;

namespace Lexer.Abstractions;

public class StateTransitionRule<TState, TTokenType>
    where TState : struct, Enum
    where TTokenType : struct, Enum
{
    public required Func<char, bool> Condition { get; init; } 
    public required Transition<TState, TTokenType> Transition { get; init; }

    public StateTransitionRule() { }

    [SetsRequiredMembers]
    public StateTransitionRule(Func<char, bool> condition, Transition<TState, TTokenType> transition)
    {
        Condition = condition;
        Transition = transition;
    }
}