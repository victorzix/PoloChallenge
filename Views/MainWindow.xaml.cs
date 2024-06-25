using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PoloChallenge.Services;
using PoloChallenge.ViewModels;

namespace PoloChallenge.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        var expectativaService = new ApiServices();
        _mainViewModel = new MainViewModel(expectativaService);
        DataContext = _mainViewModel;
    }

    private async void LoadData(object sender, RoutedEventArgs e)
    {
        try
        {
            if (IndicadorComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                string selectedValue = selectedItem.Content.ToString();
                if (selectedValue != "Escolha um Indicador")
                {
                    _mainViewModel._filter = selectedValue;
                }
                else
                {
                    _mainViewModel._filter = string.Empty;
                }
            }
            
            _mainViewModel.ResetPage();
            await _mainViewModel.LoadExpectativasAsync(_mainViewModel._filter);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void NextPageClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await _mainViewModel.NextPageAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void PreviousPageClick(object sender, RoutedEventArgs e)
    {
        try
        {
            await _mainViewModel.PreviousPageAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}