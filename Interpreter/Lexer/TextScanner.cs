using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Lexer;

public class TextScanner(string text) // TODO: Временное решение для тестов
{
    private int _position = 0;
    
    // TODO: Подумать над семантикой Peek, Advance.
    //  Что должен делать метод Advance когда дальше - конец?

    public char? ReadChar()
    {
        return text[_position];
        //TODO: Вернет ли null text[MAXINT] ??
    }
    
    public bool TryPeek([NotNullWhen(true)] out char? c)
    {
        c = IsEnd() ? null :  text[_position];
        
        return !IsEnd();
    }
    
    public void Advance()
    {
        if (_position < text.Length)
        {
            _position++;
        }
    }
    
    public bool IsEnd()
    {
        return _position == text.Length;
    }
}