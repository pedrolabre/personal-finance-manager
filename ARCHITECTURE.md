# Arquitetura do Personal Finance Manager

## Visão Geral

O Personal Finance Manager segue uma arquitetura em camadas baseada no padrão MVVM (Model-View-ViewModel), com clara separação de responsabilidades e forte tipagem.

## Diagrama de Camadas
```
┌─────────────────────────────────────────┐
│           Views (XAML)                  │
│  Interface visual do usuário            │
└─────────────────┬───────────────────────┘
                  │ Data Binding
┌─────────────────▼───────────────────────┐
│          ViewModels                     │
│  Lógica de apresentação                 │
└─────────────────┬───────────────────────┘
                  │ Chamadas de método
┌─────────────────▼───────────────────────┐
│           Services                      │
│  Lógica de negócio                      │
└─────────────────┬───────────────────────┘
                  │ CRUD operations
┌─────────────────▼───────────────────────┐
│         Repositories                    │
│  Acesso a dados                         │
└─────────────────┬───────────────────────┘
                  │ EF Core
┌─────────────────▼───────────────────────┐
│      Data (DbContext + Entities)        │
│  Mapeamento objeto-relacional           │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         SQLite Database                 │
│  Persistência de dados                  │
└─────────────────────────────────────────┘
```

## Detalhamento das Camadas

### 1. Views (Camada de Apresentação)

**Responsabilidade**: Renderização visual

**Características**:
- Arquivos XAML puros
- Code-behind mínimo (apenas `InitializeComponent()`)
- Data binding com ViewModels
- Sem lógica de negócio

**Exemplo de estrutura**:
```
Views/
├── DashboardView.xaml
├── Pendencias/
│   ├── PendenciasListView.xaml
│   ├── PendenciaFormView.xaml
│   └── PendenciaDetalhesView.xaml
```

### 2. ViewModels (Camada de Lógica de Apresentação)

**Responsabilidade**: Gerenciar estado da UI e comandos

**Características**:
- Implementam `INotifyPropertyChanged`
- Expõem propriedades bindáveis
- Contêm `ICommand` para ações do usuário
- Comunicam-se com Services
- Não conhecem Views diretamente

**Padrões utilizados**:
- `ViewModelBase`: Classe base com `SetProperty`
- `RelayCommand`: Implementação de `ICommand`
- `AsyncRelayCommand`: Para operações assíncronas

### 3. Services (Camada de Lógica de Negócio)

**Responsabilidade**: Regras de negócio e orquestração

**Características**:
- Validações complexas
- Cálculos de negócio
- Orquestração entre múltiplos repositories
- Transformação de Entities em DTOs
- Não conhecem ViewModels ou Views

**Exemplo**:
```csharp
public class PendenciaService : IPendenciaService
{
    // Valida, transforma e orquestra operações
    public async Task<PendenciaDto> CriarAsync(PendenciaDto dto)
    {
        // Validação
        ValidarPendencia(dto);
        
        // Transformação
        var entity = MapearParaEntity(dto);
        
        // Persistência
        await _repository.AddAsync(entity);
        
        // Notificação
        _messenger.Send(new PendenciaCriadaMessage());
        
        return MapearParaDto(entity);
    }
}
```

### 4. Repositories (Camada de Acesso a Dados)

**Responsabilidade**: Operações CRUD e consultas

**Características**:
- Abstrai o DbContext
- Queries específicas do domínio
- Operações assíncronas
- Include de relacionamentos

**Padrão Repository**:
```csharp
public interface IPendenciaRepository
{
    Task<IEnumerable<Pendencia>> GetAllAsync();
    Task<Pendencia?> GetByIdAsync(int id);
    Task<Pendencia> AddAsync(Pendencia entity);
    Task UpdateAsync(Pendencia entity);
    Task DeleteAsync(int id);
}
```

### 5. Data (Camada de Persistência)

**Responsabilidade**: Mapeamento objeto-relacional

**Componentes**:
- **AppDbContext**: Ponto central do EF Core
- **Entities**: POCOs com relacionamentos
- **Configurations**: Fluent API para configurações

## Padrões de Projeto Utilizados

### Repository Pattern
Abstrai acesso a dados, facilitando testes e manutenção.

### Service Layer Pattern
Centraliza lógica de negócio, evitando ViewModels "gordas".

### Dependency Injection
Gerencia dependências e facilita testes unitários.

### MVVM Pattern
Separa lógica de apresentação da interface visual.

### Messenger Pattern
Comunicação desacoplada entre ViewModels.

## Fluxo de Dados

### Criação de uma Pendência
```
1. View (PendenciaFormView)
   └─> Usuário preenche formulário
   
2. ViewModel (PendenciaFormViewModel)
   └─> Valida campos obrigatórios
   └─> Cria PendenciaDto
   └─> Chama Service
   
3. Service (PendenciaService)
   └─> Valida regras de negócio
   └─> Transforma DTO em Entity
   └─> Chama Repository
   
4. Repository (PendenciaRepository)
   └─> Adiciona ao DbContext
   └─> Salva mudanças
   
5. DbContext
   └─> Persiste no SQLite
   
6. Messenger
   └─> Notifica outros ViewModels
   
7. Navigation
   └─> Retorna para lista
```

## Sistema de Navegação

### NavigationService

Gerencia troca de views sem acoplamento:
```csharp
// Navegar para outra tela
_navigationService.NavigateTo<PendenciaFormViewModel>();

// Navegar com parâmetro
_navigationService.NavigateTo<PendenciaDetalhesViewModel>(pendenciaId);

// Voltar
_navigationService.NavigateBack();
```

