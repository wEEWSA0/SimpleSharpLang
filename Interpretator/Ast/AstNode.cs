using System.Runtime.CompilerServices;

namespace Interpretator.Ast;

/// <summary>
/// Корень семантического дерева
/// </summary>
public abstract class AstNode([CallerMemberName] string name = "");

public class Test()
{
    void Do()
    {
        try
        {

        }
        catch (Exception e)
            when (e.Message == null)
        {
            Console.WriteLine(e);
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}