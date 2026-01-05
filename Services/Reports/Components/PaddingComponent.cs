using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente que adiciona padding a outro componente
    /// </summary>
    public class PaddingComponent : BaseReportComponent
    {
        private readonly IReportComponent _child;
        private readonly double _top;
        private readonly double _bottom;
        
        public PaddingComponent(IReportComponent child, double top = 0, double bottom = 0)
        {
            _child = child;
            _top = top;
            _bottom = bottom;
        }
        
        public override bool HasContent => _child.HasContent;
        
        public override void Compose(IContainer container)
        {
            container.PaddingTop((float)_top).PaddingBottom((float)_bottom).Element(_child.Compose);
        }
    }
}
