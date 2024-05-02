using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Model.EF
{
    [Table("Credential")]
    [Serializable]
    public partial class Credential
    {
        [Key]
        [Column(Order = 0)]
        public long UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string RoleID { get; set; }

    }
}
