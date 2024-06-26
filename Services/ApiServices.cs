using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using PoloChallenge.Models;

namespace PoloChallenge.Services;

public interface IApiService
{
    Task<IEnumerable<ExpectativaMercadoMensal>> GetExpectativasAsync(string skip, string filter, string startDate,
        string endDate);
}

public class ApiServices : IApiService
{
    private static readonly HttpClient Client = new();

    public ApiServices()
    {
        Client.BaseAddress = new Uri("https://olinda.bcb.gov.br/");
    }

     public async Task<IEnumerable<ExpectativaMercadoMensal>> GetExpectativasAsync(string filter, string startDate = null, string endDate = null, string skip = "0")
    {
        try
        {
            var builder = new UriBuilder(Client.BaseAddress);
            builder.Path = "olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais";
            var query = new StringBuilder();

            query.Append($"%24skip={skip}&%24top=100");

            query.Append("&%24orderby=Data%20asc");

            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filters.Add($"Indicador%20eq%20'{Uri.EscapeDataString(filter)}'");
            }

            if (!string.IsNullOrWhiteSpace(startDate))
            {
                filters.Add($"Data%20ge%20'{DateTime.Parse(startDate).ToString("yyyy-MM-dd")}'");
            }

            if (!string.IsNullOrWhiteSpace(endDate))
            {
                filters.Add($"Data%20le%20'{DateTime.Parse(endDate).ToString("yyyy-MM-dd")}'");
            }

            if (filters.Any())
            {
                query.Append("&%24filter=");
                query.Append(string.Join("%20and%20", filters));
            }

            builder.Query = query.ToString();
            string response = await Client.GetStringAsync(builder.Uri);

            var root = JsonSerializer.Deserialize<ExpectativaMercadoMensalRoot>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return root?.Value ?? new List<ExpectativaMercadoMensal>();
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show($"Erro na requisição HTTP: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return new List<ExpectativaMercadoMensal>();
        }
    }

    public class ExpectativaMercadoMensalRoot
    {
        public IEnumerable<ExpectativaMercadoMensal> Value { get; set; }
    }
}