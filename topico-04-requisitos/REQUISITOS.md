# Requisitos do usuário da Biblioteca Comunitária

Este arquivo é a lista única dos requisitos implementados no tópico 04. Cada
requisito tem um ID fixo. Os tópicos seguintes da disciplina, a começar pelo
diagrama de casos de uso, citam os requisitos por esses mesmos IDs.

Os números dos requisitos RF01 a RF03 vêm das regras do domínio no
[`README.md`](../README.md) da raiz do repositório.

| ID | Tipo | Descrição | Critério de aceite | Teste que verifica |
|---|---|---|---|---|
| RF01 | Funcional | O número de itens que um usuário mantém retirados ao mesmo tempo depende do perfil: Morador 2, Estudante 4, Voluntário 6. | Com o limite do perfil atingido, a retirada seguinte é recusada com uma mensagem que contém "Limite de N itens", em que N é o limite do perfil. | `RF01_LimiteDeItensDependeDoPerfil` (três casos, um por perfil) |
| RF02 | Funcional | Um usuário com multa em aberto não retira item novo. | Com uma multa registrada, a retirada é recusada com uma mensagem que contém "multa em aberto", mesmo para o perfil Voluntário, o de maior limite. | `RF02_MultaEmAbertoBloqueiaRetirada` |
| RF03 | Funcional | Quitar a multa libera o usuário para retirar itens de novo. | Depois de registrar e quitar uma multa, a retirada de "Dom Casmurro" é aceita e a lista de itens do usuário passa a conter só esse título. | `RF03_QuitarMultaLiberaNovaRetirada` |
| RNF01 | Não funcional | As mensagens de recusa são escritas em português e identificam quem foi recusado. | Toda mensagem de recusa de retirada começa pela palavra "Usuário". | `RNF01_MensagensDeRecusaEstaoEmPortugues` |
| RNF02 | Não funcional | O programa do tópico roda nos três sistemas operacionais usados pelos alunos. | O comando `dotnet run --project topico-04-requisitos` imprime as três linhas da saída esperada no Windows, no macOS e no Linux, com o .NET SDK 10. | Sem teste automatizado. Verificação manual nos três sistemas; a integração contínua cobre o Linux a cada envio. |

## Como ler a tabela

- **ID**: o prefixo RF marca requisito funcional, e o prefixo RNF marca
  requisito não funcional. O número não muda depois de publicado.
- **Tipo**: pelo critério do objeto descrito, RF01 a RF03 tratam de ações do
  sistema (recusar ou aceitar uma retirada) e são funcionais, e RNF01 e RNF02
  tratam de qualidades dessas ações (idioma da mensagem e sistemas
  operacionais) e são não funcionais.
- **Critério de aceite**: a condição observável que decide se o requisito foi
  cumprido.
- **Teste que verifica**: o método de
  [`tests/ModelagemOO.Tests/RequisitosUsuarioTests.cs`](../tests/ModelagemOO.Tests/RequisitosUsuarioTests.cs)
  cujo nome começa pelo ID do requisito.

O teste `NomeVazioERecusado`, do mesmo arquivo, confere a guarda do construtor
de `Usuario` e não corresponde a nenhum requisito desta tabela.
