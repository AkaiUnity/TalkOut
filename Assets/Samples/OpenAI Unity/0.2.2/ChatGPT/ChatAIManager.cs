using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OpenAI
{
    public class ChatAIManager : MonoBehaviour
    {
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as you are a resident inside a survival vault.";
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, "chat_memory.json");

        private void Awake()
        {
            Debug.Log("Persistent path: " + Application.persistentDataPath);
            LoadMessages();
        }

        // Call this from any external script or keypress
        public async void AskAI(string userInput)
        {
            var newMessage = new ChatMessage
            {
                Role = "user",
                Content = messages.Count == 0 ? prompt + "\n" + userInput : userInput
            };

            messages.Add(newMessage);

            var response = await openai.CreateChatCompletion(new CreateChatCompletionRequest
            {
                Model = "gpt-4.1-nano",
                Messages = messages
            });

            if (response.Choices != null && response.Choices.Count > 0)
            {
                var reply = response.Choices[0].Message;
                reply.Content = reply.Content.Trim();
                messages.Add(reply);

                Debug.Log("AI: " + reply.Content);
                SaveMessages(); // save convo after response
            }
            else
            {
                Debug.LogWarning("AI didn't respond.");
            }
        }

        private void SaveMessages()
        {
            var json = JsonConvert.SerializeObject(messages);
            File.WriteAllText(SaveFilePath, json);
        }

        private void LoadMessages()
        {
            if (File.Exists(SaveFilePath))
            {
                var json = File.ReadAllText(SaveFilePath);
                messages = JsonConvert.DeserializeObject<List<ChatMessage>>(json);
            }
            else
            {
                messages = new List<ChatMessage>();
            }
        }
    }
}