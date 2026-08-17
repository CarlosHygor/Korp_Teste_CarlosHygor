# 💻 KORP ERP Frontend (Angular 22 SPA)

Interface web SPA desenvolvida em **Angular 22** com **Standalone Components**, **Reactive Forms**, **RxJS** e **Vitest** para interação com os microsserviços de **Estoque** (`Estoque.API`) e **Faturamento** (`Faturamento.API`).

---

## 🏛️ Estrutura de Arquitetura & Componentes

```text
src/app/
├── core/
├── models/
│   ├── produto.model.ts          # Interfaces DTOs de Produtos
│   ├── nota-fiscal.model.ts      # Interfaces DTOs de Notas Fiscais e Itens
│   └── paged-result.model.ts     # Interface genérica PagedResult<T>
├── services/
│   ├── produto.service.ts        # Integração HTTP com Estoque.API (Porta 5000)
│   └── nota-fiscal.service.ts    # Integração HTTP com Faturamento.API (Porta 5002)
├── features/
│   ├── produtos/
│   │   └── componentes/
│   │       ├── produto-list/         # Tabela paginada + busca + ordenação por saldo
│   │       └── produto-form-modal/   # Modal reativo de cadastro e edição
│   └── notas-fiscais/
│       └── componentes/
│           ├── nota-list/            # Tabela de NFs + filtros de status + impressão resiliente
│           └── nota-form-modal/      # FormArray dinâmico para múltiplos produtos
└── shared/
    └── components/
        ├── accessibility-bar/    # Controle de escala de fonte (A-, A+) e Alto Contraste
        ├── loading-spinner/      # Spinner overlay e inline bloqueante
        ├── status-badge/         # Badge colorido (Aberta vs Fechada)
        ├── error-modal/          # Modal rico para HTTP 422 e Resiliência HTTP 503
        └── success-modal/        # Modal de feedback de operação concluída
```

---

## ♿ Recursos de Acessibilidade Web (WCAG 2.2 Level AAA)

- **🤟 VLibras (Widget Oficial)**: Integração com a Suíte VLibras do Governo Federal para tradução de conteúdos em LIBRAS.
- **🔤 Redimensionamento de Fonte (`A-`, `A`, `A+`)**: Controle da escala de fonte no `html` de 85% a 130%.
- **👁️ Modo Alto Contraste**: Alternância instantânea via CSS Variables (`:root` -> `.high-contrast`) com fundo preto absoluto (`#000000`) e elementos em amarelo de alto contraste (`#ffff00`).
- **⌨️ Anéis de Foco de Teclado**: Suporte completo a navegação por `Tab` e `Shift+Tab` com estilo de foco `:focus-visible`.

---

## 🚀 Como Executar o Frontend

### 1. Instalar Dependências
```bash
npm install
```

### 2. Rodar Servidor de Desenvolvimento Local
```bash
npm start
```
Acesse `http://localhost:4200` no navegador. A aplicação recarrega automaticamente ao salvar alterações nos arquivos.

---

## 🧪 Suíte de Testes Unitários (Vitest)

Para executar os testes unitários da interface:

```bash
npm test
```

Os testes cobrem a inicialização dos componentes, carregamento paginado, formatação de códigos, ordenações e manipulação das abas de filtro por status.

---

## 📦 Build de Produção

Para compilar o projeto para produção:

```bash
npm run build
```

Os artefatos compilados e otimizados serão gerados no diretório `dist/frontend`.

---

## 🛡️ Tratamento de Erros e Resiliência HTTP no Client

- **HTTP 200 OK**: Transita a nota fiscal para `Fechada` em tempo real na UI, exibe o modal de sucesso e recarrega os saldos de estoque.
- **HTTP 503 Service Unavailable**: Exibe modal rico de **Aviso de Resiliência (⚡)** informando que o serviço de Estoque está indisponível e que a Nota permaneceu **ABERTA** para nova tentativa.
- **HTTP 422 Unprocessable Entity**: Exibe detalhes ricos de saldo em estoque disponível vs quantidade solicitada.
- **HTTP 409 Conflict**: Exibe aviso de duplicidade de código de produto.
