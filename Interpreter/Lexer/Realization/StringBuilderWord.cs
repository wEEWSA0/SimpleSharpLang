using System.Text;
using Interpreter.Lexer.Abstract;

namespace Interpreter.Lexer.Realization;

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