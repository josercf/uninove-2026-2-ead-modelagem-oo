# Tópico 06: Diagrama de classes e diagrama de objetos

Este exemplo desenha as classes da Biblioteca Comunitária em UML e acrescenta a classe `Emprestimo`, que liga um usuário a um item do acervo.

O diagrama de classes fica em [`classes.puml`](./classes.puml), escrito em PlantUML. Ele reúne `ItemAcervo` e as subclasses `Livro`, `Revista` e `Midia` do tópico 03, `Usuario` e a enumeração `Perfil` do tópico 04, e a classe nova `Emprestimo`, com visibilidade, herança, associações e multiplicidades.

O diagrama de objetos fica em [`objetos.puml`](./objetos.puml). Ele retrata os objetos que o `Program.cs` cria: Joana com um livro e uma revista emprestados no mesmo dia, com os mesmos valores que o programa imprime.

A classe `Emprestimo` exige exatamente um usuário e exatamente um item, e calcula a data de devolução pelo prazo do tipo real do item. Os testes ficam em `tests/ModelagemOO.Tests/DiagramaDeClassesTests.cs`. Três deles leem o arquivo `classes.puml` e conferem cada membro, cada herança e cada valor da enumeração contra o código C#.

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

   Resultado esperado: a lista de arquivos mostra a pasta `topico-06-classes-e-objetos`.

## Como executar

Siga os passos na ordem. Rode cada comando a partir da pasta raiz do repositório (a pasta chamada `uninove-2026-2-ead-modelagem-oo`).

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: o terminal mostra mensagens de "Restaurado" (ou "Restored") e não mostra nenhuma linha de erro.

### Passo 2: Rodar os testes do tópico 06

```bash
dotnet test --filter DiagramaDeClassesTests
```

Resultado esperado: a última linha da saída começa como a linha abaixo. A duração em milissegundos varia de uma execução para outra.

```text
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8, Duration: 44 ms - ModelagemOO.Tests.dll (net10.0)
```

São oito testes. Cinco conferem a classe `Emprestimo`: usuário obrigatório, item obrigatório, devolução pelo prazo da subclasse, limite do perfil e item já emprestado. Três conferem o diagrama `classes.puml` contra o código.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-06-classes-e-objetos
```

Resultado esperado: o programa imprime cinco objetos e uma recusa. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-06-classes-e-objetos`. Compare linha por linha com o que aparece na sua tela.

```text
joana : Usuario
    Nome = "Joana"
    Perfil = Morador
    Itens = [Dom Casmurro, Superinteressante]
    TemMultaEmAberto = False
domCasmurro : Livro
    Titulo = "Dom Casmurro"
    Autor = "Machado de Assis"
    Disponivel = False
superinteressante : Revista
    Titulo = "Superinteressante"
    Edicao = 480
    Disponivel = False
e1 : Emprestimo
    Usuario = joana
    Item = domCasmurro
    Retirada = 05/10/2026
    Devolucao = 20/10/2026
e2 : Emprestimo
    Usuario = joana
    Item = superinteressante
    Retirada = 05/10/2026
    Devolucao = 12/10/2026
e3 recusado: Usuário atingiu o Limite de 2 itens do perfil Morador.
```

Cada bloco começa com `nome : Classe`, a mesma notação do diagrama de objetos. O livro volta em 15 dias e a revista em 7, pelo prazo de cada subclasse. Joana é moradora, tem limite 2, e por isso o terceiro empréstimo é recusado.

## Como ver os diagramas

Os arquivos `classes.puml` e `objetos.puml` são texto. Para ver o desenho, escolha uma das opções.

1. No VS Code, instale a extensão "PlantUML" (de jebbs), abra um dos arquivos e use o comando "PlantUML: Preview Current Diagram".

   Resultado esperado: um painel ao lado mostra o desenho. Em `classes.puml`, sete caixas: `Emprestimo` no alto, `ItemAcervo` com as três subclasses embaixo, `Usuario` e `Perfil` à direita.

2. Sem instalar nada, copie o conteúdo do arquivo e cole no editor on-line do PlantUML, em `https://www.plantuml.com/plantuml`.

   Resultado esperado: o mesmo desenho aparece abaixo do texto.

A extensão do VS Code e o editor on-line usam Java ou um servidor remoto para desenhar. Se o painel do VS Code ficar em branco, use a opção 2.

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

### Erro 2: o diagrama diverge do código

Se você mudar `classes.puml` e o desenho deixar de bater com o C#, o teste que confere os membros falha. Por exemplo, trocar `+Devolver() : void` por `-Devolver() : void` em `ItemAcervo` produz:

```text
  Failed DiagramaDeClassesTests.CadaMembroDoDiagramaExisteNoCodigoComAMesmaVisibilidade [12 ms]
  Error Message:
   Assert.Empty() Failure: Collection was not empty
Collection: ["ItemAcervo.Devolver: diagrama '-', código '+'"]
```

Cada item da lista diz a classe, o membro, o que o diagrama desenha e o que o código declara. O mesmo teste acusa membro desenhado que não existe no código e tipo diferente do declarado.

Correção: decida qual dos dois está certo e ajuste o outro. Em `ItemAcervo.cs`, `Devolver` é `public`, e por isso o diagrama usa `+`.

### Erro 3: "The provided file path does not exist"

Se você rodar o comando de fora da pasta raiz do repositório, a mensagem é:

```text
The provided file path does not exist: topico-06-classes-e-objetos.
```

Correção: volte para a pasta raiz do repositório antes de rodar o comando.

```bash
cd caminho/para/uninove-2026-2-ead-modelagem-oo
dotnet run --project topico-06-classes-e-objetos
```

Troque `caminho/para/` pelo caminho real onde você clonou o repositório na sua máquina.

## Experimente mudar

Faça cada mudança separadamente. Depois de cada uma, rode `dotnet test --filter DiagramaDeClassesTests` e `dotnet run --project topico-06-classes-e-objetos`, compare com a sua resposta e desfaça a mudança.

### Mudança 1: trocar o perfil de Joana

No arquivo `Program.cs`, troque `Perfil.Morador` por `Perfil.Estudante` na linha que cria Joana.

Pergunta: a última linha da saída continua sendo uma recusa? Quantos empréstimos Joana poderia ter com esse perfil?

### Mudança 2: desenhar um atributo que não existe

No arquivo `classes.puml`, acrescente a linha `+Editora : string` dentro da caixa `Livro`.

Pergunta: qual teste falha, e qual mensagem aparece na lista de problemas?

### Mudança 3: um terceiro objeto no diagrama de objetos

No arquivo `objetos.puml`, acrescente um objeto `centralDoBrasil : Midia` e um empréstimo `e3` ligado a Joana.

Pergunta: esse retrato pode acontecer no código? Qual multiplicidade do diagrama de classes ele contraria?
