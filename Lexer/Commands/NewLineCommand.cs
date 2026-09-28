using Lexer.Abstractions;

namespace Lexer.Commands;

public class NewLineCommand : ILexerCommand
{
    public void Execute(ILexerCommandContext context)
    {
        context.Line++;
    }
}