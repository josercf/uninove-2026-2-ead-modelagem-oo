# Tópico 04: Requisitos funcionais e não funcionais

Este exemplo transforma regras da Biblioteca Comunitária em requisitos com ID e liga cada requisito ao teste que o verifica.

A lista de requisitos fica em [`REQUISITOS.md`](./REQUISITOS.md): três requisitos funcionais, RF01 a RF03, e dois não funcionais, RNF01 e RNF02. Cada linha traz a descrição, o critério de aceite e o teste correspondente.

A classe `Usuario` implementa os requisitos funcionais. O limite de itens retirados ao mesmo tempo depende do perfil (Morador 2, Estudante 4, Voluntário 6), e uma multa em aberto bloqueia a retirada, conforme as regras do domínio no [`README.md`](../README.md) da raiz.

Os testes ficam em `tests/ModelagemOO.Tests/RequisitosUsuarioTests.cs`. O nome de cada teste começa pelo ID do requisito que ele verifica, como `RF01_LimiteDeItensDependeDoPerfil`.

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

   Resultado esperado: a lista de arquivos mostra a pasta `topico-04-requisitos`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes automatizados deste tópico

```bash
dotnet test --filter RequisitosUsuarioTests
```

Resultado esperado: a última linha começa com `Passed!` e mostra `Failed: 0, Passed: 7, Skipped: 0, Total: 7`.

O arquivo tem cinco métodos de teste e sete casos. O método `RF01_LimiteDeItensDependeDoPerfil` roda três vezes, uma para cada perfil. Os outros quatro métodos conferem RF02, RF03, RNF01 e a recusa de nome vazio.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-04-requisitos
```

Resultado esperado: o programa imprime três linhas. A primeira mostra a moradora Joana com 2 de 2 itens, a segunda mostra a recusa do RF01 e a terceira mostra a recusa do RF02. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-04-requisitos`. Compare linha por linha com o que aparece na sua tela.

```
Joana (Morador) está com 2 de 2 itens.
RF01: Usuário atingiu o Limite de 2 itens do perfil Morador.
RF02: Usuário com multa em aberto não retira item novo.
```

A terceira retirada de Joana é recusada porque o perfil Morador tem limite 2. O estudante Caio tem limite 4 e nenhum item, e mesmo assim a retirada é recusada, porque ele tem multa em aberto.

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

### Erro 2: um teste de requisito falha

Se o limite de um perfil em `Usuario.cs` ficar diferente da regra do domínio, o teste do RF01 falha. Com o limite do Morador trocado de 2 para 3, a saída do `dotnet test --filter RequisitosUsuarioTests` mostra:

```
  Failed RequisitosUsuarioTests.RF01_LimiteDeItensDependeDoPerfil(perfil: Morador, limite: 2) [1 ms]
  Error Message:
   Assert.Throws() Failure: No exception was thrown
Expected: typeof(System.InvalidOperationException)
```

O nome do teste começa por `RF01`, e a linha RF01 do [`REQUISITOS.md`](./REQUISITOS.md) diz qual regra foi quebrada. Os valores entre parênteses mostram o caso que falhou: perfil Morador, limite esperado 2.

Correção: volte o valor em `Usuario.cs` para o da tabela de limites do [`README.md`](../README.md) da raiz e rode os testes de novo.

### Erro 3: "The provided file path does not exist"

Se você rodar o comando de fora da pasta raiz do repositório, a mensagem é:

```
The provided file path does not exist: topico-04-requisitos.
```

Isso significa que o comando foi rodado de dentro da pasta errada, e o
`--project` não encontrou a pasta `topico-04-requisitos` a partir de onde você
está.

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-04-requisitos
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet test --filter RequisitosUsuarioTests` e `dotnet run --project topico-04-requisitos`, compare com a sua resposta e desfaça a mudança.

### Mudança 1: quitar a multa de Caio

No arquivo `Program.cs`, logo depois de `caio.RegistrarMulta();`, adicione:

```csharp
caio.QuitarMulta();
```

Pergunta: a terceira linha da saída continua começando por `RF02:`? Qual requisito de `REQUISITOS.md` explica o que mudou?

### Mudança 2: trocar o idioma de uma mensagem

No arquivo `Usuario.cs`, troque o texto da mensagem de multa por `"User has an unpaid fine."`.

Pergunta: quais testes falham? Pelo nome de cada teste que falhou, quais requisitos deixaram de ser cumpridos?

### Mudança 3: mudar o perfil de Joana

No arquivo `Program.cs`, troque `Perfil.Morador` por `Perfil.Estudante` na linha que cria Joana.

Pergunta: a segunda linha da saída ainda mostra uma recusa? Quantos itens Joana pode retirar antes da primeira recusa?
