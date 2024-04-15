// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;
using conexion;
using conexion.model;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string url = "https://dlcdn.apache.org/httpd/httpd-2.4.59.tar.bz2";
        var pingDestination = "8.8.8.8";
        var speedTestCommand = @"c:\speedtest\speedtest";

        var currentArg = "";
        foreach(var item in args)
        {
            if (item.StartsWith("-"))
            {
                currentArg = item;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentArg))
                {
                    switch (currentArg)
                    {
                        case "-d":
                            url = item.Trim();
                            break;
                        case "-p":
                            pingDestination = item.Trim();
                            break;
                        case "-s":
                            speedTestCommand = item.Trim();
                            break;
                    }
                }
            }
        }

        ConnectionToolDownload downloader = new ConnectionToolDownload();
        ConnectionToolPing ping = new ConnectionToolPing();

        TimeSpan downloadTime = new TimeSpan();
        SpeedTestResult speedTestResult = new SpeedTestResult();

        var allTaskDone = false;      

        Task pingTask = Task.Run(async () =>
        {
            Console.WriteLine("Ping started..");
            while (!allTaskDone)
            {
                await ping.Ping(pingDestination);
            }
        });


        Task DownkloadTask = Task.Run(async () =>
        {
            Console.WriteLine("Download started..");
            downloadTime = await downloader.Download(url);
            allTaskDone = true;
        }); 

        Task SpeedTest = Task.Run(async () =>
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $@"/c {speedTestCommand} -f json-pretty",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        //UseShellExecute = false,
                        //CreateNoWindow = true
                    }
                };

                process.Start();
                Console.WriteLine("SpeedTest started..");

                var result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine(error);
                }
                else
                {
                    speedTestResult = JsonSerializer.Deserialize<SpeedTestResult>(result, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to execute speedtest: " + ex.ToString());
            }

        });

        Console.WriteLine($"Waiting for all tasks to finish..");
        await Task.WhenAll(DownkloadTask, pingTask, SpeedTest);

        if (downloadTime.Seconds < 500){
            Console.WriteLine($"Downloaded from {url} in {downloadTime.TotalSeconds} seconds");
        }
        else{
            Console.WriteLine($"Downloaded from {url} in {downloadTime.Minutes} minutes");
        }
        Console.WriteLine($"Ping {ping.Result.Where(n=>n).Count()}/{ping.Result.Count()} - { (ping.Result.Where(n=>n).Count()/ping.Result.Count()) * 100}%");
        Console.WriteLine($" Ping Latency: {speedTestResult.Ping.Latency}");
        Console.WriteLine($" Download: {speedTestResult.Download.Bandwidth / 125000} Mbps");
        Console.WriteLine($" Upload: {speedTestResult.Upload.Bandwidth / 125000} Mbps");
        Console.WriteLine($" Url Result: {speedTestResult.Result.Url}");
    }
}