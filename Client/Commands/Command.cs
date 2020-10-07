using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Commands
{
    abstract class Command
    {
        public abstract void Execute(List<string> arguments);
    }
}
