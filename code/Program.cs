using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main()
    {
        // 文字化け対策（UTF-8設定）
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== API Tester (Auto-Protocol Support) ===");

        while (true)
        {
            Console.WriteLine("\nEnter URL (or 'exit' to quit):");
            Console.Write("> ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit") break;

            // プロトコルの自動補完 (http/httpsがなければhttpsを付与)
            string url = input.Trim();
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            try
            {
                // リクエスト実行
                var response = await client.GetAsync(url);
                Console.WriteLine($"Status: {(int)response.StatusCode} ({response.StatusCode})");

                var body = await response.Content.ReadAsStringAsync();

                // レスポンスがJSONかプレーンテキストか判定して表示
                try
                {
                    using var jDoc = JsonDocument.Parse(body);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    var formattedJson = JsonSerializer.Serialize(jDoc, options);
                    Console.WriteLine("--- Response (JSON) ---");
                    Console.WriteLine(formattedJson);
                }
                catch (JsonException)
                {
                    // JSONでない場合はそのまま表示
                    Console.WriteLine("--- Response (Text) ---");
                    Console.WriteLine(body.Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }
    }
}
