using Lexer.Abstractions;

namespace Lexer.Commands;

public class AdvanceColumnCommand : ILexerCommand
{
    public AdvanceColumnCommand(int column = 1)
    {
        Column = column;
    }
    
    public int Column { get; init; }
    
    public void Execute(ILexerCommandContext context)
    {
        context.Column += Column;
    }
}