using System.Net.Sockets;
using NetworkClient;

var client = new TcpSampleClient();
await client.Run();

namespace NetworkClient
{
    internal class TcpSampleClient
    {
        public async Task Run()
        {
            using var client = new TcpClient();
            await client.ConnectAsync("10.51.0.66", 666);

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, leaveOpen: true);
            using var writer = new StreamWriter(stream, leaveOpen: true);

            async Task DisplayLines()
            {
                var newLine = await reader.ReadLineAsync();
                while(newLine != null)
                {
                    Console.WriteLine(newLine);
                    newLine = await reader.ReadLineAsync();
                }
            }

            _=DisplayLines();
            Console.WriteLine("Type a line to send to the server");
            while (true)
            {
                var lineToSend = Console.ReadLine();

                writer.WriteLine(lineToSend);
                await writer.FlushAsync();
            }
        }
    }
}