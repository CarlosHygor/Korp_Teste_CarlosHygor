# 🧪 Relatório Centralizado de Execução de Testes

> **Última execução:** `18/08/2026, 00:36:09 UTC`

---

## 📊 Resumo Geral (KPIs)

| Métrica | Valor |
| :--- | :--- |
| 🧮 **Total de Testes** | **67** |
| ✅ **Aprovados** | **67** |
| ❌ **Reprovados** | **0** |
| 📈 **Taxa de Sucesso** | **100.0%** |

---

## 📦 1. API de Estoque (.NET 8)
- **Status:** ✅ Passou
- **Aprovados:** `25/25`

| Status | Teste | Duração |
| :---: | :--- | :--- |
| ✅ | `EstornarEstoqueAsync_DeveRestabelecerSaldo_QuandoQuantidadeForValida` | `00:00:00` |
| ✅ | `EstornarEstoqueLoteAsync_DeveRestabelecerSaldoDeTodosOsProdutosAtomicamente` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_QuandoQuantidadeForInvalida_DeveLancarArgumentException(quantidadeInvalida: 0)` | `00:00:00` |
| ✅ | `AbaterEstoque_QuandoSaldoForInsuficiente_DeveRetornarStatus422UnprocessableEntity` | `00:00:00` |
| ✅ | `AbaterEstoqueLoteAsync_QuandoUmItemTiverEstoqueInsuficiente_DeveRealizarRollbackAtomicamenteDeTodosOsItens` | `00:00:01` |
| ✅ | `AbaterEstoqueLote_QuandoTodosOsItensForemValidos_DeveRetornarStatus200OK` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_ConcorrenciaSimultanea_DeveImpedirOverbookingEManterSaldoZero` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_QuandoSaldoForSuficiente_DeveAbaterSaldoEAtualizarNoRepositorio` | `00:00:00` |
| ✅ | `AbaterEstoque_QuandoSaldoForSuficiente_DeveRetornarStatus200OK` | `00:00:01` |
| ✅ | `CreateAsync_QuandoBancoLancarDbUpdateException_DeveTraduzirParaCodigoProdutoDuplicadoException` | `00:00:00` |
| ✅ | `CreateAsync_QuandoDadosForemValidos_DeveAdicionarEGrafarProduto` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_QuandoProdutoNaoExistir_DeveLancarKeyNotFoundException` | `00:00:00` |
| ✅ | `AddAsync_E_GetByCodigoAsync_DeveSalvarEBuscarProdutoRealNoDbContext` | `00:00:00` |
| ✅ | `AbaterEstoqueLoteAsync_ComIdempotencyKey_DeveAbaterApenasNaPrimeiraChamadaEIgnorarNoReenvio` | `00:00:00` |
| ✅ | `UpdateAsync_DeveAtualizarProdutoNoDbContextInMemory` | `00:00:00` |
| ✅ | `GetPaginatedAsync_ComOrdenacaoSaldo_DeveRepassarOrdenacaoParaRepositorio` | `00:00:00` |
| ✅ | `EstornarEstoqueLote_QuandoRequisicaoForValida_DeveRetornarStatus200OK` | `00:00:00` |
| ✅ | `AbaterEstoqueLoteAsync_QuandoUmItemDaListaForInexistente_DeveRealizarRollbackAtomicamenteDeTodosOsItens` | `00:00:00` |
| ✅ | `CreateAsync_QuandoSaldoForNegativo_DeveLancarArgumentException` | `00:00:00` |
| ✅ | `GetPaginatedAsync_DeveRetornarResultadoPaginadoEPropriedadesCalculadas` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_QuandoQuantidadeForInvalida_DeveLancarArgumentException(quantidadeInvalida: -5)` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_QuandoSaldoForInsuficiente_DeveLancarEstoqueInsuficienteExceptionENaoAlterarSaldo` | `00:00:00` |
| ✅ | `AbaterEstoqueLoteAsync_DeveAbaterTodosOsProdutos_QuandoTodosForemValidos` | `00:00:00` |
| ✅ | `AbaterEstoque_QuandoQuantidadeForInvalida_DeveRetornarStatus400BadRequest` | `00:00:00` |
| ✅ | `GetPaginatedAsync_ComTermoBusca_DeveRepassarTermoParaRepositorio` | `00:00:00` |

---

## 💰 2. API de Faturamento (.NET 8)
- **Status:** ✅ Passou
- **Aprovados:** `18/18`

