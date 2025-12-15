using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using presupuestoBasadoAPI.Interfaces;

namespace presupuestoBasadoAPI.Services
{
    public class IAService : IIAService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public IAService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> ConvertirAPositivoAsync(string textoBase, string nivel)
        {
            var apiKey = _config["OpenAI:ApiKey"];

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var prompt = $@"
Convierte el siguiente texto en un enunciado POSITIVO
para el Árbol de Objetivos del Marco Lógico.

Nivel del árbol: {nivel}

Instrucciones:
- Usar lenguaje institucional y formal
- Redactar como estado logrado
- En tercera persona
- No usar negaciones
- Una sola oración clara
- No enumerar ni explicar
- No usar viñetas

Texto base:
""{textoBase}""
";


            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.3
            };

            var response = await _http.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            );

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!
                .Trim();
        }
    }
}
