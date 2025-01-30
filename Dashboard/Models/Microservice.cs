using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace Dashboard.Models
{
    public record Microservice
    {
        public string Name { get; set; }

        public Version MaxProdVersion { get; set; }
        public Version MaxStagingVersion { get; set; }

        public ConcurrentDictionary<TargetEnvironment,EMicroserviceEtat> EtatParEnvironnement  { get; set;}
        public ConcurrentDictionary<TargetEnvironment,MicroserviceVersion> VersionParEnvironnement  { get; set;}

        public Microservice()
        {
            EtatParEnvironnement = new();
            VersionParEnvironnement = new();
        }

        public string GetEtatClass(TargetEnvironment environment) => EtatParEnvironnement[environment] switch
        {
            EMicroserviceEtat.Healthy => "fas fa-check status_ok",
            EMicroserviceEtat.Instable => "fas fa-arrow-alt-circle-down status_unstable",
            EMicroserviceEtat.Unhealthy => "fas fa-arrow-alt-circle-down status_error",
            EMicroserviceEtat.Error => "fas fa-times-circle status_error",
            _ => "far fa-question-circle status_error"
        };

        public string GetVersionClass(TargetEnvironment environment)
        {
            string versionClass = string.Empty;
            var version = this.VersionParEnvironnement[environment].ToVersion();

            if(version == null)
                return string.Empty;

            if(version == MaxProdVersion)
                versionClass = "version_prod";
            else if(version == MaxStagingVersion)
                versionClass = "version_staging";
            else
            {
                switch(environment.Type)
                {
                    case ETargetEnvironmentType.Production: versionClass = "version_prod"; break;
                    case ETargetEnvironmentType.Staging: versionClass = "version_staging"; break;
                    case ETargetEnvironmentType.Development: versionClass = "version_development"; break;
                }
            }

            if(environment.Type != ETargetEnvironmentType.Production && version < MaxProdVersion)
                versionClass += " version_error";
            else if(environment.Type != ETargetEnvironmentType.Staging && environment.Type != ETargetEnvironmentType.Production && version < MaxStagingVersion)
                versionClass += " version_error";

            return versionClass;
        }
    }
}
