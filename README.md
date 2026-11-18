# LiteArt POO

Projeto acadêmico da P2 de Programação Orientada a Objetos, do curso de Análise e Desenvolvimento de Sistemas da FATEC Ribeirão Preto, no 2º semestre de 2026.

**Professor:** Prof. Me. Douglas Samuel Gonçalves.

## Proposta

O LiteArt POO será um portal de estudos para vestibulares. A proposta é permitir que o aluno consulte questões por matéria, registre respostas e acompanhe suas tentativas de resolução.

Esta versão usa um recorte menor do [LiteArt](https://miro.com/app/board/uXjVH3-xMjs=/?share_link_id=589467137863), com foco na aplicação dos conceitos de orientação a objetos em C# e na organização de um portal ASP.NET Core MVC.

## Equipe

O grupo é composto por três integrantes:

| Integrante | Nome | RA |
| --- | --- | --- |
| 1 | Natan Fernado Cardeal da Costa | 2840482511044 |
| 2 | Gustavo Belotti Machado Luiz | 2840482511012 |
| 3 | Luiz Henrique | Pendente |

Os nomes completos de Gustavo Belotti e Luiz Henrique e seus respectivos RAs serão complementados posteriormente.

## Escopo inicial proposto

- Cadastrar e consultar alunos e matérias.
- Cadastrar, consultar, editar e excluir questões.
- Resolver questões e registrar tentativas por aluno.
- Consultar o histórico de tentativas e os resultados.

Nesta versão, ficam fora do escopo recursos de IA, planos pagos, simulados, metas de estudo e autenticação completa. A equipe ainda deverá validar o recorte e as regras antes de implementá-los.

### Regras de negócio propostas

- Toda questão pertence a uma matéria.
- Toda tentativa pertence a um aluno e a uma questão.
- Um aluno pode realizar várias tentativas da mesma questão; uma nova tentativa preserva as anteriores.
- A avaliação da resposta varia conforme o tipo de questão: múltipla escolha ou verdadeiro/falso.
- Uma questão com tentativas registradas não pode ser excluída sem preservar a integridade do histórico.

## Estado atual

Este repositório contém apenas a preparação inicial: documentação, estrutura MVC e cópia do frontend Flutter de referência. A aplicação C# possui somente uma página inicial de apresentação.

As entidades, os CRUDs, as regras de negócio, a persistência com Dapper, o script SQL e a adaptação visual para Razor serão desenvolvidos depois. O arquivo `Database/script.sql` está reservado e ainda não cria tabelas nem dados.

## Tecnologias

| Componente | Tecnologia | Situação |
| --- | --- | --- |
| Portal da P2 | C# e ASP.NET Core MVC, com alvo .NET 8 | Estrutura inicial |
| Interface do portal | Razor, HTML e CSS | Página inicial; telas futuras pendentes |
| Acesso a dados | Dapper e ADO.NET | Exigência do projeto; implementação e pacotes pendentes |
| Banco relacional | PostgreSQL | Proposta alinhada ao LiteArt; confirmar com a equipe |
| Frontend de referência | Flutter e Dart | Copiado sem alterações; não integrado ao MVC |

## Reaproveitamento do LiteArt

Estamos reutilizando o frontend desenvolvido para a disciplina de Programação de Dispositivos Móveis, que faz parte do LiteArt, projeto de TCC de Natan Fernado Cardeal da Costa e Gustavo Belotti. Para desenvolver esta versão da P2 de Programação Orientada a Objetos, convidamos Luiz Henrique para integrar a equipe e ajudar no projeto.

O diretório `frontend/liteart_app/` reproduz os 130 arquivos versionados do frontend na branch `main` do repositório original, no commit `3c7022394d0e90c3b94a2530a559bfca0c4a41bb`. Cada arquivo foi conferido pelo hash de objeto Git, incluindo os recursos binários.

A cópia foi obtida do checkout local e conferida com a árvore remota pelo conector GitHub. Não inclui o histórico Git nem o backend NestJS original. O novo repositório tem histórico independente.

**Atenção ao estado da origem:** essa versão do frontend contém o contador padrão do Flutter. Ela ainda não contém as telas completas do LiteArt. Caso as telas de Mobile estejam em outro checkout ou branch, deverão ser trazidas posteriormente.

O Flutter será referência para o visual e a navegação. As telas da entrega de POO deverão ser adaptadas para Razor, conforme a exigência de ASP.NET Core MVC. A aplicação Flutter não substitui as Views do portal.

## Estrutura

```text
P2_POO/
├── README.md
├── .gitignore
├── frontend/
│   └── liteart_app/              # Flutter original, como referência
└── LiteArt.Poo/
    ├── LiteArt.Poo.csproj
    ├── Controllers/
    ├── Models/
    │   ├── Enums/
    │   └── ViewModels/
    ├── Repositories/
    │   ├── Interfaces/
    │   └── Dapper/
    ├── Exceptions/
    ├── Views/
    │   ├── Shared/
    │   ├── Home/
    │   ├── Alunos/
    │   ├── Materias/
    │   ├── Questoes/
    │   └── Tentativas/
    ├── Database/
    │   └── script.sql
    ├── wwwroot/
    ├── appsettings.json
    └── Program.cs
```

As pastas reservadas usam `.gitkeep` para serem mantidas no Git. Controllers cuidarão do fluxo HTTP; Models concentrarão as regras de domínio; Repositories concentrarão o acesso a dados; Views cuidarão da apresentação. Conexões e SQL deverão ficar fora de Controllers e Views.

## Uso de inteligência artificial

A inteligência artificial pode ser utilizada como ferramenta auxiliar durante o desenvolvimento, apoiando a revisão e a melhoria do código escrito pelos integrantes.

O código deve ser desenvolvido pela equipe. Os integrantes podem enviar o próprio código à IA para solicitar verificação, identificar possíveis erros, discutir sugestões de melhoria e tirar dúvidas sobre os conceitos e as tecnologias utilizados. A IA não deve escrever o código no lugar dos integrantes.

Cada integrante é responsável por avaliar as sugestões recebidas, compreender as alterações e verificar o funcionamento do próprio código antes de incorporá-las ao projeto.
