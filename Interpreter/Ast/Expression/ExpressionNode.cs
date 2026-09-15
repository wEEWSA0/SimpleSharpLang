namespace Interpreter.Ast.Expression;

/// <summary>
/// Выражения семантического дерева
/// </summary>
/// <remarks>Всегда возвращает значение</remarks>
public abstract class ExpressionNode : AstNode
{
    public required Type ReturnType { get; init; }
}

// public class BinaryExpressionNode : ExpressionNode
// {
//     public ExpressionNode Left { get; set; }
//     public ExpressionNode Right { get; set; }
//     public BinaryOperator Operator { get; set; }
// }

public class LiteralExpressionNode : ExpressionNode
{
    public object Value { get; set; }
    public Type Type { get; set; } // int, uint, num, string...
}

public class VariableExpressionNode : ExpressionNode
{
    public string Name { get; set; }
}

public class AssignmentExpressionNode : ExpressionNode
{
    public string VariableName { get; set; }
    public ExpressionNode Value { get; set; }
}