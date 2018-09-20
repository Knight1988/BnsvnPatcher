namespace bnsvn_dat.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Info")]
    public partial class Info
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int InfoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; }

        public bool IsOutdated { get; set; }
    }
}
