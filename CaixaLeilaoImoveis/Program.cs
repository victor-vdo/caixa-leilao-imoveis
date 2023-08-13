using System.Net.Http.Headers;
using HtmlAgilityPack;

Console.WriteLine("Hello, World!");

// URL da página ASP que você deseja acessar
string url = "https://venda-imoveis.caixa.gov.br/sistema/carregaListaCidades.asp";

using (HttpClient client = new HttpClient())
{
    try
    {
        // Dados a serem enviados como multipart/form-data
        HttpContent content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("cmb_estado", "MG"),
        });

        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        // Faz uma requisição POST para a página ASP
        HttpResponseMessage response = await client.PostAsync(url, content);


        // Verifica se a resposta foi bem-sucedida
        if (response.IsSuccessStatusCode)
        {
            // Lê o conteúdo da resposta como string
            string responseContent = await response.Content.ReadAsStringAsync();
            responseContent = responseContent.Replace("</option>", string.Empty);

            var doc = new HtmlDocument();
            doc.LoadHtml(responseContent);
            var options = doc.DocumentNode.SelectNodes("//option");

            List<string> lista = options
           .Where(option => !string.IsNullOrWhiteSpace(option.InnerText) && option.InnerText != "Selecione")
           .Select(option => option.InnerText)
           .ToList();

            lista.ForEach(Console.WriteLine);
        }
        else
        {
            Console.WriteLine($"Status da requisição: {response.StatusCode}\n");
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            string responseContent = await contentTask;
            Console.WriteLine($"Mensagem da requisição:\n {responseContent}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}