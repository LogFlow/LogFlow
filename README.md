# LogFlow
An simple log agent aimed for use with Elasticsearch and Kibana.
The main use is to take a line of log from disk and tranform it to a json that can be index by ElasticSearch.

#### Install from nuget
Command line:
```bash
cmd> nuget install LogFlow
```

Package Manager console in Visual Studio:
```powershell
PM> Install-Package LogFlow
```

#### Example
Read and send a log line
```text
2013-09-11 11:53:43 WARN All log are belong to us
```

Send it to ElasticSearch formated JSON
```javascript
{ 
	'@timestamp': '2013-09-11 11:53:43'
	'LogLevel': 'WARN'
	'Message': 'All log are belong to us'	 
}
```

#### Project setup
Create a project of type Class Library
Install nuget package
```powershell
PM> Install-Package LogFlow
```
Two config files used by log flow will be added to your project. LogFlow.exe.config, setup for storage and web interface, and NLog.config, log configuration.

#### Flow
A flow has and one input, multiple processors and one output.
```csharp
public class MyLogFlow : Flow
    {
        public MyLogFlow()
        {
            CreateProcess("InsteadOfClassName")
                .FromInput(new FileInput("C:\\MyLogPath", Encoding.UTF8, true))
                .Then(new MyLogLineParser())
                .ToOutput(new ElasticSearchOutput(new ElasticSearchConfiguration()
                {
                    Host = "localhost",
                    Port = 9200,
                    IndexNameFormat = @"\m\y\L\o\g\s\-yyyyMM" //new index each month
                }));

        }
    }
```
Included inputs are **FileInput**

Included outputs are **ElasticSearchOutput** and **ConsoleOutput**

No processor are included.

#### Processors
A processor takes a result and returns a result.
Result contains the read line, the resulting json structure, event time stamp and the ability to cancel the flow and read next line.
```csharp
public class MyLogLineParser : LogProcessor
    {
        public override Result Process(Result result)
        {
            var logLine = result.Line;
            var timestampPosition = logLine.IndexOf(" ", logLine.IndexOf(" ") + 1);
            var eventTimeStamp = DateTime.Parse(logLine.Substring(0, timestampPosition).Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            var logLevelPosition = logLine.IndexOf(" ", timestampPosition + 1);
            var logLevel = logLine.Substring(timestampPosition, logLevelPosition - timestampPosition).Trim();
            var message = logLine.Substring(logLine.IndexOf(" ", logLevelPosition)).Trim();

            result.EventTimeStamp = eventTimeStamp;
            result.Json.Add("LogLevel", new JValue(logLevel));
            result.Json.Add("Message", new JValue(logLevel));

            return result;
        }
    }
```

Try not to over write the Json property only add to it because erlier steps might have added data to it.
There is MetaData to help you to transport data in the flow without it saving to the output.

#### Set up debugging
* Sucessfully build the project in Debug
* Project properties > Debug > Start external program > Select logflow.exe in debug folder.
* Press F5

#### Install as a service
LogFlow.exe install
