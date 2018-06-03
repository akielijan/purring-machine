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
        public static readonly int NO_TAPE = -1;

        private string currentState;
        public int CurrentPositionOnTape { get; private set; }
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
            CurrentPositionOnTape = NO_TAPE;
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
            CurrentPositionOnTape = NO_TAPE;
        }

        public void Run()
        {
            Reset();
            if (CurrentPositionOnTape == NO_TAPE)
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
                Instruction instruction = Instructions.First(i => i.symbol == Tape[CurrentPositionOnTape] && i.state == currentState);
                Tape[CurrentPositionOnTape] = instruction.symbolToWrite;
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
                        CurrentPositionOnTape++;
                        if (CurrentPositionOnTape == Tape.Count)
                        {
                            AddTapeData(EMPTY_SYMBOL, true);
                        }

                        break;
                    }
                case Movement.L:
                    {
                        if (CurrentPositionOnTape > 0)
                        {
                            CurrentPositionOnTape--;
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
            CurrentPositionOnTape = FindFirstNonEmptyElement(StartFromLeft);
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

        private static List<Instruction> GetIncrementBinaryValueInstructionSet()
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

        private static List<Instruction> GetDoubleCharactersInTapeInstructionSet()
        {
            return new List<Instruction>
            {
                new Instruction('#',"q0",'#',"q0",Movement.R),
                new Instruction('a',"q0",'#',"q1",Movement.R),
                new Instruction('b',"q0",'#',"q7",Movement.R),

                new Instruction('#',"q1",'#',"q2",Movement.R),
                new Instruction('a',"q1",'a',"q1",Movement.R),
                new Instruction('b',"q1",'b',"q1",Movement.R),

                new Instruction('#',"q2",'a',"q3",Movement.R),
                new Instruction('a',"q2",'a',"q2",Movement.R),
                new Instruction('b',"q2",'b',"q2",Movement.R),

                new Instruction('#',"q3",'a',"q4",Movement.L),
                new Instruction('a',"q3",'a',"SK",Movement.R),
                new Instruction('b',"q3",'b',"SK",Movement.R),

                new Instruction('#',"q4",'#',"q5",Movement.L),
                new Instruction('a',"q4",'a',"q4",Movement.L),
                new Instruction('b',"q4",'b',"q4",Movement.L),

                new Instruction('#',"q5",'#',"SK",Movement.R),
                new Instruction('a',"q5",'a',"q6",Movement.L),
                new Instruction('b',"q5",'b',"q6",Movement.L),

                new Instruction('#',"q6",'#',"q0",Movement.R),
                new Instruction('a',"q6",'a',"q6",Movement.L),
                new Instruction('b',"q6",'b',"q6",Movement.L),

                new Instruction('#',"q7",'#',"q8",Movement.R),
                new Instruction('a',"q7",'a',"q7",Movement.R),
                new Instruction('b',"q7",'b',"q7",Movement.R),

                new Instruction('#',"q8",'b',"q9",Movement.R),
                new Instruction('a',"q8",'a',"q8",Movement.R),
                new Instruction('b',"q8",'b',"q8",Movement.R),

                new Instruction('#',"q9",'b',"q10",Movement.L),
                new Instruction('a',"q9",'a',"SK",Movement.R),
                new Instruction('b',"q9",'b',"SK",Movement.R),

                new Instruction('#',"q10",'#',"q11",Movement.L),
                new Instruction('a',"q10",'a',"q10",Movement.L),
                new Instruction('b',"q10",'b',"q10",Movement.L),

                new Instruction('#',"q11",'#',"SK",Movement.R),
                new Instruction('a',"q11",'a',"q12",Movement.L),
                new Instruction('b',"q11",'b',"q12",Movement.L),

                new Instruction('#',"q12",'#',"q0",Movement.R),
                new Instruction('a',"q12",'a',"q12",Movement.L),
                new Instruction('b',"q12",'b',"q12",Movement.L)
            };
        }

        private static List<Instruction> GetPalindromeCheckingInstructionSet()
        {
            return new List<Instruction>
            {
                new Instruction('#',"q0",'#',"SA",Movement.N),
                new Instruction('a',"q0",'#',"q1",Movement.R),
                new Instruction('b',"q0",'#',"q4",Movement.R),
                new Instruction('c',"q0",'#',"q7",Movement.R),

                new Instruction('#',"q1",'#',"q2",Movement.L),
                new Instruction('a',"q1",'a',"q1",Movement.R),
                new Instruction('b',"q1",'b',"q1",Movement.R),
                new Instruction('c',"q1",'c',"q1",Movement.R),

                new Instruction('#',"q2",'#',"SA",Movement.R),
                new Instruction('a',"q2",'#',"q3",Movement.L),
                new Instruction('b',"q2",'b',"SN",Movement.R),
                new Instruction('c',"q2",'c',"SN",Movement.R),

                new Instruction('#',"q3",'#',"q0",Movement.R),
                new Instruction('a',"q3",'a',"q3",Movement.L),
                new Instruction('b',"q3",'b',"q3",Movement.L),
                new Instruction('c',"q3",'c',"q3",Movement.L),

                new Instruction('#',"q4",'#',"q5",Movement.L),
                new Instruction('a',"q4",'a',"q4",Movement.R),
                new Instruction('b',"q4",'b',"q4",Movement.R),
                new Instruction('c',"q4",'c',"q4",Movement.R),

                new Instruction('#',"q5",'#',"SA",Movement.R),
                new Instruction('a',"q5",'a',"SN",Movement.R),
                new Instruction('b',"q5",'#',"q6",Movement.L),
                new Instruction('c',"q5",'c',"SN",Movement.R),

                new Instruction('#',"q6",'#',"q0",Movement.R),
                new Instruction('a',"q6",'a',"q6",Movement.L),
                new Instruction('b',"q6",'b',"q6",Movement.L),
                new Instruction('c',"q6",'c',"q6",Movement.L),

                new Instruction('#',"q7",'#',"q8",Movement.L),
                new Instruction('a',"q7",'a',"q7",Movement.R),
                new Instruction('b',"q7",'b',"q7",Movement.R),
                new Instruction('c',"q7",'c',"q7",Movement.R),

                new Instruction('#',"q8",'#',"SA",Movement.R),
                new Instruction('a',"q8",'a',"SN",Movement.R),
                new Instruction('b',"q8",'b',"SN",Movement.R),
                new Instruction('c',"q8",'#',"q9",Movement.L),

                new Instruction('#',"q9",'#',"q0",Movement.R),
                new Instruction('a',"q9",'a',"q9",Movement.L),
                new Instruction('b',"q9",'b',"q9",Movement.L),
                new Instruction('c',"q9",'c',"q9",Movement.L)
            };
        }

        public static PurringMachine GetDefaultMachine()
        {
            PurringMachine m = new PurringMachine();
            const bool fromLeft = false;
            m.SetInstructions(GetIncrementBinaryValueInstructionSet(), fromLeft);
            m.SetTapeData("1111");
            m.Reset();
            return m;
        }
    }
}