using ClassLibrary.Entities;
using System.Text.Json;

namespace ClassLibrary.Services
{
    public class TextToSpeechService(IHttpClientFactory httpClientFactory, StorageAccountBlobService storageAccountBlobService)
    {
        private HttpClient GetClient(string clientName)
        {
            return httpClientFactory.CreateClient(clientName ?? "");
        }
        public async Task CreateJob(string jobId, string speechText)
        {
            var client = GetClient("speechService");
            string json = $$"""
                                {
                    "synthesisConfig": {
                        "voice": "en-US-JennyMultilingualNeural"
                    },
                    "inputKind": "plainText",
                    "inputs": [
                        {
                            "content": "{{speechText}}"
                        }
                    ],
                    "avatarConfig": {
                        "customized": false,
                        "talkingAvatarCharacter": "lisa",
                        "talkingAvatarStyle":"casual-sitting",
                        "videoFormat": "mp4",
                        "videoCodec": "h264",
                        "subtitleType": "soft_embedded",
                        "backgroundColor": "#FFFFFFFF"
                    }
                }
                """;
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/avatar/batchsyntheses/{jobId}?api-version=2024-08-01", content);
            response.EnsureSuccessStatusCode();
        }
        public async Task ProcessSpeechToText(string jobId, string speechText)
        {
            await CreateJob(jobId, speechText);
            var getJobClient = GetClient("speechService");
            string status = "";
            TextToSpeechResponse textToSpeechResponse;
            do
            {
                await Task.Delay(5000); // Wait for 5 seconds before checking the status again
                var response = await getJobClient.GetAsync($"avatar/batchsyntheses/{jobId}?api-version=2024-08-01");
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                textToSpeechResponse = JsonSerializer.Deserialize<TextToSpeechResponse>(responseContent, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                status = textToSpeechResponse.Status;
            } while (status != "Succeeded" && status != "Failed");
            string videoUrl = textToSpeechResponse.Outputs.Result;
            var videoClient = GetClient(null);
            var videoResponse = await videoClient.GetAsync(videoUrl);
            videoResponse.EnsureSuccessStatusCode();
            using var videoStream = await videoResponse.Content.ReadAsStreamAsync();
            await storageAccountBlobService.UplaodVideo(jobId, videoStream);
        }
    }
}
