using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PurringMachine.Models;
using Machine = PurringMachine.Models.PurringMachine;

namespace PurringMachine.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public void RunPurringMachine()
        {
            Machine machine = new Machine();
            machine.SetInstructions(new List<Instruction>(), true);
            machine.SetTapeData("".ToList());

            machine.Run();
        }
    }
}