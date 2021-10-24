using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendWeb.Model
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        
        [Column(TypeName ="varchar(25)")]
        public string Login { get; set; }
        
        [Column(TypeName = "varchar(30)")]
        public string Password { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string Email { get; set; }
        
        [Column(TypeName ="bit")]
        public bool rememberMe { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string accessLevel { get; set; }
    }
}
