using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MessagePack;
using Nuar.Options;
using Route = Nuar.Configuration.Route;

namespace Nuar.Requests
{
    internal sealed class PayloadTransformer : IPayloadTransformer
    {
        private const string ResourceIdProperty = "id";
        private readonly NuarOptions _options;
        private readonly IPayloadManager _payloadManager;
        private readonly IValueProvider _valueProvider;
        private readonly IDictionary<string, PayloadSchema> _payloads;

        public PayloadTransformer(NuarOptions options, IPayloadManager payloadManager, IValueProvider valueProvider)
        {
            _options = options;
            _payloadManager = payloadManager;
            _valueProvider = valueProvider;
            _payloads = payloadManager.Payloads;
        }

        public bool HasTransformations(string resourceId, Route route)
        {
            // Skip payload processing for GET requests or routes without transformation
            if (route.Method.ToLowerInvariant() == "get")
                return false;

            if (!string.IsNullOrWhiteSpace(resourceId) || route.Bind != null && route.Bind.Any() || route.Transform != null && route.Transform.Any())
            {
                return true;
            }

            // Check if the payloads dictionary contains a key for the route
            return _payloads.ContainsKey(GetPayloadKey(route));
        }

        public PayloadSchema Transform(string payload, string resourceId, Route route, HttpRequest request, RouteData data)
        {
            // Skip transformation for GET requests
            if (route.Method.ToLowerInvariant() == "get")
            {
                return null;
            }

            var payloadKey = GetPayloadKey(route);
            var command = _payloads.ContainsKey(payloadKey)
                ? GetObjectFromPayload(route, payload)
                : GetObject(payload);

            var commandValues = (IDictionary<string, object>)command;

            // If resourceId is not null or empty, set it in the commandValues
            if (!string.IsNullOrWhiteSpace(resourceId))
            {
                var resourceIdProperty = string.IsNullOrWhiteSpace(route.ResourceId?.Property)
                    ? _options.ResourceId?.Property
                    : route.ResourceId.Property;

                if (string.IsNullOrWhiteSpace(resourceIdProperty))
                {
                    resourceIdProperty = ResourceIdProperty;
                }

                commandValues[resourceIdProperty] = resourceId;
            }

            // Process route bindings
            foreach (var setter in route.Bind ?? Enumerable.Empty<string>())
            {
                var keyAndValue = setter.Split(':');
                var key = keyAndValue[0];
                var value = keyAndValue[1];
                commandValues[key] = _valueProvider.GetValue(value, request, data);
                var routeValue = value.Length > 2 ? value.Substring(1, value.Length - 2) : string.Empty;

                if (data.Values.TryGetValue(routeValue, out var dataValue))
                {
                    commandValues[key] = dataValue;
                }
            }

            // Process transformations
            foreach (var transformation in route.Transform ?? Enumerable.Empty<string>())
            {
                var beforeAndAfter = transformation.Split(':');
                var before = beforeAndAfter[0];
                var after = beforeAndAfter[1];

                if (commandValues.TryGetValue(before, out var value))
                {
                    commandValues.Remove(before);
                    commandValues.Add(after, value);
                }
            }

            // Return the processed payload schema
            _payloads.TryGetValue(payloadKey, out var payloadSchema);

            return new PayloadSchema(command as ExpandoObject, payloadSchema?.Schema);
        }

        private object GetObjectFromPayload(Route route, string content)
        {
            var payloadValue = _payloads[GetPayloadKey(route)].Payload;
            var request = MessagePackSerializer.Deserialize(payloadValue.GetType(), System.Text.Encoding.UTF8.GetBytes(content));

            var payloadValues = (IDictionary<string, object>)payloadValue;
            var requestValues = (IDictionary<string, object>)request;

            // Ensure the payload aligns with the expected structure
            foreach (var key in requestValues.Keys.ToList()) // Avoid modifying the collection while enumerating
            {
                if (!payloadValues.ContainsKey(key))
                {
                    requestValues.Remove(key);
                }
            }

            return request;
        }

        private static object GetObject(string content)
        {
            // Deserialize directly to an ExpandoObject using MessagePack
            return MessagePackSerializer.Deserialize<ExpandoObject>(System.Text.Encoding.UTF8.GetBytes(content));
        }

        private string GetPayloadKey(Route route)
        {
            // Generate the key based on route method and upstream path
            return _payloadManager.GetKey(route.Method, route.Upstream);
        }
    }
}
