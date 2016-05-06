using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrDragon.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nom d'usager")]
        [Required]
        [StringLength(50)]
        public String Username { get; set; }

        [Display(Name = "Prénom")]
        [Required]
        public String Name { get; set; }

        [Display(Name = "Nom")]
        [Required]
        public String Lastname { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public String Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et celui de confirmation ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

    }

    public class User
    {
        public int Id { get; set; }
        public String Username { get; set; }
        public String Name { get; set; }
        public String Lastname { get; set; }
        public String Password { get; set; }

        public User(UserViewModel userViewModel)
        {
            Id = userViewModel.Id;
            Username = userViewModel.Username;
            Name = userViewModel.Name;
            Lastname = userViewModel.Lastname;
            Password = userViewModel.Password;
        }
    }

    public class Users
    {

    }
}
