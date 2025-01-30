using System;
using System.Collections.Generic;

namespace Dashboard.Models
{
    public class IndexModel
    {
        public List<TargetEnvironment>? Environnements { get; set; }

        public List<Microservice>? Microservices {get; set; }
    }
}
