using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente para exibir mensagem quando não há conteúdo
    /// </summary>
    public class EmptyContentComponent : BaseReportComponent
    {
        private readonly string _mensagem;
        
        public EmptyContentComponent(string mensagem)
        {
            _mensagem = mensagem;
        }
        
        public override void Compose(IContainer container)
        {
            ComposeEmptyMessage(container, _mensagem);
        }
    }
}
