namespace Dashboard.Models
{
    public enum ETargetEnvironmentType
    {
        Production,
        Staging,
        Development,
    }

    public record TargetEnvironment
    {
        public string Name { get; set; } 
        public ETargetEnvironmentType Type { get; set; }
    }
}