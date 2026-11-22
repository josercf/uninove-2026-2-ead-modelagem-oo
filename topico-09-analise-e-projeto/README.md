# Tópico 09: Análise e projeto orientados a objetos

Este exemplo parte do caso de uso UC01 do tópico 05 e das classes dos tópicos 03, 04 e 06. Ele mostra o caminho do texto do caso de uso até o código em três passos: o modelo de domínio, o projeto das responsabilidades por GRASP (Especialista e Criador) e a medida do acoplamento antes e depois da redistribuição.

| Arquivo | O que contém | O que o teste confere |
|---|---|---|
| [`modelo-de-dominio.puml`](./modelo-de-dominio.puml) | 4 classes adotadas e 5 candidatas descartadas, em PlantUML | As classes do diagrama contra a tabela do rastreio |
| [`RASTREIO.md`](./RASTREIO.md) | Termo do UC01, classe de domínio, classe de projeto e teste | Cada termo no texto do UC01, cada classe no código e cada teste na suíte |
| [`Antes/`](./Antes) | Desenho em que o `Balcao` decide tudo sobre classes que só guardam dados | Seis dependências no balcão e catorze membros alheios usados |
| [`Depois/`](./Depois) | Desenho em que `Usuario` e `ItemAcervo` decidem, `RegistroDeEmprestimos` cria e o `Balcao` coordena | Quatro dependências no balcão e quatro membros alheios usados |
| [`MedidorDeAcoplamento.cs`](./MedidorDeAcoplamento.cs) | Contagem de dependências por reflexão, incluindo o corpo dos métodos | Os testes do próprio medidor, com tipos sintéticos |

## Como o acoplamento é contado

Uma classe A depende de uma classe B quando A menciona B em um campo, em uma propriedade, no parâmetro ou no retorno de um método, em uma variável local ou no corpo de um método (chamada, acesso a campo ou `new`). Só entram na conta as classes do universo medido, isto é, as que participam do UC01 em cada desenho. Tipos de `System` e a própria classe ficam de fora.

A medida de membros alheios conta quantos membros distintos de outras classes do universo o corpo do `Balcao` usa. Ela mostra o efeito do Especialista melhor que a contagem de classes.

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

### Passo 2: Rodar os testes de acoplamento

```bash
dotnet test --filter AcoplamentoTests
```

Resultado esperado: a última linha começa como a linha abaixo. A duração varia.

```text
Passed!  - Failed:     0, Passed:    33, Skipped:     0, Total:    33, Duration: 26 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 3: Rodar os testes de rastreio

```bash
dotnet test --filter RastreioTests
```

Resultado esperado:

```text
Passed!  - Failed:     0, Passed:    15, Skipped:     0, Total:    15, Duration: 109 ms - ModelagemOO.Tests.dll (net10.0)
```

### Passo 4: Rodar o exemplo

```bash
dotnet run --project topico-09-analise-e-projeto
```

Resultado esperado: o texto da próxima seção.

## Saída esperada

```text
Desenho antes:
  Empréstimo de "Dom Casmurro" registrado para Joana.
Desenho depois:
  Empréstimo de "Dom Casmurro" registrado para Joana.

Acoplamento, desenho antes:
  Balcao                 6 dependência(s): EmprestimoAnemico, ItemAnemico, Perfil, ResultadoAtendimento, TipoDeItem, UsuarioAnemico
  UsuarioAnemico         1 dependência(s): Perfil
  ItemAnemico            1 dependência(s): TipoDeItem
  EmprestimoAnemico      2 dependência(s): ItemAnemico, UsuarioAnemico
  TipoDeItem             0 dependência(s): 
  Perfil                 0 dependência(s): 
  ResultadoAtendimento   0 dependência(s): 
  Total: 10

