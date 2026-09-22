using Lexer.Abstractions;

namespace Lexer;

public class BaseLexer<TState, TToken> : LexerTokenReader<TState, TToken>
    where TState : struct, Enum
{
    public BaseLexer() : base(new StringBuilderTokenBuffer())
    {
    }
    
    public BaseLexer(ITokenBuffer tokenBuffer) : base(tokenBuffer)
    {
    }

    // TODO: Отойти от fail first
    public override IReadOnlyList<TToken> GetTokens(IFileReader fileReader)
    {
        List<TToken> tokens = [];

        while (fileReader.TryNext(out var c)) 
            // TODO: Общий метод с учетом State.None + TokenBuffer как буффер для отката (возможны разные реализации и подходы в целом)
        {
            if (!StateTransitionRules.TryGetValue(State, out var stateTransitionRules))
            {
                var errorToken = ErrorFunc.Invoke(State, new LexerContext
                {
                    Column = Column,
                    Line = Line,
                    // TokenBuffer = TokenBuffer
                }, c.Value);
                    
                tokens.Add(errorToken);
                return tokens;
            }
         
            Func<ILexerContext, ITransitionResult<TState, TToken>>? transitionFunc = null;
            
            foreach (var stateTransitionRule in stateTransitionRules)
            {
                if (stateTransitionRule.Condition.Invoke(c.Value))
                {
                    transitionFunc = stateTransitionRule.Transition;
                    break;
                }
            }
            
            if (transitionFunc is null)
            {
                var errorToken = ErrorFunc.Invoke(State, new LexerContext
                {
                    Column = Column,
                    Line = Line,
                    // TokenBuffer = TokenBuffer
                }, c.Value);
                    
                tokens.Add(errorToken);
                return tokens;
            }
            
            var result = transitionFunc.Invoke(new LexerContext
            {
                // TokenBuffer = TokenBuffer,
                Column = Column,
                Line = Line
            });

            if (result.State is not null)
                State = result.State.Value;
            if (result.Token is not null)
                tokens.Add(result.Token);
            if (result.Position is not null)
            {
                Line = result.Position.Value.Line;
                Column = result.Position.Value.Column;
            }
        }
        
        // TODO: Внутренний Flush метод мб нужен

        return tokens;
    }
    
    private record struct LexerContext : ILexerContext
    {
        public int Line { get; init; }
        public int Column { get; init; }
        // public ITokenBuffer TokenBuffer { get; init; } // TODO: Убрать прямое обращение, данные в идеале лишь в Response меняются
    }
}