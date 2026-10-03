namespace Interpreter.Lexer;

public enum LexerState
{
    Main,
    IdentifierOrKeyword,
    IntLiteral,
    CharLiteral,
    CharEscapeCharacter,
    CharReceived,
    StringLiteral,
    StringEscapeCharacter,
    SlashReceived,
    Comment,
    Or,
    And,
    Divider,
    Exclamation,
    Equals,
    LessThan,
    GreaterThan,
}