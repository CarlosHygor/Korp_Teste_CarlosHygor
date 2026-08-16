namespace Estoque.API.DTOs;

/// <summary>
/// DTO genérico para encapsulamento de respostas paginadas.
/// </summary>
public record PagedResultDto<T>(
    IEnumerable<T> Itens,
    int PaginaAtual,
    int TamanhoPagina,
    int TotalRegistros,
    int TotalPaginas,
    bool TemPaginaAnterior,
    bool TemProximaPagina
);
