using System;

namespace LogFlow
{
	public interface ILogInput
	{
		void Start(FluentProcess processContext, Result result);
		void Stop();
	}
}
