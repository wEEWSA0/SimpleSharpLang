using System.Text;
using Lexer.Abstractions;

namespace Lexer;

public class StringBuilderTokenBuffer : ITokenBuffer
{
    private readonly StringBuilder _stringBuilder;
    
    public StringBuilderTokenBuffer()
    {
        _stringBuilder = new StringBuilder();
    }

    public int Length => _stringBuilder.Length;
    
    public void Append(char c)
    {
        _stringBuilder.Append(c);
    }

    public void Clear()
    {
        _stringBuilder.Clear();
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}