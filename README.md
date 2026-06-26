# AspNetCore Condomínio (Docker & MVC)

Bem-vindo ao repositório do projeto **Docker_AspNetCore_MVC_Condominio**.  
Esta é uma aplicação desenvolvida em **ASP.NET Core MVC (8.0)** voltada para a gestão de condomínios utilizando SaaS multi-tenant.  
O sistema conta com autenticação, autorização granular baseada em perfis (Roles/Claims) e persistência automatizada utilizando **Entity Framework Core** com **SQL Server 2022**, tudo conteinerizado com **Docker**.

---

## 📖 Contexto Histórico

Em **2020**, enquanto trabalhava com suporte, iniciei meus estudos em **ASP.NET Core MVC (3.0)**.  
Daí surgiu a ideia de criar um sistema para portaria de condomínio totalmente do zero. Esse projeto foi minha primeira grande experiência prática com a tecnologia.

Agora, em **2026**, refiz todo o projeto em **ASP.NET Core MVC (8.0)**, mantendo a estrutura original (sem camadas e sem serviços) para preservar a essência do aprendizado inicial.  
A principal novidade foi a adaptação para execução em **Docker Compose**, tornando a aplicação facilmente conteinerizada.

Além disso, mantenho outro projeto em **API com DDD**, também voltado para condomínios, que pode ser visto aqui:  
👉 [Docker_RabbitMQ_AspNetCore_React_Condominio](https://github.com/WaineAlvesCarneiro/Docker_RabbitMQ_AspNetCore_React_Condominio)

---

Graças à conteinerização, você não precisa instalar o SQL Server localmente.  
O banco já deve estar rodando em um container separado (`sqlserver2022`) conectado à rede `infra_net`.  
O Docker Compose sobe apenas a aplicação `mvc_condominio`, que se conecta automaticamente ao SQL Server existente.

---

### ✅ Pré-requisitos
- Docker Desktop instalado e rodando  
- Git para clonar o repositório  
- Container do SQL Server 2022 já criado e conectado à rede `infra_net`  

### ▶️ Passo a Passo
1. Clone o repositório:

```bash
git clone https://github.com/WaineAlvesCarneiro/Docker_AspNetCore_MVC_Condominio.git
cd Docker_AspNetCore_MVC_Condominio
```

### ▶️ Suba os containers

No terminal, dentro da pasta raiz do projeto (onde está o arquivo `docker-compose.yml`), execute:

2. Certifique-se de que o SQL Server já está rodando:
Bash
```bash
docker ps
```

3. Suba a aplicação:
Bash
```bash
docker compose up -d --build
```

4. Acesse:

- Aplicação: http://localhost:5001
- SQL Server: disponível dentro da rede como sqlserver2022:1433

---

## 🔑 Credenciais para Teste

O sistema já vem com uma carga inicial de dados contendo três perfis diferentes e uma empresa/condomínio:

- 👨‍💻 **Suporte** → acesso total, gerencia empresas e cria usuários  
- 🏢 **Síndico** → gestão administrativa (imóveis, moradores, relatórios)  
- 🛡️ **Porteiro** → controle operacional (visitantes, prestadores, encomendas)  

🔐 Senha padrão para todos: **123456**

---

## 📋 Funcionalidades

- 🔒 **Controle de acesso** com Microsoft Identity (Roles/Claims)  
- 🏢 **Multi-empresa básico**: usuário Suporte cadastra empresas antes dos demais cadastros  
- 🗂️ **Módulos administrativos**: CRUD de imóveis, moradores, visitantes e prestadores  
- 📦 **Gestão de encomendas**: fluxo completo de recebimento e entrega  

---

## 🛠️ Tecnologias Utilizadas

- ⚙️ **Backend**: C# e ASP.NET Core MVC 8.0  
- 🗄️ **ORM**: Entity Framework Core (mapeamentos fluentes customizados)  
- 🔐 **Segurança**: ASP.NET Core Identity  
- 🗃️ **Banco de Dados**: SQL Server 2022 (Docker)  
- 🐳 **Ambiente**: Docker & Docker Compose  

---

## 👨‍💻 Desenvolvedor

### Criado com dedicação por **Waine Alves Carneiro**

#### 🔗 Perfil no GitHub

👉 [GitHub Waine Alves Carneiro](https://github.com/WaineAlvesCarneiro?tab=repositories)
