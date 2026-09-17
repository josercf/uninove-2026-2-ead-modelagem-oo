# Modelagem e Desenvolvimento Orientado a Objetos, EAD Uninove

Este repositório reúne os exemplos executáveis da disciplina Modelagem e Desenvolvimento Orientado a Objetos, do curso de Análise e Desenvolvimento de Sistemas da Uninove, oferta EAD de 2026.2.

Todo o código aqui usa um único case ao longo do semestre: a Biblioteca Comunitária. É um sistema de empréstimo de livros, revistas e mídias, com usuários de perfis diferentes, reserva, devolução e multa. Cada tópico da disciplina adiciona uma peça nova a esse sistema.

Os trechos de código citados nas aulas apontam para os arquivos deste repositório. Se algum trecho da aula estiver diferente do código aqui, isso é um erro do material e deve ser reportado ao professor.

## Regras do domínio da Biblioteca Comunitária

Os números desta seção valem para todos os tópicos da disciplina e para todo o
código deste repositório. Eles são a fonte única dessas regras. Aula e código
que divergirem daqui estão errados e devem ser corrigidos para estes valores.

### Prazo de empréstimo por tipo de item

| Tipo de item | Prazo |
|---|---|
| Livro | 15 dias |
| Revista | 7 dias |
| Mídia (DVD, CD) | 3 dias |

### Limite de itens simultâneos por perfil de usuário

| Perfil | Limite |
|---|---|
| Morador | 2 itens |
| Estudante | 4 itens |
| Voluntário | 6 itens |

### Operações e multa

O sistema trata quatro operações: empréstimo, reserva, devolução e multa.

A multa nasce do atraso na devolução. Cada dia de atraso cobra um valor fixo
por item em atraso.

O valor cobrado por dia é parâmetro do sistema, e não uma constante escrita no
meio do código. O exemplo que implementa a multa define esse parâmetro em um
lugar só.

Um usuário com multa em aberto não retira item novo e não faz reserva nova.

## Como começar

Estes são os dois comandos para trazer o repositório para a sua máquina.
Rode os dois, na ordem, em um terminal.

1. Clonar o repositório:

   ```bash
   git clone https://github.com/josercf/uninove-2026-2-ead-modelagem-oo.git
   ```

   Resultado esperado: aparece uma pasta nova chamada `uninove-2026-2-ead-modelagem-oo` no lugar onde você rodou o comando.

2. Entrar na pasta do repositório:

   ```bash
   cd uninove-2026-2-ead-modelagem-oo
   ```

   Resultado esperado: o terminal passa a mostrar `uninove-2026-2-ead-modelagem-oo` no caminho atual.

A partir daqui, todos os comandos deste `README.md` e dos README de cada tópico devem ser rodados de dentro desta pasta.

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
- `docs/adrs/`: as decisões arquiteturais deste repositório, uma por arquivo.

## Licença e uso didático

Este repositório está sob a licença MIT (veja o arquivo [`LICENSE`](./LICENSE)). O código foi escrito para fins didáticos, como material de apoio da disciplina, e não passou por revisão de segurança nem foi pensado para uso em produção.
