using CommunityToolkit.Mvvm.ComponentModel;
using KeyAndLicenceGenerator.Models;
using System.Collections.ObjectModel;

namespace KeyAndLicenceGenerator.ViewModels
{
    public partial class SearchPickerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string entryText;

        [ObservableProperty]
        private string entryTextChanged;

        [ObservableProperty]
        private string pickerTitle;

        [ObservableProperty]
        private int pickerSelectedIndex;

        [ObservableProperty]
        private ObservableCollection<PfxFileInfo> pickerItems;

        public SearchPickerViewModel()
        {
        }
    }
}