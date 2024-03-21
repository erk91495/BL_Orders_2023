using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;


namespace BlOrders2023.UserControls;

public class BarcodeInputTextBox: TextBox
{
    protected override void OnCharacterReceived(CharacterReceivedRoutedEventArgs e)
    {
        base.OnCharacterReceived(e);
        if (e.Character == '')
        {
            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;
            Text += '';
            Select(selectionStart + 1, selectionLength + 1);
        }
    }
}
