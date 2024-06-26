using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClosedXML.Excel;
using Microsoft.Win32;
using PoloChallenge.ViewModels;

namespace PoloChallenge.Views;

public partial class ExportWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public ExportWindow()
    {
        InitializeComponent();
        _mainViewModel = ((MainWindow)Application.Current.MainWindow).DataContext as MainViewModel;
    }

    private async void ExportButtonClick(object sender, RoutedEventArgs e)
    {
        string selectedFormat = (FormatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        int quantity;
        if (!int.TryParse(QuantityTextBox.Text, out quantity) || quantity < 0)
        {
            // Se o usuário não inserir nada, definimos a quantidade como 100
            quantity = 100;
        }
        else if (quantity > 100)
        {
            MessageBox.Show("Por favor, insira um valor válido entre 0 e 100 no campo de quantidade.", "Erro",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (selectedFormat == "CSV")
        {
            ExportCsvClick(quantity);
        }
        else if (selectedFormat == "XLSX")
        {
            ExportXlsxClick(quantity);
        }

        this.Close();
    }

    private string EscapeCsvValue(object value)
    {
        if (value == null) return string.Empty;
        string output = value.ToString();
        if (output.Contains(",") || output.Contains("\"") || output.Contains("\n"))
        {
            output = $"\"{output.Replace("\"", "\"\"")}\"";
        }

        return output;
    }

    private void ExportCsvClick(int quantity)
    {
        try
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                FileName = "expectativas.csv"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    var header = string.Join(",",
                        "Indicador",
                        "Data",
                        "Data Referência",
                        "Média",
                        "Mediana",
                        "Desvio Padrão",
                        "Mínimo",
                        "Máximo",
                        "N° de Respondentes",
                        "Base de Cálculo");

                    writer.WriteLine(header);

                    for (int i = 0; i < Math.Min(quantity, _mainViewModel.Expectativas.Count); i++)
                    {
                        var expectativa = _mainViewModel.Expectativas[i];
                        var line = string.Join(",",
                            EscapeCsvValue(expectativa.Indicador),
                            EscapeCsvValue(expectativa.Data),
                            EscapeCsvValue(expectativa.DataReferencia),
                            EscapeCsvValue(expectativa.Media),
                            EscapeCsvValue(expectativa.Mediana),
                            EscapeCsvValue(expectativa.DesvioPadrao),
                            EscapeCsvValue(expectativa.Minimo),
                            EscapeCsvValue(expectativa.Maximo),
                            EscapeCsvValue(expectativa.NumeroRespondentes),
                            EscapeCsvValue(expectativa.BaseCalculo));
                        writer.WriteLine(line);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao exportar dados: {ex.Message}", "Erro", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ExportXlsxClick(int quantity)
    {
        try
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = "expectativas.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Expectativas");

                    worksheet.Cell(1, 1).Value = "Indicador";
                    worksheet.Cell(1, 2).Value = "Data";
                    worksheet.Cell(1, 3).Value = "Data Referência";
                    worksheet.Cell(1, 4).Value = "Média";
                    worksheet.Cell(1, 5).Value = "Mediana";
                    worksheet.Cell(1, 6).Value = "Desvio Padrão";
                    worksheet.Cell(1, 7).Value = "Mínimo";
                    worksheet.Cell(1, 8).Value = "Máximo";
                    worksheet.Cell(1, 9).Value = "N° de Respondentes";
                    worksheet.Cell(1, 10).Value = "Base de Cálculo";

                    for (int i = 0; i < Math.Min(quantity, _mainViewModel.Expectativas.Count); i++)
                    {
                        var expectativa = _mainViewModel.Expectativas[i];
                        worksheet.Cell(i + 2, 1).Value = expectativa.Indicador;
                        worksheet.Cell(i + 2, 2).Value = expectativa.Data;
                        worksheet.Cell(i + 2, 3).Value = expectativa.DataReferencia;
                        worksheet.Cell(i + 2, 4).Value = expectativa.Media;
                        worksheet.Cell(i + 2, 5).Value = expectativa.Mediana;
                        worksheet.Cell(i + 2, 6).Value = expectativa.DesvioPadrao;
                        worksheet.Cell(i + 2, 7).Value = expectativa.Minimo;
                        worksheet.Cell(i + 2, 8).Value = expectativa.Maximo;
                        worksheet.Cell(i + 2, 9).Value = expectativa.NumeroRespondentes;
                        worksheet.Cell(i + 2, 10).Value = expectativa.BaseCalculo;
                    }

                    workbook.SaveAs(saveFileDialog.FileName);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao exportar dados: {ex.Message}", "Erro", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        int value;
        if (int.TryParse(e.Text, out value))
        {
            if (value < 0 || value > 100)
            {
                e.Handled = true;
            }
        }
        else
        {
            e.Handled = true;
        }
    }
}