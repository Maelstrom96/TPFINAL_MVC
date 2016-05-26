using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrDragon.Models;
using Oracle.ManagedDataAccess.Client;
using OrDragon.Exceptions;

namespace OrDragon.Controllers
{
    public class QuestionController : Controller
    {
        // GET: Question
        public ActionResult Index()
        {
            return View();
        }

        // GET: Question/Details/5
        public ActionResult Details(int id)
        {
            Question c = new Question();
            System.Data.DataSet data = c.GetQuestionById(88);
            return View();
        }

        // GET: Question/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View(new QuestionAnswerViewModel());
        }

        // POST: Question/Create
        public ActionResult Create(QuestionAnswerViewModel question)
        {
            try
            {
                Question q = new Question();
                q.Text = question.q.Text;
                q.Difficulty = question.q.Difficulty;

                List<Answer> list = new List<Answer>();
                list.Add(new Answer(question.a.Text, AnswerBoolToInt(question.a.IsGood)));
                list.Add(new Answer(question.b.Text, AnswerBoolToInt(question.b.IsGood)));
                list.Add(new Answer(question.c.Text, AnswerBoolToInt(question.c.IsGood)));
                list.Add(new Answer(question.d.Text, AnswerBoolToInt(question.d.IsGood)));

                q.Answers = list;

                q.AddQuestionDB();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, string qtext, string rep1, string rep2, string rep3, string rep4, int goodrep, ushort difficulty)
        {
            String[] reps = new String[4];

            reps[0] = rep1;
            reps[1] = rep2;
            reps[2] = rep3;
            reps[3] = rep4;

            ActionExecutingContext filterContext = new ActionExecutingContext();
            LoginStatus status = new LoginStatus();
            Questions qs = (Questions)HttpRuntime.Cache["questions"];

            if ((User)Session["User"] == null)
            {
                status.Success = false;
                status.Message = "Vous n'avez pas l'autorisation d'éffectuer cette action.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            try
            {
                if (id == null) throw new Exception();
                if (qtext == null) throw new Exception();
                if (rep1 == null) throw new Exception();
                if (rep2 == null) throw new Exception();
                if (rep3 == null) throw new Exception();
                if (rep4 == null) throw new Exception();
                if (goodrep == null) throw new Exception();
                if (difficulty == null) throw new Exception();
            }
            catch(Exception ex) {
                status.Success = false;
                status.Message = "Un des paramètre demandé est vide.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            Question q = qs.GetList().Where(i => i.Id == id).First();
            if (q == null)
            {
                status.Success = false;
                status.Message = "La question n'a pas été trouvé dans la BD.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            if (q.UpdateQuestion(q.Id, qtext, difficulty))
            {
                q.Text = qtext;
                q.Difficulty = difficulty;
            }

            bool error = false;
            for (int i = 1; i <= 4; i++ )
            {
                if (q.UpdateAnswer(q.Answers[i - 1].Id, reps[i - 1], i == goodrep))
                {
                    q.Answers[i - 1].Text = reps[i - 1];
                    q.Answers[i - 1].IsGood = i == goodrep;
                }
                else error = true;
            }

            if (!error) 
            {
                status.Success = true;
                status.Message = "La question a été modifier!";
            }
            else
            {
                status.Success = false;
                status.Message = "Une erreur c'est produite : la question n'a pas été modifié";
            }
            filterContext.Result = new JsonResult
            {
                Data = status,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }

        [HttpPost]
        public ActionResult Create(string qtext, string rep1, string rep2, string rep3, string rep4, int goodrep, ushort difficulty)
        {
            ActionExecutingContext filterContext = new ActionExecutingContext();
            LoginStatus status = new LoginStatus();

            if ((User)Session["User"] == null)
            {
                status.Success = false;
                status.Message = "Vous n'avez pas l'autorisation d'éffectuer cette action.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            Question q = new Question();
            q.Text = qtext;
            q.AddAnswer(new Answer(rep1, goodrep == 1));
            q.AddAnswer(new Answer(rep2, goodrep == 2));
            q.AddAnswer(new Answer(rep3, goodrep == 3));
            q.AddAnswer(new Answer(rep4, goodrep == 4));
            q.Difficulty = difficulty;

            Questions qs = (Questions)HttpRuntime.Cache["questions"];

            try {
                q.AddQuestionDB();

                status.Success = true;
                status.Message = "La question à été ajouté avec succès";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                qs.Import();
                return filterContext.Result;
            }
            catch (OracleException ex)            {
                OracleExceptions.Parse(ex);
                status.Message = ex.Message;
            }

            status.Success = false;

            filterContext.Result = new JsonResult
            {
                Data = status,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }

        private int AnswerBoolToInt(bool answer)
        {
            if (answer)
                return 1;
            return 0;
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            ActionExecutingContext filterContext = new ActionExecutingContext();
            LoginStatus status = new LoginStatus();

            if ((User)Session["User"] == null)
            {
                status.Success = false;
                status.Message = "Vous n'avez pas l'autorisation d'éffectuer cette action.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            Questions qs = (Questions)HttpRuntime.Cache["questions"];
            Question q = new Question();
            bool NoError = q.DeleteQuestion(id);

            if (NoError)
            {
                if (qs.GetList().Remove(qs.GetList().Single(s => s.Id == id)))
                {
                    status.Success = true;
                    status.Message = "La question à été supprimé avec succès!";
                }
                else
                {
                    status.Success = false;
                    status.Message = "La question a déjà été supprimé.";
                }
            }
            else
            {
                status.Success = false;
                status.Message = "Une erreur c'est produite, la question a peut-être déjà été supprimé.";
            }

            filterContext.Result = new JsonResult
            {
                Data = status,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }
    }
}
