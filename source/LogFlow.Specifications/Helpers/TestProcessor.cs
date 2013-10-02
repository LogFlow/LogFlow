namespace LogFlow.Specifications.Helpers
{
	public class TestProcessor : LogProcessor
	{
		public override Result Process(Result result)
		{
			return new Result();
		}
	}
}
