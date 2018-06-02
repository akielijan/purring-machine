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
        private List<char> tape;
        private List<Instruction> instructions;
        private bool startFromLeft;

        public PurringMachine()
        {
            Initialize();
        }

        private void Initialize()
        {
            currentState = START_STATE_SYMBOL;
            instructions = new List<Instruction>();
            tape = new List<char>();
            currentPositionOnTape = NO_TAPE;
        }

        public void SetInstructions(List<Instruction> i, bool fromLeft)
        {
            instructions = new List<Instruction>(i);
            startFromLeft = fromLeft;
        }

        public void ClearInstructions()
        {
            instructions.Clear();
        }

        private int FindFirstNonEmptyElement(bool fromLeft)
        {
            if (fromLeft)
            {
                for (int i = 0; i < tape.Count; i++)
                {
                    if (tape[i] != EMPTY_SYMBOL)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = tape.Count - 1; i >= 0; i--)
                {
                    if (tape[i] != EMPTY_SYMBOL)
                    {
                        return i;
                    }
                }
            }

            return NO_TAPE;
        }

        public void SetTapeData(IEnumerable<char> data)
        {
            tape = new List<char>(data);
        }

        public void AddTapeData(char symbol, bool toEnd)
        {
            if (toEnd)
            {
                tape.Add(symbol);
            }
            else
            {
                tape.Insert(0, symbol);
            }
        }

        public void ClearTape()
        {
            tape.Clear();
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
                Instruction instruction = instructions.First(i => i.symbol == tape[currentPositionOnTape] && i.state == currentState);
                tape[currentPositionOnTape] = instruction.symbolToWrite;
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
                    if (currentPositionOnTape == tape.Count)
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
            currentPositionOnTape = FindFirstNonEmptyElement(startFromLeft);
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

        public List<char> GetTape()
        {
            return tape;
        }
    }
}