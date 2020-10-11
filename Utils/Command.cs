using System.Collections.Generic;

namespace Utils
{
    public abstract class Command
    {
        public abstract void Execute(List<string> arguments);
    }
}
