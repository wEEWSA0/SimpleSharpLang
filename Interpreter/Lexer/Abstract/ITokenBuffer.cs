namespace Interpreter.Lexer.Abstract;

public interface ITokenBuffer
{
    int Length { get; }
    void Append(char c);
    void Clear();
    string ToString();
}