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
            StateTransitionRules = new Dictionary<LexerState, IReadOnlyCollection<StateTransitionRule<LexerState, TokenType>>>
            {
                [LexerState.None] = [
                    // Рекомендуемый способ
                    new()
                    {
                        Condition = c => c == 'b',
                        Transition = new(
                            LexerState.None, 
                            builder => builder
                                .AppendCurrentChar()
                                .CreateToken(TokenType.Return))
                    },
                    // Не рекомендуемый, но доступный способ
                    new()
                    {
                        Condition = c => c == 't',
                        Transition = new()
                        {
                            State = LexerState.None,
                            Commands = new LexerCommandBuilder<TokenType>()
                                .AppendCurrentChar()
                        }
                    },
                    // Не рекомендуемый, но доступный способ
                    new()
                    {
                        Condition = c => c == 'l',
                        Transition = new(
                            LexerState.None, 
                            new LexerCommandBuilder<TokenType>()
                                .AppendCurrentChar()
                                .CreateToken(TokenType.Return)
                                .ClearTokenBuffer()
                                .AdvanceColumn()) 
                    }
                ],
                [LexerState.Test] = [
                    new(c => c == '1', new(LexerState.None)),
                    new(
                        c => c == '1', 
                        new(LexerState.None)),
                    new StateTransitionRule<LexerState, TokenType>(
                        c => c == '1', 
                        new Transition<LexerState, TokenType>(LexerState.None))
                ]
            },
            ErrorFunc = (state, c) => state switch
            {
                LexerState.None => (TokenType.Error, $"Встретился неожиданный символ в начальном состоянии '{c}'"),
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