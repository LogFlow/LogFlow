using System;
using System.Linq;

namespace LogFlow
{
    public class LogFlowEngine
    {
        public bool Start()
        {
            var flows = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(Flow)));




            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}
