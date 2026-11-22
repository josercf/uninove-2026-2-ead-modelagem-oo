# Tópico 11: Princípios SOLID e qualidade de código

Este exemplo parte do código dos tópicos 05, 07, 09 e 10 e aplica os princípios SOLID em quatro recortes: responsabilidade única medida em motivos de mudança, aberto-fechado e inversão de dependência a partir do Strategy do tópico 10, refatoração guiada por teste em cinco passos e o limite de um princípio aplicado a código que não muda.

Os valores de multa por perfil (R$ 1,00, R$ 0,50 e R$ 0,25 por dia) são os dados de exemplo do tópico 10, sem relação com regra de biblioteca real.

| Recorte | Antes | Depois | O que o teste confere |
|---|---|---|---|
| Responsabilidade única | [`Responsabilidade/Antes/BalcaoDeTudo.cs`](./Responsabilidade/Antes/BalcaoDeTudo.cs), um tipo com 4 motivos de mudança | [`Responsabilidade/Depois/`](./Responsabilidade/Depois), 5 tipos com 1 motivo cada | Mesmo resultado e mesmo rastro no cenário comum, e a contagem de motivos por reflexão |
| Aberto-fechado e inversão de dependência | [`Inversao/Antes/`](./Inversao/Antes), a devolução cria 3 classes de multa | [`Inversao/Depois/`](./Inversao/Depois), a devolução recebe `IPoliticaDeMulta` | Mesma multa por perfil, política nova sem editar classe existente, dependências medidas |
| Refatoração guiada por teste | [`Refatoracao/Passo0.cs`](./Refatoracao/Passo0.cs), um método com tudo | [`Refatoracao/Passo1.cs`](./Refatoracao/Passo1.cs) a [`Passo4.cs`](./Refatoracao/Passo4.cs) | O mesmo teste contra os cinco passos |
| Limite | [`Excesso/Simples/`](./Excesso/Simples), uma expressão | [`Excesso/ComPrincipios/`](./Excesso/ComPrincipios), duas interfaces e um serviço | Mesmo texto, e uma só implementação por interface |

[`MedidorDeMotivos.cs`](./MedidorDeMotivos.cs) conta os rótulos `[MotivoDeMudanca]` dos métodos de um tipo. [`MedidorDeDependencias.cs`](./MedidorDeDependencias.cs) conta as classes concretas e as abstrações que um tipo guarda em campos e recebe no construtor. [`MedidorDeTamanhoDoTopico.cs`](./MedidorDeTamanhoDoTopico.cs) reaproveita a definição de linha com código do tópico 10: linha que não está vazia e não é comentário, chave isolada, `using` nem `namespace`.

## Como o histórico do `Balcao` foi medido

O comando a seguir lista os commits que tocaram os arquivos do `Balcao` ao longo dos tópicos.

```bash
git log --format='%h %s' -- 'topico-05-casos-de-uso/Balcao.cs' 'topico-07-sequencia-e-atividades/BalcaoComRastro.cs' 'topico-09-analise-e-projeto/Antes/Balcao.cs' 'topico-09-analise-e-projeto/Depois/Balcao.cs'
```

Saída:

```text
e519367 feat: tópico 09, análise e projeto orientados a objetos, com acoplamento medido por reflexão antes e depois do GRASP
86ca219 feat: tópico 07, diagrama de sequência e diagrama de atividades, com teste que confere o rastro contra os diagramas
213663c feat: tópico 05, casos de uso com UC01 testado por fluxo
```

Cada arquivo do `Balcao` nasceu num único commit e não foi editado depois, então o `git log` não mostra mudança dentro de um mesmo arquivo. A medida usa, por isso, as versões das pastas `topico-NN`. As três primeiras linhas da tabela abaixo são classificação feita à mão sobre o código de cada tópico. O critério é este: um motivo de mudança existe quando o corpo da classe decide uma regra, monta um texto ou grava um registro que outra pessoa ou outro documento mudaria. A última linha é medida por reflexão sobre `BalcaoDeTudo`. A tabela está em [`Responsabilidade/HistoricoDoBalcao.cs`](./Responsabilidade/HistoricoDoBalcao.cs).

| Versão do `Balcao` | Métodos públicos | Parâmetros do construtor | Motivos de mudança |
|---|---|---|---|
| `topico-05`, `Balcao` | 1 | 0 | 2: regra de empréstimo, texto das mensagens |
| `topico-07`, `BalcaoComRastro` | 2 | 2 | 4: regra de empréstimo, regra de multa, texto das mensagens, registro do rastro |
| `topico-09`, `Depois.Balcao` | 1 | 1 | 2: ordem do atendimento, texto das mensagens |
| `topico-11`, `BalcaoDeTudo` | 2 | 0 | 4, medidos por reflexão |

