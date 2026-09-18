namespace Interpreter.Lexer.Abstract;

using Position = (int line, int column);

public class LexerTokenReader<TState, TToken>
    where TState : struct, Enum
{
    private int _line = 1, _column = 1;
    private readonly ITokenBuffer _tokenBuffer;
    
    private TState _state;
    
    public record struct TransitionInfo()
    {
        public TState NewState { get; init; } // TODO: Подумать, что делать с лучаем когда NewState == OldState. Стоит ли перезаписывать?
        public bool IsNeedFlush { get; init; } = false;
        public bool IsNeedAppend { get; init; } = false;
    }
    
    public required IReadOnlyDictionary<TState, Func<char, TransitionInfo>> StateTransitions { get; init; }
    // Запрашиваем правила переходов по автомату
    
    public required IReadOnlyDictionary<TState, Func<ITokenBuffer, Position, TToken>> TokenGenerators { get; init; }
    // Запрашиваем правила генерации токенов по переходу

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
    
    public IReadOnlyList<TToken> GetTokens(IFileReader fileReader)
    {
        List<TToken> tokens = [];
    
        while (fileReader.TryNext(out var c))
        {
            if (TryGetToken(c.Value, out TToken? token))
            {
                tokens.Add(token);
            }
        }
        
        return tokens;
    }
    
    private bool TryGetToken(char c, out TToken? token)
    {
        var stateTransition = StateTransitions[_state];

        var transitionInfo =  stateTransition.Invoke(c);

        _state = transitionInfo.NewState;

        if (transitionInfo.IsNeedAppend)
        {
            _tokenBuffer.Append(c);
        }

        if (transitionInfo.IsNeedFlush)
        {
            token = TokenGenerators[_state].Invoke(_tokenBuffer, (_line, _column));
            _tokenBuffer.Clear();
            return true;
        }

        token = null;
        return false;
    }
}