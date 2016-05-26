using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace OrDragon.Models
{
    public class PlayerStats
    {
        [Display(Name = "Équipe")]
        public String Alias;
        [Display(Name = "Monnaie")]
        public int Currency;
        public int Dorritos;
        public int Dew;
        [Display(Name = "Auberges")]
        public int Inn;
        [Display(Name = "Manoir")]
        public int Manor;
        [Display(Name = "Châteaux")]
        public int Castle;

        public PlayerStats()
        {
            Alias = "";
            Currency = 0;
            Dorritos = 0;
            Dew = 0;
            Inn = 0;
            Manor = 0;
            Castle = 0;
        }
        public PlayerStats(String Name, int Cash, int Chip, int Soda, int House, int Bighouse, int BiggerHouse)
        {
            if (Name == null)
                Alias = "LesDieuxGrec";
            else
                Alias = Name;
            Currency = Cash;
            Dorritos = Chip;
            Dew = Soda;
            Inn = House;
            Manor = Bighouse;
            Castle = BiggerHouse;
        }

        // initialze our Team PlayerState, (All the team's items and buildings)
        public static PlayerStats Initialize()
        {
            PlayerStats state = new PlayerStats();
            OracleConnection con = Database.GetConnection();
            OracleCommand cmd = new OracleCommand("PLAYERSPKG", con);
            cmd.CommandText = "PLAYERSPKG.GetAvoirTeam";
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter Return = new OracleParameter("monCurseur", OracleDbType.RefCursor);
            Return.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(Return);

            OracleDataReader reader = null;

            #region init Reader Avoirs
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {

            }
            #endregion
            while (reader.Read())
            {
                state.Currency = reader.GetInt32(2);
                state.Dorritos = reader.GetInt32(1);
                state.Dew = reader.GetInt32(0);
            }
            con.Close();
            cmd.CommandText = "PLAYERSPKG.GetBatimentsTeam";

            #region init reader Batiments
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {

            }
            #endregion
            while (reader.Read())
            {
                state.Inn = reader.GetInt32(0);
                state.Manor = reader.GetInt32(1);
                state.Castle = reader.GetInt32(2);
            }
                con.Close();
            return state;
        }
    }
    public class PlayersStats
    {
        public List<PlayerStats> list;

        public PlayersStats()
        {
            list = Initialize();
        }

        // Returns a list of every teams states (items and buildings)
        public static List<PlayerStats> Initialize()
        {
            List<PlayerStats> array = new List<PlayerStats>();

            OracleConnection con = Database.GetConnection();

            OracleCommand cmd = new OracleCommand("PLAYERSPKG", con);
            cmd.CommandText = "PLAYERSPKG.GetAvoirs";
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter Return = new OracleParameter("monCurseur", OracleDbType.RefCursor);
            Return.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(Return);

            OracleDataReader items = null;
            OracleDataReader buildings = null;

            #region init readers
            try
            {
                con.Open();
                items = cmd.ExecuteReader();
                cmd.CommandText = "PLAYERSPKG.GetBatiments";
                buildings = cmd.ExecuteReader();
            }
            catch (Exception e)
            {

            }
            #endregion

            bool Reading = true;
            while (Reading)
            {
                Reading = items.Read();
                Reading = buildings.Read();

                if (Reading)
                {
                    array.Add(new PlayerStats(items.GetString(0), items.GetInt32(3), items.GetInt32(2), items.GetInt32(1), buildings.GetInt32(1), buildings.GetInt32(2), buildings.GetInt32(3)));
                }
            }
            con.Close();

            return array;
        }
    }
}