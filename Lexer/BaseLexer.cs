using Lexer.Abstractions;

namespace Lexer;

public class BaseLexer<TState, TToken, TTokenType> : LexerTokenReader<TState, TToken, TTokenType>
    where TState : struct, Enum
    where TTokenType : struct, Enum
{
    // TODO: Разделить StateTransitionRules: публичное api с билдером команд и внутренний с лишь экзекуторами команд
    
    public BaseLexer() : this(new StringBuilderTokenBuffer())
    {
    }
 
    /// <inheritdoc/>
    public BaseLexer(ITokenBuffer tokenBuffer) : base(tokenBuffer)
    {
    }
    
    // TODO: Отойти от fail first
    public override IReadOnlyList<TToken> GetTokens(IFileReader fileReader)
    {
        List<TToken> tokens = [];
        LexerContext context = new(tokens, TokenGenerationFunc)
        {
            Line = 1,
            Column = 1,
            TokenBuffer = TokenBuffer
        };

        while (fileReader.TryNext(out var c)) 
        {
            context.CurrentSymbol = c.Value;
            
            if (!StateTransitionRules.TryGetValue(context.State, out var stateTransitionRules))
            {
                var (tokenType, message) = ErrorFunc.Invoke(context.State, c.Value);
                    
                tokens.Add(TokenGenerationFunc.Invoke(tokenType, message, context));
                return tokens;
            }
         
            StateTransitionRule<TState, TTokenType>? transitionRule = null;
            
            foreach (var stateTransitionRule in stateTransitionRules)
            {
                if (stateTransitionRule.Condition.Invoke(c.Value))
                {
                    transitionRule = stateTransitionRule;
                    break;
                }
            }
            
            if (transitionRule is null)
            {
                var (tokenType, message) = ErrorFunc.Invoke(context.State, c.Value);
                    
                tokens.Add(TokenGenerationFunc.Invoke(tokenType, message, context));
                return tokens;
            }
            
            var result = transitionRule.Transition;

            context.State = result.State;
            if (!result.Commands.IsEmpty)
            {
                var commandsExecutor = result.Commands.GetExecutor();
                commandsExecutor.Execute(context);
            }
        }
        
        // TODO: Внутренний Flush метод мб нужен

        return tokens;
    }
    
    private record LexerContext(
        List<TToken> Tokens, 
        Func<TTokenType, string?, ILexerContext, TToken> TokenGenerationFunc)
        : ILexerCommandContext<TTokenType>, ILexerContext
    {
        public required ITokenBuffer TokenBuffer { get; init; }
        public required int Line { get; set; }
        public required int Column { get; set; }
        
        public char CurrentSymbol { get; set; }
        public TState State { get; set; }

        public void AddToken(TTokenType tokenType, string? value = null)
        {
            var token = TokenGenerationFunc.Invoke(
                tokenType,
                TokenBuffer.ToString(), 
                this);
            Tokens.Add(token);
        }
    }
}