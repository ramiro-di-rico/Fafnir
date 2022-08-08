# Fafnir

Fafnir is a netcore library that helps your API to reject a bunch of incoming requests from the same origin.
[![.NET](https://github.com/ramiro-di-rico/Fafnir/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/ramiro-di-rico/Fafnir/actions/workflows/dotnet-build.yml)

## Installation

Use the [Nuget](https://www.nuget.org/) package manager console to install [Fafnir](https://www.nuget.org/packages/Fafnir.Throttle.NetCore).

```bash
Install-Package Fafnir.Throttle.NetCore
```

or dotnet cli
```bash
dotnet add package Fafnir.Throttle.NetCore
```


## Usage

Add throttle service in Startup.ConfigureServices method

```netcore ConfigureServices
services.AddThrottle(Configuration);
```

and then add the following configuration in app settings.


```app settings
"ThrottleConfiguration": {
    "MaxRequests": 50,
    "Period": "00:00:30",
    "PenaltyTime": "00:00:10",
    "ErrorMessage": "throttle custom message."
}
```

or optionally configure through the 2nd overload

```netcore ConfigureServices
services.AddThrottle(
            maxRequests: 150, 
            period: TimeSpan.FromMinutes(1), 
            penaltyTime: TimeSpan.FromMinutes(5), 
            errorMessage: "My Custom error.");
```

and then add UseThrottle method in Startup.Configure method

```netcore ConfigureServices
app.UseThrottle();
```


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)