using Oracle.ManagedDataAccess.Client;
using OrDragon.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrDragon.Models
{
    public class Question
    {
        public int Id { get; set; }
        public String Text { get; set; }
        public List<Answer> Answers { get; set; }
        public Answer GoodAnswer { get; set; }

        public Question() 
        {
            Answers = new List<Answer>();
        }

        public Question(int id, string text) : this()
        {
            Id = id;
            Text = text;
        }

        public void AddAnswer(Answer answer)
        {
            Answers.Add(answer);
            if (answer.IsGood) GoodAnswer = answer;
        }
    }

    public class Answer
    {
        public String Text { get; set; }
        public bool IsGood { get; set; }

        public Answer(String text, int isGood)
        {
            Text = text;
            if (isGood == 0) IsGood = false;
            else IsGood = true;
        }
    }

    public class Questions
    {
        List<Question> questions = new List<Question>();
        public void Import()
        {
            questions.Clear();
            OracleConnection conn = Database.GetConnection();
            conn.Open();

            OracleCommand Oracmd = new OracleCommand("QUESTIONSPKG", conn);
            Oracmd.CommandText = "QUESTIONSPKG.GetQuestionsAnswers";
            Oracmd.CommandType = CommandType.StoredProcedure;

            // RESULTAT
            OracleParameter OrapameResultat = new
            OracleParameter("SORTIE", OracleDbType.RefCursor);
            OrapameResultat.Direction = ParameterDirection.ReturnValue;
            Oracmd.Parameters.Add(OrapameResultat);

            try
            {
                OracleDataReader Oraread = Oracmd.ExecuteReader();

                int tempQuestionID = -1;
                Question currentQuestion = null;

                while(Oraread.Read())
                {
                    if (tempQuestionID == -1 || tempQuestionID != Oraread.GetInt32(0))
                    {
                        if (currentQuestion != null) questions.Add(currentQuestion);
                        currentQuestion = new Question(Oraread.GetInt32(0), Oraread.GetString(1));
                        currentQuestion.AddAnswer(new Answer(Oraread.GetString(2), Oraread.GetInt32(3)));
                        tempQuestionID = currentQuestion.Id;
                    }
                    else
                    {
                        currentQuestion.AddAnswer(new Answer(Oraread.GetString(2), Oraread.GetInt32(3)));
                    }
                }

                if (currentQuestion != null) questions.Add(currentQuestion);
            }
            catch (OracleException ex)
            {
                OracleExceptions.Parse(ex);
            }
            finally
            {
                conn.Close();
            }
        }
        public List<Question> GetList()
        {
            return questions;
        }
    }
}
