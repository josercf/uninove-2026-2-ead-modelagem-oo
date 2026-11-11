# Tópico 07: Diagrama de sequência e diagrama de atividades

Este exemplo parte do `Balcao.Emprestar` do tópico 05 e do `Emprestimo` do tópico 06 e registra, a cada execução, as mensagens trocadas entre os objetos e as ações da devolução. Os testes comparam esse registro com os dois diagramas do tópico.

O diagrama de sequência fica em [`sequencia.puml`](./sequencia.puml). Ele mostra o empréstimo de uma sacola de itens: o atendente chama o balcão, que consulta o item, pede a retirada ao usuário e só então empresta o item. Dois fragmentos `alt` desenham as duas recusas, item já emprestado e retirada recusada por multa ou limite do perfil. Um fragmento `loop` desenha a passagem por cada item da sacola.

O diagrama de atividades fica em [`atividades.puml`](./atividades.puml). Ele mostra a devolução: receber o item, um `fork` com duas ações independentes (marcar o item como disponível, e decidir se há multa), o `join` e o comprovante.

A classe `BalcaoComRastro` repete as etapas do construtor de `Emprestimo` do tópico 06 com cada mensagem à vista, porque o construtor esconde as chamadas ao usuário e ao item dentro de uma única linha. O teste `EmprestarSacolaDaOMesmoResultadoQueOConstrutorDeEmprestimo` confere que a data de devolução e o texto das recusas são os mesmos dos do construtor.

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

### Passo 2: Rodar os testes do tópico 07

```bash
dotnet test --filter SequenciaEAtividadesTests
```

Resultado esperado: a última linha da saída começa como a linha abaixo. A duração varia de uma execução para outra.

```text
Passed!  - Failed:     0, Passed:    13, Skipped:     0, Total:    13, Duration: 120 ms - ModelagemOO.Tests.dll (net10.0)
```

São treze testes. Oito conferem as mensagens contra `sequencia.puml`: o rastro de um item aceito, a sacola de três itens, o item já emprestado, a multa em aberto, a sacola vazia, o rastro fora de ordem, a cobertura de todas as mensagens e a equivalência com `Emprestimo`. Cinco conferem a devolução e `atividades.puml`.

### Passo 3: Rodar o exemplo

```bash
dotnet run --project topico-07-sequencia-e-atividades
```

Resultado esperado: o programa imprime as mensagens do empréstimo, os resultados, as ações da devolução e o comprovante. O texto completo está na próxima seção.

## Saída esperada

Esta é a saída exata do `dotnet run --project topico-07-sequencia-e-atividades`.

```text
Mensagens do empréstimo, na ordem:
  Atendente -> Balcao : EmprestarSacola(usuario, itens, retirada)
  Balcao -> ItemAcervo : Disponivel
  ItemAcervo --> Balcao : bool
  Balcao -> Usuario : Retirar(titulo)
  Usuario --> Balcao : ok
  Balcao -> ItemAcervo : Emprestar(retirada)
  ItemAcervo --> Balcao : devolucao
  Balcao -> ItemAcervo : Disponivel
  ItemAcervo --> Balcao : bool
  Balcao -> Usuario : Retirar(titulo)
  Usuario --> Balcao : ok
  Balcao -> ItemAcervo : Emprestar(retirada)
  ItemAcervo --> Balcao : devolucao
  Balcao -> ItemAcervo : Disponivel
  ItemAcervo --> Balcao : bool
  Balcao -> Usuario : Retirar(titulo)
  Usuario --> Balcao : InvalidOperationException
  Balcao --> Atendente : resultados

Resultados:
  aceito   Empréstimo de "Dom Casmurro" registrado para Joana. Devolução em 20/10/2026.
  aceito   Empréstimo de "Superinteressante" registrado para Joana. Devolução em 12/10/2026.
  recusado Usuário atingiu o Limite de 2 itens do perfil Morador.

Ações da devolução, na ordem:
  Receber o item no balcão
  Marcar o item como disponível
  Calcular a multa
  Registrar a multa no usuário
  Emitir o comprovante

Comprovante: Dom Casmurro, 3 dias de atraso, multa de R$ 4,50.
Joana com multa em aberto: True
```

