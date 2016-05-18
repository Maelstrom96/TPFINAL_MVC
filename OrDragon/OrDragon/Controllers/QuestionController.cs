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
            Question q = new Question();
            q.Text = qtext;
            q.AddAnswer(new Answer(rep1, goodrep == 1));
            q.AddAnswer(new Answer(rep2, goodrep == 2));
            q.AddAnswer(new Answer(rep3, goodrep == 3));
            q.AddAnswer(new Answer(rep4, goodrep == 4));
            q.Difficulty = difficulty;

            try {
                q.AddQuestionDB();

            }
            catch (OracleException ex)            {
                OracleExceptions.Parse(ex);
            }

            return null;
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
