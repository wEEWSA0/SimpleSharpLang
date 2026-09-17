namespace Interpreter.Lexer.Abstract;

public interface IWord
{
    int Length { get; }
    void Append(char c);
    void Clear();
}