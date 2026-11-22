# Tópico 12: Ferramentas CASE e prática de modelagem

Este exemplo parte de todos os diagramas da disciplina, os oito arquivos `.puml` dos tópicos 05 a 08, e trata o modelo como código: texto versionado, regenerado do código C# por reflexão e conferido por teste. O programa mede quatro coisas em torno do mesmo cenário da Biblioteca Comunitária.

| Recorte | O que existe no repositório | O que o teste confere |
|---|---|---|
| Modelo como código | Os 8 arquivos `.puml` dos tópicos 05 a 08, lidos por [`Medidas/InventarioDeDiagramas.cs`](./Medidas/InventarioDeDiagramas.cs) | 8 arquivos, 372 linhas, e o renome de `Usuario` em 19 linhas de 4 arquivos |
| Geração e engenharia reversa | [`Geracao/GeradorDeDiagramaDeClasses.cs`](./Geracao/GeradorDeDiagramaDeClasses.cs) escreve os blocos de classe por reflexão e grava [`classes-gerado.puml`](./classes-gerado.puml) | O gerado do código não diverge do `classes.puml` do tópico 06, e o arquivo gravado é o que o gerador escreve hoje |
| Prática integrada | Requisito RF-12 em [`Requisito/`](./Requisito), com os quatro diagramas alterados em [`Requisito/Depois/`](./Requisito/Depois) | O comportamento, o rastro contra a sequência, a guarda do estado, as classes contra o código e as linhas alteradas |
| Limite | [`Desvio/UsuarioQueDivergiu.cs`](./Desvio/UsuarioQueDivergiu.cs), um `Usuario` editado sem atualizar o diagrama | O comparador aponta cada membro que diverge, nos dois sentidos |

Os valores da Biblioteca Comunitária (prazos de 15, 7 e 3 dias, limites de 2, 4 e 6 itens por perfil) são os dos tópicos anteriores. O requisito RF-12, "um empréstimo com reserva para o item não pode ser renovado", é fictício e existe só para este exemplo.

## Como o gerador decide o que desenhar

A convenção é a de `classes.puml` do tópico 06 e está no código do gerador.

| Elemento do código C# | Como aparece no PlantUML |
|---|---|
| Campo | Linha de membro, com `-`, `+` ou `#` conforme o acesso |
| Propriedade cujo tipo é outra classe do modelo, ou coleção dela | Fora da caixa: é uma associação, desenhada por seta |
| Propriedade de tipo simples ou de enumeração | Linha de membro |
| Construtor público | Omitido |
| Construtor protegido ou privado | Linha de membro |
| Método | Linha de membro, com parâmetros e tipo de retorno |
| Membro abstrato | `{abstract}` antes do nome |
| Subclasse de outra classe do modelo | Linha `Base <|-- Filha` |

Associações com multiplicidade e papel, dependências, a linha oculta de posicionamento e as notas não saem do código. A seção "Saída esperada" mede quantas linhas de `classes.puml` ficam nessa situação.

## Critérios para comparar texto versionado e ferramenta gráfica

A tabela declara cada critério e diz o que foi medido neste repositório. A coluna da ferramenta gráfica descreve uma característica geral do formato de arquivo desse tipo de ferramenta e não foi medida aqui. Conferir o produto escolhido antes de decidir.

| Critério | PlantUML versionado, medido neste repositório | Ferramenta gráfica de edição visual, por característica geral |
|---|---|---|
| Revisão de uma mudança | O requisito RF-12 altera 4 diagramas e 50 linhas, lidas em um `git diff` comum | O diff depende do formato do arquivo da ferramenta |
| Geração a partir do código | 1 gerador por reflexão, com 0 divergências contra `classes.puml` | Depende de a ferramenta importar código C# |
| Renome de uma classe | `Usuario` aparece em 19 linhas de 4 arquivos e é editado à mão | Ferramentas que guardam um modelo único renomeiam em um ponto |
| Posição dos elementos | Calculada pelo PlantUML, com ajuste limitado a dicas como `-[hidden]-` | Posição livre, arrastando os elementos |
| Conferência por teste | 23 testes leem o texto e comparam com o código | Exige exportar o modelo para um formato que o teste leia |

## Pré-requisitos

1. .NET SDK 10.0 ou superior.

   Comando para confirmar:

   ```bash
   dotnet --version
   ```

   Resultado esperado: um número igual ou maior que `10.0.0`.

2. O repositório clonado e atualizado. Veja a seção "Como começar" no [`README.md`](../README.md) da raiz.

