using UnityEngine;
using System.Speech.Recognition;
using System.IO;
using UnityEngine.UI;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
public class SpeechRecognitionResponse
{
    public string text;
}
public class SpeechRec : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Text text;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;
    private string textToSpeech;
    private const string API_STT_URL = "https://router.huggingface.co/hf-inference/models/openai/whisper-large-v3";
    private const string API_LLM_URL = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";


    private void Update()
    {
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            StopRecording();
        }
    }

    public void StartRecording()
    {
        Debug.Log("Recording...");
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    public void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        ConvertSpeechToText();
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
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
        }
        catch (Exception e)
        {
            // Handle any errors that occurred during the process
            Debug.LogError($"Error fetching: {e.Message}");
            text.text = "Failed to fetch";
        }
        finally
        {
            await SendToChatGeneration();
        }
    }

    public async Task SendToChatGeneration()
    {
        try
        {
            string body = $"{{\"inputs\": \" {textToSpeech} \"}}";
            // Create and send the web request
            using UnityWebRequest request = UnityWebRequest.Post(API_LLM_URL, body, "application/json");


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
            var responseText = JsonConvert.DeserializeObject<GeneratedText[]>(jsonResponse);

            // Display the joke text in the UI
            text.text = responseText.First().generated_text;
        }
        catch (Exception e)
        {
            // Handle any errors that occurred during the process
            Debug.LogError($"Error fetching: {e.Message}");
            text.text = "Failed to fetch";
        }
        finally
        {
        }
    }
}
