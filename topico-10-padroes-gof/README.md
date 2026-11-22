# Tópico 10: Padrões de projeto essenciais GoF

Este exemplo aplica três padrões do catálogo GoF (Strategy, Observer e Factory Method) a código da Biblioteca Comunitária e mostra, para cada um, o desenho antes e o desenho depois com o mesmo cenário de teste. Um quarto conjunto mostra o limite: uma regra constante embrulhada em três padrões, com a contagem de tipos e de linhas das duas versões.

Os valores de multa por perfil (R$ 1,00, R$ 0,50 e R$ 0,25 por dia) são dados de exemplo desta aula, sem relação com regra de biblioteca real. O prazo polimórfico dos itens vem do tópico 03, e o `Perfil` e o `Usuario` vêm do tópico 04.

| Padrão | Antes | Depois | Cenário que os dois cumprem |
|---|---|---|---|
| Strategy | [`Antes/CalculadoraDeMultaComSwitch.cs`](./Antes/CalculadoraDeMultaComSwitch.cs), um `switch` sobre o perfil | [`Depois/PoliticaDeMulta.cs`](./Depois/PoliticaDeMulta.cs), uma política por perfil | Multa de cada perfil para 0, 1 e 10 dias de atraso |
| Observer | [`Antes/DevolucaoComAvisoDireto.cs`](./Antes/DevolucaoComAvisoDireto.cs), a devolução chama cada interessado | [`Depois/Devolucao.cs`](./Depois/Devolucao.cs), interessados inscritos | As duas mensagens da devolução, no prazo e com atraso |
| Factory Method | [`Antes/CadastroComSwitch.cs`](./Antes/CadastroComSwitch.cs), um `switch` sobre o tipo | [`Depois/FabricasDeItem.cs`](./Depois/FabricasDeItem.cs), uma fábrica por tipo | Tipo e prazo do item criado, e as mensagens de recusa |
| Limite | [`Excesso/Simples/`](./Excesso/Simples), uma constante e um método | [`Excesso/ComPadroes/`](./Excesso/ComPadroes), Strategy, Factory e Observer | Resultado de `Permitida` para 0, 1, 2 e 5 renovações |

[`MedidorDeTamanho.cs`](./MedidorDeTamanho.cs) conta os tipos de um namespace por reflexão e as linhas com código dos arquivos. Linha com código é a que não está vazia e não é comentário, chave isolada, `using` nem `namespace`.

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

### Passo 2: Rodar os testes do Strategy

```bash
dotnet test --filter StrategyMultaTests
```

Resultado esperado: a última linha começa como a linha abaixo. A duração varia.

```text
Passed!  - Failed:     0, Passed:    24, Skipped:     0, Total:    24, Duration: 59 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 3: Rodar os testes do Observer

```bash
dotnet test --filter ObserverDevolucaoTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:     6, Skipped:     0, Total:     6, Duration: 16 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 4: Rodar os testes do Factory Method

```bash
dotnet test --filter FactoryItensTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:    13, Skipped:     0, Total:    13, Duration: 39 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 5: Rodar os testes do limite

```bash
dotnet test --filter ExcessoDePadroesTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9, Duration: 35 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 6: Rodar o exemplo

```bash
dotnet run --project topico-10-padroes-gof
```

Resultado esperado: o texto da próxima seção.

## Saída esperada

