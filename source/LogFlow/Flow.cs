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

	public static class FluentProcessExtensions
	{
		public static FluentProcess AddProcess(this FluentProcess fluentProcess, ILogProcess process)
		{
			fluentProcess.Processes.Add(process);
			return fluentProcess;
		}
	}
}