3. Opcional, para desenhar os `.puml`: Java 11 ou superior e o `plantuml.jar`. Os testes não precisam dele, porque leem o texto.

## Como executar

Rode cada comando a partir da pasta raiz do repositório.

### Passo 1: Restaurar as dependências

```bash
dotnet restore
```

Resultado esperado: mensagens de "Restaurado" (ou "Restored") e nenhuma linha de erro.

### Passo 2: Rodar os testes do modelo como código

```bash
dotnet test --filter ModeloComoCodigoTests
```

Resultado esperado: a última linha começa como a linha abaixo. A duração varia.

```text
Passed!  - Failed:     0, Passed:    12, Skipped:     0, Total:    12, Duration: 271 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 3: Rodar os testes do requisito novo

```bash
dotnet test --filter RequisitoNovoTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:    11, Skipped:     0, Total:    11, Duration: 411 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 4: Rodar o exemplo

```bash
dotnet run --project topico-12-pratica-de-modelagem
```

Resultado esperado: o texto da próxima seção.

### Passo 5: Regravar o diagrama gerado

```bash
dotnet run --project topico-12-pratica-de-modelagem -- --gravar
```

Resultado esperado: a linha `Gravado:` com o caminho de `classes-gerado.puml`. O arquivo não muda se o código não mudou, e o teste `ArquivoClassesGeradoVersionadoEhOQueOGeradorEscreveHoje` reprova se mudou e ninguém regravou.

## Saída esperada

```text
1. Modelo como código: os arquivos .puml dos tópicos 05 a 08
  arquivo               linhas   bytes
  casos-de-uso.puml         36    1189
  classes.puml              84    2466
  objetos.puml              46    1420
  atividades.puml           34    1050
  sequencia.puml            50    1796
  componentes.puml          37     987
  estados.puml              39    1320
  implantacao.puml          46    1813
  total: 372 linhas em 8 arquivos
  renomear Usuario em texto: 19 linhas em 4 arquivos

2. Diagrama de classes gerado por reflexão a partir do código C#
abstract class ItemAcervo {
  -_emprestado : bool
  +Titulo : string
  +Disponivel : bool
  +{abstract} PrazoEmDias : int
  #ItemAcervo(titulo : string)
  +Emprestar(retirada : DateOnly) : DateOnly
  +Devolver() : void
}

class Livro {
  +Autor : string
  +PrazoEmDias : int
}

class Revista {
  +Edicao : int
  +PrazoEmDias : int
}

class Midia {
  +Formato : string
  +PrazoEmDias : int
}

class Usuario {
  -_itens : List<string>
  +Nome : string
  +Perfil : Perfil
  +Itens : IReadOnlyList<string>
  +TemMultaEmAberto : bool
  +Limite : int
  +RegistrarMulta() : void
  +QuitarMulta() : void
  +Retirar(titulo : string) : void
}

enum Perfil <<enumeration>> {
  Morador
  Estudante
  Voluntario
}

class Balcao {
  +Emprestar(usuario : Usuario, titulo : string) : ResultadoAtendimento
}

class Emprestimo {
  +Retirada : DateOnly
  +Devolucao : DateOnly
}

ItemAcervo <|-- Livro
ItemAcervo <|-- Revista
ItemAcervo <|-- Midia

  comparação com classes.puml: 0 divergência(s)
  linhas de modelo em classes.puml: 55; recuperadas do código: 47
  não recuperadas (o código não as diz):
    Emprestimo "0..6" --> "+Usuario 1" Usuario
    Emprestimo "0..*" --> "+Item 1" ItemAcervo
    Usuario -[hidden]- Perfil
    Balcao ..> Usuario
    note bottom of Usuario
    {Emprestimo por Usuario <= Limite}
    Morador 2, Estudante 4, Voluntario 6
    end note

3. Requisito RF-12 (reserva bloqueia renovação): diagramas alterados
  diagrama              acrescentadas  removidas
  casos-de-uso.puml                 5          0
  classes.puml                     22          1
  sequencia.puml                   15          0
  estados.puml                      6          1
  4 de 8 diagramas alterados, 50 linhas no total

4. Código que diverge do diagrama
  Usuario: membro ausente no código: +Limite : int
  Usuario: membro ausente no código: +QuitarMulta() : void
  Usuario: membro ausente no diagrama: +Limite : long
  Usuario: membro ausente no diagrama: +Suspender() : void
```

## Teste que falha quando o código diverge do modelo

