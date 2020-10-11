using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMaster.Commands
{
    abstract class Command
    {
        public abstract void Execute(List<string> arguments);
    }
}
