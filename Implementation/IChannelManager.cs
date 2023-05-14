namespace WcfApplicationInsights.Implementation
{
    using System;
    using System.ServiceModel;
    using Microsoft.ApplicationInsights;

    internal interface IChannelManager : IDefaultCommunicationTimeouts
    {
        TelemetryClient TelemetryClient { get; }

        ClientContract OperationMap { get; }

        string RootOperationIdHeaderName { get; }

        string ParentOperationIdHeaderName { get; }

        string SoapRootOperationIdHeaderName { get; }

        string SoapParentOperationIdHeaderName { get; }

        string SoapHeaderNamespace { get; }

        bool IgnoreChannelEvents { get; }
    }
}
