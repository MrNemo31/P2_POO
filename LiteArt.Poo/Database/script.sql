-- LiteArt POO - PostgreSQL (proposta a confirmar com a equipe).
-- Arquivo reservado para a próxima etapa; não contém SQL executável.
-- Implementar o DDL das entidades e relacionamentos após validar a modelagem.
-- Incluir de 5 a 10 registros fictícios por tabela no DML da entrega final.

-- LITEART POO
-- Banco de Dados: PostgreSQL
-- Criacao das tabelas


-- 1. Tabela de administradores
CREATE TABLE administradores (
    id_administrador INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE
);

-- 2. Tabela de alunos
CREATE TABLE alunos (
    id_aluno INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE
);

-- 3. Tabela de materias
CREATE TABLE materias (
    id_materia INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao TEXT,
    id_administrador INTEGER NOT NULL,

    CONSTRAINT fk_materias_administradores
        FOREIGN KEY (id_administrador)
        REFERENCES administradores(id_administrador)
);

-- 4. Tabela de questoes
CREATE TABLE questoes (
    id_questao INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    enunciado TEXT NOT NULL,
    ativa BOOLEAN NOT NULL DEFAULT TRUE,
    tipo_questao VARCHAR(30) NOT NULL,
    resposta_correta BOOLEAN,
    id_administrador INTEGER NOT NULL,

    CONSTRAINT chk_tipo_questao
        CHECK (tipo_questao IN (
            'MULTIPLA_ESCOLHA',
            'VERDADEIRO_FALSO'
        )),

    CONSTRAINT chk_resposta_verdadeiro_falso
        CHECK (
            (tipo_questao = 'VERDADEIRO_FALSO'
                AND resposta_correta IS NOT NULL)
            OR
            (tipo_questao = 'MULTIPLA_ESCOLHA'
                AND resposta_correta IS NULL)
        ),

    CONSTRAINT fk_questoes_administradores
        FOREIGN KEY (id_administrador)
        REFERENCES administradores(id_administrador)
);

-- 5. Tabela de relacionamento entre questoes e materias
CREATE TABLE questoes_materias (
    id_questao INTEGER NOT NULL,
    id_materia INTEGER NOT NULL,

    PRIMARY KEY (id_questao, id_materia),

    FOREIGN KEY (id_questao)
        REFERENCES questoes(id_questao)
        ON DELETE CASCADE,

    FOREIGN KEY (id_materia)
        REFERENCES materias(id_materia)
        ON DELETE CASCADE
);

-- 6. Tabela de alternativas
CREATE TABLE alternativas (
    id_alternativa INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_questao INTEGER NOT NULL,
    texto TEXT NOT NULL,
    correta BOOLEAN NOT NULL DEFAULT FALSE,

    FOREIGN KEY (id_questao)
        REFERENCES questoes(id_questao)
        ON DELETE CASCADE
);

-- 7. Tabela de tentativas
CREATE TABLE tentativas (
    id_tentativa INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_aluno INTEGER NOT NULL,
    id_questao INTEGER NOT NULL,
    resposta TEXT NOT NULL,
    acertou BOOLEAN NOT NULL,
    data_hora TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    FOREIGN KEY (id_aluno)
        REFERENCES alunos(id_aluno),

    FOREIGN KEY (id_questao)
        REFERENCES questoes(id_questao)
);


-- INSERCAO DOS DADOS FICTICIOS (DML)


-- 1. Cadastro de administradores
INSERT INTO administradores (nome, email)
VALUES
('Carlos Mendes', 'carlos@liteart.com'),
('Ana Oliveira', 'ana@liteart.com'),
('Mariana Costa', 'mariana@liteart.com'),
('Pedro Almeida', 'pedro@liteart.com'),
('Juliana Santos', 'juliana@liteart.com');


-- 2. Cadastro de alunos
INSERT INTO alunos (nome, email)
VALUES
('Lucas Ferreira', 'lucas@gmail.com'),
('Mariana Souza', 'mariana@gmail.com'),
('Gabriel Oliveira', 'gabriel@gmail.com'),
('Beatriz Santos', 'beatriz@gmail.com'),
('Rafael Costa', 'rafael@gmail.com');


-- 3. Cadastro de materias
INSERT INTO materias (nome, descricao, id_administrador)
VALUES
('Matematica', 'Conteudos de matematica para vestibulares.', 1),
('Portugues', 'Gramatica, interpretacao de texto e literatura.', 2),
('Historia', 'Historia geral e historia do Brasil.', 3),
('Geografia', 'Geografia fisica, humana e economica.', 4),
('Biologia', 'Conteudos de biologia para vestibulares.', 5);


-- 4. Cadastro de questoes
INSERT INTO questoes
(enunciado, ativa, tipo_questao, resposta_correta, id_administrador)
VALUES
('Quanto e 7 x 8?', TRUE, 'MULTIPLA_ESCOLHA', NULL, 1),

('Qual e a capital do Brasil?', TRUE, 'MULTIPLA_ESCOLHA', NULL, 2),

('A Revolucao Francesa ocorreu em 1789.',
 TRUE, 'VERDADEIRO_FALSO', TRUE, 3),

('A fotossintese ocorre principalmente nas mitocondrias.',
 TRUE, 'VERDADEIRO_FALSO', FALSE, 5),

('Qual e o maior planeta do Sistema Solar?',
 TRUE, 'MULTIPLA_ESCOLHA', NULL, 4);


-- 5. Relacionamento entre questoes e materias
INSERT INTO questoes_materias (id_questao, id_materia)
VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 5),
(5, 4);


-- 6. Cadastro de alternativas
INSERT INTO alternativas (id_questao, texto, correta)
VALUES
-- Questao 1
(1, '54', FALSE),
(1, '56', TRUE),
(1, '64', FALSE),
(1, '48', FALSE),

-- Questao 2
(2, 'Sao Paulo', FALSE),
(2, 'Brasilia', TRUE),
(2, 'Rio de Janeiro', FALSE),
(2, 'Salvador', FALSE),

-- Questao 5
(5, 'Terra', FALSE),
(5, 'Marte', FALSE),
(5, 'Jupiter', TRUE),
(5, 'Venus', FALSE);


-- 7. Cadastro de tentativas
INSERT INTO tentativas
(id_aluno, id_questao, resposta, acertou)
VALUES
-- Lucas Ferreira
(1, 1, '56', TRUE),
(1, 2, 'Sao Paulo', FALSE),

-- Mariana Souza
(2, 1, '64', FALSE),
(2, 3, 'VERDADEIRO', TRUE),

-- Gabriel Oliveira
(3, 2, 'Brasilia', TRUE),
(3, 4, 'VERDADEIRO', FALSE),

-- Beatriz Santos
(4, 5, 'Jupiter', TRUE),
(4, 3, 'VERDADEIRO', TRUE),

-- Rafael Costa
(5, 1, '56', TRUE),
(5, 4, 'FALSO', TRUE),

-- Segunda tentativa de Lucas na questao 1
(1, 1, '56', TRUE);
