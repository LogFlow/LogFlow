using System.Collections.Generic;

namespace LogFlow
{
	public abstract class Flow
	{
        public FluentProcess FluentProcess;

        protected FluentProcess CreateProcess(string name, ILogInput input)
        {
            var fluentProcess = new FluentProcess();
            fluentProcess.Name = name;
            fluentProcess.Input = input;
            FluentProcess = fluentProcess;
            return fluentProcess;
        }        
	}

    public class FluentProcess
    {
        public string Name { get; set; }
        public ILogInput Input { get; set; }
		public List<ILogProcess> Processes = new List<ILogProcess>();
    }

    public static class FluencyFlow
    {
        public static FluentProcess AddProcess(this FluentProcess fluentProcess, ILogProcess process)
        {
            fluentProcess.Processes.Add(process);
            return fluentProcess;
        }
    }
}
