using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public string ip;
    public string username;
    
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TextMeshProUGUI textPrefab;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int port;

    private StreamReader _reader;
    private StreamWriter _writer;
    private TcpClient _client;
    private NetworkStream _stream;
    private bool isConnected;

    private void OnDestroy()
    {
        _client?.Close();
        _client?.Dispose();
    }

    public new async void SendMessage(string text)
    {
        if (!isConnected || string.IsNullOrEmpty(text))
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

        if (_client != null)
        {
            _client.Close();
            foreach (Transform elem in scrollViewContent)
            {
                Destroy(elem.gameObject);
            }
        }
        
        try
        {
            _client = new TcpClient();

            WriteText("Attempting connection to " + ip + "...");
            await _client.ConnectAsync(ip, port);
            WriteText("Connected to " + ip + ".");
        
            _stream = _client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);

            _=DisplayLines();
            isConnected = true;
        
            _writer.WriteLine(username);
            await _writer.FlushAsync();
        }
        catch (Exception e)
        {
            WriteText("Failed to connect.");
            throw;
        }
    }

    public void WriteText(string text)
    {
        bool shouldScroll = scrollRect.verticalNormalizedPosition < 0.005f;
        TextMeshProUGUI newMessage = Instantiate(textPrefab, scrollViewContent);
        newMessage.text = text;
        if (shouldScroll)
        {
            StartCoroutine(AutoScroll());
        }
    }

    private readonly WaitForEndOfFrame waitForEndOfFrame = new();

    private IEnumerator AutoScroll()
    {
        yield return waitForEndOfFrame;
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
