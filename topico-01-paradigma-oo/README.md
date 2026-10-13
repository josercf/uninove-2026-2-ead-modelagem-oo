# Tópico 01: Paradigma orientado a objetos

Este exemplo escreve a mesma regra da Biblioteca Comunitária em dois estilos. A regra calcula a data de devolução de um empréstimo a partir da data de retirada e do prazo em dias.

O arquivo `EmprestimoProcedural.cs` usa o estilo procedural: uma função recebe os dados soltos e devolve a data. O arquivo `Emprestimo.cs` usa o estilo orientado a objetos: os dados e a regra ficam dentro do mesmo objeto, e o construtor recusa um prazo que não seja positivo.

O programa roda as duas versões com os mesmos dados e mostra a diferença quando o prazo é zero.

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

   Resultado esperado: a lista de arquivos mostra a pasta `topico-01-paradigma-oo`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados deste tópico

```bash
dotnet test --filter EmprestimoTests
```

Resultado esperado: a última linha da saída começa como a linha abaixo. A duração em milissegundos varia de uma execução para outra.

```text
Passed!  - Failed:     0, Passed:     5, Skipped:     0, Total:     5, Duration: 40 ms - ModelagemOO.Tests.dll (net10.0)
```

Os cinco testes ficam em `tests/ModelagemOO.Tests/EmprestimoTests.cs`. Eles conferem que as duas versões calculam a mesma data, que a versão procedural aceita prazo zero e que o objeto recusa prazo zero, prazo negativo e título vazio.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-01-paradigma-oo
```

Resultado esperado: o programa imprime quatro linhas. A primeira é `Procedural: devolver em 06/10/2026` e a última é `Objeto com prazo 0: recusado (prazoEmDias)`. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-01-paradigma-oo`. Compare linha por linha com o que aparece na sua tela.

```
Procedural: devolver em 06/10/2026
Objeto: 'Dom Casmurro' volta em 06/10/2026
Procedural com prazo 0: 21/09/2026
Objeto com prazo 0: recusado (prazoEmDias)
```

As duas primeiras linhas mostram que os dois estilos chegam à mesma data para um livro retirado em 21/09/2026 com prazo de 15 dias.

As duas últimas linhas mostram a diferença. A função procedural devolve uma data igual à da retirada quando recebe prazo zero. O objeto não chega a ser criado, e o erro informa o nome do parâmetro recusado.

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

### Erro 2: "The provided file path does not exist"

Se você rodar o comando de fora da pasta raiz do repositório, a mensagem é:

```
The provided file path does not exist: topico-01-paradigma-oo.
```

Isso significa que o comando foi rodado de dentro da pasta errada, e o
`--project` não encontrou a pasta `topico-01-paradigma-oo` a partir de
onde você está.

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-01-paradigma-oo
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet run --project topico-01-paradigma-oo` de novo e compare com a sua resposta.

### Mudança 1: passar um prazo negativo para a função procedural

No arquivo `Program.cs`, troque o argumento `0` de `CalcularDevolucao(retirada, 0)` por `-3`.

Pergunta: a data impressa fica antes ou depois da data de retirada? Algum erro aparece na tela?

### Mudança 2: criar um empréstimo de revista

No arquivo `Program.cs`, depois da linha que cria o `emprestimo`, adicione:

```csharp
var revista = new Emprestimo("Revista do Bairro", retirada, 7);
Console.WriteLine($"Revista volta em {revista.Devolucao:dd'/'MM'/'yyyy}");
```

Pergunta: qual data você espera ver, sabendo que o prazo da revista na Biblioteca Comunitária é de 7 dias?

### Mudança 3: criar um empréstimo sem título

No arquivo `Program.cs`, dentro do bloco `try`, troque `"Dom Casmurro"` por `""`.

Pergunta: o bloco `catch` ainda captura o erro? Repare que o construtor confere o título antes de conferir o prazo, e que o erro lançado para título vazio é de outro tipo.
