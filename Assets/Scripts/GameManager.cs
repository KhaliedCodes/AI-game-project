using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LLMUnity;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    public GameObject shield;
    public GameObject laser;
    public LLMCharacter llmCharacter;
    public static GameManager Instance;
    private List<string> skills = new() { "Shield", "Laser", "Red", "Blue" };
    private static AudioClip clip;
    private static byte[] bytes;
    private static bool recording;
    public static string textToSpeech;
    private const string API_STT_URL = "https://router.huggingface.co/hf-inference/models/openai/whisper-large-v3";
    public static string selectedEnemyType = "RedEnemy";
    [SerializeField] private Text text;
    [SerializeField] private MyGeminiAPI geminiAPI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // playerText.onSubmit.AddListener(onInputFieldSubmit);
        // playerText.Select();
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);

    }
    void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    string ConstructSkillPrompt(string message)
    {
        string prompt = "Which of the following choices matches best the input?\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string skill in skills) prompt += $"{skill}\n";
        prompt += "\nAnswer directly with the choice";
        return prompt;
    }

    public async void onInputFieldSubmit(string message)
    {
        geminiAPI.InputPrompt = message;
        await geminiAPI.SendPrompt();
        // string skill = await llmCharacter.Chat(ConstructSkillPrompt(message)); text.text = skill;
        switch (geminiAPI.ResponseAction)
        {
            case "Laser":
                SkillsFunctions.activate(laser);
                StartCoroutine(deactivate(laser));
                break;
            case "Shield":
                SkillsFunctions.activate(shield);
                StartCoroutine(deactivate(shield));
                break;
            case "Red":
                selectedEnemyType = "RedEnemy";
                break;
            case "Blue":
                selectedEnemyType = "BlueEnemy";
                break;
        }
    }

    private IEnumerator deactivate(GameObject skill)
    {
        yield return new WaitForSeconds(3f);
        SkillsFunctions.deactivate(skill);
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter.remote && llmCharacter.llm != null && llmCharacter.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }


    public static void StartRecording()
    {
        Debug.Log("Recording...");
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    public async void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        await ConvertSpeechToText();
    }

    private static byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }


    public async Task ConvertSpeechToText()
    {
        // string body = bytes;
        try
        {
            var wwwForm = new WWWForm();
            wwwForm.AddBinaryData("image", bytes, "imagedata.raw");
            // Create and send the web request
            using UnityWebRequest request = UnityWebRequest.Post(API_STT_URL, "", "text/html");
            request.uploadHandler = new UploadHandlerRaw(bytes)
            {
                contentType = "text/html"
            };

            request.SetRequestHeader("Authorization", "Bearer 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");
            await request.SendWebRequest();


            // Check if the request was successful
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                return;
            }

            // Parse the JSON response into our ChuckNorrisJoke class
            string jsonResponse = request.downloadHandler.text;
            Debug.Log(jsonResponse);
            // var joke = JsonUtility.FromJson<ChuckNorrisJoke>("{\"generated_text\":" + jsonResponse + "}");
            var responseText = JsonConvert.DeserializeObject<SpeechRecognitionResponse>(jsonResponse);

            // Display the joke text in the UI
            textToSpeech = responseText.text;
            onInputFieldSubmit(responseText.text);
        }
        catch (Exception e)
        {
            // Handle any errors that occurred during the process
            Debug.LogError($"Error fetching: {e.Message}");
            text.text = "Failed to fetch";
        }
        finally
        {
            // await SendToChatGeneration();
        }
    }

}
