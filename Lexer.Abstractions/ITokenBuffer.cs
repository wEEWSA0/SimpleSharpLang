namespace Lexer.Abstractions;

public interface ITokenBuffer
{
    int Length { get; }
    void Append(char c);
    void Clear();
    string ToString();
}