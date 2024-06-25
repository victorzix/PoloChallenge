using System.Net.Http;
using System.Text.Json;
using PoloChallenge.Models;

namespace PoloChallenge.Services;

public interface IApiService
{
    Task<IEnumerable<ExpectativaMercadoMensal>> GetExpectativasAsync(string skip);
}

public class ApiServices : IApiService
{
    private static readonly HttpClient Client = new();

    public ApiServices()
    {
        Client.BaseAddress = new Uri("https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/");
    }

    public async Task<IEnumerable<ExpectativaMercadoMensal>> GetExpectativasAsync(string skip = "0")
    {
        var response = await Client.GetStringAsync($"ExpectativaMercadoMensais?%24skip={skip}&%24top=10");
        var root = JsonSerializer.Deserialize<ExpectativaMercadoMensalRoot>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return root?.Value ?? new List<ExpectativaMercadoMensal>();
    }
    
    public class ExpectativaMercadoMensalRoot
    {
        public IEnumerable<ExpectativaMercadoMensal> Value { get; set; }
    }
    
    public async Task<IEnumerable<ExpectativaMercadoMensal>> GetExpectativasPeloIndicador(string skip = "0")
    {
        
    }
}