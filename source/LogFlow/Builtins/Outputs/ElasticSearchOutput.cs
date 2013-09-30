using System.Collections.Generic;
using System.Threading;

namespace LogFlow.Builtins.Outputs
{
	public class ElasticSearchOutput : ILogProcess
	{
        private readonly HashSet<string> indexNames = new HashSet<string>();

        public ElasticSearchOutput(ElasticSearchConfiguration configuration)
	    {
            //Ensure index
	    }

	    public Result ExecuteProcess(Result result)
	    {
	        //Build es properties
            //Validate data to be saved
            //Ensure index esist
            //Save

            return result;
	    }

        private bool EnsureIndexExists(string indexName, PipelineContext pipelineContext)
        {
            if (indexNames.Contains(indexName))
                return OutputFlow.Successfull;

            while (true)
            {
                if (pipelineContext.CancelationRequested)
                    return OutputFlow.Failed;

                if (CreateIndex(indexName))
                {
                    indexNames.Add(indexName);
                    return OutputFlow.Successfull;
                }

                Thread.Sleep(10000);
            }
        }
	}
}
