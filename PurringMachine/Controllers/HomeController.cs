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
        private Machine machine;

        public ActionResult Index()
        {
            LoadMachineState();
            if (machine == null)
            {
                machine = new Machine();
                bool fromLeft = false;
                machine.SetInstructions(GetDefaultInstructionSet(), fromLeft);
                machine.SetTapeData("1111");
                machine.Reset();
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
            //todo: settings view should set up the instruction set and initial 
            machine.Run();
            SaveMachineState();
            return Json(new { Data = new string(machine.GetTape().ToArray()) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetMachine()
        {
            LoadMachineState();
            //todo: settings view should set up the instruction set and initial 

            machine.Reset();
            SaveMachineState();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult NextStep()
        {
            LoadMachineState();
            //todo: settings view should set up the instruction set and initial 
            if (machine.IsFinished())
            {
                throw new NotSupportedException("Machine has already stopped!");
            }

            machine.ProcessInstruction();
            SaveMachineState();
            return Json(new { Data = new string(machine.GetTape().ToArray()) }, JsonRequestBehavior.AllowGet);
        }

        private List<Instruction> GetDefaultInstructionSet()
        {
            return new List<Instruction>
            {
                new Instruction ('#',"q0",'#',"q0",Movement.L),
                new Instruction ('0',"q0",'1',"SK",Movement.L),
                new Instruction ('1',"q0",'0',"q1",Movement.L),

                new Instruction ('#',"q1",'1',"SK",Movement.L),
                new Instruction ('0',"q1",'1',"SK",Movement.L),
                new Instruction ('1',"q1",'0',"q1",Movement.L)
            };
        }

        private void LoadMachineState()
        {
            this.machine = GetMachine();
        }

        private void SaveMachineState()
        {
            SetMachine(this.machine);
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