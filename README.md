# Personal Finance Manager (PFM)

Aplicação desktop para controle financeiro pessoal, desenvolvida em C# com WPF (MVVM), persistência local em SQLite e geração de relatórios.

## Documentação

- Documentação completa (HTML): `PFM-Documentation-v1.html`
- Arquitetura e estrutura do projeto: `ARCHITECTURE.md`
- Histórico de mudanças: `CHANGELOG.md`

## Principais funcionalidades

- Dashboard com visão geral (totais, progresso e próximos vencimentos)
- Pendências (dívidas/compromissos) com suporte a parcelamento e status
- Cartões de crédito com controle de limite e utilização
- Recebimentos com acompanhamento parcial e atrasos
- Acordos (gestão de parcelamentos)
- Relatórios (PDF) e opções de exportação

## Stack / Tecnologias

- .NET `net10.0-windows` + WPF
- Entity Framework Core 10 + SQLite
- MVVM
- Dependency Injection (`Microsoft.Extensions.DependencyInjection`)
- QuestPDF (relatórios)
- CsvHelper (importação)

## Requisitos

- Windows 10/11
- .NET SDK compatível com `net10.0-windows`
	- Se você preferir usar uma versão estável do .NET, ajuste o `TargetFramework` no `personal-finance-manager.csproj`.

## Como executar (desenvolvimento)

1) Restaurar dependências e compilar

```bash
dotnet restore
dotnet build
```

2) Preparar o banco de dados (SQLite)

Este projeto usa migrations já existentes. Para aplicar as migrations via CLI, instale o `dotnet-ef` (uma vez):

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

3) Executar

```bash
dotnet run
```

## Onde ficam os dados (SQLite)

O arquivo do banco de dados é criado em:

```text
%LocalAppData%\PersonalFinanceManager\finance.db
```

Exemplo (Windows): `C:\Users\<usuario>\AppData\Local\PersonalFinanceManager\finance.db`

Observação: esse arquivo contém seus dados pessoais e deve ficar fora do repositório (o `.gitignore` já cobre `*.db`, `*.sqlite*`, exports, etc.).

## Implantação (publish)

Publicar como binário para Windows (framework-dependent):

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

Saída típica:

```text
bin\Release\net10.0-windows\win-x64\publish\
```

Se quiser gerar um pacote self-contained (não exige .NET instalado na máquina destino):

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

## Licença

Licenciado sob a licença MIT. Veja o arquivo `LICENSE`.

Desenvolvedor: Pedro Labre

## Contribuição

Sugestões, issues e PRs são bem-vindos. Para mudanças maiores, prefira abrir uma issue descrevendo o contexto e o objetivo.
