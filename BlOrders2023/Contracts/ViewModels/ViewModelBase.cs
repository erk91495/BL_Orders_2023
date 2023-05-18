using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlOrders2023.Contracts.ViewModels
{
    public abstract class ViewModelBase: ObservableRecipient, INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        private readonly Dictionary<string, ICollection<string>> _validationErrors = new();
        public bool HasErrors => _validationErrors.Count > 0;

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_validationErrors.ContainsKey(propertyName))
                return new List<string>().ToList();

            return _validationErrors[propertyName];
        }

        protected void SetErrors(string key, ICollection<string> errors)
        {
            if (errors.Any())
                _validationErrors[key] = errors;
            else
                _ = _validationErrors.Remove(key);

            OnErrorsChanged(key);
        }
    }
}
