using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Nuar.WebApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nuar.Swagger
{
    internal sealed class WebApiDocumentFilter : IDocumentFilter
    {
        private readonly WebApiEndpointDefinitions _definitions;
        private const string InBody = "body";
        private const string InQuery = "query";

        private readonly Func<OpenApiPathItem, string, (OpenApiOperation operation, OperationType type)> _getOperation =
            (item, method) =>
            {
                switch (method.ToLowerInvariant())
                {
                    case "get":
                        item.AddOperation(OperationType.Get, new OpenApiOperation());
                        return (item.Operations[OperationType.Get], OperationType.Get);
                    case "head":
                        item.AddOperation(OperationType.Head, new OpenApiOperation());
                        return (item.Operations[OperationType.Head], OperationType.Head);
                    case "options":
                        item.AddOperation(OperationType.Options, new OpenApiOperation());
                        return (item.Operations[OperationType.Options], OperationType.Options);
                    case "post":
                        item.AddOperation(OperationType.Post, new OpenApiOperation());
                        return (item.Operations[OperationType.Post], OperationType.Post);
                    case "put":
                        item.AddOperation(OperationType.Put, new OpenApiOperation());
                        return (item.Operations[OperationType.Put], OperationType.Put);
                    case "patch":
                        item.AddOperation(OperationType.Patch, new OpenApiOperation());
                        return (item.Operations[OperationType.Patch], OperationType.Patch);
                    case "delete":
                        item.AddOperation(OperationType.Delete, new OpenApiOperation());
                        return (item.Operations[OperationType.Delete], OperationType.Delete);
                }

                return (default, default);
            };

        public WebApiDocumentFilter(WebApiEndpointDefinitions definitions)
        {
            _definitions = definitions;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var definition in _definitions)
            {
                var pathItem = new OpenApiPathItem();
                var (operation, type) = _getOperation(pathItem, definition.Method);
                if (operation == null)
                {
                    continue;
                }

                operation.Responses = new OpenApiResponses();
                operation.Parameters = new List<OpenApiParameter>();

                foreach (var parameter in definition.Parameters)
                {
                    if (parameter.In == InBody)
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = parameter.Name,
                            Schema = new OpenApiSchema
                            {
                                Type = parameter.Type,
                                Example = new OpenApiString(JsonConvert.SerializeObject(parameter.Example))
                            }
                        });
                    }
                    else if (parameter.In == InQuery)
                    {
                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = parameter.Name,
                            Schema = new OpenApiSchema
                            {
                                Type = parameter.Type,
                                Example = new OpenApiString(JsonConvert.SerializeObject(parameter.Example))
                            }
                        });
                    }
                }

                foreach (var response in definition.Responses)
                {
                    operation.Responses.Add(response.StatusCode.ToString(), new OpenApiResponse
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            {
                                "application/json", new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = response.Type,
                                        Example = new OpenApiString(JsonConvert.SerializeObject(response.Example))
                                    }
                                }
                            }
                        }
                    });
                }

                if (!swaggerDoc.Paths.ContainsKey(definition.Path))
                {
                    swaggerDoc.Paths.Add(definition.Path, pathItem);
                }
                else
                {
                    swaggerDoc.Paths[definition.Path].AddOperation(type, operation);
                }
            }
        }
    }
}
