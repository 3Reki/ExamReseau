﻿using System.Net.Sockets;
using NetworkServer;

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
        private Dictionary<CustomClient, StreamWriter> _clients = new ();

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
                    CustomClient tmpClient = new CustomClient(client);
                    _clients.Add(tmpClient, writer);
                    Console.WriteLine("Client connected");
                    while(string.IsNullOrEmpty(tmpClient.userName))
                    {
                        var tmpUserName = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(tmpUserName))
                        {
                            tmpClient.userName = tmpUserName;
                        }
                    }

                    var nextLine = await reader.ReadLineAsync();
                    while (nextLine != null)
                    {
                        Console.WriteLine(nextLine.ToString());
                        foreach (var kvp in _clients.Where(kvp => kvp.Key != tmpClient))
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