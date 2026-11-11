# Tópico 08: Diagramas de estados, de componentes e de implantação

Este exemplo parte do `Emprestimo` do tópico 06 e do `ParametrosDaBiblioteca` do tópico 07. A classe nova `CicloDoEmprestimo` acompanha um empréstimo por três estados (`Ativo`, `Atrasado` e `Devolvido`) e três eventos (`VerificarPrazo`, `Renovar` e `Devolver`). O comportamento dos tópicos 06 e 07 não muda: o `CicloDoEmprestimo` usa o `Emprestimo` e o `ParametrosDaBiblioteca` como estão.

Os três diagramas ficam na pasta do tópico e são texto PlantUML.

| Arquivo | Diagrama | O que o teste confere |
|---|---|---|
| [`estados.puml`](./estados.puml) | Máquina de estados do empréstimo | As 4 setas aceitas, as 5 transições proibidas, as 2 guardas e o histórico de um ciclo completo |
| [`componentes.puml`](./componentes.puml) | Os 6 projetos do repositório e suas dependências | Cada seta contra os `ProjectReference` dos arquivos `.csproj` |
| [`implantacao.puml`](./implantacao.puml) | Onde cada artefato roda hoje | Cada artefato contra os projetos e arquivos do repositório, e o executor contra o `ci.yml` |

O diagrama de estados mostra 9 combinações de estado e evento: 4 viram setas e 5 viram transição proibida. Uma transição proibida não tem seta em UML. Por isso `estados.puml` a registra numa linha de comentário `' proibida: Estado : Evento`, que o teste lê e compara com as recusas do código.

O diagrama de implantação não tem servidor de aplicação nem banco de dados, porque o repositório não tem nenhum. A biblioteca é um programa de console e uma suíte de testes que rodam na máquina de quem estuda e no executor `ubuntu-latest` da integração contínua. O teste `ImplantacaoNaoDeclaraServidorNemBancoDeDados` reprova o diagrama se alguém desenhar um nó que não existe.

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

### Passo 2: Rodar os testes de estados

```bash
dotnet test --filter EstadosTests
```

Resultado esperado: a última linha da saída começa como a linha abaixo. A duração varia de uma execução para outra.

```text
Passed!  - Failed:     0, Passed:    20, Skipped:     0, Total:    20, Duration: 33 ms - ModelagemOO.Tests.dll (net10.0)
```

São vinte testes. Cinco cobrem as transições válidas e quatro cobrem as guardas. Seis cobrem as transições proibidas: cinco casos de uma mesma teoria e a devolução repetida. Cinco conferem o código contra `estados.puml`.

### Passo 3: Rodar os testes de componentes e de implantação

```bash
dotnet test --filter ComponentesEImplantacaoTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7, Duration: 202 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 4: Rodar o exemplo

```bash
dotnet run --project topico-08-estados-componentes-implantacao
```

Resultado esperado: o programa imprime a vida de um empréstimo, do estado inicial à devolução, as duas recusas e o histórico de transições. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-08-estados-componentes-implantacao`.

```text
Estado inicial: Ativo, devolução prevista em 20/10/2026.
Renovação em 18/10/2026: Ativo, nova devolução prevista em 02/11/2026.
Segunda renovação: O empréstimo já foi renovado 1 vez.
Verificação em 03/11/2026: Atrasado.
Devolução em 05/11/2026: Devolvido, 3 dias de atraso, multa de R$ 4,50.
Segunda devolução: Transição proibida: Devolvido não aceita Devolver.

Transições executadas, na ordem:
  Ativo --> Ativo : Renovar
  Ativo --> Atrasado : VerificarPrazo
  Atrasado --> Devolvido : Devolver
```

A segunda renovação é recusada por uma guarda (contador de renovações), e o estado continua `Ativo`. A segunda devolução é recusada por transição proibida, e a recusa não entra no histórico.

## Como ver os diagramas

1. No VS Code, instale a extensão "PlantUML" (de jebbs), abra um dos três arquivos e use o comando "PlantUML: Preview Current Diagram".

   Resultado esperado: um painel ao lado mostra o desenho. Em `estados.puml`, três estados, um estado inicial, um final e quatro setas rotuladas. Em `componentes.puml`, seis componentes e catorze setas. Em `implantacao.puml`, dois nós de máquina, cada um com um nó de ambiente .NET e seus artefatos.

2. Sem instalar nada, cole o conteúdo do arquivo no editor on-line do PlantUML, em `https://www.plantuml.com/plantuml`.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

O SDK do .NET não está instalado, ou não está no PATH do terminal. Veja a correção no `README.md` do [tópico 06](../topico-06-classes-e-objetos/README.md).

### Erro 2: o diagrama diverge do código

Se você mudar uma seta de `estados.puml`, os testes que comparam o código com o diagrama falham. Por exemplo, trocar `Atrasado --> Devolvido : Devolver` por `Atrasado --> Ativo : Devolver` reprova `OsEventosAceitosPeloCodigoSaoAsSetasDeEstadosPuml` e `OHistoricoDeUmCicloCompletoEhUmCaminhoDeEstadosPuml`. Decida qual dos dois está certo e ajuste o outro.

### Erro 3: "The provided file path does not exist"

Rode o comando de dentro da pasta raiz do repositório, a pasta chamada `uninove-2026-2-ead-modelagem-oo`.

## Limites conhecidos

- O estado `Atrasado` só existe depois que alguém chama `VerificarPrazo`. Um empréstimo vencido que ninguém verificou continua `Ativo`, e o teste `EstadoAtrasadoSoExisteDepoisDeVerificarPrazo` mostra que `Devolver` calcula o atraso pelas datas, sem consultar o estado. O estado guardado e o estado calculado a partir da data podem divergir.
- O limite de renovações (`Renovacoes`) vive ao lado do `enum`, e não num estado próprio. Com limite de uma renovação, a alternativa seria desdobrar `Ativo` em dois estados. Com limite de cinco, seriam seis estados `Ativo`. Este exemplo mantém um contador e uma guarda.
- O `Usuario` do tópico 04 não tem operação de retirar um título da lista `Itens`. Isso vem do tópico 07 e continua igual.

## Experimente mudar

Faça cada mudança separadamente, rode `dotnet test --filter EstadosTests` e desfaça a mudança.

### Mudança 1: permitir renovar um empréstimo atrasado

Em `CicloDoEmprestimo.cs`, no método `Renovar`, troque a condição `Estado != EstadoDoEmprestimo.Ativo` por `Estado == EstadoDoEmprestimo.Devolvido`.

Pergunta: quais testes falham, e o que a mensagem de erro mostra sobre as linhas `proibida` de `estados.puml`?

### Mudança 2: acrescentar um estado

Acrescente `Perdido` ao `enum` em `EstadoDoEmprestimo.cs`.

Pergunta: qual teste falha, e quantas células de estado por evento o diagrama passa a ter de cobrir?

### Mudança 3: acrescentar uma dependência

Em `Topico08.csproj`, remova a linha de referência ao `Topico04.csproj` e rode `dotnet build`.

Pergunta: o projeto compila? Que teste de `ComponentesEImplantacaoTests` passa a falhar quando a referência some?
