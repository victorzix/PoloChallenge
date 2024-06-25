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
            await _mainViewModel.LoadExpectativasAsync();
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
    
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedValue = selectedItem.Content.ToString();
                MessageBox.Show($"Indicador selecionado: {selectedValue}", "Seleção de Indicador", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}