O teste `DiagramaGeradoDoCodigoNaoDivergeDoClassesPumlVersionado` compara o diagrama gerado com `classes.puml` nos dois sentidos. O teste `CadaMembroDoDiagramaExisteNoCodigoComAMesmaVisibilidade`, do tópico 06, compara só no sentido do diagrama para o código. As duas mutações abaixo foram feitas em `topico-04-requisitos/Usuario.cs` e desfeitas depois.

Mutação 1, método novo no código sem atualizar o diagrama (`public void Suspender() => RegistrarMulta();`):

```text
Failed ModeloComoCodigoTests.DiagramaGeradoDoCodigoNaoDivergeDoClassesPumlVersionado [49 ms]
 Assert.Empty() Failure: Collection was not empty
Collection: [Usuario: membro ausente no diagrama: +Suspender() : void]
```

O teste do tópico 06 continuou aprovado nessa mutação, porque o diagrama não perdeu nenhum membro que o código tem.

Mutação 2, `Limite` de `int` para `long`:

```text
Failed ModeloComoCodigoTests.DiagramaGeradoDoCodigoNaoDivergeDoClassesPumlVersionado [275 ms]
Collection: [Usuario: membro ausente no código: +Limite : int, Usuario: membro ausente no diagrama: +Limite : long]
Failed DiagramaDeClassesTests.CadaMembroDoDiagramaExisteNoCodigoComAMesmaVisibilidade [84 ms]
```

As mutações do requisito novo também foram conferidas: desligar o bloqueio em `RenovacaoComReserva` reprova 2 testes (a recusa e o rastro do ramo com reserva), trocar o rótulo `TemReservaPara(item)` reprova 2 (os dois rastros) e tirar `e sem reserva` da guarda de `estados.puml` reprova 2 (a guarda e a contagem de linhas).

## Solução de problemas

### Erro 1: `DiagramaGeradoDoCodigoNaoDivergeDoClassesPumlVersionado` lista divergências

Alguém mudou uma classe dos tópicos 03 a 06 sem atualizar `classes.puml`, ou o contrário. Cada linha da lista diz a classe, o sentido (`ausente no código` ou `ausente no diagrama`) e o membro. Atualize o lado que ficou para trás.

### Erro 2: `ArquivoClassesGeradoVersionadoEhOQueOGeradorEscreveHoje` falha

O código mudou e `classes-gerado.puml` não foi regravado. Rode o Passo 5 e faça o commit do arquivo.

### Erro 3: os números de linhas alteradas ou de ocorrências de `Usuario` falham

Alguém editou um `.puml` dos tópicos 05 a 08 ou um de `Requisito/Depois/`. Os testes fixam os números medidos. Atualize os números do teste se a mudança foi intencional.

## Limites conhecidos

- O comparador confere membros, cabeçalho e heranças. A ordem dos membros dentro da caixa e as associações com multiplicidade não entram.
- Dos quatro diagramas do requisito, três são conferidos contra o código: classes (membros), sequência (ordem das mensagens do rastro) e estados (a guarda e o comportamento). O de casos de uso só tem a estrutura conferida, porque o repositório não liga um caso de uso a uma classe.
- A engenharia reversa recupera 47 das 55 linhas de modelo de `classes.puml`. As 8 restantes dependem de decisão de quem modela.
- Exceções e interfaces não entram no gerador, porque o modelo do tópico 06 não as desenha.
- O inventário conta o texto dos tópicos 05 a 08. O `modelo-de-dominio.puml` do tópico 09 fica fora, porque o recorte da aula são os diagramas de UML dos tópicos 05 a 08.
- A coluna da ferramenta gráfica na tabela de critérios é descrição geral, e não medida.

## Experimente mudar

Faça cada mudança separadamente, rode o teste indicado e desfaça a mudança.

### Mudança 1: um membro novo em `Livro`

Em `topico-03-heranca-polimorfismo/Livro.cs`, acrescente `public string Isbn { get; init; } = "";`.

Pergunta: que teste reprova, e o que a mensagem diz sobre o lado que ficou desatualizado?

### Mudança 2: renomear uma classe só no código

Renomeie `Reserva` para `ReservaDeItem` em `Requisito/Reserva.cs` e no restante do código C#.

Pergunta: quantos arquivos `.puml` você precisa editar, e que teste mostra isso?

### Mudança 3: a reserva passa a bloquear também a devolução

Escreva primeiro o teste de que `Devolver` com reserva avisa a fila. Depois liste os diagramas que precisam mudar.

Pergunta: a contagem de diagramas e de linhas alteradas ficou maior ou menor que a do RF-12?
