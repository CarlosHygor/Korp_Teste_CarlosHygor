namespace Estoque.API.Exceptions;

public class CodigoProdutoDuplicadoException : Exception
{
    public string Codigo { get; }

    public CodigoProdutoDuplicadoException(string codigo)
        : base($"Já existe um produto cadastrado com o código '{codigo}'.")
    {
        Codigo = codigo;
    }
}
