namespace Interpreter.Lexer;

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
    Int,
    Bool,
    String,
    Char,
    Num,
    Uint,
    True,
    False,
    Struct,
    If,
    Else,
    While,
    Return,
    Break,
    Continue,
    IntLiteral,
    StringLiteral,
    CharLiteral,
    Plus,
    Minus,
    Multiply,
    Divide,
    And,
    Or,
    Assign,
    Equals,
    NotEquals,
    LessThan,
    GreaterThan,
    LessThanOrEquals,
    GreaterThanOrEquals,
    OpenParenthesis,
    CloseParenthesis,
    OpenBracket,
    CloseBracket,
    OpenBrace,
    CloseBrace,
    Point,
    Semicolon,
    Error,
}