using System.Diagnostics.CodeAnalysis;

namespace Lexer.Abstractions;

public class Transition<TState, TTokenType>
    where TState : struct, Enum
    where TTokenType : struct, Enum
{
    public Transition() { }
    
    [SetsRequiredMembers]
    public Transition(TState state, LexerCommandBuilder<TTokenType>? builder = null)
    {
        State = state;
        Commands = builder ?? new LexerCommandBuilder<TTokenType>();
    }
    
    [SetsRequiredMembers]
    public Transition(TState state, Action<LexerCommandBuilder<TTokenType>> commandBuilderAction)
    {
        Commands = new LexerCommandBuilder<TTokenType>();
        commandBuilderAction.Invoke(Commands);
        State = state;
    }
    
    public required TState State { get; init; }
    public required LexerCommandBuilder<TTokenType> Commands { get; init; }
}