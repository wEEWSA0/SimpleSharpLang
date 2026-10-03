using Lexer.Abstractions;

namespace Lexer.Commands;

public class CreateTokenFromDictionaryCommand<TTokenType> : ILexerCommand<TTokenType>
    where TTokenType : struct, Enum
{
    public CreateTokenFromDictionaryCommand(Dictionary<string, TTokenType> tokenDictionary, TTokenType failureToken)
    {
        _tokenDictionary = tokenDictionary;
        _failureToken = failureToken;
    }

    private Dictionary<string, TTokenType> _tokenDictionary;
    private TTokenType _failureToken;
    
    public void Execute(ILexerCommandContext<TTokenType> context)
    {
        if (!_tokenDictionary.TryGetValue(context.TokenBuffer.ToString(), out TTokenType token))
        {
            context.AddToken(_failureToken);
            return;
        }
        
        context.AddToken(token);
    }
}