### DataTemplates Automáticos

Views são resolvidas automaticamente via DataTemplates:
```xaml
<DataTemplate DataType="{x:Type vm:PendenciasListViewModel}">
    <views:PendenciasListView/>
</DataTemplate>
```

## Sistema de Mensagens

### Messenger Pattern

Comunicação pub/sub entre ViewModels:
```csharp
// Publisher
_messenger.Send(new PendenciaCriadaMessage(id));

// Subscriber
_messenger.Register<PendenciaCriadaMessage>(this, msg => {
    _ = CarregarPendenciasAsync();
});
```


## Estrutura de Pastas Essencial

```
personal-finance-manager/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── Core/                  # Comandos, navegação, DI, logging, mapping, messaging
│   ├── Commands/
│   ├── DependencyInjection/
│   ├── Dialogs/
│   ├── Logging/
│   ├── Mapping/
│   ├── Messaging/
│   └── Navigation/
├── Data/                  # DbContext, Entities, Configurations
│   ├── AppDbContext.cs
│   ├── AppDbContextFactory.cs
│   ├── Entities/
│   └── Configurations/
├── Helpers/               # Helpers utilitários
├── Models/                # DTOs, Enums, AppSettings
│   ├── DTOs/
│   ├── Enums/
│   └── AppSettings.cs
├── Repositories/          # Interfaces e Implementações
│   ├── Interfaces/
│   └── Implementations/
├── Resources/             # Converters, Styles, Icons
│   ├── Converters/
│   ├── Icons/
│   └── Styles/
├── Services/              # Lógica de negócio, importação, notificações, relatórios
│   ├── Implementations/
│   ├── Import/
│   ├── Interfaces/
│   ├── Notifications/
│   └── Reports/
├── ViewModels/            # ViewModels organizados por domínio
│   ├── Acordos/
│   ├── Base/
│   ├── Cartoes/
│   ├── Configuracoes/
│   ├── Import/
│   ├── Importacao/
│   ├── Pendencias/
│   ├── Recebimentos/
│   ├── Relatorios/
│   ├── Reports/
│   ├── Settings/
│   ├── DashboardViewModel.cs
│   ├── MainViewModel.cs
├── Views/                 # Views XAML organizadas por domínio
│   ├── Acordos/
│   ├── Cartoes/
│   ├── Configuracoes/
│   ├── Import/
│   ├── Importacao/
│   ├── Pendencias/
│   ├── Recebimentos/
│   ├── Relatorios/
│   ├── Reports/
│   ├── Settings/
│   ├── DashboardView.xaml / DashboardView.xaml.cs
├── Migrations/            # Migrations do EF Core
├── LICENSE
├── README.md
├── CHANGELOG.md
├── ARCHITECTURE.md
├── personal-finance-manager.sln
├── personal-finance-manager.csproj
└── .gitignore
```

**Observações:**
- Apenas arquivos e pastas funcionais e essenciais estão listados.
- Pastas como exemplos/, TempModels/, arquivos de teste e logs não fazem parte da estrutura essencial e devem ser removidos em produção.

## Boas Práticas Implementadas

### 1. Async/Await
Todas operações de I/O são assíncronas.

### 2. Null Safety
Uso de nullable reference types (`string?`).

### 3. Validation
Validação em múltiplas camadas.

### 4. Error Handling
Try-catch com mensagens amigáveis.

### 5. Separation of Concerns
Cada camada tem responsabilidade clara.

### 6. Testability
Interfaces facilitam mocking para testes.

## Escalabilidade

### Adicionar Novo Módulo

1. Criar Entity em `Data/Entities/`
2. Criar Configuration em `Data/Configurations/`
3. Adicionar DbSet no `AppDbContext`
4. Criar Repository (Interface + Implementation)
5. Criar Service (Interface + Implementation)
6. Criar DTOs em `Models/DTOs/`
7. Criar ViewModels em `ViewModels/`
8. Criar Views em `Views/`
9. Registrar no DI (`App.xaml.cs`)
10. Adicionar DataTemplate (`App.xaml`)

### Manter Arquivos Pequenos

Se um arquivo ultrapassar 150 linhas:
- Extrair métodos auxiliares para classes Helper
- Dividir ViewModels grandes em sub-ViewModels
- Criar Services especializados

## Decisões Arquiteturais

### Por que SQLite?
- Arquivo único, fácil backup
- Sem necessidade de servidor
- Excelente para desktop
- Suporte completo do EF Core

### Por que MVVM?
- Separação clara de responsabilidades
- Facilita testes unitários
- Data binding reduz código
- Padrão recomendado para WPF

### Por que Repository + Service?
- Repository: Acesso a dados puro
- Service: Lógica de negócio complexa
- Separação facilita manutenção e testes

## Performance

### Lazy Loading
Evitado para prevenir N+1 queries.

### Eager Loading
Uso de `.Include()` para carregar relacionamentos.

### Async Operations
Evita bloqueio da UI thread.

### ObservableCollection
Atualização automática de listas.

## Segurança

### Validação
- Input validation em ViewModels
- Business validation em Services
- Database constraints em Configurations

### SQL Injection
Protegido nativamente pelo EF Core (queries parametrizadas).

---

Esta arquitetura foi projetada para ser:
- ✅ Escalável
- ✅ Manutenível
- ✅ Testável
- ✅ Limpa
- ✅ Compatível com ferramentas de IA (Copilot)
