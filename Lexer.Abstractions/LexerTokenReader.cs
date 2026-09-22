namespace Lexer.Abstractions;

/// <summary>
/// Лексер-каркас
/// </summary>
public abstract class LexerTokenReader<TState, TToken>
    where TState : struct, Enum
{
    protected int Line = 1, Column = 1;
    protected readonly ITokenBuffer TokenBuffer;
    
    protected TState State;
    
    public required Func<TState, ILexerContext, char, TToken> ErrorFunc { get; init; }
    public required IReadOnlyDictionary<TState, IReadOnlyCollection<IStateTransitionRule<TState, TToken>>> StateTransitionRules { get; init; }

    /// <summary>
    /// Создает новый экземпляр класса <see cref="LexerTokenReader{TState, TToken}"/>.
    /// </summary>
    /// <param name="tokenBuffer">
    /// Буфер для временного хранения символов при создании токенов.
    /// Должен быть пустым и не использоваться где-либо ещё, вне одного экземпляра класса <see cref="LexerTokenReader{TState, TToken}"/>
    /// </param>
    /// <exception cref="ArgumentNullException">Выбрасывается в случае передачи <c>null</c> в качестве <see cref="tokenBuffer"/></exception>
    protected LexerTokenReader(ITokenBuffer tokenBuffer)
    {
        ArgumentNullException.ThrowIfNull(tokenBuffer);
        TokenBuffer = tokenBuffer;
    }
    
    public abstract IReadOnlyList<TToken> GetTokens(IFileReader fileReader);
}