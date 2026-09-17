using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Interpreter.Lexer;

public class LexerReader(TextScanner scanner)
{
    private readonly Dictionary<string, TokenType> _keyWords = new()
    {
        { "break", TokenType.Break }
    };
    
    private readonly Dictionary<State, Func<char, TransitionInfo>> _stateProcessors = new()
    {
        { 'a', new TransitionInfo(){NewState = State.IdentifierOrKeyword,  IsNeedFlush = true, IsNeedAppend = true} },
    };

    private enum State
    {
        None,
        IdentifierOrKeyword,
        StringLiteral,
        CharLiteral,
        IntLiteral
    }

    private class TransitionInfo //TODO: Нейминг, скорее всего не класс, вынос, подумать
    {
        public State NewState { get; init; } //TODO: Подумать, что делать с лучаем когда NewState == OldState. Стоит ли перезаписывать?
        public bool IsNeedFlush { get; init; } = false;
        public bool IsNeedAppend { get; init; } = false;
    }
    
    State _state;
    IWord _currentValue;

    public IReadOnlyList<Token> GetTokens(IFileReader fileReader)
    {
        List<Token> tokens = [];
    
        while (fileReader.TryNext(out var c))
        {
            if (TryGetToken(c, out Token? token))
            {
                tokens.Add(token);
            }
        }
        
        return tokens;
    }

    private bool TryGetToken(char c, out Token? token)
    {
        var _stateProcessor = _stateProcessors[_state];

        var transitionInfo =  _stateProcessor.Invoke(c);

        _state = transitionInfo.NewState;

        if (transitionInfo.IsNeedAppend)
        {
            _currentValue.Append(c);
        }

        if (transitionInfo.IsNeedFlush)
        {
            token = GenerateToken();
            _currentValue.Clear();
            return true;
        }

        token = null;
        return false;
    }

    private Token GenerateToken()
    {
        return _state switch
        {
            State.IdentifierOrKeyword => _keyWords.TryGetValue(_currentValue, out var tokenType)
                ? new Token(tokenType){Line = 67, Column = 67}
                : new Token(TokenType.Identifier, _currentValue){Line = 67, Column = 67},
            State.StringLiteral => new Token(TokenType.StringLiteral, _currentValue){Line = 67, Column = 67},
            State.IntLiteral => new Token(TokenType.IntLiteral, _currentValue){Line= 67, Column = 67},
        };
    }

    /*public IReadOnlyList<Token> Tokenize() // TODO: Подумать над сигнатурой и реализацией
    {
        List<Token> tokens = new();

        Token token = ReadToken();
        
        for (;
             token.Type != TokenType.Error && token.Type != TokenType.EndOfFile;
             token = ReadToken())
        {
            tokens.Add(token);
        }
        
        tokens.Add(token);

        return tokens;
    }
    
    private Token ReadToken()
    {
        SkipWhiteSpaces();
        
        if (!scanner.TryPeek(out char? c))
        {
            return new Token(TokenType.EndOfFile) { Column = 67, Line = 67 };
        }
        
        return
            char.IsAsciiLetter(c.Value)
            ? ParseIdentifierOrKeyword()

            : char.IsAsciiDigit(c.Value)
            ? ParseIntLiteral()

            : c == '\"'
            ? ParseStringLiteral()

            : c == '\''
            ? ParseCharLiteral()

            : ParseSingleSymbol(); //TODO: Rename
    }

    private Token ParseIdentifierOrKeyword()
    {
        if (TryGetValue(c=> scanner.IsEnd() || char.IsWhiteSpace(c.Value), out string? value))
        {
            return _keyWords.TryGetValue(value, out TokenType type)
                ? new Token(type) { Column = 67, Line = 67 }
                : new Token(TokenType.Identifier, value) { Column = 67, Line = 67 };
        }

        return new Token(TokenType.Error) { Column = 67, Line = 67 }; // TODO: Здесь можно теоеретически выводить инфу об ошибке
    }
    
    private Token ParseIntLiteral()
    {
        return TryGetValue(c => scanner.IsEnd() || !char.IsAsciiDigit(c.Value), out string? value)
            ? new Token(TokenType.IntLiteral, value) { Column = 67, Line = 67 }
            : new Token(TokenType.Error) { Column = 67, Line = 67 };
    }
    
    private Token ParseStringLiteral()
    {
        scanner.Advance(); // Пропуск открывающей кавычки

        return TryGetValue(c => c == '"', out string? value)
            ? new Token(TokenType.StringLiteral, value) { Column = 67, Line = 67 }
            : new Token(TokenType.Error, "Ошибка строкового литерала: Отсутствует закрывающая кавычка") { Column = 67, Line = 67 }; //TODO: Подумать точно ли такая ошибка
    }

    private Token ParseCharLiteral()
    {
        scanner.Advance(); // Пропуск открывающей кавычки

        // TODO: Подумать не слишком ли сложно с тернарным оператором. Подумать над описанием ошибок
        return !TryGetValue(c => c == '\'', out string? value)
            ? new Token(TokenType.Error, "Ошибка символьного литерала: Отсутствует закрывающая кавычка") { Column = 67, Line = 67 }
            : !IsValidCharValue(value)
                ? new Token(TokenType.Error, "Ошибка символьного литерала: Значение не является валидным символом") { Column = 67, Line = 67 }
                : new Token(TokenType.CharLiteral, value) { Column = 67, Line = 67 };
    }
    
    private Token ParseSingleSymbol() //TODO: Придумать нормальное название
    {
        scanner.TryPeek(out char? c);
        scanner.Advance();

        return c switch
        {
            '(' => new Token(TokenType.OpenParenthesis) { Column = 67, Line = 67 }, // TODO: Доделать остальные символы
            ')' => new Token(TokenType.CloseParenthesis) { Column = 67, Line = 67 },
            _ => new Token(TokenType.Error) { Column = 67, Line = 67 },
        };
    }

    private void SkipWhiteSpaces()
    {
        while (scanner.TryPeek(out char? c) && char.IsWhiteSpace(c.Value))
        {
            scanner.Advance();
        }
    }
    
    //TODO: ПРОВЕРИТЬ НУЖНО ЛИ ПРОПУСКАТЬ ПОСЛЕДНИЙ СИМВОЛ - ОСТАНОВКИ. (КАК БУДТО НУЖНО)
    
    private bool TryGetValue(Func<char?, bool> stopCondition, [NotNullWhen(true)] out string? value) // TODO: Подумать над неймингом, возможно сигнатурой
    {
        StringBuilder sb = new StringBuilder();
        char? c;
        
        while (scanner.TryPeek(out c) && !stopCondition.Invoke(c.Value)) // Считываем пока не конец и пока не сработала остановка
        {
            sb.Append(c);
            scanner.Advance();
        }

        if (stopCondition.Invoke(c))
        {
            value = sb.ToString();
            scanner.Advance(); // TODO: Подумать, может быть избавиться от этого
        }
        else
        {
            value = null; //TODO: Отрефакторить
        }

        return stopCondition.Invoke(c); //TODO: Temp
    }

    private bool IsValidCharValue(string value) 
    {
        return value.Length == 1; //TODO: Добавить обработку ESCAPE-последовательностей
    }*/
}