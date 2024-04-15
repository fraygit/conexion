// See https://aka.ms/new-console-template for more information
using System.Threading.Tasks.Dataflow;
using conexion;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        string url = "https://dlcdn.apache.org/httpd/httpd-2.4.59.tar.bz2";
        ConnectionToolDownload downloader = new ConnectionToolDownload();
        ConnectionToolPing ping = new ConnectionToolPing();

        TimeSpan downloadTime = new TimeSpan();

        var allTaskDone = false;      

        Task pingTask = Task.Run(async () =>
        {
            while (!allTaskDone)
            {
                await ping.Ping("8.8.8.8");
            }
        });


        Task DownkloadTask = Task.Run(async () =>
        {
            downloadTime = await downloader.Download(url);
            allTaskDone = true;
        }); 

        await Task.WhenAll(DownkloadTask, pingTask);
        if (downloadTime.Seconds < 500){
            Console.WriteLine($"Downloaded from {url} in {downloadTime.TotalSeconds} seconds");
        }
        else{
            Console.WriteLine($"Downloaded from {url} in {downloadTime.Minutes} minutes");
        }
        Console.WriteLine($"Ping {ping.Result.Where(n=>n).Count()}/{ping.Result.Count()} - { (ping.Result.Where(n=>n).Count()/ping.Result.Count()) * 100}%");
    }
}