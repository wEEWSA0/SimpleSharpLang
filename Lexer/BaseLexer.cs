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
            context.CurrentChar = c.Value;
            
            if (!StateTransitionRules.TryGetValue(context.State, out var stateTransitionRules))
            {
                return InvokeErrorFunc(tokens, context, c.Value);
            }
         
            var transitionRule = stateTransitionRules.FirstOrDefault(rule => rule.Condition.Invoke(c.Value));
            
            if (transitionRule is null)
            {
                return InvokeErrorFunc(tokens, context, c.Value);
            }
            
            var transition = transitionRule.Transition;
            context.State = transition.State;
            
            if (!transition.Commands.IsEmpty)
            {
                var commandsExecutor = transition.Commands.GetExecutor();
                commandsExecutor.Execute(context);
            }
        }
        
        return tokens;
    }

    private IReadOnlyList<TToken> InvokeErrorFunc(List<TToken> tokens, LexerContext context, char c)
    {
        var (tokenType, message) = ErrorFunc.Invoke(context.State, c);
                    
        tokens.Add(TokenGenerationFunc.Invoke(tokenType, message, context));
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
        
        public char CurrentChar { get; set; }
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