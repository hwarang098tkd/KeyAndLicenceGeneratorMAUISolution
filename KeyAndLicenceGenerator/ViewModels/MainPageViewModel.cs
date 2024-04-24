using System.Windows.Input;

namespace KeyAndLicenceGenerator.ViewModels
{
    public class MainPageViewModel
    {
        public ICommand NavigateToKeysCommand { get; }
        public ICommand NavigateToLicencesCommand { get; }

        public MainPageViewModel()
        {
            NavigateToKeysCommand = new Command(async () => await Shell.Current.GoToAsync("//KeysGeneratorPage"));
            NavigateToLicencesCommand = new Command(async () => await Shell.Current.GoToAsync("//LicenceGeneratorPage"));
        }
    }
}