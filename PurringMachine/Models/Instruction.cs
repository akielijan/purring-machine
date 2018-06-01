using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PurringMachine.Models
{
    public class Instruction
    {
        public char symbol;
        public string state;
        public char symbolToWrite;
        public string nextState;
        public Movement movement;

        public Instruction(char symbol, string state, char nextSymbol, string nextState, Movement movement)
        {
            this.symbol = symbol;
            this.state = state;
            this.symbolToWrite = nextSymbol;
            this.nextState = nextState;
            this.movement = movement;
        }
    }
}