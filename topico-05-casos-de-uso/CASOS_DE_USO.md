# Casos de uso da Biblioteca Comunitária

Este arquivo lista os casos de uso do sistema da Biblioteca Comunitária e traz
a especificação completa do UC01, Realizar empréstimo. O diagrama está em
[`casos-de-uso.puml`](./casos-de-uso.puml), escrito em PlantUML.

Os requisitos citados aqui são os de
[`topico-04-requisitos/REQUISITOS.md`](../topico-04-requisitos/REQUISITOS.md),
pelos mesmos IDs.

## Atores

| Ator | Quem é | Como usa o sistema |
|---|---|---|
| Atendente | Pessoa que trabalha no balcão, como Dona Marta | Registra empréstimo, devolução e pagamento de multa no computador do balcão |
| Usuário | Morador, estudante ou voluntário cadastrado | Faz a reserva sozinho, pelo catálogo on-line da biblioteca |

No empréstimo, o usuário conversa com a atendente e não toca no sistema. Por
isso ele não aparece ligado ao UC01 no diagrama. Ele aparece ligado ao UC03,
porque faz a reserva sem a atendente. O catálogo on-line é uma decisão de
modelagem deste tópico; o código do repositório ainda não implementa reserva.

## Lista de casos de uso

| ID | Caso de uso | Ator principal | Relação | Requisitos |
|---|---|---|---|---|
| UC01 | Realizar empréstimo | Atendente | Inclui UC05 | RF01, RF02, RF03, RNF01 |
| UC02 | Devolver item | Atendente | Estendido por UC06 | Ainda sem ID |
| UC03 | Reservar item | Usuário | Inclui UC05 | Ainda sem ID |
| UC04 | Pagar multa | Atendente | Nenhuma | RF03 |
| UC05 | Verificar pendências | Nenhum, só é executado dentro de UC01 e UC03 | Incluído por UC01 e UC03 | RF01, RF02 |
| UC06 | Cobrar multa por atraso | Nenhum, só é executado dentro de UC02 | Estende UC02 quando a devolução acontece depois do prazo | Ainda sem ID |

Critério usado para as duas relações:

- **`<<include>>`**: o caso incluído roda **sempre** que o caso base roda.
  Todo empréstimo e toda reserva verificam pendências.
- **`<<extend>>`**: o caso que estende roda **só quando uma condição vale**.
  A cobrança de multa acontece apenas na devolução feita depois do prazo.

## UC01 Realizar empréstimo

| Campo | Conteúdo |
|---|---|
| ID | UC01 |
| Ator principal | Atendente |
| Interessado | Usuário, que quer levar o item para casa |
| Pré-condição | O usuário está cadastrado com um perfil (Morador, Estudante ou Voluntário) e entregou no balcão o item escolhido |
| Pós-condição de sucesso | O título passa a constar na lista de itens do usuário |
| Pós-condição de recusa | A lista de itens do usuário não muda |
| Requisitos rastreados | RF01, RF02, RF03, RNF01 |
| Código | `Balcao.Emprestar`, em [`Balcao.cs`](./Balcao.cs) |
| Testes | `tests/ModelagemOO.Tests/CasosDeUsoTests.cs`, métodos `UC01_*` |

### Fluxo principal

1. O usuário entrega o item à atendente e informa o nome.
2. A atendente informa ao sistema o usuário e o título do item.
3. O sistema verifica as pendências do usuário (inclui UC05): multa em aberto
   (RF02) e limite do perfil (RF01).
4. O sistema registra o empréstimo e acrescenta o título à lista de itens do
   usuário.
5. O sistema mostra a confirmação `Empréstimo de "<título>" registrado para
   <nome>.`, e o caso de uso termina.

### Fluxos alternativos

| Fluxo | Começa no passo | Condição | O que acontece | Requisito | Teste |
|---|---|---|---|---|---|
| A1 | 3 | O usuário já está com o limite do perfil | O sistema recusa com `Usuário atingiu o Limite de N itens do perfil P.`, e o caso de uso termina | RF01, RNF01 | `UC01_A1_LimiteDoPerfilAtingido_RF01` |
| A2 | 3 | O usuário tem multa em aberto | O sistema recusa com `Usuário com multa em aberto não retira item novo.`, e a atendente oferece o UC04, Pagar multa | RF02, RNF01 | `UC01_A2_MultaEmAberto_RF02` |
| A3 | Fim do A2 | O usuário quitou a multa pelo UC04 | O caso de uso volta ao passo 2 e segue o fluxo principal | RF03 | `UC01_A3_AposQuitarAMulta_RF03` |

O fluxo principal é verificado pelo teste `UC01_FluxoPrincipal_EmprestimoAceito`.

A guarda da multa vem antes da guarda do limite em `Usuario.Retirar`. Um
usuário com multa e com o limite atingido cai no A2.

## Rastreio

| Requisito | Caso de uso e fluxo | Teste do caso de uso | Teste do requisito |
|---|---|---|---|
| RF01 | UC01, passo 3 e fluxo A1 | `UC01_A1_LimiteDoPerfilAtingido_RF01` | `RF01_LimiteDeItensDependeDoPerfil` |
| RF02 | UC01, passo 3 e fluxo A2 | `UC01_A2_MultaEmAberto_RF02` | `RF02_MultaEmAbertoBloqueiaRetirada` |
| RF03 | UC01, fluxo A3 | `UC01_A3_AposQuitarAMulta_RF03` | `RF03_QuitarMultaLiberaNovaRetirada` |
| RNF01 | UC01, mensagens dos fluxos A1 e A2 | `UC01_A1_*` e `UC01_A2_*` conferem o início "Usuário" | `RNF01_MensagensDeRecusaEstaoEmPortugues` |
