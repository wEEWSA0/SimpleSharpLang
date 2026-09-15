using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Lexer;

public class LexerReader
{
    private int _line = 1, _column = 1;
    private LexerReadState _state = LexerReadState.None;
    private readonly IWord _word;
    
    private readonly Func<char, LexerStageResult?>[] _stages;

    public LexerReader(IWord word)
    {
        _word = word;
        _stages = [
            TryProcessEndOfLine
        ];
    }
    
    public bool ReadToken(char c, [NotNullWhen(true)] out Token? token)
    {
        // TODO: Обработка комментариев
        // TODO: Обработка строк
        
        for (var i = 0; i < _stages.Length; i++)
        {
            var stage = _stages[i];
            var result = stage.Invoke(c);

            if (!result.HasValue)
                continue;

            if (result.Value.SkipNextStages || i + 1 == _stages.Length)
            {
                token = result.Value.Token;
                return token != null;
            }
        }
        
        token = null;
        return false;
    }

    public bool Flush([NotNullWhen(true)] out Token? token)
    {
        // TODO: Доделать
        token = null;
        return token is not null;
    }
    
    LexerStageResult? TryProcessEndOfLine(char c)
    {
        if (c is '\n' or '\r' && _state is not LexerReadState.EndOfLine)
        {
            // Всегда одиночный перенос строки
            if (c is '\n')
            {
                _line++;
                _column = 1;
                return new LexerStageResult
                {
                    SkipNextStages = true
                    //Token = new Token(TokenType.EndOfLine) TODO: Для строк и комментариев
                };
            }

            // Перенос из потенциально двух символов "\r\n"
            _state = LexerReadState.EndOfLine;
            return new LexerStageResult
            {
                SkipNextStages = true
            };
        }
        
        if (_state is LexerReadState.EndOfLine)
        {
            _state = LexerReadState.None;
            _line++;
            _column = 1;

            return new LexerStageResult
            {
                // Пропускаем стадии, если текущий символ '\n' (итог в виде "\r\n"),
                // не пропускаем, если иной, то есть символом переноса строки был '\r'
                SkipNextStages = c is '\n'
            };
        }

        return null;
    }
    
    private record struct LexerStageResult
    {
        public bool SkipNextStages { get; init; }
        public Token? Token { get; init; }
    }
    
    private enum LexerReadState
    {
        None,
        EndOfLine
    }
}