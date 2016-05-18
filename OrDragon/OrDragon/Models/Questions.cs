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
    public class Question
    {
        public int Id { get; set; }
        [Display(Name="Question")]
        public String Text { get; set; }
        [Range(1,3)]
        [Display(Name="Difficulty")]
        public int Difficulty { get; set; }
        public List<Answer> Answers { get; set; }
        public Answer GoodAnswer { get; set; }

        public Question() 
        {
            Answers = new List<Answer>();
        }

        public Question(int id, string text, int Diff) : this()
        {
            Id = id;
            Text = text;
            Difficulty = Diff;
        }

        public void AddAnswer(Answer answer)
        {
            Answers.Add(answer);
            if (answer.IsGood) GoodAnswer = answer;
        }

        public void AddQuestionDB(OracleConnection con)
        {
            OracleCommand cmd = new OracleCommand("QuestionsPKG", con);
            cmd.CommandText = "QuestionsPKG.CreateQuestion";
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter Enonce = new OracleParameter("PEnoncer", OracleDbType.Varchar2);
            Enonce.Value = Text;
            Enonce.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(Enonce);

            OracleParameter Diff = new OracleParameter("PDiff", OracleDbType.Int32);
            Diff.Value = Difficulty;
            Diff.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(Diff);

            cmd.ExecuteNonQuery();

            cmd.CommandText = "CreateAnswer";

            Enonce = new OracleParameter("PReponse", OracleDbType.Varchar2);
            Enonce.Direction = ParameterDirection.Input;

            Diff = new OracleParameter("PFlag", OracleDbType.Int32);
            Diff.Direction = ParameterDirection.Input;
            for (int i = 0; i < Answers.Count; ++i)
            {
                cmd.Parameters.Clear();
                Enonce.Value = Answers[i].Text;
                Diff.Value = Answers[i].Text;

                cmd.Parameters.Add(Enonce);
                cmd.Parameters.Add(Diff);

                cmd.ExecuteNonQuery();
            }
        }
    }
    public class QuestionAnswerViewModel
    {
        public Question q;
        public Answer a;
        public Answer b;
        public Answer c;
        public Answer d;

        public QuestionAnswerViewModel()
        {
            q = new Question();
            a = new Answer();
            b = new Answer();
            c = new Answer();
            d = new Answer();
        }
    }

    public class Answer
    {
        [Display(Name="Answers")]
        public String Text { get; set; }
        public bool IsGood { get; set; }

        public Answer()
        {

        }
        public Answer(String text, int isGood)
        {
            Text = text;
            if (isGood == 0) IsGood = false;
            else IsGood = true;
        }

        public void setIsGood(bool value)
        {
            IsGood = value;
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
                        currentQuestion = new Question(Oraread.GetInt32(0), Oraread.GetString(1), Oraread.GetInt32(2));
                        currentQuestion.AddAnswer(new Answer(Oraread.GetString(3), Oraread.GetInt32(4)));
                        tempQuestionID = currentQuestion.Id;
                    }
                    else
                    {
                        currentQuestion.AddAnswer(new Answer(Oraread.GetString(3), Oraread.GetInt32(4)));
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