| Status | Teste | Duração |
| :---: | :--- | :--- |
| ✅ | `GetPaginatedAsync_ComFiltroStatus_DeveRepassarFiltroParaRepositorio` | `00:00:00` |
| ✅ | `ImprimirAsync_DeveManterNotaAbertaEPropagarExcecao_QuandoAbateEmLoteFalhar` | `00:00:00` |
| ✅ | `GetPaginatedAsync_ComOrdenacao_DeveRepassarOrdenacaoParaRepositorio` | `00:00:00` |
| ✅ | `Create_QuandoPayloadForValido_DeveRetornarStatus201Created` | `00:00:00` |
| ✅ | `GetByNumeracaoAsync_DeveRetornarNotaFiscal_QuandoNumeracaoExistir` | `00:00:00` |
| ✅ | `CreateAsync_DeveLancarArgumentException_QuandoNotaNaoPossuirItens` | `00:00:00` |
| ✅ | `GetPaginatedAsync_DeveRetornarResultadoPaginadoEPropriedadesCalculadas` | `00:00:00` |
| ✅ | `GetByNumeracao_QuandoNumeracaoExistir_DeveRetornarStatus200OKEPayloadValido` | `00:00:00` |
| ✅ | `Create_QuandoListaDeItensForVazia_DeveRetornarStatus400BadRequest` | `00:00:00` |
| ✅ | `ImprimirAsync_DeveAbaterEstoqueEmLoteEAtualizarStatusParaFechada_QuandoNotaEstiverAberta` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_DeveLancarArgumentException_QuandoQuantidadeForZeroOuNegativa` | `00:00:00` |
| ✅ | `ImprimirAsync_DeveLancarNotaFiscalStatusInvalidoException_QuandoNotaJaEstiverFechada` | `00:00:00` |
| ✅ | `AbaterEstoqueAsync_DeveInvocarEstoqueClient_QuandoItemForValido` | `00:00:00` |
| ✅ | `Imprimir_QuandoNotaEstiverAbertaEEstoqueComSucesso_DeveRetornarStatus200OKENotaFechada` | `00:00:00` |
| ✅ | `ImprimirAsync_DeveDispararEstornoDeEstoque_QuandoUpdateDoBancoFaturamentoFalhar` | `00:00:00` |
| ✅ | `CreateAsync_DeveCriarNotaFiscalComStatusAbertaEDataUtc_QuandoDadosForemValidos` | `00:00:00` |
| ✅ | `GetAll_DeveRetornarStatus200OKEListaDeNotas` | `00:00:00` |
| ✅ | `CreateAsync_DevePersistirNotaComStatusInicialAbertaEItensRelacionados` | `00:00:00` |

---

## 🌐 3. Frontend (Angular / Vitest)
- **Status:** ✅ Passou
- **Aprovados:** `24/24`

| Status | Teste | Duração |
| :---: | :--- | :--- |
| ✅ | `[app.spec.ts] deve criar a aplicação principal` | `< 100ms` |
| ✅ | `[app.spec.ts] deve renderizar o título do sistema no header` | `< 100ms` |
| ✅ | `[app.spec.ts] deve possuir as abas de navegação para Produtos e Notas Fiscais` | `< 100ms` |
| ✅ | `[nota-list.spec.ts] deve criar o componente` | `< 100ms` |
| ✅ | `[nota-list.spec.ts] deve carregar notas fiscais paginadas ao inicializar` | `< 100ms` |
| ✅ | `[nota-list.spec.ts] deve formatar a numeração com 4 dígitos` | `< 100ms` |
| ✅ | `[nota-list.spec.ts] deve alternar a expansão da linha de itens` | `< 100ms` |
| ✅ | `[nota-list.spec.ts] deve filtrar por status ao clicar na aba` | `< 100ms` |
| ✅ | `[produto-form-modal.spec.ts] deve criar o componente` | `< 100ms` |
| ✅ | `[produto-form-modal.spec.ts] deve validar formulário como inválido quando campos estiverem vazios` | `< 100ms` |
| ✅ | `[produto-form-modal.spec.ts] deve chamar produtoService.create ao enviar formulário válido` | `< 100ms` |
| ✅ | `[produto-form-modal.spec.ts] deve exibir mensagem de erro HTTP 409 quando o código for duplicado` | `< 100ms` |
| ✅ | `[produto-list.spec.ts] deve criar o componente` | `< 100ms` |
| ✅ | `[produto-list.spec.ts] deve carregar produtos paginados ao inicializar` | `< 100ms` |
| ✅ | `[produto-list.spec.ts] deve retornar classe e texto de status de saldo corretos` | `< 100ms` |
| ✅ | `[produto-list.spec.ts] deve abrir o modal de cadastro ao clicar no botão` | `< 100ms` |
| ✅ | `[error-modal.spec.ts] deve criar o componente` | `< 100ms` |
| ✅ | `[error-modal.spec.ts] não deve exibir modal se erro for nulo` | `< 100ms` |
| ✅ | `[error-modal.spec.ts] deve exibir mensagem de erro e fechar ao clicar no botão` | `< 100ms` |
| ✅ | `[error-modal.spec.ts] deve renderizar detalhes de estoque insuficiente quando fornecidos` | `< 100ms` |
| ✅ | `[status-badge.spec.ts] deve criar o componente` | `< 100ms` |
| ✅ | `[status-badge.spec.ts] deve renderizar status Aberta corretamente por padrão` | `< 100ms` |
| ✅ | `[status-badge.spec.ts] deve renderizar status Fechada corretamente` | `< 100ms` |
| ✅ | `[status-badge.spec.ts] deve converter número enum 1 para Fechada` | `< 100ms` |

---

*Relatório gerado automaticamente por `scripts/generate-test-report.mjs`.*
