# 🏦 Transfer Bank

Uma aplicação bancária com .NET 8 e PostgreSQL, utilizando autenticação JWT e Docker.

## 🚀 Início Rápido

### Pré-requisitos
<ul>
  <li>.NET 8 SDK</li>
  <li>Docker</li>
</ul>

## 💻 Instalação

<h3>1. Docker (Recomendado) 🐳</h3>
<pre>
<code>
# Clone o repositório
git clone https://github.com/seu-usuario/transfer-bank.git
cd transfer-bank

# Construa e inicie os containers
docker compose up -d --build
</code>
</pre>

<h3>2. Desenvolvimento Local 💻</h3>

<h4>Configuração do Banco de Dados</h4>

## 🔧 Configuração
<pre>
<code>
# Restaure os pacotes
dotnet restore

# Aplique as migrações
dotnet ef database update

# Rode a aplicação
dotnet run
</code>
</pre>

## ✨ Recursos
<ul>
  <li>🔒 Autenticação JWT</li>
  <li>💳 Gerenciamento de Transações</li>
  <li>🗃️ Banco de Dados PostgreSQL</li>
  <li>📖 Documentação Swagger</li>
</ul>

## 🛠️ Tecnologias
<div>
  <img src="https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg" alt=".NET" width="50" style="margin-right: 10px;">
  <img src="https://upload.wikimedia.org/wikipedia/commons/2/29/Postgresql_elephant.svg" alt="PostgreSQL" width="50" style="margin-right: 10px;">
  <img src="https://www.docker.com/wp-content/uploads/2022/03/Moby-logo.png" alt="Docker" width="50">
</div>

## 🌐 Testes das Rotas com POSTMAN

### 🛠️ Configuração
Para testar as rotas no Postman, lembre-se de configurar o **Bearer Token** no cabeçalho de **Authorization** para rotas protegidas. O token é gerado ao realizar o login.  

----

### TransferBank/User

#### 🔹 POST - Register
Registra um novo usuário.  
**URL:**  
`http://localhost:5078/api/signup`  
**Body:**
```json
{
    "email": "teste2@email.com",
    "name": "teste2",
    "password": "123123teste@"
}
```
----

🔹 POST - Login
Realiza o login do usuário e retorna um token.
**URL:**
`http://localhost:5078/api/signin`
**Body:**
```json
{
    "email": "teste@email.com",
    "password": "123123teste@"
}
```
----

```json
🔹 DELETE - Delete
Exclui um usuário pelo ID.
URL:
http://localhost:5078/api/user/{id}
Exemplo:
http://localhost:5078/api/user/101f2908-b83a-402c-939e-3a51269d3827
```
----

TransferBank/Transactions
🔹 POST - Deposit
Realiza um depósito para um usuário.
**URL:**
http://localhost:5078/api/deposit
**Body:**

```json
{
    "userId": "da10c18d-9471-4f5c-ad03-13a3df054d44",
    "amount": 10,
    "paymentMethod": "debitcard"
}
```
----

🔹 POST - Transfer
Realiza uma transferência entre usuários.
**URL:**
http://localhost:5078/api/transfer
**Body:**

```json
{
    "receiverId": "b9c9e50b-72dd-403a-a24c-a17c1390b538",
    "amount": 1.50,
    "paymentMethod": "pix"
}
```
----

```json
🔹 GET - History
Retorna o histórico de transações de um usuário com paginação.
URL:
http://localhost:5078/api/history?userId={id}&page={page}&pageSize={pageSize}
Exemplo:
http://localhost:5078/api/history?userId=101f2908-b83a-402c-939e-3a51269d3827&page=1&pageSize=10

----

🔹 POST - Reverse
Reverte uma transação pelo ID.
URL:
http://localhost:5078/api/reverse/?transactionId={id}
Exemplo:
http://localhost:5078/api/reverse/?transactionId=0193adbf-e2ae-7043-aea2-1bbb1277cafa

----

🔹 GET - Export
Exporta o histórico de transações com filtros.
URL:
Para os últimos 30 dias:
http://localhost:5078/api/export?filter=last30days&userId={id}

Para um mês específico (mês/ano):
http://localhost:5078/api/export?filter=month&monthYear={MM/AA}&userId={id}
Exemplo:
http://localhost:5078/api/export?filter=month&monthYear=05/24&userId=f0266dfb-4689-4bc6-b8b2-7d245079e45e
```



