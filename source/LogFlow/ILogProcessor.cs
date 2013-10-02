namespace LogFlow
{
	public interface ILogProcessor : INeedContext
	{
		Result Process(Result result);
	}
}
