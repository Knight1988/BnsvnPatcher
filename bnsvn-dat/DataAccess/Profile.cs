namespace bnsvn_dat.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Profile")]
    public partial class Profile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProfileId { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Path { get; set; }

        [StringLength(50)]
        public string Version { get; set; }

        public virtual User User { get; set; }
    }
}