Acoplamento, desenho depois:
  Balcao                 4 dependência(s): ItemAcervo, RegistroDeEmprestimos, ResultadoAtendimento, Usuario
  RegistroDeEmprestimos  3 dependência(s): Emprestimo, ItemAcervo, Usuario
  Usuario                1 dependência(s): Perfil
  ItemAcervo             0 dependência(s): 
  Livro                  1 dependência(s): ItemAcervo
  Revista                1 dependência(s): ItemAcervo
  Midia                  1 dependência(s): ItemAcervo
  Emprestimo             2 dependência(s): ItemAcervo, Usuario
  Perfil                 0 dependência(s): 
  ResultadoAtendimento   0 dependência(s): 
  Total: 13

Membros de outras classes que o balcão usa:
  antes : 14
  depois: 4
```

O acoplamento do `Balcao` cai de 6 para 4 dependências, e os membros alheios que ele usa caem de 14 para 4. O total do desenho sobe de 10 para 13, porque o desenho novo tem mais classes (`RegistroDeEmprestimos`, `Livro`, `Revista`, `Midia`) e cada uma depende de outra.

## Como ver o diagrama

1. No VS Code, instale a extensão "PlantUML" (de jebbs), abra `modelo-de-dominio.puml` e use o comando "PlantUML: Preview Current Diagram".

   Resultado esperado: quatro caixas (`Usuario`, `Perfil`, `ItemAcervo`, `Emprestimo`) com três associações e sem métodos.

2. Sem instalar nada, cole o conteúdo do arquivo no editor on-line do PlantUML, em `https://www.plantuml.com/plantuml`.

## Se der errado

### Erro 1: o terminal não reconhece o comando `dotnet`

O SDK do .NET não está instalado, ou não está no PATH do terminal. Veja a correção no `README.md` do [tópico 06](../topico-06-classes-e-objetos/README.md).

### Erro 2: um teste de rastreio falha com "não existe nos assemblies"

O nome de uma classe de projeto em `RASTREIO.md` não bate com o código. Confira o namespace e a grafia em `Topico04.Usuario`, `Topico03.ItemAcervo` e nas demais linhas.

### Erro 3: "The provided file path does not exist"

Rode o comando de dentro da pasta raiz do repositório, a pasta chamada `uninove-2026-2-ead-modelagem-oo`.

## Limites conhecidos

- O universo medido é uma escolha. O total sobe de 10 para 13 porque o desenho novo inclui `Livro`, `Revista` e `Midia`, que substituem o `switch` de prazos do balcão antigo. Trocar o universo muda o total, e a comparação vale só entre universos declarados.
- A contagem mede menções, não força nem frequência. Uma chamada e vinte chamadas à mesma classe contam uma dependência. Por isso o exemplo também conta membros alheios.
- Quatro das nove candidatas a classe saíram do modelo de domínio. Criar `Multa` ou `Reserva` agora acrescentaria classes sem comportamento e sem teste de caso de uso: o teste `CandidataDescartadaNaoViraClasseNoCodigo` reprova essa antecipação. Se o UC03 for implementado, `Reserva` volta ao modelo.
- Os dois desenhos são pequenos. Os números mostram o mecanismo da medida e não valem como padrão de qualidade para outro sistema.

## Experimente mudar

Faça cada mudança separadamente, rode `dotnet test --filter AcoplamentoTests` e desfaça a mudança.

### Mudança 1: o balcão volta a ler o limite

Em `Depois/Balcao.cs`, acrescente antes de `registro.Registrar` a linha `_ = usuario.Limite;`.

Pergunta: que teste falha, e o que ele diz sobre a responsabilidade de conhecer o limite do perfil?

### Mudança 2: o balcão volta a criar o empréstimo

Em `Depois/Balcao.cs`, troque a chamada a `registro.Registrar` por `new Topico06.Emprestimo(usuario, item, retirada);`.

Pergunta: quais dependências do balcão mudam, e que teste de Criador reprova?

### Mudança 3: acrescentar a classe `Multa`

Crie uma classe `Multa` vazia em `Depois/`.

Pergunta: que teste de `RastreioTests` falha, e por que o rastreio a trata como antecipação?
