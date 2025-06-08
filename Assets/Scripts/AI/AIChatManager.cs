using OpenAI;
using System.Collections.Generic;
using UnityEngine;

public class AIChatManager : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();
    public SpeakerProfile npcSpeaker;

    public async void GetAIResponse(SpeakerProfile npcSpeakerProfile)
    {
        if (npcSpeaker == null)
        {
            Debug.LogWarning("SpeakerProfile is null. Can't generate a response.");
            return;
        }

        // Grab the chat log from the MessageManager
        string chatLogPrompt = GetMessageLogPrompt();

        // Build the system prompt from the SpeakerProfile PLUS the conversation log
        string systemPrompt =
            $"You are {npcSpeaker.speakerName}. Backstory: {npcSpeaker.speakerBackStory}. " +
            $"Your personality you should always be: {npcSpeaker.speakerPersonality}. " +
            $"Your acknowledge is {npcSpeaker.speakerAcknowledge}. " +
            $"Stay in character always.\n\n" +
            $"Here is the conversation so far:\n{chatLogPrompt}";

        // Create the chat messages
        List<ChatMessage> chatMessages = new List<ChatMessage>
        {
            new ChatMessage()
            {
                Role = "system",
                Content = systemPrompt
            }
        };

        Debug.Log("🧠 Sending combined system prompt + chat log to GPT...");

        try
        {
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-4.1-nano",
                Messages = chatMessages
            });

            // Grab the response
            string aiResponse = completionResponse.Choices[0].Message.Content.Trim();
            Debug.Log($"🤖 AI ({npcSpeaker.speakerName}) responded: {aiResponse}");

            // Add the AI’s response back to the MessageManager
            MessageManager.Instance.SendMessageFromSpeaker(aiResponse, npcSpeaker);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error getting AI response: {e.Message}");
        }
    }

    private string GetMessageLogPrompt()
    {
        if (MessageManager.Instance == null)
        {
            Debug.LogWarning("⚠️ MessageManager instance not found!");
            return "";
        }

        List<Message> messageList = MessageManager.Instance.GetMessageList();

        if (messageList == null || messageList.Count == 0)
        {
            Debug.LogWarning("⚠️ No messages in the list!");
            return "";
        }

        string prompt = "";

        foreach (Message msg in messageList)
        {
            prompt += $"{msg.Speaker.speakerName}: {msg.RawText}\n";
        }

        return prompt.Trim(); // Remove any extra trailing newlines
    }
}
