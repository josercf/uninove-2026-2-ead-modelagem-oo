# Tópico 05: Diagrama de casos de uso

Este exemplo modela os casos de uso da Biblioteca Comunitária e implementa o caso de uso UC01, Realizar empréstimo, com o fluxo principal e três fluxos alternativos.

O diagrama fica em [`casos-de-uso.puml`](./casos-de-uso.puml), escrito em PlantUML: dois atores (Atendente e Usuário), quatro casos de uso ligados aos atores, um caso incluído (Verificar pendências) e um caso que estende outro (Cobrar multa por atraso).

A especificação textual do UC01 fica em [`CASOS_DE_USO.md`](./CASOS_DE_USO.md). Ela cita os requisitos RF01, RF02, RF03 e RNF01 do [`REQUISITOS.md`](../topico-04-requisitos/REQUISITOS.md) do tópico 04, pelos mesmos IDs.

A classe `Balcao` implementa o UC01 e reaproveita a classe `Usuario` do tópico 04, que já aplica as regras de limite e de multa. Os testes ficam em `tests/ModelagemOO.Tests/CasosDeUsoTests.cs`, um por fluxo do UC01.

## Pré-requisitos

Antes de começar, confira se você tem o que precisa.

1. .NET SDK instalado.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`. Se aparecer "comando não encontrado", veja a seção "Se der errado" mais abaixo.

2. O repositório clonado e atualizado na sua máquina. Se ainda não clonou, veja a seção "Como começar" no [`README.md`](../README.md) da raiz do repositório.

   Comando para confirmar (rode dentro da pasta do repositório):

   ```bash
   ls
   ```

   Resultado esperado: a lista de arquivos mostra a pasta `topico-05-casos-de-uso`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes do UC01

```bash
dotnet test --filter CasosDeUsoTests
```

Resultado esperado: a última linha da saída começa como a linha abaixo. A duração em milissegundos varia de uma execução para outra.

```text
Passed!  - Failed:     0, Passed:     4, Skipped:     0, Total:     4, Duration: 32 ms - ModelagemOO.Tests.dll (net10.0)
```

São quatro testes, um por fluxo do UC01: o fluxo principal e os fluxos alternativos A1, A2 e A3.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-05-casos-de-uso
```

Resultado esperado: o programa imprime dez linhas, duas por atendimento. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-05-casos-de-uso`. Compare linha por linha com o que aparece na sua tela.

```text
Fluxo principal: Joana (Morador) pede "Dom Casmurro", aceito.
    Empréstimo de "Dom Casmurro" registrado para Joana.
Fluxo principal: Joana (Morador) pede "Superinteressante", aceito.
    Empréstimo de "Superinteressante" registrado para Joana.
A1, RF01: Joana (Morador) pede "Central do Brasil", recusado.
    Usuário atingiu o Limite de 2 itens do perfil Morador.
A2, RF02: Caio (Estudante) pede "Vidas Secas", recusado.
    Usuário com multa em aberto não retira item novo.
A3, RF03: Caio (Estudante) pede "Vidas Secas", aceito.
    Empréstimo de "Vidas Secas" registrado para Caio.
```

Joana é moradora e tem limite 2: os dois primeiros pedidos seguem o fluxo principal, e o terceiro cai no fluxo A1. Caio é estudante, tem multa em aberto e cai no fluxo A2; depois de quitar a multa, o mesmo pedido segue o fluxo A3 e é aceito.

## Como ver o diagrama

O arquivo `casos-de-uso.puml` é texto. Para ver o desenho, escolha uma das opções.

1. No VS Code, instale a extensão "PlantUML" (de jebbs), abra `casos-de-uso.puml` e use o comando "PlantUML: Preview Current Diagram".

   Resultado esperado: um painel ao lado mostra os dois atores, a fronteira "Sistema da Biblioteca Comunitária" e os seis casos de uso.

2. Sem instalar nada, copie o conteúdo do arquivo e cole no editor on-line do PlantUML, em `https://www.plantuml.com/plantuml`.

   Resultado esperado: o mesmo desenho aparece abaixo do texto.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

No macOS ou Linux (terminal zsh, o padrão do macOS), a mensagem é:

```text
zsh: command not found: dotnet
```

No Windows (PowerShell ou Prompt de Comando), a mensagem é:

```text
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

### Erro 2: um teste de fluxo alternativo falha

Se o `Balcao` devolver `Aceito` igual a `true` também na recusa, os testes dos fluxos A1 e A2 falham. A saída do `dotnet test --filter CasosDeUsoTests` mostra:

```text
  Failed CasosDeUsoTests.UC01_A2_MultaEmAberto_RF02 [6 ms]
  Error Message:
   Assert.False() Failure
Expected: False
Actual:   True
```

O nome do teste diz o caso de uso (UC01), o fluxo (A2) e o requisito (RF02). A linha A2 da tabela de fluxos alternativos em [`CASOS_DE_USO.md`](./CASOS_DE_USO.md) descreve o comportamento esperado.

Correção: no bloco `catch` de `Balcao.cs`, o resultado da recusa precisa ser criado com `false`.

### Erro 3: "The provided file path does not exist"

Se você rodar o comando de fora da pasta raiz do repositório, a mensagem é:

```text
The provided file path does not exist: topico-05-casos-de-uso.
```

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-05-casos-de-uso
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet test --filter CasosDeUsoTests` e `dotnet run --project topico-05-casos-de-uso`, compare com a sua resposta e desfaça a mudança.

### Mudança 1: trocar o perfil de Joana

No arquivo `Program.cs`, troque `Perfil.Morador` por `Perfil.Estudante` na linha que cria Joana.

Pergunta: o pedido de "Central do Brasil" ainda cai no fluxo A1? Qual linha da saída mudou?

### Mudança 2: tirar a quitação da multa

No arquivo `Program.cs`, apague a linha `caio.QuitarMulta();`.

Pergunta: a última linha da saída passa a mostrar qual mensagem? Pela especificação, em qual fluxo o pedido terminou?

### Mudança 3: um fluxo A4 na especificação

No arquivo `CASOS_DE_USO.md`, acrescente um fluxo A4 para o item que já está emprestado a outra pessoa.

Pergunta: algum teste de `CasosDeUsoTests.cs` cobre o A4? O que o `Balcao` precisaria saber para tratar esse fluxo?
