using System.Text;

namespace Interpreter.Lexer;

public class LexerParser
{
    public IReadOnlyList<Token> ParseSourceCode(Stream fileStream)
    {
        const int bufferSize = 4096;
        
        ArgumentNullException.ThrowIfNull(fileStream);

        if (!fileStream.CanRead)
            throw new ArgumentException("Поток недоступен для чтения", nameof(fileStream));

        if (fileStream.CanSeek)
            fileStream.Seek(0, SeekOrigin.Begin);

        var tokens = new List<Token>();

        using var reader = new StreamReader(
            fileStream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: bufferSize,
            leaveOpen: true);

        var buffer = new char[bufferSize];
        int read;
        var lexerReader = new LexerReader(new StringBuilderWord());
        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < read; i++)
            {
                if (!lexerReader.ReadToken(buffer[i], out var token))
                    continue;
                
                tokens.Add(token);
            }
        }

        if (lexerReader.Flush(out var lastToken))
        {
            tokens.Add(lastToken);
        }

        return tokens;
    }
}