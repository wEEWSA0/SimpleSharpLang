namespace Lexer.Abstractions;

/// <summary>
/// Лексер-каркас
/// </summary>
public abstract class LexerTokenReader<TState, TToken, TTokenType>
    where TState : struct, Enum
    where TTokenType : struct, Enum
{
    protected readonly ITokenBuffer TokenBuffer;
    
    public required Func<TTokenType, string?, ILexerContext, TToken> TokenGenerationFunc { get; init; }
    public required Func<TState, char, (TTokenType Type, string Message)> ErrorFunc { get; init; }
    public required IReadOnlyDictionary<TState, IReadOnlyCollection<StateTransitionRule<TState, TTokenType>>> StateTransitionRules { get; init; }

    /// <summary>
    /// Создает новый экземпляр класса <see cref="LexerTokenReader{TState, TToken, TTokenType}"/>.
    /// </summary>
    /// <param name="tokenBuffer">
    /// Буфер для временного хранения символов при создании токенов.
    /// Должен быть пустым и не использоваться где-либо ещё, вне одного экземпляра класса <see cref="LexerTokenReader{TState, TToken, TTokenType}"/>
    /// </param>
    /// <exception cref="ArgumentNullException">Выбрасывается в случае передачи <c>null</c> в качестве <see cref="tokenBuffer"/></exception>
    protected LexerTokenReader(ITokenBuffer tokenBuffer)
    {
        ArgumentNullException.ThrowIfNull(tokenBuffer);
        TokenBuffer = tokenBuffer;
    }
    
    public abstract IReadOnlyList<TToken> GetTokens(IFileReader fileReader);
}