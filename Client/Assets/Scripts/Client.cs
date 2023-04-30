using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    public string ip;
    public string username;
    
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int port;

    private StreamReader _reader;
    private StreamWriter _writer;
    private TcpClient _client;
    private NetworkStream _stream;
    private bool isConnected;

    private void OnDestroy()
    {
        _client.Close();
        _client.Dispose();
    }

    public new async void SendMessage(string text)
    {
        if (!isConnected)
        {
            return;
        }
        
        _writer.WriteLine(text);
        _inputField.text = string.Empty;
        await _writer.FlushAsync();
    }

    public async void Init()
    {
        async Task DisplayLines()
        {
            var newLine = await _reader.ReadLineAsync();
                
            while(newLine != null)
            {
                WriteText(newLine);
                newLine = await _reader.ReadLineAsync();
            }
        }

        _client?.Close();
        _client = new TcpClient();

        await _client.ConnectAsync(ip, port);
        
        _stream = _client.GetStream();
        _reader = new StreamReader(_stream);
        _writer = new StreamWriter(_stream);

        _textField.text = "";

        _=DisplayLines();
        isConnected = true;
        
        _writer.WriteLine(username);
        await _writer.FlushAsync();
    }

    public void WriteText(string text)
    {
        _textField.text += text + "\n";
    }
}
