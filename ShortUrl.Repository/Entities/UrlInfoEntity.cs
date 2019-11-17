using System;
using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Repository.Entities
{
    public class UrlInfoEntity : BaseEntity
    {        
        [Required]
        public string OriginalUrl { get; set; }

        [Required]
        [MinLength(3)]
        public string ShortCode { get; set; }
               
        public long UrlHits { get; set; }
    }
}
