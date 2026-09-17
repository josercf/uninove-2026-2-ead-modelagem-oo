# Tópico 02: Classes e Objetos

Este exemplo mostra como uma classe descreve um livro do acervo da Biblioteca Comunitária, e como um objeto criado a partir dela guarda e protege seu próprio estado.

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

   Resultado esperado: a lista de arquivos mostra a pasta `topico-02-classes-e-objetos`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados

```bash
dotnet test
```

Resultado esperado: a última linha mostra `Passed! - Failed: 0, Passed: 5, Skipped: 0, Total: 5`.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-02-classes-e-objetos
```

Resultado esperado: o programa imprime o título e o autor do livro, depois o empréstimo, depois a tentativa de empréstimo repetido. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-02-classes-e-objetos`. Compare linha por linha com o que aparece na sua tela.

```
Titulo:     Dom Casmurro
Autor:      Machado de Assis
Disponivel: True

Emprestando o livro...
Disponivel: False

Tentando emprestar de novo...
Erro: O livro 'Dom Casmurro' ja esta emprestado.
```

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
The provided file path does not exist: topico-02-classes-e-objetos.
```

Isso significa que o comando foi rodado de dentro da pasta errada, e o
`--project` não encontrou a pasta `topico-02-classes-e-objetos` a partir de
onde você está.

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-02-classes-e-objetos
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet run --project topico-02-classes-e-objetos` de novo e compare com a sua resposta.

### Mudança 1: devolver o livro antes de emprestar de novo

No arquivo `Program.cs`, depois da linha `livro.Emprestar();` (a primeira), adicione:

```csharp
livro.Devolver();
```

Pergunta: o que você espera que aconteça na segunda tentativa de empréstimo? Ela ainda vai lançar o erro, ou vai funcionar sem erro dessa vez?

### Mudança 2: tentar alterar o título depois de criar o livro

No arquivo `Program.cs`, depois da linha que cria o `livro`, adicione:

```csharp
livro.Titulo = "Outro Título";
```

Pergunta: você espera que o programa compile normalmente, ou que apareça um erro de compilação? Por quê a propriedade `Titulo` foi declarada só com `get`?

### Mudança 3: criar um segundo livro

No arquivo `Program.cs`, logo depois de criar `livro`, adicione uma segunda variável:

```csharp
var segundoLivro = new Livro("O Cortiço", "Aluísio Azevedo", 1890);
```

E, depois do trecho que empresta o primeiro `livro`, imprima o estado do segundo:

```csharp
Console.WriteLine($"Segundo livro disponivel: {segundoLivro.Disponivel}");
```

Pergunta: emprestar o primeiro livro muda a disponibilidade do segundo? Cada objeto criado a partir da classe `Livro` guarda o próprio estado, ou todos compartilham o mesmo estado?
