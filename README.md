# BankMore - Banco Digital

BankMore é uma aplicação mínima de banco digital, implementada em **.NET 8**, usando **Clean Architecture** e **CQRS** com **MediatR**. O sistema permite cadastro de contas, autenticação, movimentações, transferências e consulta de saldo, focando apenas nas funcionalidades essenciais para um teste técnico.

---

## Arquitetura

O projeto segue a **Clean Architecture**, organizada em:

- **Domain**: Entidades, interfaces e regras de negócio.
- **Application**: Commands, Queries, Handlers, Exceptions.
- **Infrastructure**: Repositórios, EF Core, SQLite, JWT, Bcrypt.  
  > Banco local usando **SQLite**, criado automaticamente via `EnsureCreated()` para facilitar testes sem precisar de migrations.
- **API**: Controllers, Endpoints, autenticação JWT, Swagger.

**Padrões aplicados**:  
- CQRS (Commands e Queries separados)  
- MediatR para orquestração de Commands e Queries  
- Async/await em toda a aplicação  
- Idempotência para movimentações e transferências

---

## Funcionalidades

| Funcionalidade                 | Status |
|--------------------------------|--------|
| Cadastro de conta               | ✅     |
| Login / JWT                     | ✅     |
| Inativar conta                  | ✅     |
| Consulta de saldo               | ✅     |
| Movimentação (Crédito/Débito)  | ✅     |
| Transferência entre contas      | ✅     |

**Observações importantes:**
- Movimentações e transferências são **idempotentes**, baseadas em `IdentificacaoRequisicao`.
- Saldo é calculado somando créditos (`C`) e debitando débitos (`D`).
- Os repositórios não chamam `SaveChanges` internamente; a persistência é controlada nos Handlers.
- Testes unitários implementados para `MovimentarContaCommandHandler`.

---

## Endpoints Principais

### ContaCorrenteController

| Endpoint                       | Método | Descrição                          | Request Body Exemplo |
|--------------------------------|--------|-----------------------------------|--------------------|
| `/api/ContaCorrente/cadastrar` | POST   | Cadastro de nova conta            | `{ "nomeTitular": "Ana", "numeroConta": "100001" }` |
| `/api/ContaCorrente/login`     | POST   | Autenticação e geração de JWT      | `{ "numeroConta": "100001" }` |
| `/api/ContaCorrente/movimentacao` | POST | Depósito (`C`) ou saque (`D`)     | `{ "identificacaoRequisicao": "req001", "numeroConta": "100001", "valor": 100.50, "tipo": "C" }` |
| `/api/ContaCorrente/inativar`  | POST   | Inativar conta                     | `{ "numeroConta": "100001" }` |
| `/api/ContaCorrente/saldo`     | GET    | Consulta de saldo                  | N/A |

### TransferenciaController

| Endpoint            | Método | Descrição                      | Request Body Exemplo |
|--------------------|--------|--------------------------------|--------------------|
| `/api/Transferencia` | POST   | Transferência entre contas     | `{ "identificacaoRequisicao": "req001", "numeroContaDestino": "100002", "valor": 50 }` |

---

## Tecnologias e Pacotes

- **.NET 8**
- **Entity Framework Core** com **SQLite**
- **MediatR** (Commands/Queries)
- **JWT Bearer** para autenticação
- **Bcrypt** para hashing de senhas
- **Swagger** para documentação de API

---

## Testes

- Testes unitários para movimentações (`MovimentarContaCommandHandler`) cobrindo:
  - Criação de movimento
  - Validação de conta
  - Idempotência
- Testes de endpoints podem ser feitos via **Postman** ou **Swagger UI**.

---

## Exemplos de Requests

### Depósito
POST /api/ContaCorrente/movimentacao
Content-Type: application/json

{
"identificacaoRequisicao": "req001",
"numeroConta": "100001",
"valor": 100.50,
"tipo": "C"
}

### Saque
POST /api/ContaCorrente/movimentacao
Content-Type: application/json

{
"identificacaoRequisicao": "req002",
"numeroConta": "100001",
"valor": 50.25,
"tipo": "D"
}

### Transferência
POST /api/Transferencia
Content-Type: application/json

{
"identificacaoRequisicao": "req003",
"numeroContaDestino": "100002",
"valor": 75
}
