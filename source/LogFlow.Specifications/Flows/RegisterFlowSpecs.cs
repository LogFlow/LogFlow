using System.Linq;
using LogFlow.Specifications.Flows;
using Machine.Specifications;


namespace LogFlow.Specifications
{
    [Subject(typeof(LogFlow))]
    public class when_adding_a_working_flow
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () => builder.BuildAndRegisterFlow(new WorkingLogFlow());

        private It should_be_saved
            = () => builder.Flows.Count.ShouldEqual(1);
    }

    [Subject(typeof(LogFlow))]
    public class when_adding_an_empty_flow
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () => builder.BuildAndRegisterFlow(new EmptyLogFlow());

        private It should_not_be_saved
            = () => builder.Flows.Count.ShouldEqual(0);
    }

    [Subject(typeof(LogFlow))]
    public class when_adding_a_flow_without_a_name
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () => builder.BuildAndRegisterFlow(new LogFlowWithoutName());

        private It should_be_saved
            = () => builder.Flows.Count.ShouldEqual(1);

        private It should_be_named_after_the_type
            = () => builder.Flows.Single().LogType.ShouldEqual("LogFlowWithoutName");
    }

    [Subject(typeof(LogFlow))]
    public class when_adding_a_flow_without_a_input
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () => builder.BuildAndRegisterFlow(new LogFlowWithoutInput());

        private It should_not_be_saved
            = () => builder.Flows.Count.ShouldEqual(0);
    }

    [Subject(typeof(LogFlow))]
    public class when_adding_a_flow_without_a_process
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () => builder.BuildAndRegisterFlow(new LogFlowWithoutProcess());

        private It should_not_be_saved
            = () => builder.Flows.Count.ShouldEqual(0);
    }

    [Subject(typeof(LogFlow))]
    public class when_adding_a_flow_with_the_same_name
    {
        static FlowBuilder builder = new FlowBuilder();

        Establish context = () =>
        {
            builder.BuildAndRegisterFlow(new WorkingLogFlow());
            builder.BuildAndRegisterFlow(new WorkingLogFlow());
        };

        private It should_not_be_saved
            = () => builder.Flows.Count.ShouldEqual(1);
    }


}
