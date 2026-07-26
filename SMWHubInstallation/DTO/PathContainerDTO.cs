using System.Text.Json;
using System.Text.Json.Serialization;

namespace SMWHubInstallation.DTO
{
    public class PathContainerDTO
    {
        [JsonRequired]
        [JsonInclude]
        public required string FolderConfigPath { get; set; }
        [JsonRequired]
        [JsonInclude]
        public required string SpriteListPath {  get; init; }
        [JsonRequired]
        [JsonInclude]
        public required string UberasmListPath { get; init; }
        [JsonRequired]
        [JsonInclude]
        public required string BlockListPath { get; init; }
        [JsonRequired]
        [JsonInclude]
        public required string PatchListPath { get; init; }
        public static PathContainerDTO FromJson(string json)
        {
            return JsonSerializer.Deserialize<PathContainerDTO>(json)!;
        }
        public void Save(string outputPath)
        {
            string content = JsonSerializer.Serialize(this);
            File.WriteAllText(outputPath, content);
        }
    }
}
