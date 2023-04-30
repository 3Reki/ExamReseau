using TMPro;
using UnityEngine;

public class ConnectionPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TextMeshProUGUI ipPlaceholder;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Client client;

    public void Connect()
    {
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            return;
        }
        client.ip = ipInputField.text == "" ? ipPlaceholder.text : ipInputField.text;
        client.username = usernameInputField.text;
        client.Init();
    }
}
