using Lexer.Abstractions;

namespace Lexer.Commands;

public record AppendCurrentCharCommand : ILexerCommand
{
    public void Execute(ILexerCommandContext context)
    {
        context.TokenBuffer.Append(context.CurrentSymbol);
    }
}