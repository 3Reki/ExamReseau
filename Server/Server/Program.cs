using System.Net.Sockets;
using NetworkServer;

var server = new TcpSampleServer();
await server.Run();

namespace NetworkServer
{
    internal class TcpSampleServer
    {        
        private Dictionary<TcpClient, StreamWriter> _clients = new ();

        public async Task Run()
        {
            var server = TcpListener.Create(666);
            server.Start();

            while(true)
            {
                var client = await server.AcceptTcpClientAsync();
                _ = Serve(client);
            }
        }

        private async Task Serve(TcpClient client)
        {    
            try
            {
                using (client)
                {
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream, leaveOpen: true);
                    using var writer = new StreamWriter(stream, leaveOpen: true);
                    _clients.Add(client, writer);
                    Console.WriteLine("Client connected");


                    var nextLine = await reader.ReadLineAsync();
                    while (nextLine != null)
                    {
                        Console.WriteLine(nextLine.ToString());
                        foreach (var kvp in _clients.Where(kvp => kvp.Key != client))
                        {
                            
                            kvp.Value.WriteLine(nextLine);
                            await kvp.Value.FlushAsync();
                        }

                        nextLine = await reader.ReadLineAsync();
                    }
                }                
            }    
            catch(Exception)
            {
                Console.WriteLine("Wtf ça marche pas");
            }
            finally
            {
                _clients.Remove(client);
            }
        }
    }
}