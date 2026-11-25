# LiteArt POO

Projeto acadêmico da P2 de Programação Orientada a Objetos, do curso de Análise e Desenvolvimento de Sistemas da FATEC Ribeirão Preto, no 2º semestre de 2026.

**Professor:** Prof. Me. Douglas Samuel Gonçalves.

## Proposta

O LiteArt POO é um portal web de questões de vestibular, desenvolvido em C# com ASP.NET Core MVC. A interface reúne ferramentas administrativas para gerenciar os cadastros e o banco de questões, além de permitir selecionar um aluno para registrar respostas e consultar seu histórico de tentativas.

Esta versão usa um recorte menor do [LiteArt](https://miro.com/app/board/uXjVH3-xMjs=/?share_link_id=589467137863), com foco na aplicação dos conceitos de orientação a objetos em C# e na organização de uma aplicação MVC com persistência em PostgreSQL.

## Equipe

O grupo é composto por três integrantes:

| Integrante | Nome | RA |
| --- | --- | --- |
| 1 | Natan Fernado Cardeal da Costa | 2840482511044 |
| 2 | Gustavo Belotti Machado Luiz | 2840482511012 |
| 3 | Luiz Henrique Conti Malvestio | 2840482511008 |

## Funcionalidades

- Listar, cadastrar e editar administradores.
- Listar, cadastrar, editar e excluir alunos e matérias, respeitando os vínculos existentes.
- Listar, cadastrar, editar e excluir questões, com filtro por matéria.
- Associar uma questão a uma ou mais matérias.
- Trabalhar com questões de múltipla escolha e verdadeiro/falso.
- Selecionar um aluno, responder questões e receber o resultado após o envio.
- Consultar o histórico por aluno, com uma linha para cada tentativa e indicação de acerto ou erro.

A Home oferece acesso ao banco de questões, à resolução e às listagens de matérias, alunos e administradores. O menu também disponibiliza o histórico.

Recursos de IA no portal, planos pagos, simulados, metas de estudo e autenticação completa ficam fora do escopo desta versão.

## Regras de negócio

- Toda questão pertence a uma ou mais matérias. Uma questão vinculada a várias matérias aparece uma única vez na listagem e pode ser encontrada pelo filtro de qualquer uma delas.
- Matérias e questões possuem um administrador responsável. A edição de uma matéria permite trocar esse administrador sem alterar o ID da matéria.
- Toda tentativa pertence a um aluno e a uma questão.
- Um aluno pode realizar várias tentativas da mesma questão; uma nova tentativa preserva as anteriores.
- A avaliação da resposta varia conforme o tipo da questão, por meio das implementações de `AvaliarResposta` em `MultiplaEscolha` e `VerdadeiroFalso`.
- Em múltipla escolha, a resposta registrada é o ID da alternativa, armazenado como texto. Em verdadeiro/falso, a resposta é representada por `true` ou `false`.
- Questões inativas não recebem novas tentativas.
- O histórico utiliza o resultado armazenado em cada tentativa, sem recalculá-lo durante a consulta.
- A exclusão de questões com tentativas é bloqueada. Alunos com tentativas e matérias vinculadas a questões também não são excluídos.
- A edição de questões preserva os IDs das alternativas existentes e bloqueia a remoção de alternativas utilizadas em tentativas.

## Estado atual

A aplicação possui telas Razor integradas aos controllers e ao banco de dados para o gerenciamento de administradores, alunos, matérias e questões, além dos fluxos de resolução e histórico. A interface utiliza o padrão visual do LiteArt, com fundo creme, texto marrom e botões principais em laranja.

O projeto contém entidades de domínio, ViewModels, interfaces de acesso a dados e implementações com Dapper. A interface genérica `IRepository<T>` define as operações comuns dos repositórios. A gravação de questões, matérias associadas e alternativas utiliza transações para manter a consistência dos relacionamentos.

O arquivo `LiteArt.Poo/Database/script.sql` cria as tabelas, os relacionamentos e os dados iniciais. A conexão é obtida de `ConnectionStrings:DefaultConnection`; o `Program.cs` também carrega o arquivo de configuração local `appsettings.dev.json`.

## Tecnologias

| Componente | Tecnologia | Uso no projeto |
| --- | --- | --- |
| Portal | C# e ASP.NET Core MVC, com alvo .NET 8 | Controllers, rotas e organização MVC |
| Interface do portal | Razor, HTML, CSS e JavaScript | Telas, formulários, navegação e interações |
| Acesso a dados | Dapper, ADO.NET e Npgsql | Consultas e persistência com SQL parametrizado |
| Banco relacional | PostgreSQL | Cadastros, relacionamentos e tentativas |
| Frontend de referência | Flutter e Dart | Referência do projeto Mobile, sem integração ao MVC |

## Reaproveitamento do LiteArt

Estamos utilizando como referência o frontend desenvolvido para a disciplina de Programação de Dispositivos Móveis, que faz parte do LiteArt, projeto de TCC de Natan Fernado Cardeal da Costa e Gustavo Belotti. Para desenvolver esta versão da P2 de Programação Orientada a Objetos, convidamos Luiz Henrique para integrar a equipe e ajudar no projeto.

A cópia do frontend Mobile permanece em `frontend/liteart_app/` como referência. As telas utilizadas pelo portal de POO foram desenvolvidas em Razor, HTML, CSS e JavaScript dentro de `LiteArt.Poo/`. A aplicação Flutter e o backend NestJS do projeto original não integram a aplicação MVC.

## Estrutura

```text
P2_POO/
├── README.md
├── .gitignore
├── frontend/
│   └── liteart_app/              # Frontend Mobile de referência
└── LiteArt.Poo/
    ├── LiteArt.Poo.csproj
    ├── Controllers/
    ├── Data/                     # Fábrica de conexões com PostgreSQL
    ├── Models/
    │   ├── Entities/             # Entidades e regras de domínio
    │   └── ViewModels/           # Dados e validações das telas
    ├── Repositories/
    │   ├── Interfaces/
    │   └── Dapper/
    ├── Exceptions/
    ├── Previa/                   # Rotas existentes de apresentação da Home
    ├── Views/
    │   ├── Shared/
    │   ├── Home/
    │   ├── Administradores/
    │   ├── Alunos/
    │   ├── Materias/
    │   ├── Questoes/
    │   ├── Estudo/
    │   └── Historico/
    ├── Database/
    │   └── script.sql
    ├── wwwroot/                  # CSS, JavaScript e imagens
    ├── appsettings.json
    └── Program.cs
```

Os controllers coordenam as requisições HTTP; as entidades concentram as regras de domínio; os ViewModels organizam os dados dos formulários e das telas; os repositórios concentram o acesso ao banco; e as Views cuidam da apresentação.

## Uso de inteligência artificial

A inteligência artificial pode ser utilizada como ferramenta auxiliar durante o desenvolvimento, apoiando a revisão e a melhoria do código escrito pelos integrantes.

O código deve ser desenvolvido pela equipe. Os integrantes podem enviar o próprio código à IA para solicitar verificação, identificar possíveis erros, discutir sugestões de melhoria e tirar dúvidas sobre os conceitos e as tecnologias utilizados. A IA não deve escrever o código no lugar dos integrantes.

Cada integrante é responsável por avaliar as sugestões recebidas, compreender as alterações e verificar o funcionamento do próprio código antes de incorporá-las ao projeto.
