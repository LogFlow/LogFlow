using Nancy;
using System.Linq;

namespace LogFlow
{
	public class HealthModule : NancyModule
	{
		public HealthModule()
		{
			Get["/"] = _ =>
			{
				var flows = LogFlowEngine.FlowBuilder.Flows.Select(f => new { f.LogType, f.IsBroken, f.BrokenStart, Processes = f.Processes.Count });
				return Response.AsJson(flows);
			};
		}
	}
}
