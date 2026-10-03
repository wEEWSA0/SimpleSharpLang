using Lexer.Abstractions;

namespace Lexer.Commands;

public record CreateTokenCommand<TTokenType>(TTokenType TokenType) : ILexerCommand<TTokenType>
    where TTokenType : struct, Enum
{
    public void Execute(ILexerCommandContext<TTokenType> context)
    {
        context.AddToken(TokenType);
    }
}