As 18 mensagens são três passagens do `loop`. As duas primeiras seguem o ramo "retirada aceita". A terceira termina no ramo "multa em aberto ou limite do perfil atingido", porque Joana é moradora e o limite é 2. O texto de cada mensagem é o texto da linha correspondente em `sequencia.puml`.

## Como ver os diagramas

Os arquivos `sequencia.puml` e `atividades.puml` são texto.

1. No VS Code, instale a extensão "PlantUML" (de jebbs), abra um dos arquivos e use o comando "PlantUML: Preview Current Diagram".

   Resultado esperado: um painel ao lado mostra o desenho. Em `sequencia.puml`, quatro linhas de vida (Atendente, Balcao, Usuario e ItemAcervo) e três quadros aninhados, um `loop` e dois `alt`. Em `atividades.puml`, duas barras de sincronização e um losango de decisão entre elas.

2. Sem instalar nada, cole o conteúdo do arquivo no editor on-line do PlantUML, em `https://www.plantuml.com/plantuml`.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

O SDK do .NET não está instalado, ou não está no PATH do terminal. Veja a correção no `README.md` do [tópico 06](../topico-06-classes-e-objetos/README.md).

### Erro 2: o diagrama diverge do código

Se você mudar uma mensagem de `sequencia.puml`, os testes que comparam o rastro com o diagrama falham. Por exemplo, trocar `Retirar(titulo)` por `Retirar(item)` produz, entre outros:

```text
  Failed SequenciaEAtividadesTests.MultaEmAbertoRecusaNoUsuarioEDeixaOItemLivre [22 ms]
  Error Message:
   o rastro não é um caminho de sequencia.puml:
Atendente -> Balcao : EmprestarSacola(usuario, itens, retirada)
Balcao -> ItemAcervo : Disponivel
ItemAcervo --> Balcao : bool
Balcao -> Usuario : Retirar(titulo)
Usuario --> Balcao : InvalidOperationException
Balcao --> Atendente : resultados
```

O teste mostra o rastro inteiro que o código produziu, e a linha `Retirar(titulo)` já não existe no diagrama. Correção: decida qual dos dois está certo e ajuste o outro.

### Erro 3: "The provided file path does not exist"

Rode o comando de dentro da pasta raiz do repositório, a pasta chamada `uninove-2026-2-ead-modelagem-oo`.

## Limites conhecidos

- `Usuario` não tem operação para retirar um título da lista `Itens`. A devolução marca o item como disponível e registra a multa, e o título continua na lista do usuário. O limite por perfil só deixa de bloquear quando o `Usuario` ganhar essa operação, o que pertence a um tópico posterior.
- O `fork` do diagrama de atividades indica que as duas ações não dependem uma da outra. O código executa as duas em sequência numa única linha de execução, e o teste não exige uma ordem entre elas.

## Experimente mudar

Faça cada mudança separadamente, rode `dotnet test --filter SequenciaEAtividadesTests` e desfaça a mudança.

### Mudança 1: trocar a ordem de duas mensagens

Em `sequencia.puml`, troque de lugar as linhas `Balcao -> Usuario : Retirar(titulo)` e `Balcao -> ItemAcervo : Disponivel`.

Pergunta: quais testes falham, e o que a mensagem de erro mostra?

### Mudança 2: acrescentar uma ação ao diagrama de atividades

Em `atividades.puml`, acrescente `:Avisar o usuário;` antes de `:Emitir o comprovante;`.

Pergunta: qual teste falha, e que ajuste no `BalcaoComRastro.Devolver` faz o teste voltar a passar?

### Mudança 3: mudar o valor da multa

No `Program.cs`, troque `1.50m` por `2.00m` na linha que cria `ParametrosDaBiblioteca`.

Pergunta: qual é o valor do comprovante? Em quantos lugares do código o valor aparece escrito?
