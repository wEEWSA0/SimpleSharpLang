namespace Interpreter.Lexer.Abstract;

public class LexerTokenReader<TState, TToken>
    where TState : struct, Enum
{
    private int _line = 1, _column = 1;
    private readonly ITokenBuffer _tokenBuffer;
    
    private TState _state;
    
    public required Func<TState, ILexerOptions, char, TToken> ErrorFunc { get; init; }
    public required IReadOnlyDictionary<TState, List<(Func<char, bool> Condition, Func<ILexerOptions, IResponse<TState, TToken>> ResponseAction)>> StateTransitions { get; init; }

    /// <summary>
    /// Создает новый экземпляр класса <see cref="LexerTokenReader{TState, TToken}"/>.
    /// </summary>
    /// <param name="tokenBuffer">
    /// Буфер для временного хранения символов при создании токенов.
    /// Должен быть пустым и не использоваться где-либо ещё, вне одного экземпляра класса <see cref="LexerTokenReader{TState, TToken}"/>
    /// </param>
    /// <exception cref="ArgumentNullException">Выбрасывается в случае передачи <c>null</c> в качестве <see cref="tokenBuffer"/></exception>
    public LexerTokenReader(ITokenBuffer tokenBuffer)
    {
        ArgumentNullException.ThrowIfNull(tokenBuffer);
        _tokenBuffer = tokenBuffer;
    }
    
    // TODO: Отойти от fail first
    public IReadOnlyList<TToken> GetTokens(IFileReader fileReader)
    {
        List<TToken> tokens = [];

        while (fileReader.TryNext(out var c)) 
            // TODO: Общий метод с учетом State.None + _tokenBuffer как буффер для отката (возможны разные реализации и подходы в целом)
        {
            if (!StateTransitions.TryGetValue(_state, out var stateTransitions))
            {
                var errorToken = ErrorFunc.Invoke(_state, new LexerOptions
                {
                    Column = _column,
                    Line = _line,
                    TokenBuffer = _tokenBuffer
                }, c.Value);
                    
                tokens.Add(errorToken);
                return tokens;
            }
         
            Func<ILexerOptions, IResponse<TState, TToken>>? transitionFunc = null;
            
            foreach (var stateTransition in stateTransitions)
            {
                if (stateTransition.Condition.Invoke(c.Value))
                {
                    transitionFunc = stateTransition.ResponseAction;
                    break;
                }
            }
            
            if (transitionFunc is null)
            {
                var errorToken = ErrorFunc.Invoke(_state, new LexerOptions
                {
                    Column = _column,
                    Line = _line,
                    TokenBuffer = _tokenBuffer
                }, c.Value);
                    
                tokens.Add(errorToken);
                return tokens;
            }
            
            var result = transitionFunc.Invoke(new LexerOptions
            {
                TokenBuffer = _tokenBuffer,
                Column = _column,
                Line = _line
            });

            if (result.State is not null)
                _state = result.State.Value;
            if (result.Token is not null)
                tokens.Add(result.Token);
            if (result.Position is not null)
            {
                _line = result.Position.Value.Line;
                _column = result.Position.Value.Column;
            }
        }
        
        // TODO: Внутренний Flush метод мб нужен

        return tokens;
    }
    
    private record struct LexerOptions : ILexerOptions
    {
        public int Line { get; init; }
        public int Column { get; init; }
        public ITokenBuffer TokenBuffer { get; init; } // TODO: Убрать прямое обращение, данные в идеале лишь в Response меняются
    }
}