namespace Estoque.API.Exceptions;

public class ProdutoNaoEncontradoException : KeyNotFoundException
{
    public ProdutoNaoEncontradoException(string mensagem)
        : base(mensagem)
    {
    }

    public ProdutoNaoEncontradoException(int id)
        : base($"Produto com ID {id} não encontrado.")
    {
    }

    public ProdutoNaoEncontradoException(string codigo, bool porCodigo)
        : base($"Produto com código '{codigo}' não encontrado no estoque.")
    {
    }
}
