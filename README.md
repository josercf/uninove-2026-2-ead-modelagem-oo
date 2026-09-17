# Modelagem e Desenvolvimento Orientado a Objetos, EAD Uninove

Este repositório reúne os exemplos executáveis da disciplina Modelagem e Desenvolvimento Orientado a Objetos, do curso de Análise e Desenvolvimento de Sistemas da Uninove, oferta EAD de 2026.2.

Todo o código aqui usa um único case ao longo do semestre: a Biblioteca Comunitária. É um sistema de empréstimo de livros, revistas e mídias, com usuários de perfis diferentes, reserva, devolução e multa. Cada tópico da disciplina adiciona uma peça nova a esse sistema.

Os trechos de código citados nas aulas apontam para os arquivos deste repositório. Se algum trecho da aula estiver diferente do código aqui, isso é um erro do material e deve ser reportado ao professor.

## Tópicos

| Tópico | Assunto | Pasta |
|---|---|---|
| 02 | Classes e objetos | [`topico-02-classes-e-objetos`](./topico-02-classes-e-objetos) |

Os demais tópicos da disciplina serão adicionados a esta tabela conforme as próximas aulas forem publicadas.

## Como rodar tudo de uma vez

Estes comandos rodam a partir da pasta raiz do repositório (a pasta onde está este `README.md`).

1. Restaurar as dependências de todos os projetos:

   ```bash
   dotnet restore
   ```

2. Compilar a solução inteira:

   ```bash
   dotnet build
   ```

   Resultado esperado: a mensagem final mostra `Build succeeded`.

3. Rodar todos os testes automatizados de todos os tópicos:

   ```bash
   dotnet test
   ```

   Resultado esperado: todos os testes aparecem como `Passed`.

Para rodar um exemplo específico, entre na pasta do tópico indicada na tabela acima e siga o `README.md` de dentro dela. Cada tópico tem seu próprio passo a passo, com a saída esperada e uma seção de ajuda caso algo dê errado.

## Pré-requisitos gerais

- .NET SDK 10.0 ou superior instalado. Confirme com `dotnet --version`.
- Git instalado, para clonar o repositório.

## Estrutura do repositório

- `topico-NN-nome-do-assunto/`: um projeto executável por tópico da disciplina.
- `tests/ModelagemOO.Tests/`: os testes automatizados de todos os tópicos, em um único projeto xUnit.
- `.github/workflows/ci.yml`: a integração contínua, que compila e testa o repositório a cada envio de código.

## Licença e uso didático

Este repositório está sob a licença MIT (veja o arquivo [`LICENSE`](./LICENSE)). O código foi escrito para fins didáticos, como material de apoio da disciplina, e não passou por revisão de segurança nem foi pensado para uso em produção.
