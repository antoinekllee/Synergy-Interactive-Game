using UnityEngine;
using UnityEngine.Events;
using Michsky.MUIP; 
using TMPro;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField = null; 
    // [SerializeField] private TextMeshProUGUI chatText = null; 

    [SerializeField][TextArea(4,6)] private string startMessage = "";

    public UnityEvent <string> onMessage = new UnityEvent <string>();

    [SerializeField] private GameObject chatTextObject = null;
    [SerializeField] private GameObject chatTextList = null;
    [SerializeField] private ScrollRect scrollRect = null;

    private void Start()
    {
        // chatText.text = startMessage;

        // Remove all children of chatTextList
        foreach (Transform child in chatTextList.transform)
            Destroy(child.gameObject);
        
        // Add the start message to the chat
        TextMeshProUGUI chatText = Instantiate(chatTextObject, chatTextList.transform).GetComponent<TextMeshProUGUI>();
        chatText.text = startMessage;
        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public void UpdateChat(string result) // get response from chatgpt
    {
        // message += "\n" + result;
        // chatText.text = message;
        // Debug.Log ("SETTING TEXT TO " + result); 

        string newMessage = "<b>Assistant</b>\n" + result;

        TextMeshProUGUI chatText = Instantiate(chatTextObject, chatTextList.transform).GetComponent<TextMeshProUGUI>();
        chatText.text = newMessage;
        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public void Send() // for user to send message to chatgpt
    {
        if (inputField.text == "") 
        {
            Debug.Log("No message to send");
            return;
        }
        
        string userMessage = "<b>You</b>\n" + inputField.text;
        GameObject chatText = Instantiate(chatTextObject, chatTextList.transform);
        chatText.GetComponent<TextMeshProUGUI>().text = userMessage;
        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = new Vector2(0, 0);

        onMessage.Invoke(inputField.text); 
        inputField.text = "";
    }
}
