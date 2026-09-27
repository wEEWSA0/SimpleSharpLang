using Lexer.Abstractions;
using Lexer.Commands;

namespace Lexer;

public static class LexerCommandsExtensions
{
    public static LexerCommandBuilder<TTokenType> AppendCurrentChar<TTokenType>(
        this LexerCommandBuilder<TTokenType> builder)
        where TTokenType : struct, Enum
    {
        builder.AddCommand(new AppendCurrentCharCommand());
        return builder;
    }
    
    public static LexerCommandBuilder<TTokenType> ClearTokenBuffer<TTokenType>(
        this LexerCommandBuilder<TTokenType> builder)
        where TTokenType : struct, Enum
    {
        builder.AddCommand(new ClearTokenBufferCommand());
        return builder;
    }
    
    public static LexerCommandBuilder<TTokenType> CreateToken<TTokenType>(
        this LexerCommandBuilder<TTokenType> builder,
        TTokenType tokenType)
        where TTokenType : struct, Enum
    {
        builder.AddCommand(new CreateTokenCommand<TTokenType>(tokenType));
        return builder;
    }
    
    public static LexerCommandBuilder<TTokenType> AdvanceColumn<TTokenType>(
        this LexerCommandBuilder<TTokenType> builder,
        int column = 1)
        where TTokenType : struct, Enum
    {
        builder.AddCommand(new AdvanceColumnCommand(column));
        return builder;
    }
    
    public static LexerCommandBuilder<TTokenType> NewLine<TTokenType>(
        this LexerCommandBuilder<TTokenType> builder)
        where TTokenType : struct, Enum
    {
        builder.AddCommand(new NewLineCommand());
        return builder;
    }
}