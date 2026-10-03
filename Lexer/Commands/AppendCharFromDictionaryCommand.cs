using Lexer.Abstractions;

namespace Lexer.Commands;

public class AppendCharFromDictionaryCommand : ILexerCommand
{
    public AppendCharFromDictionaryCommand(Dictionary<char, char> escapeDictionary)
    {
        _escapeDictionary = escapeDictionary;
    }

    private Dictionary<char, char> _escapeDictionary;
    
    public void Execute(ILexerCommandContext context)
    {
        context.TokenBuffer.Append(_escapeDictionary[context.CurrentChar]);
    }
}