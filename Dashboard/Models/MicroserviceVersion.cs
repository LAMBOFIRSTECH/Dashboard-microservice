using System;

namespace Dashboard.Models
{
    public record MicroserviceVersion : System.IComparable
    {
        public bool InError { get; set; }
        public string? VersionPrincipale { get; set; } 
        public string? VersionSnapshot { get; set; }

        public int CompareTo(object obj)
        {
            if(obj is MicroserviceVersion msVersion)
            {
                var versionCurrent = this.ToVersion();
                var versionObj = msVersion.ToVersion();

                if(versionCurrent == null)
                    return -1;

                return versionCurrent.CompareTo(versionObj);
            }

            throw new ArgumentException(nameof(obj), "Must be a MicroserviceVersion");
        }

        public System.Version ToVersion()
        {
            if(string.IsNullOrEmpty(VersionPrincipale))
                return null;

            //try
            {
                return new System.Version($"{VersionPrincipale}.{VersionSnapshot?.Replace("snapshot", "")}");
            }
            /*catch
            {
                return null;
            }*/
        }
    }
}
