using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    [SerializeField] private InputField _inputField;
    
    private StreamReader _reader;
    private StreamWriter _writer;
    private TcpClient _client;
    private NetworkStream _stream;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    

    public void SendMessage(string text)
    {
        DateTime dat_time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        _writer.WriteLine($"{dat_time.AddSeconds(DateTime.Now.Second)} : {text}");
        //await _writer.FlushAsync();
    }

    public async void Init()
    {
        _client = new TcpClient();
        await _client.ConnectAsync("10.51.0.177", 666);
        
        _stream = _client.GetStream();
        _reader = new StreamReader(_stream);
        _writer = new StreamWriter(_stream);
    }
    
    public async Task Run()
    {
        // Connexion
            
        async Task DisplayLines()
        {
            var newLine = await _reader.ReadLineAsync();
                
            while(newLine != null)
            {
                Console.WriteLine(newLine);
                newLine = await _reader.ReadLineAsync();
            }
        }

        _=DisplayLines();
        Console.WriteLine("Type a line to send to the server");
        while (true)
        {
            var lineToSend = Console.ReadLine();
            //Ligne envoyée
                
            //_writer.WriteLine($"{dat_time.AddSeconds(DateTime.Now.Second)} : {lineToSend}");
            await _writer.FlushAsync();
        }
    }
}
