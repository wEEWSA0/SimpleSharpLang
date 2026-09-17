using Interpreter.Lexer;

namespace Tests;

public class ParseTokens
{
    [Theory]
    [MemberData(nameof(GetWhiteSpaceAndCommentsData))]
    [MemberData(nameof(GetKeywordsAndIdentifiersData))]
    [MemberData(nameof(GetLiteralsData))]
    public void TokenizeLexemes(string code, List<Token> expected)
    {
        LexerReader reader = new(new TextScanner(code));
        List<Token> tokens = reader.Tokenize().ToList();

        Assert.Equal(tokens, expected);
    }


    public static TheoryData<string, List<Token>> GetWhiteSpaceAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "               ",
                [
                    new Token(TokenType.EndOfFile) { Line = 67, Column = 67 }
                ]
            },
            {
                "   // Hello, World!     ",
                [
                    new Token(TokenType.EndOfFile) { Line = 67, Column = 67 }
                ]
            }
            
        };
    } 
    
    public static TheoryData<string, List<Token>> GetKeywordsAndIdentifiersData()
    {
        return new TheoryData<string, List<Token>>
        {
        };
    } 
    
    public static TheoryData<string, List<Token>> GetLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
        };
    }
}