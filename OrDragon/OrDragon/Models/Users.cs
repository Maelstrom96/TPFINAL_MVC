using Oracle.ManagedDataAccess.Client;
using OrDragon.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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

        public User()
        {

        }

        public User(UserViewModel userViewModel)
        {
            Id = userViewModel.Id;
            Username = userViewModel.Username;
            Name = userViewModel.Name;
            Lastname = userViewModel.Lastname;
            Password = Security.HashSHA1(userViewModel.Password);
        }

        public User Clone()
        {
            User user = new User();

            user.Id = this.Id;
            user.Lastname = this.Lastname;
            user.Name = this.Name;
            user.Password = this.Password;
            user.Username = this.Username;

            return user;
        }
    }

    public static class Users
    {
        /// <exception cref="">Why it's thrown.</exception>
        public static void Register(User user)
        {
            OracleConnection conn = Database.GetConnection();
            conn.Open();

            OracleCommand Oracmd = new OracleCommand("USERSPKG", conn);
            Oracmd.CommandText = "USERSPKG.REGISTER";
            Oracmd.CommandType = CommandType.StoredProcedure;

            // USERNAME
            OracleParameter OraUsername = new OracleParameter("UUSERNAME", OracleDbType.Varchar2);
            OraUsername.Value = user.Username;
            OraUsername.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraUsername);

            // PASSWORD
            OracleParameter OraPassword = new OracleParameter("UPASSWORD", OracleDbType.Varchar2);
            OraPassword.Value = user.Password;
            OraPassword.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraPassword);

            // NAME
            OracleParameter OraName = new OracleParameter("UNAME", OracleDbType.Varchar2);
            OraName.Value = user.Name;
            OraName.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraName);

            // USERNAME
            OracleParameter OraLastName = new OracleParameter("ULASTNAME", OracleDbType.Varchar2);
            OraLastName.Value = user.Lastname;
            OraLastName.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraLastName);

            try {
                Oracmd.ExecuteNonQuery();
            }
            catch (OracleException ora)
            {
                OracleExceptions.Parse(ora);
            }
            finally
            {
                conn.Close();
            }
        }

        public static User Login(User user)
        {
            User loggedinUser = new User();
            OracleConnection conn = Database.GetConnection();
            conn.Open();

            OracleCommand Oracmd = new OracleCommand("USERSPKG", conn);
            Oracmd.CommandText = "USERSPKG.AUTH";
            Oracmd.CommandType = CommandType.StoredProcedure;

            // RESULTAT
            OracleParameter OrapameResultat = new
            OracleParameter("CURUSERS", OracleDbType.RefCursor);
            OrapameResultat.Direction = ParameterDirection.ReturnValue;
            Oracmd.Parameters.Add(OrapameResultat);

            // USERNAME
            OracleParameter OraUsername = new OracleParameter("UUSERNAME", OracleDbType.Varchar2);
            OraUsername.Value = user.Username;
            OraUsername.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraUsername);

            // PASSWORD
            OracleParameter OraPassword = new OracleParameter("UPASSWORD", OracleDbType.Varchar2);
            OraPassword.Value = user.Password;
            OraPassword.Direction = ParameterDirection.Input;
            Oracmd.Parameters.Add(OraPassword);

            try
            {
                OracleDataReader Oraread = Oracmd.ExecuteReader();
                Oraread.Read();

                loggedinUser.Id = Oraread.GetInt32(0);
                loggedinUser.Username = Oraread.GetString(1);
                loggedinUser.Password = Oraread.GetString(2);
                loggedinUser.Name = Oraread.GetString(3);
                loggedinUser.Lastname = Oraread.GetString(4);
            }            catch(OracleException ex)            {
                OracleExceptions.Parse(ex);
            }            finally
            {
                conn.Close();
            }

            return loggedinUser;
        }
    }
}
