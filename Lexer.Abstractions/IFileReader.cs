using System.Diagnostics.CodeAnalysis;

namespace Lexer.Abstractions;

public interface IFileReader
{
    bool TryNext([NotNullWhen(true)] out char? c);
}
