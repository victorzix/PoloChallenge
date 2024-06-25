using System.Text;
using System.Text.RegularExpressions;
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

                string startDate = StartDateTextBox.Text;
                string endDate = EndDateTextBox.Text;

                string startErrorMessage = string.Empty;
                string endErrorMessage = string.Empty;

                bool isStartDateValid = ValidateDate(startDate, out startErrorMessage);
                bool isEndDateValid = ValidateDate(endDate, out endErrorMessage);

                if (!isStartDateValid || !isEndDateValid)
                {
                    ErrorMessageTextBlock.Text = $"{startErrorMessage} {endErrorMessage}";
                    return;
                }

                _mainViewModel.ResetPage();
                await _mainViewModel.LoadExpectativasAsync(_mainViewModel._filter, startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateDate(string date, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(date))
                return true;

            if (!DateTime.TryParseExact(date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                errorMessage = "Data inválida. ";
                return false;
            }

            if (parsedDate > DateTime.Today)
            {
                errorMessage = "Data não pode ser futura. ";
                return false;
            }

            return true;
        }

        private void DateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);

            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                string text = textBox.Text;

                if (e.Text.Length > 0)
                {
                    text += e.Text;
                }

                if (text.Length == 2 || text.Length == 5)
                {
                    textBox.Text = text + "/";
                    textBox.CaretIndex = textBox.Text.Length;
                }
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); // Somente números
            return !regex.IsMatch(text);
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