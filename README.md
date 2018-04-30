# Stone Splunk Http Events Collector Client

An easy to use and high performance .NET Standard Class Library to publish events on Splunk Http Events Collector.

* [Code of Conduct](codeOfConduct/README.md)
* [CONTRIBUTING.md](contributing/README.md)
* [LICENSE.md](license/README.md)

## Dependences
1. NETStandard.Library v2.0.0
2. Microsoft.Extensions.Configuration.Abstractions v2.0.0
3. Newtonsoft.Json v10.0.3

## Installing

```
Install-Package StoneCo.SplunkHECClient
```

## Uninstalling

```
Uninstall-Package StoneCo.SplunkHECClient
```

## Examples

### Base code

```C#

class Human
{
	public int Age { get; set; }
	
	public string Name { get; set; }
	
	public Gender Gender { get; set; }
}

enum Gender
{
	Male,
	Female
}

```

### Methods


#### Send
Sends a request synchronously.

***Signature***

```C#

ISplunkHECResponse Send(ISplunkHECRequest request);

```

***Example***

```C#

ISplunkHECClientConfiguration config = new SplunkHECClientConfiguration {
    Endpoint = new Uri("http://127.0.0.1:8800/services/collector"),
    Token = "12161270-ba9d-478a-9cf5-8fa9b68ba239"                
};

ISplunkHECClient splunkHECClient = new SplunkHECClient(config);

Human human = new Human
{
    Age = 5,
    Gender = Gender.Female,
    Name = "Stone"
};

ISplunkHECDocument doc = new SplunkHECDocument(human);
ISplunkHECRequest request = new SplunkHECRequest(doc);

splunkHECClient.Send(request);

```


#### SendAsync
Sends a request asynchronously.


#### HealthCheck
Sends a health check request to Splunk Http Events Collector synchronously.

#### HealthCheckAsync
Sends a health check request to Splunk Http Events Collector asynchronously.


### Events

#### BeforeSend
Event raised before send the request to Splunk Http Events Collector.

#### AfterSend
Event raised after send the request to Splunk Http Events Collector.


## License
This library is distributed under the MIT license.