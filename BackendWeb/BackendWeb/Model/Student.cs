using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BackendWeb.Model
{
    public class Student
    {
        [Key]
        public int Id_student { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Surname { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string Name { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Patronymic { get; set; }
        
        [Column(TypeName ="date")]
        public DateTime Date_birth { get; set; }
        
        [Column(TypeName ="varchar(50)")]
        public string Place_residence { get; set; }
        
        [Column(TypeName ="varchar(25)")]
        public string Telephone { get; set;}

        [Column(TypeName ="varchar(20)")]
        public string Email { get; set; }
        
        [Column(TypeName ="varchar(60)")]
        public string FullName_father { get; set; }
        
        [Column(TypeName ="varchar(25)")]
        public string Father_telephone { get; set; }

        [Column(TypeName = "varchar(60)")]
        public string FullName_mother { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Mother_telephone { get; set; }
    }
}
