using System.Net.Sockets;
using System.Text.Json;
using NetworkServer;
using static System.Net.Mime.MediaTypeNames;

var server = new TcpSampleServer();
await server.Run();

namespace NetworkServer
{

    public struct CustomClient
    {
        public CustomClient(TcpClient ptcpClient, string puserName = "")
        {
            tcpClient = ptcpClient;
            userName = puserName;
        }
        public TcpClient tcpClient { get; set; }
        public string userName { get; set; }

        public static bool operator ==(CustomClient c1, CustomClient c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CustomClient c1, CustomClient c2)
        {
            return !c1.Equals(c2);
        }
    }

    internal class TcpSampleServer
    {        
        private List<string> messages = new List<string>();
        private Dictionary<CustomClient, StreamWriter> _clients = new ();

        public async Task Run()
        {
            var server = TcpListener.Create(666);
            server.Start();
            if (File.Exists("jsonTextSave.json"))
            {
                messages = JsonUtils.ReadFromFile<List<string>>("jsonTextSave.json");
            }

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
                    CustomClient tmpClient = new CustomClient(client);
                    _clients.Add(tmpClient, writer);
                    
                    while(string.IsNullOrEmpty(tmpClient.userName))
                    {
                        var tmpUserName = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(tmpUserName))
                        {
                            tmpClient.userName = tmpUserName;
                        }
                    }
                    var message = $"{tmpClient.userName} is connected";
                    foreach (var kvp in _clients)
                    {
                        kvp.Value.WriteLine(message);
                        await kvp.Value.FlushAsync();
                    }
                    Console.WriteLine($"{tmpClient.userName} connected");
                    foreach(var tmpMessage in messages)
                    {
                        writer.WriteLine(tmpMessage);
                        await writer.FlushAsync();
                    }
                    
                    var nextLine = await reader.ReadLineAsync();
                    while (nextLine != null)
                    {
                        message = $"[{DateTime.Now.ToString(("dd/MM/yyyy"))} {DateTime.Now.ToString("HH:mm")}] {tmpClient.userName} : {nextLine}";
                        messages.Add(message);
                        if(messages.Count > 100)
                        {
                            messages.RemoveAt(0);
                        }
                        JsonUtils.WriteToFile(messages, "jsonTextSave.json");
                        
                        foreach (var kvp in _clients)
                        {
                            kvp.Value.WriteLine(message);
                            await kvp.Value.FlushAsync();
                        }
                        
                        nextLine = await reader.ReadLineAsync();
                    }
                }
            }    
            catch(Exception e)
            {
                Console.WriteLine($"Wtf ça marche pas : {e}");
            }
            finally
            {
                foreach(KeyValuePair<CustomClient, StreamWriter> entry in _clients)
                {
                    if(entry.Key.tcpClient == client)
                    {
                        _clients.Remove(entry.Key);
                    }
                }
            }
        }
    }
}