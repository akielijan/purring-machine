using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PurringMachine.Models;
using Machine = PurringMachine.Models.PurringMachine;

namespace PurringMachine.Controllers
{
    public class HomeController : Controller
    {
        public Machine Machine { get; private set; }

        public ActionResult Index()
        {
            LoadMachineState();
            if (Machine == null)
            {
                Machine = Machine.GetDefaultMachine();
                SaveMachineState();
            }

            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RunMachine()
        {
            LoadMachineState();
            Machine.Run();
            SaveMachineState();
            return Json(new { Data = new string(Machine.Tape.ToArray()) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetMachine()
        {
            LoadMachineState();
            Machine.Reset();
            SaveMachineState();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult NextStep()
        {
            LoadMachineState();
            if (Machine.IsFinished())
            {
                return Json(new { Data = "The machine has stopped", Success = false}, JsonRequestBehavior.AllowGet);
            }

            Machine.ProcessInstruction();
            SaveMachineState();
            return Json(new { Data = new string(Machine.Tape.ToArray()), Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveSettings(string instructions, string input, bool fromLeft)
        {
            LoadMachineState();
            Machine.SetInstructions(ParseInstructions(instructions), fromLeft);
            Machine.SetTapeData(input);
            SaveMachineState();
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTapeData(int n)
        {
            LoadMachineState();
            return Json(new { Data = GetCharactersFromTape(n), Success = true }, JsonRequestBehavior.AllowGet);
        }

        public string GetCharactersFromTape(int characterCount)
        {
            List<char> tapeData;
            if (Machine.CurrentPositionOnTape == Machine.NO_TAPE)
            {
                tapeData = new string(Machine.EMPTY_SYMBOL, characterCount).ToList();
            }
            else
            {
                tapeData = new List<char>();
                for (int i = 0; i < characterCount / 2; ++i)
                {
                    Machine.Move(Movement.L);
                }
                for (int i = 0; i < characterCount; ++i)
                {
                    tapeData.Add(Machine.Tape[Machine.CurrentPositionOnTape]);
                    Machine.Move(Movement.R);
                }
                for (int i = 0; i < (characterCount / 2) + 1; ++i)
                {
                    Machine.Move(Movement.L);
                }
            }
            return new string(tapeData.ToArray());
        }

        private List<Instruction> ParseInstructions(string instructions)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Instruction>>(instructions);
        }

        private void LoadMachineState()
        {
            this.Machine = GetMachine();
        }

        private void SaveMachineState()
        {
            SetMachine(this.Machine);
        }

        private Machine GetMachine()
        {
            return Session["Machine"] as Machine;
        }

        private void SetMachine(Machine machine)
        {
            Session["Machine"] = machine;
        }
    }
}