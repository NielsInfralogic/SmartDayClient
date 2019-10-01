using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDayClient.Models
{
    public class Result
    {
        public Entity entity { get; set; }
        public string backendService { get; set; } = "";
        public bool hasError { get; set; } = false;
        public string errorMessage { get; set; } = "";
        public Result()
        {
            entity = new Entity();
        }
        
    }

    public class Entity
    {
        public string id { get; set; } = "";
        public string externalId { get; set; } = "";
    }
}
