namespace Lexer.Abstractions;

public interface ILexerCommand
{
    void Execute(ILexerCommandContext context);
}

public interface ILexerCommand<out TTokenType>
    where TTokenType : struct, Enum
{
    void Execute(ILexerCommandContext<TTokenType> context);
}