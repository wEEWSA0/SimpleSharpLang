using Lexer;
using Lexer.Abstractions;

namespace Interpreter.Lexer;

using TransitionRule = StateTransitionRule<LexerState, TokenType>; //TODO: Подумать о нейминге StateTransitionRule

internal static class SimpleSharpRules
{
    private static readonly Dictionary<char, char> EscapeCharacters = new()
    {
        ['n'] = '\n',
        ['t'] = '\t',
        ['r'] = '\r',
        ['\\'] = '\\',
        ['\''] = '\'',
        ['"'] = '"',
    };

    private static readonly Dictionary<string, TokenType> Dividers = new()
    {
        [";"] = TokenType.Semicolon,
        ["."] = TokenType.Point,
        [","] = TokenType.Comma,
        ["("] = TokenType.OpenParenthesis, 
        [")"] = TokenType.CloseParenthesis,
        ["["] = TokenType.OpenBracket,
        ["]"] = TokenType.CloseBracket,
        ["{"] = TokenType.OpenBrace,
        ["}"] = TokenType.CloseBrace,
        ["+"] = TokenType.Plus,
        ["-"] = TokenType.Minus,
        ["*"] = TokenType.Multiply,
        ["/"] = TokenType.Divide
    };
    
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["while"] = TokenType.While,
        ["int"] = TokenType.Int, 
        ["uint"] = TokenType.Uint, 
        ["num"] = TokenType.Num,
        ["bool"] = TokenType.Bool,
        ["char"] = TokenType.Char,
        ["string"] = TokenType.String,
        ["return"] = TokenType.Return,
        ["break"] = TokenType.Break,
        ["continue"] = TokenType.Continue,
        ["struct"] = TokenType.Struct,
        ["true"] = TokenType.True,
        ["false"] = TokenType.False
    };
    
    private static List<TransitionRule> GetMainStateRules(Action<LexerCommandBuilder<TokenType>> onTransitionRules)
    {
        return [
            new()
            {
                
                Condition = c => c == '_' || char.IsAsciiLetter(c) ,
                Transition = new (
                    LexerState.IdentifierOrKeyword,
                    builder =>
                    {
                        onTransitionRules.Invoke(builder);
                        builder.AppendCurrentChar();
                    })
            },
            new ()
            {
                Condition = char.IsAsciiDigit,
                Transition = new (
                    LexerState.IntLiteral,
                    builder =>
                    {
                        onTransitionRules.Invoke(builder);
                        builder.AppendCurrentChar();
                    })
            },
            new()
            {
                Condition = c => c == '"',
                Transition = new(
                    LexerState.StringLiteral,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '\'',
                Transition = new(
                    LexerState.CharLiteral,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '/',
                Transition = new (
                    LexerState.SlashReceived,
                    builder =>
                    {
                        onTransitionRules.Invoke(builder);
                        builder.AppendCurrentChar();
                    })
            },
            new ()
            {
                Condition = c => Dividers.ContainsKey(c.ToString()),
                Transition = new (
                    LexerState.Divider,
                    builder =>
                    {
                        onTransitionRules.Invoke(builder);
                        builder.AppendCurrentChar();
                    })
            },
            new ()
            {
                Condition = c => c == '|',
                Transition = new(
                    LexerState.Or,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '&',
                Transition = new(
                    LexerState.And,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '!',
                Transition = new(
                    LexerState.Exclamation,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '=',
                Transition = new(
                    LexerState.Equals,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '<',
                Transition = new(
                    LexerState.LessThan,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = c => c == '>',
                Transition = new(
                    LexerState.GreaterThan,
                    onTransitionRules.Invoke)
            },
            new ()
            {
                Condition = _ => true, // TODO: Подумать нормально ли
                Transition = new(
                    LexerState.Main,
                    onTransitionRules.Invoke)
            },
        ];
    }

    private static readonly List<TransitionRule> IntLiteralRules =
        GetMainStateRules(b => b
                .CreateToken(TokenType.IntLiteral)
                .ClearTokenBuffer())
            .Prepend(new()
            {
                Condition = char.IsAsciiDigit,
                Transition = new( 
                    LexerState.IntLiteral,
                    builder => builder
                        .AppendCurrentChar())
            })
            .ToList();
            
    private static readonly List<TransitionRule> CharLiteralRules = 
    [
        new()
        {
            Condition = c => c == '\\',
            Transition = new(
                LexerState.CharEscapeCharacter)
        },
        new()
        {
            Condition = c => c != '\'' && c != '\n',
            Transition = new(
                LexerState.CharReceived,
                builder => builder
                    .AppendCurrentChar())
        },
    ];
    
    private static readonly List<TransitionRule> CharEscapeCharacterRules = 
    [
        new()
        {
            Condition = EscapeCharacters.ContainsKey,
            Transition = new(
                LexerState.CharReceived,
                builder => builder
                    .AppendCharFromDictionary(EscapeCharacters))
        },
    ];
    
    private static readonly List<TransitionRule> CharReceivedRules = 
    [
        new()
        {
            Condition = c => c == '\'',
            Transition = new(
                LexerState.Main,
                builder => builder
                    .CreateToken(TokenType.CharLiteral)
                    .ClearTokenBuffer())
        },
    ];
    
    private static readonly List<TransitionRule> StringLiteralRules = 
    [
        new()
        {
            Condition = c => c == '\\',
            Transition = new(
                LexerState.StringEscapeCharacter)
        },
        new()
        {
            Condition = c => c == '"',
            Transition = new(
                LexerState.Main,
                builder => builder
                    .CreateToken(TokenType.StringLiteral)
                    .ClearTokenBuffer())
        },
        new()
        {
            Condition = c => c != '\n',
            Transition = new(
                LexerState.StringLiteral,
                builder => builder
                    .AppendCurrentChar())
        },
    ];
    
    private static readonly List<TransitionRule> StringEscapeCharacterRules = 
    [
        new()
        {
            Condition = EscapeCharacters.ContainsKey,
            Transition = new(
                LexerState.StringLiteral,
                builder => builder
                    .AppendCharFromDictionary(EscapeCharacters))
        },
    ];

    private static readonly List<TransitionRule> IdentifierOrKeywordRules =
        GetMainStateRules(b => b
            .CreateTokenFromDictionary(Keywords, TokenType.Identifier)
            .ClearTokenBuffer())
                .Prepend(new()
                {
                    Condition = c => c == '_' || char.IsAsciiLetter(c) || char.IsAsciiDigit(c),
                    Transition = new( 
                    LexerState.IdentifierOrKeyword,
                    builder => builder
                        .AppendCurrentChar())
                })
                .ToList();
    
    private static readonly List<TransitionRule> SlashRules = 
        GetMainStateRules(b => b
                .CreateToken(TokenType.Divide)
                .ClearTokenBuffer())
            .Prepend(new()
            {
                Condition = c => c == '/',
                Transition = new( 
                    LexerState.Comment)
            })
            .ToList();
    
    private static readonly List<TransitionRule> CommentRules = 
    [
        new()
        {
            Condition = c => c == '\n',
            Transition = new( 
                LexerState.Main)
        },
        new()
        {
            Condition = _ => true,
            Transition = new( 
                LexerState.Comment)
        },
    ];
    
    private static List<TransitionRule> GetLogicalOperatorRules(char ch, TokenType tokenType)
    {
        return
        [
            new()
            {
                Condition = c => c == ch,
                Transition = new( 
                    LexerState.Main, 
                    builder => builder
                        .CreateToken(tokenType)
                        .ClearTokenBuffer())
            },
        ];
    }
    
    private static List<TransitionRule> GetComparisonOperatorRules(TokenType successToken, TokenType failureToken)
    {
        return GetMainStateRules(b =>
        {
            b.CreateToken(failureToken);
            b.ClearTokenBuffer();
        })
            .Prepend(new()
            {
                Condition = c => c == '=',
                Transition = new(
                    LexerState.Main,
                    builder => builder
                        .CreateToken(successToken)
                        .ClearTokenBuffer())
            })
            .ToList();
    }

    public static Dictionary<LexerState, IReadOnlyCollection<TransitionRule>> Rules = new()
    {
        [LexerState.Main] = GetMainStateRules(_ => {}),
        [LexerState.IdentifierOrKeyword] = IdentifierOrKeywordRules,
        [LexerState.StringLiteral] = StringLiteralRules,
        [LexerState.StringEscapeCharacter] = StringEscapeCharacterRules,
        [LexerState.CharLiteral] = CharLiteralRules,
        [LexerState.CharReceived] = CharReceivedRules,
        [LexerState.CharEscapeCharacter] = CharEscapeCharacterRules,
        [LexerState.IntLiteral] = IntLiteralRules,
        [LexerState.SlashReceived] = SlashRules,
        [LexerState.Comment] = CommentRules,
        [LexerState.Divider] = GetMainStateRules(b => b
            .CreateTokenFromDictionary(Dividers, TokenType.Error)
            .ClearTokenBuffer()),
        [LexerState.Or] = GetLogicalOperatorRules('|', TokenType.Or),
        [LexerState.And] = GetLogicalOperatorRules('&', TokenType.And),
        [LexerState.Exclamation] = GetComparisonOperatorRules(TokenType.NotEquals, TokenType.Not),
        [LexerState.Equals] = GetComparisonOperatorRules(TokenType.Equals, TokenType.Assign),
        [LexerState.LessThan] = GetComparisonOperatorRules(TokenType.LessThanOrEquals, TokenType.LessThan),
        [LexerState.GreaterThan] = GetComparisonOperatorRules(TokenType.GreaterThanOrEquals, TokenType.GreaterThan),
    };
}
