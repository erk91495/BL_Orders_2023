using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.Models;
using BlOrders2023.Reporting;
using BlOrders2023.Reporting.ReportClasses;

namespace BlOrders2023.ViewModels.ReportControls;
public interface IReportViewModel<T>
    where T : IReport
{
    public virtual Type ReportType => typeof(T);
    
    public virtual string ReportName
    {
        get
        {
            var displayNameAtt = (typeof(T).GetCustomAttribute(typeof(DisplayNameAttribute), true) as DisplayNameAttribute);
            return (displayNameAtt != null ? displayNameAtt.DisplayName : nameof(T));
        }
    }    
    
    public abstract string ReportDescription { get; }

    public ReportCategory ReportCategories
    {
        get;
    }
    public List<PromptTypes> Prompts
    {
        get;
    }
    public Task<object?[]> GetData(object[] userInputs);
}
