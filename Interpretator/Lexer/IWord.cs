namespace Interpretator.Lexer;

public interface IWord
{
    int Length { get; }
    void Append(char c);
    void Clear();
}