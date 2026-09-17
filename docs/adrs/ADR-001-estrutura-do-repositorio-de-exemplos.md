# ADR-001: Estrutura do repositório de exemplos

**Data:** 2026-09-17

**Status:** Aceita

**Decisores:** José Romualdo (professor, disciplina Modelagem e Desenvolvimento Orientado a Objetos)

## Contexto

Este repositório vai crescer ao longo do semestre até reunir 13 tópicos
(o Tópico 02, criado primeiro, mais os 12 seguintes). Duas escolhas
estruturais precisam valer para todos eles, e não fazia sentido decidir de
novo a cada tópico novo: o formato do arquivo de solução do .NET, e a
organização dos projetos de teste. O SDK 10 do .NET passou a gerar, por
padrão, uma solução no formato `.slnx` em vez do tradicional `.sln`. E cada
tópico precisa de testes automatizados que comprovem o comportamento do
exemplo, o que levanta a pergunta de ter um projeto de teste por tópico ou
um único projeto para todos.

## Decisão

Usamos o formato `.sln` (não o `.slnx` padrão do SDK 10) e um único projeto
de testes, `tests/ModelagemOO.Tests`, cobrindo todos os tópicos.

## Motivações

- **`.sln` em vez de `.slnx`:** o `.sln` ainda é o formato mais reconhecido
  por versões mais antigas do Visual Studio, do VS Code e de extensões de
  terceiros que alunos podem ter instaladas nas suas máquinas. O `.slnx` é
  um formato novo (XML), e alunos de ADS em ambiente EAD, que já relatam
  dificuldade com a ferramenta, não deveriam precisar descobrir por que a
  IDE deles não abre o arquivo de solução esperado.
- **Um único projeto de testes:** os alunos rodam `dotnet test` a partir da
  raiz do repositório e esperam ver todos os testes do curso em uma única
  execução, sem precisar descobrir em qual pasta cada teste mora. Isso
  também reduz a quantidade de arquivos de configuração (`.csproj`,
  pacotes NuGet) que precisam ser mantidos e atualizados a cada novo
  tópico.

## Riscos conhecidos e mitigações

- **Risco:** um único projeto de testes cresce e fica difícil de navegar
  conforme os 13 tópicos forem adicionados.
  **Mitigação:** cada tópico ganha sua própria classe de teste (por
  exemplo, `LivroTests.cs` para o Tópico 02), nomeada pela classe testada,
  em vez de misturar tudo em um arquivo só.
- **Risco:** o `.sln` manual pode ficar desatualizado se um novo projeto
  for criado sem passar por `dotnet sln add`.
  **Mitigação:** o `README.md` de cada novo tópico deve documentar o
  comando `dotnet sln add`, e a CI (`dotnet build` na raiz) falha se algum
  projeto não estiver referenciado corretamente na solução.

## Consequências

Positivas: um único comando (`dotnet test` na raiz) roda todos os testes do
curso, e a solução abre sem aviso em qualquer versão recente de IDE que os
alunos já tenham instalada.

Negativas: o projeto de testes cresce a cada tópico novo, sem isolamento
entre eles a nível de projeto (só a nível de arquivo e de classe), e o
`.sln` precisa ser atualizado manualmente (via `dotnet sln add`) toda vez
que um projeto novo for criado.

## ADRs relacionadas

Nenhuma até o momento. Este é o primeiro ADR do repositório.
