namespace AMNHAC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Video")]
    public partial class Video
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string id { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(255)]
        public string title { get; set; }

        [StringLength(150)]
        public string author { get; set; }

        [StringLength(255)]
        public string link { get; set; }

        [Column(TypeName = "image")]
        public byte[] hinh { get; set; }
    }
}
