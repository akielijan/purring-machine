using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurringMachine.Models
{
    public class PurringMachine
    {
        public static readonly string NEUTRAL_FINISH_STATE_SYMBOL = "SK";
        public static readonly string POSITIVE_FINISH_STATE_SYMBOL = "SA";
        public static readonly string NEGATIVE_FINISH_STATE_SYMBOL = "SN";
        public static readonly string START_STATE_SYMBOL = "q0";
        public static readonly char EMPTY_SYMBOL = '#';
        private static readonly int NO_TAPE = -1;

        private string currentState;
        private int currentPositionOnTape;
        public List<char> Tape { get; private set; }
        public List<Instruction> Instructions { get; private set; }
        public bool StartFromLeft { get; private set; }

        public PurringMachine()
        {
            Initialize();
        }

        private void Initialize()
        {
            currentState = START_STATE_SYMBOL;
            Instructions = new List<Instruction>();
            Tape = new List<char>();
            currentPositionOnTape = NO_TAPE;
        }

        public void SetInstructions(List<Instruction> i, bool fromLeft)
        {
            Instructions = new List<Instruction>(i);
            StartFromLeft = fromLeft;
        }

        public void ClearInstructions()
        {
            Instructions.Clear();
        }

        private int FindFirstNonEmptyElement(bool fromLeft)
        {
            if (fromLeft)
            {
                for (int i = 0; i < Tape.Count; i++)
                {
                    if (Tape[i] != EMPTY_SYMBOL)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = Tape.Count - 1; i >= 0; i--)
                {
                    if (Tape[i] != EMPTY_SYMBOL)
                    {
                        return i;
                    }
                }
            }

            return NO_TAPE;
        }

        public void SetTapeData(IEnumerable<char> data)
        {
            Tape = new List<char>(data);
        }

        public void AddTapeData(char symbol, bool toEnd)
        {
            if (toEnd)
            {
                Tape.Add(symbol);
            }
            else
            {
                Tape.Insert(0, symbol);
            }
        }

        public void ClearTape()
        {
            Tape.Clear();
            currentPositionOnTape = NO_TAPE;
        }

        public void Run()
        {
            Reset();
            if (currentPositionOnTape == NO_TAPE)
            {
                return;
            }

            while (!IsFinished())
            {
                ProcessInstruction();
            }
        }

        public void ProcessInstruction()
        {
            try
            {
                Instruction instruction = Instructions.First(i => i.symbol == Tape[currentPositionOnTape] && i.state == currentState);
                Tape[currentPositionOnTape] = instruction.symbolToWrite;
                currentState = instruction.nextState;
                Move(instruction.movement);
            }
            catch (InvalidOperationException) //there is no instruction for current state and symbol
            {
                currentState = NEUTRAL_FINISH_STATE_SYMBOL;
            }
        }

        public void Move(Movement move)
        {
            switch (move)
            {
                case Movement.R:
                {
                    currentPositionOnTape++;
                    if (currentPositionOnTape == Tape.Count)
                    {
                        AddTapeData(EMPTY_SYMBOL, true);
                    }

                    break;
                }
                case Movement.L:
                {
                    if (currentPositionOnTape > 0)
                    {
                        currentPositionOnTape--;
                    }
                    else
                    {
                        AddTapeData(EMPTY_SYMBOL, false);
                    }

                    break;
                }
            }
        }

        public void Reset()
        {
            currentState = START_STATE_SYMBOL;
            currentPositionOnTape = FindFirstNonEmptyElement(StartFromLeft);
        }

        public bool IsFinished()
        {
            return IsFinishStatePositive() || IsFinishStateNegative() || IsFinishStateNeutral();
        }

        public bool IsFinishStatePositive()
        {
            return currentState == POSITIVE_FINISH_STATE_SYMBOL;
        }

        public bool IsFinishStateNegative()
        {
            return currentState == NEGATIVE_FINISH_STATE_SYMBOL;
        }

        public bool IsFinishStateNeutral()
        {
            return currentState == NEUTRAL_FINISH_STATE_SYMBOL;
        }

        [Obsolete("Please use Tape property instead.")]
        public List<char> GetTape()
        {
            return Tape;
        }

        private static List<Instruction> GetDefaultInstructionSet()
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

        public static PurringMachine GetDefaultMachine()
        {
            PurringMachine m = new PurringMachine();
            const bool fromLeft = false;
            m.SetInstructions(GetDefaultInstructionSet(), fromLeft);
            m.SetTapeData("1111");
            m.Reset();
            return m;
        }
    }
}