As duas primeiras colunas são conferidas por teste (`HistoricoDoBalcaoPorVersoesDosTopicos`). A terceira das três primeiras linhas não é medida por máquina: é a leitura do código segundo o critério acima.

## Pré-requisitos

1. .NET SDK 10.0 ou superior.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`.

2. O repositório clonado e atualizado. Veja a seção "Como começar" no [`README.md`](../README.md) da raiz.

## Como executar

Rode cada comando a partir da pasta raiz do repositório.

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: mensagens de "Restaurado" (ou "Restored") e nenhuma linha de erro.

### Passo 2: Rodar os testes da responsabilidade única

```bash
dotnet test --filter ResponsabilidadeUnicaTests
```

Resultado esperado: a última linha começa como a linha abaixo. A duração varia.

```text
Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17, Duration: 198 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 3: Rodar os testes de aberto-fechado e inversão de dependência

```bash
dotnet test --filter InversaoDeDependenciaTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:    14, Skipped:     0, Total:    14, Duration: 101 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 4: Rodar os testes da refatoração, passo a passo

O arquivo `RefatoracaoPorPassosTests.cs` é o mesmo para os cinco passos. O comando abaixo roda só os casos de um passo. Troque `Passo0` por `Passo1`, `Passo2`, `Passo3` e `Passo4` e confira que o resultado é verde em todos.

```bash
dotnet test --filter "FullyQualifiedName~RefatoracaoPorPassosTests&DisplayName~Passo0"
```

Resultado esperado: 8 testes aprovados em `Passo0`, `Passo1`, `Passo3` e `Passo4`, e 9 em `Passo2`, porque o nome do teste `OMetodoPrincipalFicaMenorQueOOriginalDepoisDoPasso2` também contém `Passo2`.

```text
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8, Duration: 217 ms - ModelagemOO.Tests.dll (net10.0)
```

Para rodar o arquivo inteiro:

```bash
dotnet test --filter RefatoracaoPorPassosTests
```

```text
Passed!  - Failed:     0, Passed:    45, Skipped:     0, Total:    45, Duration: 148 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 5: Rodar os testes do limite

```bash
dotnet test --filter ExcessoDePrincipiosTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9, Duration: 124 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 6: Rodar o exemplo

```bash
dotnet run --project topico-11-solid
```

Resultado esperado: o texto da próxima seção.

## Saída esperada

```text
Responsabilidade única, mesmo cenário nos dois desenhos:
  antes:  Empréstimo de "Dom Casmurro" registrado para Joana.
          devolução: 3 dia(s), multa R$ 1,50
  depois: Empréstimo de "Dom Casmurro" registrado para Joana.
          devolução: 3 dia(s), multa R$ 1,50

Motivos de mudança por versão do Balcão:
  topico-05          2 motivo(s)
  topico-07          4 motivo(s)
  topico-09 depois   2 motivo(s)
  topico-11 antes    4 motivo(s)
  depois, BalcaoDeAtendimento  1 motivo(s)
  depois, RastroDoBalcao       1 motivo(s)
  depois, RegraDeEmprestimo    1 motivo(s)
  depois, RegraDeMulta         1 motivo(s)
  depois, TextoDoBalcao        1 motivo(s)

Aberto-fechado e inversão de dependência, multa de 3 dias:
  Morador    antes R$ 3,00  depois R$ 3,00
  Estudante  antes R$ 1,50  depois R$ 1,50
  Voluntario antes R$ 0,75  depois R$ 0,75
  dependências antes:  3 concreta(s), 0 abstração(ões)
  dependências depois: 0 concreta(s), 1 abstração(ões)

Refatoração em passos, o mesmo teste em cada passo:
  Passo0  1 método(s), 15 linhas, método principal 14 linhas: Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50.
  Passo1  2 método(s), 17 linhas, método principal 14 linhas: Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50.
  Passo2  3 método(s), 18 linhas, método principal  6 linhas: Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50.
  Passo3  5 método(s), 20 linhas, método principal  5 linhas: Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50.
  Passo4  4 método(s), 12 linhas, método principal  6 linhas: Dom Casmurro: 3 dia(s) de atraso, multa de R$ 1,50.

