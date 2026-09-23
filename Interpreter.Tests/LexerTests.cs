using System.Text;
using Interpreter.Lexer;

namespace Interpreter.Tests;

public class LexerTests
{
    [Theory]
    [MemberData(nameof(GetIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetIntLiterals))]
    [MemberData(nameof(GetCharLiteralsData))]
    [MemberData(nameof(GetStringLiteralsData))]
    [MemberData(nameof(GetWhitespacesAndCommentsData))]
    [MemberData(nameof(GetPunctuationData))]
    public void TokenizeLexemes(string code, List<Token> expected)
    {
        LexerFileParser reader = new();
        Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
        List<Token> tokens = reader.ParseSourceCode(stream).ToList();

        Assert.Equal(tokens, expected);
    }

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "bool string char int unit num struct",
                [
                    new Token(TokenType.Bool) { Line = 67, Column = 67 },
                    new Token(TokenType.String) { Line = 67, Column = 67 },
                    new Token(TokenType.Char) { Line = 67, Column = 67 },
                    new Token(TokenType.Int) { Line = 67, Column = 67 },
                    new Token(TokenType.Uint) { Line = 67, Column = 67 },
                    new Token(TokenType.Num) { Line = 67, Column = 67 },
                    new Token(TokenType.Struct) { Line = 67, Column = 67 },
                ]
            },
            {
                "true false",
                [
                    new Token(TokenType.True) { Line = 67, Column = 67 },
                    new Token(TokenType.False) { Line = 67, Column = 67 },
                ]
            }   ,         
            {
                "if else while break continue return",
                [
                    new Token(TokenType.If) { Line = 67, Column = 67 },
                    new Token(TokenType.Else) { Line = 67, Column = 67 },
                    new Token(TokenType.While) { Line = 67, Column = 67 },
                    new Token(TokenType.Break) { Line = 67, Column = 67 },
                    new Token(TokenType.Continue) { Line = 67, Column = 67 },
                    new Token(TokenType.Return) { Line = 67, Column = 67 },
                ]
            } ,           
            {
                "hello h42 ab_44",
                [
                    new Token(TokenType.Identifier, "hello") { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "h42") { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "ab_44") { Line = 67, Column = 67 },
                ]
            },
            {
                "IF stRing",
                [
                    new Token(TokenType.Identifier, "IF") { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "StRing") { Line = 67, Column = 67 },
                ]
            },
            {
                "1Str",
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            }
        };
    } 
    
    public static TheoryData<string, List<Token>> GetIntLiterals()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                " 12 0 0011 ",
                [
                    new Token(TokenType.IntLiteral, "12") { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "0") { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "0011") { Line = 67, Column = 67 },
                ]
            },
        };
    } 
    
    public static TheoryData<string, List<Token>> GetCharLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                @" 'c' '\t' '\'' ",
                [
                    new Token(TokenType.CharLiteral, "c") { Line = 67, Column = 67 },
                    new Token(TokenType.CharLiteral, "\t") { Line = 67, Column = 67 },
                    new Token(TokenType.CharLiteral, "'") { Line = 67, Column = 67 },
                ]
            },
            {
                " 'a ",
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            },
            {
                " '' ",
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            },
            {
                " 'ab' ",
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            },
            {
                @" '\r\'' ",
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            },
        };
    } 
    
    public static TheoryData<string, List<Token>> GetStringLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """   ""    "a"   "Hello, World"  """,
                [
                    new Token(TokenType.StringLiteral, "") { Line = 67, Column = 67 },
                    new Token(TokenType.StringLiteral, "a") { Line = 67, Column = 67 },
                    new Token(TokenType.StringLiteral, "Hello, World") { Line = 67, Column = 67 },
                ]
            },
            {
                """ "Hello, \"User\""  "Bye!\n"  """,
                [
                    new Token(TokenType.StringLiteral, "Hello, \"User\"") { Line = 67, Column = 67 },
                    new Token(TokenType.StringLiteral, "Bye!\n") { Line = 67, Column = 67 },
                ]
            },
            {
                """ "Hello """,
                [
                    new Token(TokenType.Error) { Line = 67, Column = 67 },
                ]
            },
        };
    } 
    
    public static TheoryData<string, List<Token>> GetWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "  \n     \r    \t\t\t   ",
                [
                ]
            },
            {
                " //Hello, World   ",
                [
                ]
            },
        };
    } 
    
    public static TheoryData<string, List<Token>> GetPunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "  x + y - a * b / 4 ",
                [
                    new Token(TokenType.Identifier, "x") { Line = 67, Column = 67 },
                    new Token(TokenType.Plus) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "y") { Line = 67, Column = 67 },
                    new Token(TokenType.Minus) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "a") { Line = 67, Column = 67 },
                    new Token(TokenType.Minus) { Line = 67, Column = 67 },
                    new Token(TokenType.Multiply) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "b") { Line = 67, Column = 67 },
                    new Token(TokenType.Divide) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "4") { Line = 67, Column = 67 },
                ]
            },
            {
                " x > 4 && y < 7 || a == b || c != b ",
                [
                    new Token(TokenType.Identifier, "x") { Line = 67, Column = 67 },
                    new Token(TokenType.GreaterThan) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "4") { Line = 67, Column = 67 },
                    new Token(TokenType.And) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "y") { Line = 67, Column = 67 },
                    new Token(TokenType.LessThan) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "7") { Line = 67, Column = 67 },
                    new Token(TokenType.Or) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "a") { Line = 67, Column = 67 },
                    new Token(TokenType.Equals) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "b") { Line = 67, Column = 67 },
                    new Token(TokenType.Or) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "c") { Line = 67, Column = 67 },
                    new Token(TokenType.NotEquals) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "b") { Line = 67, Column = 67 },
                ]
            },
            {
                " x >= y || b <= 2 ",
                [
                    new Token(TokenType.Identifier, "x") { Line = 67, Column = 67 },
                    new Token(TokenType.GreaterThanOrEquals) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "4") { Line = 67, Column = 67 },
                    new Token(TokenType.Or) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "b") { Line = 67, Column = 67 },
                    new Token(TokenType.LessThanOrEquals) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "2") { Line = 67, Column = 67 },
                ]
            },
            {
                " if (arr[5] < 2){} ",
                [
                    new Token(TokenType.If) { Line = 67, Column = 67 },
                    new Token(TokenType.OpenParenthesis) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "arr") { Line = 67, Column = 67 },
                    new Token(TokenType.OpenBracket) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "5") { Line = 67, Column = 67 },
                    new Token(TokenType.CloseBracket) { Line = 67, Column = 67 },
                    new Token(TokenType.LessThan, "b") { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "2") { Line = 67, Column = 67 },
                    new Token(TokenType.CloseParenthesis) { Line = 67, Column = 67 },
                    new Token(TokenType.OpenBrace) { Line = 67, Column = 67 },
                    new Token(TokenType.CloseBrace) { Line = 67, Column = 67 },
                ]
            },
            {
                " point.x = 7 ",
                [
                    new Token(TokenType.Identifier, "point") { Line = 67, Column = 67 },
                    new Token(TokenType.Point) { Line = 67, Column = 67 },
                    new Token(TokenType.Identifier, "x") { Line = 67, Column = 67 },
                    new Token(TokenType.Assign) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "7") { Line = 67, Column = 67 },
                ]
            },
            {
                " foo(5, 4); ",
                [
                    new Token(TokenType.Identifier, "foo") { Line = 67, Column = 67 },
                    new Token(TokenType.OpenParenthesis) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "5") { Line = 67, Column = 67 },
                    new Token(TokenType.Comma) { Line = 67, Column = 67 },
                    new Token(TokenType.IntLiteral, "4") { Line = 67, Column = 67 },
                    new Token(TokenType.CloseParenthesis) { Line = 67, Column = 67 },
                    new Token(TokenType.Semicolon) { Line = 67, Column = 67 },
                ]
            },
        };
    } 
}