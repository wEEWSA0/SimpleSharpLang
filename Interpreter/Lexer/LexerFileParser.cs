using System.Text;
using Lexer;
using Lexer.Abstractions;

namespace Interpreter.Lexer;

public class LexerFileParser
{
    private readonly LexerTokenReader<LexerState, Token> _lexerTokenReader;

    public LexerFileParser()
    {
        _lexerTokenReader = new BaseLexer<LexerState, Token>
        {
            StateTransitionRules = new Dictionary<LexerState, IReadOnlyCollection<IStateTransitionRule<LexerState, Token>>>(),
            ErrorFunc = (_, options, c) => 
                new Token(TokenType.Error, $"Встретился неожиданный символ '{c}'") 
                {
                    Line = options.Line,
                    Column = options.Column
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