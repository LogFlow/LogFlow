using System;

namespace LogFlow.Builtins.Inputs
{
	public class ConsoleInput : LogInput
	{
		private Tuple<Guid, string> _unprocessed;

		public override Result GetLine()
		{
			var result = new Result();

			if (_unprocessed != null)
			{
				result.Line = _unprocessed.Item2;
				_unprocessed = new Tuple<Guid, string>(result.Id, result.Line);
				return result;
			}

			while (string.IsNullOrWhiteSpace(result.Line))
			{
				result.Line = Console.ReadLine();
			}

			_unprocessed = new Tuple<Guid, string>(result.Id, result.Line);
			return result;
		}

		public override void LineIsProcessed(Guid resultId)
		{
			if (_unprocessed == null)
			{
				return;
			}

			if (_unprocessed.Item1 != resultId)
			{
				throw new InvalidOperationException("Wrong id dude!");
			}

			_unprocessed = null;
		}
	}
}
