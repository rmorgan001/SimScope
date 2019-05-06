# SimScope - ASCOM Telescope Control Simulator
SimScope is a C# ASCOM telescope driver for use with astronomy software.  Modeled after the ASCOM .Net Simulator this project uses the local server model or hub to control its own internal telescope mount. It is built using C#, WPF, and a variation of MVVM.  It's purpose is to help those that want to create their own mount drivers in understanding the fundamentals of telescope control. Included is a mount library that mimics a small set of mount commands and works in N-Hemi and S-Hemi observatory configurations.

![Alt text](Docs/SimScope.png?raw=true "Title")

## Solution Projects

* ASCOM.SimScope.Telescope - COM/.Net Class Library implimenting the ASCOM device interface for V3 telescope driver.
* Benchmarks - Console application for using the BenchmarkDotNet performance library to perform basic performance testing.
* Mount - Class Library that contains the communication command set of actions, Queues for command executions and responses, and the                simulator that mimics a mount.
* Principles - Class Library that contains a number of fundamental methods including Coordinates, Conversions, HiResoulution dates,               julian dates, Timers, Time, and unit functions.
* SimServer - WPF application that run the ASCOM local server.
* Unit Test - basic unit tests.

### Prerequisites

You will need to install the ASCOM platform located at https://ascom-standards.org/Downloads/Index.htm

```
Give examples
```

### Installing

A step by step series of examples that tell you how to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

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

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 

## Authors

* **Billie Thompson** - *Initial work* - [PurpleBooth](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
