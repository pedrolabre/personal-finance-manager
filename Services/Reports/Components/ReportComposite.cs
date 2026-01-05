using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PersonalFinanceManager.Services.Reports.Components
{
    /// <summary>
    /// Componente composto que agrega múltiplos componentes filhos
    /// Implementa o padrão Composite para estruturas hierárquicas
    /// </summary>
    public class ReportComposite : BaseReportComponent
    {
        private readonly List<IReportComponent> _children = new();
        
        public override bool HasContent => _children.Any(c => c.HasContent);
        
        /// <summary>
        /// Adiciona um componente filho à composição
        /// </summary>
        public void Add(IReportComponent component)
        {
            _children.Add(component);
        }
        
        /// <summary>
        /// Remove um componente filho da composição
        /// </summary>
        public void Remove(IReportComponent component)
        {
            _children.Remove(component);
        }
        
        /// <summary>
        /// Renderiza todos os componentes filhos sequencialmente
        /// </summary>
        public override void Compose(IContainer container)
        {
            container.Column(column =>
            {
                foreach (var child in _children.Where(c => c.HasContent))
                {
                    column.Item().Element(child.Compose);
                }
            });
        }
    }
}
