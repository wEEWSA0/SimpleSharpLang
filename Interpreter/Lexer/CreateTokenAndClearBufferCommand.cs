using Lexer.Abstractions;

namespace Interpreter.Lexer;

// Пример создания своей кастомной команды
public record CreateTokenAndClearBufferCommand(TokenType TokenType) : ILexerCommand<TokenType>
{
    public void Execute(ILexerCommandContext<TokenType> context)
    {
        context.AddToken(TokenType);
        context.TokenBuffer.Clear();
    }
}

public static class CreateTokenAndClearBufferCommandExtensions
{
    public static LexerCommandBuilder<TokenType> CreateTokenAndClearBuffer(
        this LexerCommandBuilder<TokenType> builder,
        TokenType tokenType = TokenType.Identifier)
    {
        builder.AddCommand(new CreateTokenAndClearBufferCommand(tokenType));
        return builder;
    }
}