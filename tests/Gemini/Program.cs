using System.Net;
using System.Text.Json;
using presupuestoBasadoAPI.Dto;
using presupuestoBasadoAPI.Services;

// Ejecutar: dotnet run --project tests/Gemini/Gemini.csproj (sin red, claves ni base de datos).
var handler = new GeminiSimulado();
using var http = new HttpClient(handler);
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Gemini:ApiKey"] = "clave-simulada", ["Gemini:Model"] = "gemini-3.5-flash-lite"
}).Build();
var service = new IAService(http, config);
IAConvertirTextoDto[] nodos = [
    new() { Id = "fin", Nivel = "FIN", TextoBase = "Baja calidad de vida" },
    new() { Id = "central", Nivel = "OBJETIVO_CENTRAL", TextoBase = "Acceso insuficiente al agua" }
];
handler.Texto = """[{"id":"central","textoPositivo":"Acceso suficiente al agua"},{"id":"fin","textoPositivo":"Calidad de vida mejorada"}]""";
var result = await service.ConvertirArbolAsync(nodos);
Check(result.Count == 2 && handler.Llamadas == 1, "Un árbol debe usar una sola solicitud.");
Check(handler.Url!.Contains("gemini-3.5-flash-lite:generateContent") && !handler.Url.Contains("clave"), "URL sin secretos.");
Check(handler.Clave == "clave-simulada", "Clave en cabecera.");
using (var body = JsonDocument.Parse(handler.Body!))
{
    Check(body.RootElement.GetProperty("generationConfig").GetProperty("responseMimeType").GetString() == "application/json", "Salida estructurada.");
}
foreach (var respuesta in new[] { "[]", "null", "[null,null]", "no es JSON",
    """[{"id":"fin","textoPositivo":"Bien"},{"id":"fin","textoPositivo":"Duplicado"}]""",
    """[{"id":"fin","textoPositivo":"Bien"},{"id":"otro","textoPositivo":"Ajeno"}]""",
    """[{"id":"fin","textoPositivo":"Bien"},{"id":"central","textoPositivo":" "}]""" })
{
    handler.Texto = respuesta;
    await Falla<InvalidOperationException>(() => service.ConvertirArbolAsync(nodos));
}
handler.Finish = "MAX_TOKENS";
await Falla<InvalidOperationException>(() => service.ConvertirArbolAsync(nodos));
handler.Finish = "STOP";
handler.Status = HttpStatusCode.TooManyRequests;
try { await service.ConvertirArbolAsync(nodos); throw new Exception("Se esperaba cuota agotada."); }
catch (HttpRequestException e) { Check(e.StatusCode == HttpStatusCode.TooManyRequests, "Conservar 429."); }
handler.Status = HttpStatusCode.NotFound;
try { await service.ConvertirArbolAsync(nodos); throw new Exception("Se esperaba modelo no disponible."); }
catch (HttpRequestException e) { Check(e.StatusCode == HttpStatusCode.NotFound, "Conservar 404 para diagnosticar modelos retirados."); }
var llamadas = handler.Llamadas;
await Falla<ArgumentException>(() => service.ConvertirArbolAsync([nodos[0], nodos[0]]));
await Falla<ArgumentException>(() => service.ConvertirArbolAsync([]));
await Falla<ArgumentException>(() => service.ConvertirAPositivoAsync("texto", "INVALIDO"));
config["Gemini:ApiKey"] = "PON_AQUI_TU_CLAVE_GEMINI";
await Falla<InvalidOperationException>(() => service.ConvertirArbolAsync(nodos));
Check(handler.Llamadas == llamadas, "Validar antes de consumir cuota.");
config["Gemini:ApiKey"] = "clave-simulada";
handler.Status = HttpStatusCode.OK;
handler.Texto = """[{"id":"texto","textoPositivo":"Texto positivo"}]""";
config["Gemini:Model"] = null;
Check(await service.ConvertirAPositivoAsync("Texto negativo", "FIN") == "Texto positivo", "Compatibilidad individual.");
Check(handler.Url!.Contains("gemini-3.5-flash-lite:generateContent"), "Modelo predeterminado actualizado.");
using var cancelacion = new CancellationTokenSource();
cancelacion.Cancel();
await Falla<OperationCanceledException>(() => service.ConvertirArbolAsync(nodos, cancelacion.Token));
Console.WriteLine("OK: solicitud única, validación, respuestas incompletas, cuota, configuración, cancelación y endpoint individual.");

static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
static async Task Falla<T>(Func<Task> action) where T : Exception
{
    try { await action(); } catch (T) { return; }
    throw new Exception($"Se esperaba {typeof(T).Name}.");
}
class GeminiSimulado : HttpMessageHandler
{
    public string Texto = "[]", Finish = "STOP";
    public HttpStatusCode Status = HttpStatusCode.OK;
    public int Llamadas;
    public string? Url, Clave, Body;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        Llamadas++;
        Url = request.RequestUri!.ToString();
        Clave = request.Headers.GetValues("x-goog-api-key").Single();
        Body = await request.Content!.ReadAsStringAsync(ct);
        return new HttpResponseMessage(Status) { Content = new StringContent(JsonSerializer.Serialize(new
        {
            candidates = new[] { new { finishReason = Finish, content = new { parts = new[] { new { text = Texto } } } } }
        })) };
    }
}
