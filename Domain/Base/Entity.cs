using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Base
{
    [Serializable]
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        public DateTime UpdatedAt { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
