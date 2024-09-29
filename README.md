# Nuar Framework

![Nuar Logo](./docs/logo/Nuar_128.png)

## Overview

**Nuar** is a lightweight, flexible, and highly customizable API gateway framework built for modern microservices architectures. It offers advanced routing, request processing, and middleware extensibility for handling upstream and downstream requests.

The framework is designed to provide a robust and easy-to-configure solution for managing API traffic in distributed environments, supporting policies, authentication, and load balancing out of the box.

## Features

- **Microservices API Gateway:** A framework specifically designed for API routing and traffic management in microservices.
- **Customizable Request Pipelines:** Add custom middleware, request handlers, and response transformers.
- **Extensible Extensions:** Support for adding custom extensions such as logging, authentication, CORS, and more.
- **YAML-based Configuration:** Configure your services, routes, and policies using easy-to-understand YAML files.
- **Supports Policies and Authentication:** Apply authentication and authorization policies at the route or service level.
- **Integration with Third-Party Tools:** Includes support for Swagger documentation, JWT authentication, and tracing.

## Installation

To install **Nuar**, add the following NuGet package to your project:

```bash
dotnet add package Nuar --version 1.0.0
