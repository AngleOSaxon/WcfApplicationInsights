﻿using System;
using System.ServiceModel.Channels;

namespace Microsoft.ApplicationInsights.Wcf.Implementation
{
    internal static class WcfExtensions
    {
        public static HttpRequestMessageProperty GetHttpRequestHeaders(this IOperationContext operation)
        {
            if ( operation.HasIncomingMessageProperty(HttpRequestMessageProperty.Name) )
            {
                return (HttpRequestMessageProperty)operation.GetIncomingMessageProperty(HttpRequestMessageProperty.Name);
            }
            return null;
        }
        public static HttpResponseMessageProperty GetHttpResponseHeaders(this IOperationContext operation)
        {
            if ( operation.HasOutgoingMessageProperty(HttpResponseMessageProperty.Name) )
            {
                return (HttpResponseMessageProperty)operation.GetOutgoingMessageProperty(HttpResponseMessageProperty.Name);
            }
            return null;
        }

        public static HttpRequestMessageProperty GetHttpRequestHeaders(this Message message)
        {
            HttpRequestMessageProperty headers = null;
            if ( message.Properties.ContainsKey(HttpRequestMessageProperty.Name) )
            {
                headers = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
            } else
            {
                headers = new HttpRequestMessageProperty();
                message.Properties.Add(HttpRequestMessageProperty.Name, headers);
            }
            return headers;
        }
    }
}
