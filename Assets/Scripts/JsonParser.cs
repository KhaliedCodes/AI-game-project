using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JsonParser : MonoBehaviour
{
    // UI Text component to display our JSON data
    [SerializeField] private TextMeshProUGUI _jsonViewText;

    // Reference to a JSON file asset
    [SerializeField] private UnityEngine.Object _jsonFile;

    void Start()
    {
        DisplayJsonValue();
    }
    // Displays raw JSON data as a string
    public void DisplayJsonData()
    {
        _jsonViewText.text = _jsonFile.ToString();
    }

    // Demonstrates how to parse and access specific values from JSON
    // In this example, we're accessing: { "user": { "firstName": "value" } }
    public void DisplayJsonValue()
    {
        JObject json = JObject.Parse(_jsonFile.ToString());
        string firstName = json["user"]["firstName"].ToString();
        _jsonViewText.text = firstName;
    }
}
