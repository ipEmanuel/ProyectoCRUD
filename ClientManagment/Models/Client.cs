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

        [Column(TypeName = "DateTime2")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        //[Required(ErrorMessage = "Este campo obligatorio")]
        [Column(TypeName = "varchar(300)")]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Este campo obligatorio")]
        [DisplayName("No deseado?")]
        public bool isNotWished { get; set; } //0 es deseado y 1 no deseado

        [NotMapped]
        public bool State { get; set; } //0 es deseado y 1 no deseado

        [DisplayName("Imagen")]
        public string ImagePath { get; set; }

        [DisplayName("Patente")]
        [Column(TypeName = "varchar(12)")]
        public string Patent { get; set; }

        public ICollection<Group> Groups { get; set; }

    }
}
