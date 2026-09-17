using System.Diagnostics.CodeAnalysis;

namespace Interpreter.Lexer.Abstract;

public interface IFileReader
{
    bool TryNext([NotNullWhen(true)] out char? c);
}
