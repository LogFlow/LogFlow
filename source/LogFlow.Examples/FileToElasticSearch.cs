﻿using System.IO;
using LogFlow.Builtins.Inputs;
using LogFlow.Builtins.Outputs;
using LogFlow.Builtins.Processors;

namespace LogFlow.Examples
{
	public class FileToElasticSearch : Flow
	{
		public FileToElasticSearch()
		{
			CreateProcess()
				.FromInput(new FileInput(Path.Combine(Directory.GetCurrentDirectory(), "*.txt")))
				.Then(new SetEventTimeStampToNow())
				.Then(new ElasticSearchOutput(new ElasticSearchConfiguration()));
		}
	}
}