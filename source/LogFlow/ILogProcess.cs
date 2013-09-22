using Newtonsoft.Json.Linq;

namespace LogFlow
{
	public interface ILogProcess
	{
		Result ExecuteProcess(string logLine, JObject incomingResult);
	}
}
