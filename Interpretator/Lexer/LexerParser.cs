using System.Text;

namespace Interpretator.Lexer;

public class LexerParser
{
    private static readonly Dictionary<string, TokenType> _keywords = new()
    {
        // Управляющие конструкции
        ["struct"] = TokenType.Struct,
        ["var"] = TokenType.Var,
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["while"] = TokenType.While,
        ["return"] = TokenType.Return,
        ["break"] = TokenType.Break,
        ["continue"] = TokenType.Continue,
    
        // Значения
        ["true"] = TokenType.BoolLiteral,
        ["false"] = TokenType.BoolLiteral,
        ["null"] = TokenType.Null
    };
    
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

        return tokens;
    }
}

public class Token
{
    public string? Value { get; private init; }
    public TokenType Type { get; private init; }
    public required int Line { get; init; }
    public required int Column { get; init; }

    public Token(TokenType type, string? value = null)
    {
        // TODO: Проверки возможности существования/несуществования значения для типа
        Value = value;
        Type = type;
    }
}

public enum TokenType
{
    Identifier,
    Struct,
    Var,
    If,
    Else,
    While,
    Return,
    Break,
    Continue,
    BoolLiteral,
    Null,
    StringLiteral,
    StringBegin,
    StringEnd,
    CharLiteral,
    CharBegin,
    CharEnd
}

public enum LexerReadState
{
    None,
    EndOfLine
}