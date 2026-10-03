using Lexer.Abstractions;

namespace Lexer.Commands;

public record ClearTokenBufferCommand : ILexerCommand
{
    public void Execute(ILexerCommandContext context)
    {
        context.TokenBuffer.Clear();
    }
}