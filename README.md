# SimScope - ASCOM Telescope Control Simulator
SimScope is a C# ASCOM telescope driver for use with astronomy software.  Modeled after the ASCOM .Net Simulator this project uses the local server model or hub to control its own internal telescope mount. It is built using C#, WPF, and a variation of MVVM.  It's purpose is to help those that want to create their own mount drivers in understanding the fundamentals of telescope control. Included is a mount library that mimics a small set of mount commands and works in N-Hemi and S-Hemi observatory and axes configurations.

![Alt text](Docs/SimScope.png?raw=true "SimScope")

## Solution Projects

* ASCOM.SimScope.Telescope - COM/.Net Class Library implimenting the ASCOM device interface for V3 telescope driver.
* Benchmarks - Console application for using the BenchmarkDotNet performance library to perform basic performance testing.
* Mount - Class Library that contains the communication command set of actions, Queues for command executions and responses, and the                simulator that mimics a mount.
* Principles - Class Library that contains a number of fundamental methods including Coordinates, Conversions, HiResoulution dates,               julian dates, Timers, Time, and unit functions.
* SimServer - WPF application that run the ASCOM local server.
* Unit Test - basic unit tests.

### Prerequisites

Install the ASCOM platform and developer tools located at https://ascom-standards.org/Downloads/Index.htm

### Installing

To run SimScope download and build the solition.  Your option to remove the Benchmarks and Unit Test projects. they are not needed to run SimScope. After a sucessful debug build you must register the COM objects with ASCOM.  To do this navigate to the debug build directory and locate SimServer.exe.  Run SimServer.exe /register from a command prompt with adminstrator privedledges.  This will register SimServer with ASCOM.  The driver "ASCOM SimScope Telescope" will show up in the ASCOM chooser when selecting a driver from your astronomy software.  You can also test with some of the installed ASCOM tools such as Conform, Profile, or Diagnostics. 

```
Run SimServer.exe /register from an adminstrator command prompt
```
![Alt text](Docs/Chooser.png?raw=true "Chooser")

## Creating a new driver

If you want to use this project to create your own telescope driver there are a number of steps needed.
1. Recommend to copy the solution to another work location
2. Remove the Benchmarks and Unit Projects, unless you wan to keep them.
3. Change all the project names and namespaces to new names. I recommend you keep the word ASCOM as the first part of the driver project.  Preface the other project names with your own.  
4. Setup the project references and usings with new names.

```
ASCOM.MyDriver.Telescope
ACME.MyDriver.Mount
ACME.MyDriver.Principles
ACME.MyDriver.Server
```
5. Search solution and replace all the GUIDS with new ones except for those in the IClassFactory.cs file
6. Modify all the AssemblyInfo.cs with your information
7. In ASCOM.SimScope.Telescope/Telescope.cs change the ServedClassName and ProgId attributes to match your new project name.
8. Check your App.config and App.xaml for namespace changes.
9. After sucessful build, register your exe with ASCOM Run "ACME.MyDriver.exe /register" from an adminstrator command prompt.
10. If it does not show up in the ASCOM Chooser, unregister using "ACME.MyDriver.exe /unregister" fix the problem then re-register again.

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* Visual Studio 2017 Community edition
* .Net Framework 4.6.1
* BenchmarkDotNet version="0.11.5" nuget package https://github.com/dotnet/BenchmarkDotNet
* ASCOM platform and developer tools

## Contributing

Please read [CONTRIBUTING.md](https://github.com/rmorgan001/SimScope/blob/master/Docs/Contributing.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Robert Morgan** - *Initial work* - https://github.com/rmorgan001

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* ASCOM development team
* etc
