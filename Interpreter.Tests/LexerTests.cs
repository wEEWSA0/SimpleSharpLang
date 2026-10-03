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

        Assert.Equal(expected, tokens);
    }

    public static TheoryData<string, List<Token>> GetIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                " bool string char int uint num struct ",
                [
                    new Token(TokenType.Bool, "bool") { Line = 1, Column = 1 },
                    new Token(TokenType.String, "string") { Line = 1, Column = 1 },
                    new Token(TokenType.Char, "char") { Line = 1, Column = 1 },
                    new Token(TokenType.Int, "int") { Line = 1, Column = 1 },
                    new Token(TokenType.Uint, "uint") { Line = 1, Column = 1 },
                    new Token(TokenType.Num, "num") { Line = 1, Column = 1 },
                    new Token(TokenType.Struct, "struct") { Line = 1, Column = 1 },
                ]
            },
            {
                "true false ",
                [
                    new Token(TokenType.True, "true") { Line = 1, Column = 1 },
                    new Token(TokenType.False, "false") { Line = 1, Column = 1 },
                ]
            }   ,         
            {
                "if else while break continue return ",
                [
                    new Token(TokenType.If, "if") { Line = 1, Column = 1 },
                    new Token(TokenType.Else, "else") { Line = 1, Column = 1 },
                    new Token(TokenType.While, "while") { Line = 1, Column = 1 },
                    new Token(TokenType.Break, "break") { Line = 1, Column = 1 },
                    new Token(TokenType.Continue, "continue") { Line = 1, Column = 1 },
                    new Token(TokenType.Return, "return") { Line = 1, Column = 1 },
                ]
            } ,           
            {
                "hello h42 ab_44 ",
                [
                    new Token(TokenType.Identifier, "hello") { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "h42") { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "ab_44") { Line = 1, Column = 1 },
                ]
            },
            {
                "IF stRing ",
                [
                    new Token(TokenType.Identifier, "IF") { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "stRing") { Line = 1, Column = 1 },
                ]
            },
            //TODO: Решить нужно ли на этот кейс кидать ошибку или парсить как число + идентификатор
            // {
            //     "1Str ",
            //     [
            //         new Token(TokenType.Error) { Line = 1, Column = 1 },
            //     ]
            // }
        };
    } 
    
    public static TheoryData<string, List<Token>> GetIntLiterals()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                " 12 0 0011 ",
                [
                    new Token(TokenType.IntLiteral, "12") { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "0") { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "0011") { Line = 1, Column = 1 },
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
                    new Token(TokenType.CharLiteral, "c") { Line = 1, Column = 1 },
                    new Token(TokenType.CharLiteral, "\t") { Line = 1, Column = 1 },
                    new Token(TokenType.CharLiteral, "'") { Line = 1, Column = 1 },
                ]
            },
            {
                " 'a ",
                [
                    new Token(TokenType.Error, "Ожидался символ '") { Line = 1, Column = 1 },
                ]
            },
            {
                " '' ",
                [
                    new Token(TokenType.Error, "Символьный литерал не может быть пустым") { Line = 1, Column = 1 },
                ]
            },
            {
                " 'ab' ",
                [
                    new Token(TokenType.Error, "Ожидался символ '") { Line = 1, Column = 1 },
                ]
            },
            {
                @" '\r\'' ",
                [
                    new Token(TokenType.Error, "Ожидался символ '") { Line = 1, Column = 1 },
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
                    new Token(TokenType.StringLiteral) { Line = 1, Column = 1 },
                    new Token(TokenType.StringLiteral, "a") { Line = 1, Column = 1 },
                    new Token(TokenType.StringLiteral, "Hello, World") { Line = 1, Column = 1 },
                ]
            },
            {
                """ "Hello, \"User\""  "Bye!\n"  """,
                [
                    new Token(TokenType.StringLiteral, "Hello, \"User\"") { Line = 1, Column = 1 },
                    new Token(TokenType.StringLiteral, "Bye!\n") { Line = 1, Column = 1 },
                ]
            },
            {
                """
                 "Hello
                 
                """,
                [
                    new Token(TokenType.Error, "Ожидался символ \"") { Line = 1, Column = 1 },
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
            // {
            //     " x + y - a * b / 4 ",
            //     [
            //         new Token(TokenType.Identifier, "x") { Line = 1, Column = 1 },
            //         new Token(TokenType.Plus, "+") { Line = 1, Column = 1 },
            //         new Token(TokenType.Identifier, "y") { Line = 1, Column = 1 },
            //         new Token(TokenType.Minus, "-") { Line = 1, Column = 1 },
            //         new Token(TokenType.Identifier, "a") { Line = 1, Column = 1 },
            //         new Token(TokenType.Multiply, "*") { Line = 1, Column = 1 },
            //         new Token(TokenType.Identifier, "b") { Line = 1, Column = 1 },
            //         new Token(TokenType.Divide, "/") { Line = 1, Column = 1 },
            //         new Token(TokenType.IntLiteral, "4") { Line = 1, Column = 1 },
            //     ]
            // },
            {
                " x > 4 && y < 7 || a == b || c != b ",
                [
                    new Token(TokenType.Identifier, "x") { Line = 1, Column = 1 },
                    new Token(TokenType.GreaterThan) { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "4") { Line = 1, Column = 1 },
                    new Token(TokenType.And) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "y") { Line = 1, Column = 1 },
                    new Token(TokenType.LessThan) { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "7") { Line = 1, Column = 1 },
                    new Token(TokenType.Or) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "a") { Line = 1, Column = 1 },
                    new Token(TokenType.Equals) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "b") { Line = 1, Column = 1 },
                    new Token(TokenType.Or) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "c") { Line = 1, Column = 1 },
                    new Token(TokenType.NotEquals) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "b") { Line = 1, Column = 1 },
                ]
            },
            {
                " x >= y || b <= 2 ",
                [
                    new Token(TokenType.Identifier, "x") { Line = 1, Column = 1 },
                    new Token(TokenType.GreaterThanOrEquals) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "y") { Line = 1, Column = 1 },
                    new Token(TokenType.Or) { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "b") { Line = 1, Column = 1 },
                    new Token(TokenType.LessThanOrEquals) { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "2") { Line = 1, Column = 1 },
                ]
            },
            {
                " if (arr[5] < 2){} ",
                [
                    new Token(TokenType.If, "if") { Line = 1, Column = 1 },
                    new Token(TokenType.OpenParenthesis, "(") { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "arr") { Line = 1, Column = 1 },
                    new Token(TokenType.OpenBracket, "[") { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "5") { Line = 1, Column = 1 },
                    new Token(TokenType.CloseBracket, "]") { Line = 1, Column = 1 },
                    new Token(TokenType.LessThan) { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "2") { Line = 1, Column = 1 },
                    new Token(TokenType.CloseParenthesis, ")") { Line = 1, Column = 1 },
                    new Token(TokenType.OpenBrace, "{") { Line = 1, Column = 1 },
                    new Token(TokenType.CloseBrace, "}") { Line = 1, Column = 1 },
                ]
            },
            {
                " point.x = 7 ",
                [
                    new Token(TokenType.Identifier, "point") { Line = 1, Column = 1 },
                    new Token(TokenType.Point, ".") { Line = 1, Column = 1 },
                    new Token(TokenType.Identifier, "x") { Line = 1, Column = 1 },
                    new Token(TokenType.Assign) { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "7") { Line = 1, Column = 1 },
                ]
            },
            {
                " foo(5, 4); ",
                [
                    new Token(TokenType.Identifier, "foo") { Line = 1, Column = 1 },
                    new Token(TokenType.OpenParenthesis, "(") { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "5") { Line = 1, Column = 1 },
                    new Token(TokenType.Comma, ",") { Line = 1, Column = 1 },
                    new Token(TokenType.IntLiteral, "4") { Line = 1, Column = 1 },
                    new Token(TokenType.CloseParenthesis, ")") { Line = 1, Column = 1 },
                    new Token(TokenType.Semicolon, ";") { Line = 1, Column = 1 },
                ]
            },
        };
    } 
}