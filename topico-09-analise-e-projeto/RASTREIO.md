# Rastreio do UC01 ao modelo de domínio e ao código

Este arquivo liga cada termo do texto do caso de uso UC01, Realizar empréstimo
([`CASOS_DE_USO.md`](../topico-05-casos-de-uso/CASOS_DE_USO.md)), a uma classe
do modelo de domínio ([`modelo-de-dominio.puml`](./modelo-de-dominio.puml)), a
uma classe de projeto em C# e a um teste. O teste `RastreioTests` confere cada
linha: o termo existe no texto do caso de uso, a classe existe no código e o
teste existe na suíte.

## Conceitos adotados

| Termo no texto | Passo do UC01 | Classe de domínio | Classe de projeto | Teste |
|---|---|---|---|---|
| Usuário | 1 e 2 | Usuario | Topico04.Usuario | UC01_FluxoPrincipal_EmprestimoAceito |
| perfil | 3 | Perfil | Topico04.Perfil | RF01_LimiteDeItensDependeDoPerfil |
| item | 2 | ItemAcervo | Topico03.ItemAcervo | DevolucaoVemDoPrazoDaSubclasse |
| empréstimo | 4 | Emprestimo | Topico06.Emprestimo | UsuarioMoradorNaoPassaDeDoisEmprestimos |

## Candidatas descartadas

| Termo no texto | Candidata | Origem | Motivo |
|---|---|---|---|
| Atendente | Atendente | Ator do UC01 | Ator fora do sistema: quem usa o balcão não é objeto do domínio |
| multa em aberto | Multa | Passo 3 e fluxo A2 | Estado booleano do usuário (RF02), sem comportamento próprio |
| pendências | Pendencia | Passo 3, inclui UC05 | Consulta sobre o usuário, sem dados guardados |
| limite do perfil | Limite | Passo 3 e fluxo A1 | Valor derivado do perfil, atributo de Usuario |
| reserva | Reserva | UC03, fora do UC01 | Caso de uso ainda não implementado no código |

## Classes de projeto sem classe de domínio

| Passo do UC01 | Classe de projeto | Papel GRASP | Teste |
|---|---|---|---|
| 2 e 5 | Topico09.Depois.Balcao | Controlador | OsDoisDesenhosDecidemIgual |
| 4 | Topico09.Depois.RegistroDeEmprestimos | Criador | NoDesenhoDepoisSoORegistroCriaEmprestimo |
