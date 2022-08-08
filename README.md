# Fafnir

Fafnir is a netcore library that helps your API to reject a bunch of incoming requests from the same origin.
[![.NET](https://github.com/ramiro-di-rico/Fafnir/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/ramiro-di-rico/Fafnir/actions/workflows/dotnet-build.yml)

## Installation

Use the [Nuget](https://www.nuget.org/) package manager console to install [Fafnir](https://www.nuget.org/packages/Fafnir.Throttle.NetCore)

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
    "Period": "00:01:00",
    "PenaltyTime": "00:05:00",
    "ErrorMessage": "throttle custom message."
}
```

or optionally configure through the 2nd overload

```netcore ConfigureServices
services.AddThrottle(
            maxRequests: 50, 
            period: TimeSpan.FromMinutes(1), 
            penaltyTime: TimeSpan.FromMinutes(5), 
            errorMessage: "My Custom error.");
```

and then add UseThrottle method in Startup.Configure method

```netcore ConfigureServices
app.UseThrottle();
```

The configuration above will just allow to get through 50 requests from the same origin in a time window of 1 minute. 
If the amount of requests is exceeded, then the requester will get an error (Http status code 429) until the penalty of 5 minutes is completed.



## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)