using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlOrders2023.ViewModels.Converters;
using Microsoft.UI.Xaml.Data;
using Syncfusion.UI.Xaml.DataGrid;

namespace BlOrders2023.UserControls;
internal class FloatGridNumericColumn : GridNumericColumn
{
    // Override the SetDisplayBindingConverter method

    protected override void SetDisplayBindingConverter()

    {

        // Set the converter for the DisplayBinding property

        if ((DisplayBinding as Binding).Converter == null)

            (DisplayBinding as Binding).Converter = new FloatToDoubleConverter();



        // Set the converter for the ValueBinding property

        if ((ValueBinding as Binding).Converter == null)

            (ValueBinding as Binding).Converter = new FloatToDoubleConverter();

    }

}
