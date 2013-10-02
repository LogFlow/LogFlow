namespace LogFlow
{
	public interface ILogOutput : INeedContext
	{
		void Process(Result result);
	}
}