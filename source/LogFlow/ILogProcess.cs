using Newtonsoft.Json.Linq;

namespace LogFlow
{
	interface ILogProcess
	{
		Result ExecuteProcess(string logLine, JObject incomingResult);
	}
}
