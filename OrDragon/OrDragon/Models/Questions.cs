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
        [Required]
        [Display(Name="Question")]
        public String Text { get; set; }
        [Required]
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

        public void AddQuestionDB()
        {
            OracleConnection con = Database.GetConnection();
            con.Open();
            OracleTransaction trans = null;

            try
            {
                trans = con.BeginTransaction(IsolationLevel.ReadCommitted);

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

                cmd.CommandText = "QuestionsPKG.CreateAnswer";

                Enonce = new OracleParameter("PReponse", OracleDbType.Varchar2);
                Enonce.Direction = ParameterDirection.Input;

                Diff = new OracleParameter("PFlag", OracleDbType.Int32);
                Diff.Direction = ParameterDirection.Input;
                for (int i = 0; i < Answers.Count; ++i)
                {
                    cmd.Parameters.Clear();
                    Enonce.Value = Answers[i].Text;
                    if (Answers[i].IsGood) Diff.Value = 1;
                    else Diff.Value = 0;

                    cmd.Parameters.Add(Enonce);
                    cmd.Parameters.Add(Diff);

                    cmd.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception e)
            {
                trans.Rollback();
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetQuestionById(int ID)
        {
            DataSet data = new DataSet();
            OracleConnection con = Database.GetConnection();
            OracleCommand cmd = new OracleCommand("QUESTIONSPKG", con);
            cmd.CommandText = "QUESTIONSPKG.GetQuestionById";
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter Ref = new OracleParameter("Cur", OracleDbType.RefCursor);
            Ref.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(Ref);

            OracleParameter Id = new OracleParameter("Pid", OracleDbType.Int32);
            Id.Value = ID;
            Id.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(Id);

            OracleDataAdapter adapt = new OracleDataAdapter(cmd);
            con.Open();
            adapt.Fill(data, "Question");
            con.Close();
            return data;
        }

        public bool DeleteQuestion(int ID)
        {
            try {
                OracleConnection con = Database.GetConnection();
                OracleCommand cmd = new OracleCommand("QUESTIONSPKG", con);
                cmd.CommandText = "QUESTIONSPKG.DeleteQuestion";
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter Id = new OracleParameter("PNumQuestion", OracleDbType.Int32);
                Id.Value = ID;
                Id.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(Id);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool UpdateQuestion(int Id, String question, int difficulty)
        {
            try {
                // TODO
                OracleConnection con = Database.GetConnection();
                OracleCommand cmd = new OracleCommand("QUESTIONPKG", con);
                cmd.CommandText = "QUESTIONSPKG.UpdateQuestion";
                cmd.CommandType = CommandType.StoredProcedure;

                OracleParameter ID = new OracleParameter("PNumQuestion", OracleDbType.Int32);
                ID.Direction = ParameterDirection.Input;
                ID.Value = Id;
                cmd.Parameters.Add(ID);

                OracleParameter Question = new OracleParameter("PEnonce", OracleDbType.Varchar2);
                Question.Direction = ParameterDirection.Input;
                Question.Value = question;
                cmd.Parameters.Add(Question);

                OracleParameter Diff = new OracleParameter("PDiff", OracleDbType.Int32);
                Diff.Direction = ParameterDirection.Input;
                Diff.Value = difficulty;
                cmd.Parameters.Add(Diff);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool UpdateAnswer(int Id, String Rep, bool isTrue)
        {
            OracleConnection con = Database.GetConnection();
            OracleCommand cmd = new OracleCommand("QUESTIONSPKG");
            cmd.CommandText = "QUESTIONSPKG.UpdateAnswer";
            cmd.CommandType = CommandType.StoredProcedure;

            OracleParameter ID = new OracleParameter("PNumReponse", OracleDbType.Int32);
            ID.Direction = ParameterDirection.Input;
            ID.Value = Id;
            cmd.Parameters.Add(ID);

            OracleParameter Answer = new OracleParameter("PReponse", OracleDbType.Varchar2);
            Answer.Direction = ParameterDirection.Input;
            Answer.Value = Rep;
            cmd.Parameters.Add(Answer);

            OracleParameter Flag = new OracleParameter("PFlag", OracleDbType.Int32);
            Flag.Direction = ParameterDirection.Input;
            if (isTrue) Flag.Value = 1;
            else Flag.Value = 0;
            cmd.Parameters.Add(Flag);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
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
        [Required]
        [Display(Name="Réponse")]
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

        public Answer(String text, bool isGood_)
        {
            Text = text;
            IsGood = isGood_;
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
