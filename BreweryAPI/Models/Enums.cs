namespace BreweryAPI.Models
{
    public enum SortOption
    {
        Name,
        City,
        Distance
    }

    public enum DataSource
    {
        Database,
        Api
    }

    public static class Constants
    {
        public static class Cache
        {
            public const string AllBreweriesKey = "all-breweries";
            public const string BreweryPrefix = "brewery-";
            public const int AllBreweriesCacheMinutes = 10;
            public const int IndividualBreweryCacheMinutes = 5;
        }

        public static class Pagination
        {
            public const int DefaultPage = 1;
            public const int DefaultPageSize = 20;
            public const int MaxPageSize = 200;
            public const int MinPageSize = 1;
        }

        public static class Validation
        {
            public const int MinQueryLength = 2;
            public const int MaxUsernameLength = 50;
            public const int MaxPasswordLength = 100;
        }

        public static class Api
        {
            public const string BaseUrl = "https://api.openbrewerydb.org/v1/";
            public const string UserAgent = "BreweryAPI/1.0";
            public const int TimeoutSeconds = 30;
            public const int MaxBreweriesPerPage = 200;
        }

        public static class Http
        {
            public const int RetryCount = 3;
            public const int CircuitBreakerThreshold = 5;
            public const int CircuitBreakerTimeoutSeconds = 30;
        }

        public static class Security
        {
            public const int JwtExpirationHours = 2;
        }

        public static class ErrorMessages
        {
            public const string RequestBodyRequired = "Request body is required";
            public const string ValidationFailed = "Validation failed";
            public const string InvalidCredentials = "Invalid credentials";
            public const string ServerConfigurationError = "Server configuration error";
            public const string JwtConfigurationError = "JWT configuration error";
            public const string InvalidJwtConfiguration = "Invalid JWT configuration";
            public const string QueryCannotBeEmpty = "Query cannot be empty";
            public const string QueryTooShort = "Query must be at least 2 characters long";
            public const string BreweryIdRequired = "Brewery ID is required";
            public const string BreweryNotFound = "Brewery not found";
            public const string InvalidSortByParameter = "Invalid sortBy parameter. Valid options are: name, city, distance";
            public const string CoordinatesRequiredForDistance = "userLat and userLng are required when sorting by distance";
        }

        public static class Configuration
        {
            public const string DataSourceKey = "DataSource";
            public const string DefaultDataSource = "Database";
            public const string DefaultUserSection = "DefaultUser";
            public const string JwtSettingsSection = "JwtSettings";
            public const string UsernameKey = "Username";
            public const string PasswordKey = "Password";
            public const string SecretKey = "SecretKey";
            public const string IssuerKey = "Issuer";
            public const string AudienceKey = "Audience";
            public const string CorsOriginsKey = "CorsOrigins";
        }

        public static class Cors
        {
            public const string DefaultPolicyName = "DefaultPolicy";
            public const string DevelopmentOrigins = "http://localhost:3000,http://localhost:3001,http://localhost:8080,https://localhost:3000,https://localhost:3001,https://localhost:8080";
            public const string ProductionOrigins = "*"; // Configure per environment
        }
    }
}
