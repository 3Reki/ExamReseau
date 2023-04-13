using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private string ip;
    [SerializeField] private int port;
    
    private StreamReader _reader;
    private StreamWriter _writer;
    private TcpClient _client;
    private NetworkStream _stream;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public async void SendMessage(string text)
    {
        DateTime dat_time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        _writer.WriteLine($"{dat_time.AddSeconds(DateTime.Now.Second)} : {text}");
        Debug.Log("Log");
        await _writer.FlushAsync();
    }

    public async void Init()
    {
        _client = new TcpClient();
        await _client.ConnectAsync(ip, port);
        
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
