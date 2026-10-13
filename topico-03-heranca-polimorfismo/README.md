# Tópico 03: Encapsulamento, herança e polimorfismo

Este exemplo organiza os três tipos de item do acervo da Biblioteca Comunitária, livro, revista e mídia, a partir de uma classe base comum, `ItemAcervo`.

A classe `ItemAcervo` guarda o que todo item tem: o título, a situação de empréstimo e as operações `Emprestar` e `Devolver`. O atributo `_emprestado` é privado, e só a própria classe base altera esse valor.

As classes `Livro`, `Revista` e `Midia` herdam de `ItemAcervo`. Cada uma acrescenta um dado próprio e informa o próprio prazo de empréstimo: 15 dias para livro, 7 para revista e 3 para mídia, conforme as regras do domínio no [`README.md`](../README.md) da raiz.

O programa guarda os três itens numa única lista do tipo `ItemAcervo` e chama `Emprestar` em cada um. A mesma chamada devolve três datas diferentes, uma para cada tipo real de objeto.

## Pré-requisitos

Antes de começar, confira se você tem o que precisa.

1. .NET SDK instalado.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`. Se aparecer "comando não encontrado", veja a seção "Se der errado" mais abaixo.

2. O repositório clonado na sua máquina. Se ainda não clonou, veja a seção
   "Como começar" no [`README.md`](../README.md) da raiz do repositório
   antes de continuar.

   Comando para confirmar (rode dentro da pasta do repositório):

   ```bash
   ls
   ```

   Resultado esperado: a lista de arquivos mostra a pasta `topico-03-heranca-polimorfismo`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados deste tópico

```bash
dotnet test --filter ItemAcervoTests
```

Resultado esperado: a última linha começa com `Passed!` e mostra `Failed: 0, Passed: 5, Skipped: 0, Total: 5`.

Os cinco testes ficam em `tests/ModelagemOO.Tests/ItemAcervoTests.cs`. Eles conferem o prazo de cada tipo, as três datas de devolução calculadas a partir da mesma lista, a recusa do segundo empréstimo, a recusa da devolução de um item que não saiu e a recusa de título vazio pela classe base.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-03-heranca-polimorfismo
```

Resultado esperado: o programa imprime três linhas, uma por item, com o tipo, o título, o prazo e a data de devolução. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-03-heranca-polimorfismo`. Compare linha por linha com o que aparece na sua tela.

```
Livro    Dom Casmurro         prazo 15 dias, devolver em 20/10/2026
Revista  Superinteressante    prazo  7 dias, devolver em 12/10/2026
Midia    Central do Brasil    prazo  3 dias, devolver em 08/10/2026
```

Os três itens saem no mesmo dia, 05/10/2026. A data de devolução muda porque cada objeto responde `PrazoEmDias` com o valor da própria classe.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

No macOS ou Linux (terminal zsh, o padrão do macOS), a mensagem é:

```
zsh: command not found: dotnet
```

No Windows (PowerShell ou Prompt de Comando), a mensagem é:

```
'dotnet' is not recognized as an internal or external command, operable program or batch file.
```

As duas mensagens dizem a mesma coisa: o SDK do .NET não está instalado, ou não está no PATH do terminal.

Correção no Windows:

```powershell
winget install Microsoft.DotNet.SDK.10
```

Correção no macOS:

```bash
brew install --cask dotnet-sdk
```

Depois de instalar, feche o terminal, abra um novo e rode `dotnet --version` de novo para confirmar.

### Erro 2: CS0534 ao criar um tipo novo de item

Se você criar uma classe nova que herda de `ItemAcervo` e esquecer de informar o prazo, a compilação falha com esta mensagem:

```
error CS0534: 'Jogo' does not implement inherited abstract member 'ItemAcervo.PrazoEmDias.get'
```

A propriedade `PrazoEmDias` é `abstract` na classe base. Toda classe que herda de `ItemAcervo` precisa declarar o próprio prazo com `override`.

Correção: acrescente à classe nova uma linha como a das classes `Livro`, `Revista` e `Midia`, com o prazo do tipo novo.

```csharp
public override int PrazoEmDias => 10;
```

O número `10` é só um exemplo. As regras do domínio no `README.md` da raiz definem prazo apenas para livro, revista e mídia.

### Erro 3: "The provided file path does not exist"

Se você rodar o comando de fora da pasta raiz do repositório, a mensagem é:

```
The provided file path does not exist: topico-03-heranca-polimorfismo.
```

Isso significa que o comando foi rodado de dentro da pasta errada, e o
`--project` não encontrou a pasta `topico-03-heranca-polimorfismo` a partir de
onde você está.

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-03-heranca-polimorfismo
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet run --project topico-03-heranca-polimorfismo` de novo e compare com a sua resposta.

### Mudança 1: emprestar o mesmo item duas vezes

No arquivo `Program.cs`, depois do laço `foreach`, adicione:

```csharp
acervo[0].Emprestar(retirada);
```

Pergunta: o programa imprime uma quarta linha, ou termina com uma exceção? Qual título aparece na mensagem?

### Mudança 2: alterar o atributo privado de fora da classe

No arquivo `Program.cs`, depois do laço `foreach`, adicione:

```csharp
acervo[0]._emprestado = false;
```

Pergunta: você espera que o programa compile? Compare a mensagem que aparece com o modificador usado na declaração de `_emprestado`, no arquivo `ItemAcervo.cs`.

### Mudança 3: trocar a ordem dos itens na lista

No arquivo `Program.cs`, troque de lugar as linhas `new Livro(...)` e `new Midia(...)` dentro da lista `acervo`.

Pergunta: a data de devolução do livro muda quando ele passa a ser o último da lista? O que define o prazo usado em cada linha, a posição na lista ou a classe do objeto?
