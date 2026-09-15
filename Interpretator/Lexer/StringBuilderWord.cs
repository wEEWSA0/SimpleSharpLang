using System.Text;

namespace Interpretator.Lexer;

public class StringBuilderWord(StringBuilder stringBuilder) : IWord
{
    public StringBuilderWord() : this(new StringBuilder())
    {
    }

    public int Length => stringBuilder.Length;
    
    public void Append(char c)
    {
        stringBuilder.Append(c);
    }

    public void Clear()
    {
        stringBuilder.Clear();
    }
}