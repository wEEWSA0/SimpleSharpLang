namespace Interpreter.Lexer.Abstract;

public class LexerTokenReader<TState, TToken>
    where TState : struct, Enum
{
    private int _line = 1, _column = 1;
    private readonly IWord _word;
    private readonly TState _defaultState;
    
    private TState _state;
    
    public required Func<TState, ILexerOptions, char, TToken> ErrorFunc { get; init; }
    public required IReadOnlyList<(Func<char, bool> Condition, Func<ILexerOptions, IResponse<TState, TToken>> ResponseAction)> StartupEvents { get; init; }
    public required IReadOnlyDictionary<TState, List<(Func<char, bool> Condition, Func<ILexerOptions, IResponse<TState, TToken>> ResponseAction)>> StateEvents { get; init; }

    public LexerTokenReader(IWord word, TState defaultState)
    {
        _word = word;
        _word.Clear();
        _defaultState = defaultState;
    }
    
    // TODO: Отойти от fail first
    public IReadOnlyList<TToken> GetTokens(IFileReader fileReader)
    {
        List<TToken> tokens = [];

        while (fileReader.TryNext(out var c)) 
            // TODO: Общий метод с учетом State.None + _word как буффер для отката (возможны разные реализации и подходы в целом)
        {
            Func<ILexerOptions, IResponse<TState, TToken>>? func = null;
            
            if (EqualityComparer<TState>.Default.Equals(_state, _defaultState))
            {
                foreach (var startupEvent in StartupEvents)
                {
                    if (startupEvent.Condition.Invoke(c.Value))
                    {
                        func = startupEvent.ResponseAction;
                        break;
                    }
                }
            }
            else
            {
                if (!StateEvents.TryGetValue(_state, out var events))
                {
                    var errorToken = ErrorFunc.Invoke(_state, new LexerOptions
                    {
                        Column = _column,
                        Line = _line,
                        Word = _word
                    }, c.Value);
                    
                    tokens.Add(errorToken);
                    return tokens;
                }
                
                foreach (var stateEvent in events)
                {
                    if (stateEvent.Condition.Invoke(c.Value))
                    {
                        func = stateEvent.ResponseAction;
                        break;
                    }
                }
            }
            
            if (func is null)
            {
                var errorToken = ErrorFunc.Invoke(_state, new LexerOptions
                {
                    Column = _column,
                    Line = _line,
                    Word = _word
                }, c.Value);
                    
                tokens.Add(errorToken);
                return tokens;
            }
            
            var result = func.Invoke(new LexerOptions
            {
                Word = _word,
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
        public IWord Word { get; init; } // TODO: Убрать прямое обращение, данные в идеале лишь в Response меняются
    }
}