Limite, mensagem de confirmação com um texto só:
  simples:       Empréstimo de "Dom Casmurro" registrado para Joana.
  com princípios: Empréstimo de "Dom Casmurro" registrado para Joana.

Tamanho de cada versão (tipos e linhas com código):
  Responsabilidade, antes    1 tipo(s),  35 linhas
  Responsabilidade, depois   5 tipo(s),  43 linhas
  Inversão, antes            1 tipo(s),  13 linhas
  Inversão, depois           1 tipo(s),   8 linhas
  Confirmação, simples       1 tipo(s),   2 linhas
  Confirmação, princípios    6 tipo(s),  16 linhas
```

O `Balcao` com quatro motivos de mudança em um tipo passa a ter cinco tipos com um motivo cada, e o código sobe de 35 para 43 linhas. A inversão de dependência reduz a devolução de 13 para 8 linhas porque as três políticas já existem no tópico 10 e não são contadas nos dois lados. A mensagem de confirmação, que tem um texto só, passa de 1 tipo e 2 linhas para 6 tipos e 16 linhas.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

O SDK do .NET não está instalado, ou não está no PATH do terminal. Veja a correção no `README.md` do [tópico 06](../topico-06-classes-e-objetos/README.md).

### Erro 2: `ContagemDeTiposEDeLinhasDasDuasVersoes` ou `TamanhoDeCadaPassoEmMetodosELinhasComCodigo` falha com números diferentes

Alguém editou um arquivo de `Excesso/` ou de `Refatoracao/`. Os testes fixam os números medidos. Atualize os números do teste se a mudança foi intencional.

### Erro 3: "ModelagemOO.sln não encontrada acima do executável"

Os testes de medida procuram a solução subindo a partir da pasta do executável. Rode `dotnet test` de dentro do repositório clonado, sem copiar a pasta `bin` para outro lugar.

## Limites conhecidos

- O rótulo `[MotivoDeMudanca]` é declaração de quem escreveu o código. O teste confere que todo método de `Antes/` e `Depois/` tem rótulo e conta os rótulos distintos, mas não verifica que o rótulo descreve o corpo do método.
- A contagem de motivos do histórico do `Balcao` nos tópicos 05, 07 e 09 é leitura feita à mão, porque o `git log` guarda um commit por arquivo. Só a última linha da tabela é medida por reflexão.
- O aberto-fechado vale para a variação que a interface previu. `IPoliticaDeMulta.Calcular(int)` recebe só os dias de atraso. Uma multa que dependa do tipo do item exige ampliar a interface e editar as três políticas existentes.
- A inversão de dependência move a escolha da política para fora da devolução. Quem monta o objeto, como `PoliticasPorPerfil.Para`, continua conhecendo as classes concretas.
- Os cinco passos da refatoração preservam o resultado dos sete casos do teste. Um comportamento que nenhum caso cobre, como um valor de multa para um perfil que não está no teste, pode mudar sem que o teste perceba.
- As duas interfaces de `Excesso/ComPrincipios` têm uma implementação cada, e a medida mostra só o custo. Se um segundo idioma ou um segundo canal de entrega surgisse, as interfaces passariam a ter uso.
- A contagem de linhas ignora comentários e chaves isoladas e depende do estilo de formatação do código. Os números comparam as versões deste repositório.

## Experimente mudar

Faça cada mudança separadamente, rode o teste indicado e desfaça a mudança.

### Mudança 1: um erro de digitação no passo 3

Em `Refatoracao/Passo3.cs`, troque `0.50m` por `0.60m` em `ValorDoEstudante`.

Pergunta: que passos do `RefatoracaoPorPassosTests` falham, e por que o `Passo2` continua verde?

### Mudança 2: uma política nova sem editar a devolução

Em `tests/ModelagemOO.Tests/InversaoDeDependenciaTests.cs`, copie o teste `PoliticaNovaEntraNoDesenhoDepoisSemEditarClasseExistente` e troque a carência de 2 para 5 dias.

Pergunta: que arquivo de `Inversao/Depois/` você precisou editar, e que arquivo de `Inversao/Antes/` você precisaria editar para o mesmo perfil?

### Mudança 3: a devolução esquece de registrar a multa

Em `Responsabilidade/Depois/BalcaoDeAtendimento.cs`, troque `usuario.RegistrarMulta();` por `_ = usuario;`.

Pergunta: que teste de `ResponsabilidadeUnicaTests` falha, e o que ele diz sobre quem decide a multa em aberto?
