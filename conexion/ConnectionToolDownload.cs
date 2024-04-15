using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

public class ConnectionToolDownload
{
    public async Task<TimeSpan> Download(string url)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            stopwatch.Stop();
            
            if (response.IsSuccessStatusCode)
            {
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                Console.WriteLine($"Downloaded from {url} in {stopwatch.Elapsed}");
                return (stopwatch.Elapsed);
            }
            else
            {
                Console.WriteLine($"Failed to download from {url}. Status code: {response.StatusCode}");
                return (stopwatch.Elapsed);
            }
        }
    }
}