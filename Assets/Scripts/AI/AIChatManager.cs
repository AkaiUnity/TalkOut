using OpenAI;
using System.Collections.Generic;
using UnityEngine;

public class AIChatManager : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();
    public SpeakerProfile npcSpeaker;
    [SerializeField] GameObject NPCMain;
    [SerializeField] StoryManager storyManager;

    public async void GetAIResponse(SpeakerProfile npcSpeakerProfile)
    {
        if (npcSpeaker == null)
        {
            Debug.LogWarning("SpeakerProfile is null. Can't generate a response.");
            return;
        }

        // Grab the chat log from the MessageManager
        string chatLogPrompt = GetMessageLogPrompt();

        // Get the first day log and player backstory
        string firstDayLog = storyManager.FirstDay;
        string playerBackstory = storyManager.PlayerBacklog;
        string residents = storyManager.Residents;

        // Build the system prompt with those sweet extras
        string systemPrompt =
            $"Here is the main story about what happened: {firstDayLog}\n" +
            $"Here is the list of survivors inside the vault: {residents}\n" +
            $"Player backstory: {playerBackstory}\n\n" +
            $"Here is the whole conversation so far:\n{chatLogPrompt} and you should continue this chat.\n" +
            $"You are one of the survivor, you are {npcSpeaker.speakerName}.\n" +
            $"Backstory: {npcSpeaker.speakerBackStory}.\n" +
            $"Your personality is: {npcSpeaker.speakerPersonality}.\n" +
            $"Your action that you know you are capable to do if the player ask is: {npcSpeaker.speakerAction}.\n" +
            $"Stay in character always.\n" +
            $"When writing speech text, you MUST put it inside quotation marks like this: \"Hello there.\"\n" +
            $"Keep your responses short and impactful. only 3 or 4 sentences max but aim to have a shorter like real human\n\n";

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
                Model = "gpt-4.1",
                Messages = chatMessages
            });

            // Grab the response
            string aiResponse = completionResponse.Choices[0].Message.Content.Trim();
            Debug.Log($"🤖 AI ({npcSpeaker.speakerName}) responded: {aiResponse}");

            // Add the AI’s response back to the MessageManager
            MessageManager.Instance.SendMessageFromSpeaker(aiResponse, npcSpeaker);
            NPCMain.GetComponent<ShakePortrait>().TriggerShake();
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
