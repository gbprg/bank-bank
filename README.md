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
docker-compose up --build
</code>
</pre>

<h3>2. Desenvolvimento Local 💻</h3>

<h4>Configuração do Banco de Dados</h4>
<ul>
  <li>Instale o PostgreSQL</li>
  <li>Crie o banco de dados <code>transfer_bank</code></li>
  <li>Configure a conexão em <code>appsettings.json</code></li>
</ul>

## 🔧 Configuração

<h4>Variáveis de Ambiente</h4>
<table>
  <thead>
    <tr>
      <th>Variável</th>
      <th>Descrição</th>
      <th>Valor Padrão</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>DefaultConnection</td>
      <td>String de conexão do banco</td>
      <td><code>Host=localhost;Database=transfer_bank</code></td>
    </tr>
    <tr>
      <td>Jwt:Key</td>
      <td>Chave de autenticação JWT</td>
      <td><code>ChaveSuperSeguraMuitoGrande1234567890!</code></td>
    </tr>
    <tr>
      <td>Jwt:Issuer</td>
      <td>Chave de autenticação JWT</td>
      <td><code>TransferBankApp</code></td>
    </tr>
    <tr>
      <td>Jwt:Audience</td>
      <td>Chave de autenticação JWT</td>
      <td><code>TransferBankUsers</code></td>
    </tr>
  </tbody>
</table>

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


