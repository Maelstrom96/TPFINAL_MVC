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
                qs.GetList().Add(q);

                status.Success = true;
                status.Message = "La question à été ajouté avec succès";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

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

        // GET: Question/Edit/5
        public ActionResult Edit(int id)
        {
            Question q = new Question();
            bool allo = q.UpdateQuestion(88, "Edited Question", 1);
            bool allo2 = q.UpdateAnswer(225, "Edited Answer", false);
            return View();
        }

        // POST: Question/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Question/Delete/5
        public ActionResult Delete(int id)
        {
            Question q = new Question();
            bool allo = q.DeleteQuestion(88);
            return View();
        }

        // POST: Question/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
