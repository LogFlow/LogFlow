using LogFlow.Specifications.Helpers;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace LogFlow.Specifications.Flows
{
	[Subject(typeof(Flow))]
	public class when_starting_simple_flow
	{
		static FlowBuilder builder = new FlowBuilder();
		static JArray tags;

		Establish context = () =>
		{
			var flowToTest = new SimpleFlow();
			builder.BuildAndRegisterFlow(flowToTest);
			builder.StartFlow(flowToTest);

			tags = ReportToCurrentResultProcess.CurrentResult.Json["tags"] as JArray;

		};

		private It should_have_a_result_with_two_tags
			= () => tags.Count.ShouldEqual(2);

		private It should_have_a_duck_in_first_position
			= () => tags.First.ShouldEqual("Duck");
	}
}