```text
Strategy, multa por perfil, 3 dias de atraso:
  Morador    antes R$ 3,00  depois R$ 3,00
  Estudante  antes R$ 1,50  depois R$ 1,50
  Voluntario antes R$ 0,75  depois R$ 0,75

Observer, aviso de devolução:
  antes:
    Aviso a Joana: "Dom Casmurro" devolvido com 3 dia(s) de atraso.
    Painel: "Dom Casmurro" disponível no acervo.
  depois:
    Aviso a Joana: "Dom Casmurro" devolvido com 3 dia(s) de atraso.
    Painel: "Dom Casmurro" disponível no acervo.

Factory, cadastro de itens do acervo:
  livro    antes Livro (15 dias)  depois Livro (15 dias)
  revista  antes Revista (7 dias)  depois Revista (7 dias)
  midia    antes Midia (3 dias)  depois Midia (3 dias)

Limite, renovação com uma única regra constante:
  0 renovação(ões) feita(s): simples True, com padrões True
  1 renovação(ões) feita(s): simples False, com padrões False

Tamanho de cada versão (tipos e linhas com código):
  Strategy, antes     1 tipo(s),   9 linhas
  Strategy, depois    6 tipo(s),  23 linhas
  Observer, antes     1 tipo(s),   8 linhas
  Observer, depois    5 tipo(s),  22 linhas
  Factory, antes      1 tipo(s),  12 linhas
  Factory, depois     5 tipo(s),  26 linhas
  Renovação, simples  1 tipo(s),   3 linhas
  Renovação, padrões  6 tipo(s),  20 linhas
```

A versão simples da renovação tem 1 tipo e 3 linhas. A versão com padrões tem 6 tipos e 20 linhas e decide exatamente igual, porque a regra tem uma variante só e ninguém pediu o aviso do observador.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

O SDK do .NET não está instalado, ou não está no PATH do terminal. Veja a correção no `README.md` do [tópico 06](../topico-06-classes-e-objetos/README.md).

### Erro 2: `ContagemDeClassesEDeLinhasDasDuasVersoes` falha com números diferentes

Alguém editou um arquivo de `Excesso/`. O teste fixa 1 tipo e 3 linhas na versão simples e 6 tipos e 20 linhas na versão com padrões. Atualize os números do teste se a mudança foi intencional.

### Erro 3: "ModelagemOO.sln não encontrada acima do executável"

Os testes de medida procuram a solução subindo a partir da pasta do executável. Rode `dotnet test` de dentro do repositório clonado, sem copiar a pasta `bin` para outro lugar.

## Limites conhecidos

- O Strategy não elimina a escolha do perfil. O `switch` sai do cálculo e fica em `PoliticasPorPerfil.Para`, num ponto só. O desenho depois tem 6 tipos e 23 linhas contra 1 tipo e 9 linhas do antes, e esse custo só compensa se os perfis variarem ou crescerem.
- O Observer avisa na ordem de inscrição e não isola falhas. O teste `ObservadorQueFalhaInterrompeOsSeguintes` mostra que um observador que lança exceção impede os seguintes de receber o aviso, e que a devolução já tinha ocorrido.
- O Factory Method troca o `switch` por um dicionário de fábricas. A fábrica nova entra sem editar o cadastro, mas o registro dos tipos padrão continua escrito em `ComFabricasPadrao`.
- A contagem de linhas ignora comentários e chaves isoladas e depende do estilo de formatação do código. Os números comparam as duas versões deste repositório e não valem como medida de outro projeto.
- Os dois desenhos são pequenos e escritos para o exemplo. Eles mostram o mecanismo, e o custo ou o ganho de um padrão em um sistema real depende de quantas variantes e de quantos interessados esse sistema tem.

## Experimente mudar

Faça cada mudança separadamente, rode o teste indicado e desfaça a mudança.

### Mudança 1: a multa do estudante sobe

Em `Depois/PoliticaDeMulta.cs`, troque `0.50m` por `0.60m` em `MultaDoEstudante`.

Pergunta: quantos testes de `StrategyMultaTests` falham, e por que o teste do desenho antes continua verde?

### Mudança 2: o sujeito esquece de avisar

Em `Depois/Devolucao.cs`, troque `observador.Atualizar(fato);` por `_ = observador;`.

Pergunta: que testes de `ObserverDevolucaoTests` falham, e o que eles dizem sobre quem depende do aviso?

### Mudança 3: um tipo novo de item

Em `tests/ModelagemOO.Tests/FactoryItensTests.cs`, copie o teste `TipoNovoEntraPorFabricaRegistrada` e registre uma fábrica de `DVD` com prazo de 2 dias.

Pergunta: que arquivo de `Depois/` você precisou editar, e que arquivo de `Antes/` você precisaria editar para o mesmo tipo?
