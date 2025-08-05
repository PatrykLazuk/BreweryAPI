using System.Text.Json.Serialization;

namespace BreweryAPI.Models
{
    public class Brewery
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("brewery_type")]
        public string? BreweryType { get; set; }

        [JsonPropertyName("address_1")]
        public string? Address1 { get; set; }

        [JsonPropertyName("address_2")]
        public string? Address2 { get; set; }

        [JsonPropertyName("address_3")]
        public string? Address3 { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state_province")]
        public string? StateProvince { get; set; }

        [JsonPropertyName("postal_code")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("website_url")]
        public string? WebsiteUrl { get; set; }

        // Helper properties if needed
        [JsonIgnore]
        public double? LatitudeValue => Latitude;

        [JsonIgnore]
        public double? LongitudeValue => Longitude;
    }
}