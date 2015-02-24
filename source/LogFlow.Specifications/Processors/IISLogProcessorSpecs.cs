using System;
using LogFlow.Builtins.Processors.IISLog;
using LogFlow.Storage;
using Machine.Specifications;
using Newtonsoft.Json.Linq;

namespace LogFlow.Specifications.Processors
{
    [Subject(typeof(IISLogProcessor))]
    public class When_processing_logs_with_only_comments
    {
        static IISLogProcessor Processor = new IISLogProcessor();
        static Result Result;

        Establish context = () =>
        {
            var storage = new InMemoryStateStorage();
            var logContext = new LogContext("IIS", storage);

            Result = new Result(logContext)
            {
                Line = "# comment goes here.."
            };

            Processor.Process(Result);
        };

        It should_returned_canceled_result = () => Result.Canceled.ShouldBeTrue();
    }

    [Subject(typeof(IISLogProcessor))]
    public class When_processing_log_line
    {
        static IISLogProcessor Processor = new IISLogProcessor();
        static Result Result;

        Establish context = () =>
        {
            var storage = new InMemoryStateStorage();
            var logContext = new LogContext("IIS", storage);

            Processor.SetContext(logContext);
            Processor.Process(new Result(logContext)
            {
                Line = "#Fields: date time s-computername cs-method cs-uri-stem cs-uri-query s-port c-ip cs-host sc-status sc-win32-status sc-bytes cs-bytes time-taken"
            });

            Result = new Result(logContext)
            {
                Line = "2015-02-24 15:19:09 MACHINE GET /path/to/resource/ query=string 80 192.168.0.1 www.foo.bar 200 0 18027 541 811"
            };

            Processor.Process(Result);
        };

        It should_not_returned_canceled_result = () => Result.Canceled.ShouldBeFalse();

        It should_have_date = () => Result.EventTimeStamp.ShouldEqual(DateTime.Parse("2015-02-24 15:19:09"));
        It should_have_machine = () => Result.Json.Value<string>("s-computername").ShouldEqual("MACHINE");
        It should_have_method = () => Result.Json.Value<string>("cs-method").ShouldEqual("GET");
        It should_have_uri_stem = () => Result.Json.Value<string>("cs-uri-stem").ShouldEqual("/path/to/resource/");
        It should_have_uri_query = () => Result.Json.Value<string>("cs-uri-query").ShouldEqual("query=string");
        It should_have_port = () => Result.Json.Value<int>("s-port").ShouldEqual(80);
        It should_have_ip = () => Result.Json.Value<string>("c-ip").ShouldEqual("192.168.0.1");
        It should_have_host = () => Result.Json.Value<string>("cs-host").ShouldEqual("www.foo.bar");
        It should_have_status = () => Result.Json.Value<int>("sc-status").ShouldEqual(200);
        It should_have_win32_status = () => Result.Json.Value<int>("sc-win32-status").ShouldEqual(0);
        It should_have_sent_bytes = () => Result.Json.Value<int>("sc-bytes").ShouldEqual(18027);
        It should_have_received_bytes = () => Result.Json.Value<int>("cs-bytes").ShouldEqual(541);
        It should_have_time_taken = () => Result.Json.Value<int>("time-taken").ShouldEqual(811);

        It should_have_parsed_status = () => Result.Json.GetValue("sc-status").Type.ShouldEqual(JTokenType.Integer);
        It should_have_parsed_port = () => Result.Json.GetValue("s-port").Type.ShouldEqual(JTokenType.Integer);
        It should_have_parsed_win32_status = () => Result.Json.GetValue("sc-win32-status").Type.ShouldEqual(JTokenType.Integer);
        It should_have_parsed_sent_bytes = () => Result.Json.GetValue("sc-bytes").Type.ShouldEqual(JTokenType.Integer);
        It should_have_parsed_received_bytes = () => Result.Json.GetValue("cs-bytes").Type.ShouldEqual(JTokenType.Integer);
        It should_have_parsed_time_taken = () => Result.Json.GetValue("time-taken").Type.ShouldEqual(JTokenType.Integer);
    }
}