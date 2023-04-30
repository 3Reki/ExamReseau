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
    [SerializeField] private TMP_Text _textField;
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
        string message = $"{DateTime.Now.ToString(("dd/MM/yyyy"))} {DateTime.Now.ToString("HH:mm")} : {text}";
        _writer.WriteLine(message);
        WriteText(message);
        _inputField.text = string.Empty;
        await _writer.FlushAsync();
    }

    public async void Init()
    {
        _client = new TcpClient();
        await _client.ConnectAsync(ip, port);
        
        _stream = _client.GetStream();
        _reader = new StreamReader(_stream);
        _writer = new StreamWriter(_stream);

        _textField.text = "";
        
        async Task DisplayLines()
        {
            var newLine = await _reader.ReadLineAsync();
                
            while(newLine != null)
            {
                WriteText(newLine);
                newLine = await _reader.ReadLineAsync();
            }
        }
        _=DisplayLines();
    }
    
    // C'est plus utile ce truc si ?
    public async Task Run()
    {
        // Connexion
        async Task DisplayLines()
        {
            var newLine = await _reader.ReadLineAsync();
                
            while(newLine != null)
            {
                WriteText(newLine);
                newLine = await _reader.ReadLineAsync();
            }
        }
        _=DisplayLines();
        Debug.Log("Type a line to send to the server");
        while (true)
        {
            
            var lineToSend = Console.ReadLine();
            //Ligne envoyée
            _writer.WriteLine($"{DateTime.Now.ToString(("dd/mm/yyyy"))} : {lineToSend}");
            await _writer.FlushAsync();
        }
    }

    public void WriteText(string text)
    {
        _textField.text += text + "\n";
    }
}
