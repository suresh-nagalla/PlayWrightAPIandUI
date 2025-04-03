namespace APIAutomation.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Tag
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Pet
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("category")]
        public Category? Category { get; set; }

        [JsonProperty("photoUrls")]
        public List<string> PhotoUrls { get; set; } = new List<string>();

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; } = new List<Tag>();

        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;
    }
}