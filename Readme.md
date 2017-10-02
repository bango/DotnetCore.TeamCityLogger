# DotnetCore.TeamCityLogger

#### Table of Contents

1. [Description](#description)
2. [Usage](#usage)

## Description
This library contains a dotnet core logger that can be used be used to log test results to a TeamCity server using service messages as descibed in the TeamCity documentation: https://confluence.jetbrains.com/display/TCD10/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ReportingTests

## Usage
In order to use the library you can either:
* Add the library as a reference to the test project
* Add the DotnetCore.TeamCityLogger.dll to the dotnet extensions directory. E.g (note that your sdk install location make differ):
    * Windows: C:\Program Files\dotnet\sdk\2.0.0\Extensions
    * Linux: /usr/share/dotnet/sdk/2.0.0/Extensions/
After installation the logger can be used like a normal dotnet test logger: `dotnet test --logger teamcity`