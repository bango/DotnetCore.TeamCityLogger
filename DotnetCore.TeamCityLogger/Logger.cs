using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DotnetCore.TeamCityLogger
{
    [FriendlyName("teamcity")]
    [ExtensionUri("logger://bango.net/teamcity/v1")]
    public class TeamCityLogger : ITestLogger
    {
        private const string StartingMessagePattern = @"^\[.+\]\s+Starting:\s+(.+)$";
        private Stack<string> suiteNames;

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            suiteNames = new Stack<string>();
            events.TestResult += this.TestResultHandler;
            events.TestRunMessage += this.TestRunMessageHandler;
            events.TestRunComplete += this.TestRunCompleteHandler;
        }

        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            string testName = e.Result.TestCase.DisplayName;

            if (e.Result.Outcome == TestOutcome.Skipped)
            {
                WriteServiceMessage($"testIgnored name='{testName}'");
                return;
            }

            WriteServiceMessage($"testStarted name='{testName}'");

            foreach (var message in e.Result.Messages)
            {
                if (message.Category == TestResultMessage.StandardOutCategory)
                {
                    WriteServiceMessage($"testStdOut name='{testName}'] out='{message.Text}'");
                }
                else if (message.Category == TestResultMessage.StandardErrorCategory)
                {
                    WriteServiceMessage($"testStdErr name='{testName}'] out='{message.Text}'");
                }
            }

            if (e.Result.Outcome == TestOutcome.Failed)
            {
                WriteServiceMessage($"testFailed name='{testName}' message='{e.Result.ErrorMessage}' details='{e.Result.ErrorStackTrace}'");
            }

            WriteServiceMessage($"testFinished name='{testName}' duration='{e.Result.Duration.TotalMilliseconds}'");
        }

        private void TestRunMessageHandler(object sender, TestRunMessageEventArgs e)
        {
            Match match = Regex.Match(e.Message, StartingMessagePattern);
            if (match.Success && match.Groups.Count == 2)
            {
                string suiteName = match.Groups[1].Value;
                WriteServiceMessage($"testSuiteStarted name='{suiteName}'");
                suiteNames.Push(suiteName);
            }
        }

        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            if (suiteNames.Count > 0)
            {
                WriteServiceMessage($"testSuiteFinished name='{suiteNames.Pop()}'");
            }
        }

        private static void WriteServiceMessage(string message)
        {
            Console.WriteLine($"##teamcity[{message.Replace("\r\n","\\r\\n").Replace("\n","\\n")}]");
        }
    }
}
