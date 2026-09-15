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
    EndOfLine,
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