using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagment.Models
{
    public class Group
    {
        [Key]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(300)")]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        public ICollection<Client> Clients { get; set; }
    }
}
