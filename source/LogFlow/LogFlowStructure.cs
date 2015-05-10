using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LogFlow
{
	public class LogStructureWithoutInput
	{
		private readonly LogFlowStructure _flowStructure;

		internal LogStructureWithoutInput(LogFlowStructure flowStructure)
		{
			_flowStructure = flowStructure;
		}

		public LogStructureWithInput FromInput(LogInput logInput)
		{
			logInput.SetContext(_flowStructure.Context);
			_flowStructure.Input = logInput;
			return new LogStructureWithInput(_flowStructure);
		}
	}

	public class LogStructureWithInput
	{
		private readonly LogFlowStructure _flowStructure;

		internal LogStructureWithInput(LogFlowStructure flowStructure)
		{
			_flowStructure = flowStructure;
		}

		public LogStructureWithInput Then(ILogProcessor processor)
		{
			processor.SetContext(_flowStructure.Context);
			_flowStructure.Processors.Add(processor);
			return new LogStructureWithInput(_flowStructure);
		}

		[Obsolete("Use then method")]
		public void ToOutput(ILogOutput output)
		{
			output.SetContext(_flowStructure.Context);
			_flowStructure.Output = output;
		}
	}

	internal class LogFlowStructure 
	{
		public LogContext Context { get; set; }
		public ILogInput Input { get; set; }
		public readonly List<ILogProcessor> Processors = new List<ILogProcessor>();
		public ILogOutput Output { get; set; }

		private IEnumerable<IStartable> GetAllStartable()
		{
			if (Input is IStartable) yield return Input as IStartable;

			foreach (var processor in Processors.OfType<IStartable>())
			{
				yield return processor;
			}

			if (Output is IStartable) yield return Output as IStartable;
		}

		public void StartAll()
		{
			Task.WaitAll(GetAllStartable().Select(x => Task.Run(() => x.Start())).ToArray());
		}

		public void StopAll()
		{
			Task.WaitAll(GetAllStartable().Select(x => Task.Run(() => x.Stop())).ToArray());
		}
	}
}
