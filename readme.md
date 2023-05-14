# Summary

This is a hacked-up copy of work done by the Application Insights team in the [Application Insights SDK Lab repository](https://github.com/microsoft/ApplicationInsights-SDK-Labs/tree/master/WCF).  The original work enabled AppInsights instrumentation of non-HTTP WCF request and dependency calls on both clients and servers in .NET Framework.  I was not involved with any of that work, have no affiliation with that team, and have never worked for Microsoft.

This repository is an attempt to port that instrumentation over to .NET 6.0 and later.  As little code as possible is being changed, but a number of features are disabled for the foreseeable future--most notably any WCF Server monitoring.  Only WCF Clients using the System.ServiceModel libraries (not the CoreWCF libraries) can be monitored.

# Usage

This assumes you already have Application Insights installed and working in your project.  If you do not have Application Insights installed, review [this guide from Microsoft for an ASP.NET Core application](https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?tabs=netcorenew%2Cnetcore6) or [this guide for all other apps](https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service).

1. Install the package `WcfApplicationInsights.ServiceModel`
2. When configuring your client's `ChannelFactory`, add a reference to the `WcfApplicationInsights` namespace
3. Then inject the configured App Insights `TelemetryClient`
4. Instantiate a new `ClientTelemetryEndpointBehavior` and pass in the `TelemetryClient`
5. Add the new `ClientTelemetryEndpointBehavior` to the channel factory's endpoint behaviors

A complete example is:

``` csharp
var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

var netTcpBinding = new NetTcpBinding()
{
    Security = new NetTcpSecurity
    {
        Transport = new TcpTransportSecurity
        {
            ClientCredentialType = TcpClientCredentialType.None
        },
        Mode = SecurityMode.Transport
    }
};
var address = new EndpointAddress("<your service address>");
var channelFactory = new ChannelFactory<IEchoService>(netTcpBinding, address);
channelFactory.Endpoint.EndpointBehaviors.Add(new ClientTelemetryEndpointBehavior(telemetryClient));
```

Once the `ClientTelemetryEndpointBehavior` is added, all new connections and WCF service calls should be automatically logged to your Application Insights resource.

# Future Goals

- Autoinstrumentation
  - The existing autoinstrumentation relied on the `Microsoft.AI.Agent.Intercept` package, which was not ported to .NET Core.  It appears to use bytecode analysis to find and intercept certain calls, which would be challenging and probably undesirable to reimplement.
  - However, the `System.ServiceModel` implementations in .NET Core and onwards do appear to implement `EventSource` methods that could be listened for.
- Server Support
  - Client WCF libraries were ported over to .NET Core early on, but server libraries were not, so that `System.ServiceModel` only contains classes necessary for building clients.  The [CoreWCF](https://github.com/CoreWCF/CoreWCF) project later implemented server support, but had to reimplement many of the same classes already in `System.ServiceModel`.  Consequently supporting non-Framework WCF servers will require changing all the namespace references to point at the `CoreWCF` versions instead, as well as uncommenting and fixing up all the disabled serverside code
  - This can probably be done with careful use of conditional compilation, but needs work