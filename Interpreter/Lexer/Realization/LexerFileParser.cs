using System.Text;
using Interpreter.Lexer.Abstract;

namespace Interpreter.Lexer.Realization;

public class LexerFileParser
{
    private readonly LexerTokenReader<LexerState, Token> _lexerTokenReader;

    public LexerFileParser()
    {
        _lexerTokenReader = new LexerTokenReader<LexerState, Token>(new StringBuilderWord(), LexerState.None)
        {
            StartupEvents = [],
            StateEvents = new Dictionary<LexerState, List<(Func<char, bool> Condition, Func<ILexerOptions, IResponse<LexerState, Token>> ResponseAction)>>(),
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