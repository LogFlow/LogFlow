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
				var flows = LogFlowEngine.FlowBuilder.Flows.Select(f => new { f.FlowStructure.Context.LogType, CurrentStatus = f.CurrentStatus.ToString() });
				return Response.AsJson(flows);
			};
		}
	}
}
