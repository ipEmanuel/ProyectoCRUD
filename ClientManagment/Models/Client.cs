using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ClientManagment.Models
{
    public class Client
    {

        [Key]
        public Guid ClientId { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(50)")]
        [DisplayName("Apellido")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        //[Column(TypeName = "varchar(50)")]
        [DisplayName("DNI")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        //[Column(TypeName = "varchar(50)")]
        [DisplayName("Edad")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(300)")]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [DisplayName("Imagen")]
        public string ImagePath { get; set; }
        
    }
}
