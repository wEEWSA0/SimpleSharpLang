using System.Text;
using Lexer;
using Lexer.Abstractions;

namespace Interpreter.Lexer;

public class LexerFileParser
{
    private readonly LexerTokenReader<LexerState, Token, TokenType> _lexerTokenReader;

    public LexerFileParser()
    {
        _lexerTokenReader = new BaseLexer<LexerState, Token, TokenType>
        {
            StateTransitionRules = SimpleSharpRules.Rules,
            
            ErrorFunc = (state, c) => state switch
            {
                LexerState.Main => (TokenType.Error, $"Встретился неожиданный символ в начальном состоянии '{c}'"),
                LexerState.StringLiteral => (TokenType.Error, "Ожидался символ \""),
                LexerState.StringEscapeCharacter => (TokenType.Error, $"Не найдено подходяее экранирование для '{c}'"),
                LexerState.CharLiteral => (TokenType.Error, "Символьный литерал не может быть пустым"),
                LexerState.CharReceived => (TokenType.Error, "Ожидался символ '"),
                LexerState.CharEscapeCharacter => (TokenType.Error, $"Не найдено подходяее экранирование для '{c}'"),
                _ => (TokenType.Error, $"Встретился неожиданный символ '{c}'")
            },
            TokenGenerationFunc = (type, value, context) => new Token(type, value)
            {
                Line = context.Line,
                Column = context.Column
            }
        };
    }
    
    public IReadOnlyList<Token> ParseSourceCode(Stream fileStream)
    {
        const int bufferSize = 4096;
        
        ArgumentNullException.ThrowIfNull(fileStream);

        if (!fileStream.CanRead)
            throw new ArgumentException("Поток недоступен для чтения", nameof(fileStream));

        if (fileStream.CanSeek)
            fileStream.Seek(0, SeekOrigin.Begin);
        
        using var fileReader = new StreamPerSymbolReader(fileStream, Encoding.UTF8, bufferSize);
        
        return _lexerTokenReader.GetTokens(fileReader);
    }
}