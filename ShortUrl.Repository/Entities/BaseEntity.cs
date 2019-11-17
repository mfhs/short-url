using System;
using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Repository.Entities
{
    public class BaseEntity
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
        
        public DateTime ExpireDate { get; set; }

        public string CreatedBy { get; set; }
    }
}
