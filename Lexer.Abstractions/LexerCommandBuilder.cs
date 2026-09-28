using System.Diagnostics;

namespace Lexer.Abstractions;

public sealed class LexerCommandBuilder<TTokenType>
    where TTokenType : struct, Enum
{
    private readonly List<LexerCommandData> _commands = [];
    
    public bool IsEmpty => _commands.Count == 0;
    
    public LexerCommandBuilder<TTokenType> AddCommand(ILexerCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commands.Add(new LexerCommandData(command));
        return this;
    }

    public LexerCommandBuilder<TTokenType> AddCommand(ILexerCommand<TTokenType> command)
    {
        ArgumentNullException.ThrowIfNull(command);
        _commands.Add(new LexerCommandData(command));
        return this;
    }

    internal CommandExecutor<TTokenType> GetExecutor()
    {
        // IDEA: Перенести commands в CommandExecutor, чтобы избавиться от замыкания
        // (значимой выгоды в сравнении с текущим подходом нет)
        
        // Для освобождения от привязки к билдеру
        // Замыкание только локальной переменной позволит GC собрать билдер
        var commands = _commands.ToArray();
        return new CommandExecutor<TTokenType>(context =>
        {
            foreach (var command in commands)
            {
                if (command.SimpleCommand != null)
                {
                    command.SimpleCommand.Execute(context);
                    continue;
                }

                // Недостижимый код, проверка в случае неправильной работы программы
                if (command.CommandWithTokenType == null)
                    throw new UnreachableException("Команда без реализации");
                
                command.CommandWithTokenType.Execute(context);
            }
        });
    }
    
    private record struct LexerCommandData
    {
        public LexerCommandData(ILexerCommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            SimpleCommand = command;
        }
    
        public LexerCommandData(ILexerCommand<TTokenType> command)
        {
            ArgumentNullException.ThrowIfNull(command);
            CommandWithTokenType = command;
        }
    
        public ILexerCommand? SimpleCommand { get; private init; }
        public ILexerCommand<TTokenType>? CommandWithTokenType { get; private init; }
    }
}

internal sealed class CommandExecutor<TTokenType>
    where TTokenType : struct, Enum
{
    private readonly Action<ILexerCommandContext<TTokenType>> _action;
    
    internal CommandExecutor(Action<ILexerCommandContext<TTokenType>> action)
    {
        _action = action;
    }

    internal void Execute(ILexerCommandContext<TTokenType> context)
    {
        _action.Invoke(context);
    }
}