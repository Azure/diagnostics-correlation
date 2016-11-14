#Introduction
Correlation is .NET library to propagate correlation context across web applications and allow. 
It handles incoming requests to extract context, intercepts outgoing requests and injects context to them.
Existing application instrumentation with the library involves several-line of code on app startup.
#Concepts
##Events correlation
![Context](https://cloud.githubusercontent.com/assets/2347409/20274491/a8f0c286-aa49-11e6-8431-d2e9f7cdfbb1.PNG)
Library supports two identifiers:
>-  `correlation-id` identifies operation (transaction, workflow), which may involve multiple services interaction. Stored in HTTP request header `x-ms-request-id`
>- `request-id` identifies particular request; it's created on caller side for any outgoing request and it's scope is limited to this call only.  It allows to distinguish multiple calls from service-a to service-b within the same operation and trace outgoing request on both sides 
By default HTTP request header `x-ms-request-root-id` is used

##Context
Library provides `CorrelationContext` which is a property bag. `CorrelationId` and `RequestId` are filled by the library, users can add their own properties

###Context Factory
Library provides `IContextFactory` and it's platform-specific implementations to parse context from the HTTP request
###Context Injector
Library provides `IContextInjector` and it's platform-specific implementations to inject context into the HTTP request

###Logging
The library is intended to work with any logging framework. `CorrelationContext` is public, and users are responsible to write it along with trace event.

##Request flow
![Request flow](https://cloud.githubusercontent.com/assets/2347409/20234728/529891b8-a835-11e6-99ac-bda19ba2234a.PNG)

###Incoming requests
Library provides set of platform specific handlers for incoming request handling.
When incoming request is received, library calls `IContextFactory` and stores context in the `AsyncLocal` variable.
Following handlers provided by the library:
 >- ASP.NET Core Middleware:
 `Microsoft.Diagnostics.Correlation.AspNetCore.Middleware.CorrelationContextTracingMiddleware`
 >- ASP.NET DiagnosticSource listener
`Microsoft.Diagnostics.Correlation.AspNetCore.ContextTracingInstrumentation`
 >- OWIN Middleware
 `Microsoft.Diagnostics.Correlation.Common.Owin.CorrelationContextTracingMiddleware`
 >- HttpModule
 `Microsoft.Diagnostics.Correlation.Http.CorrelationTracingHttpModule`
 >- MVC and WebAPI ActionFilters
 `Microsoft.Diagnostics.Correlation.Mvc.CorrelationTracingFilter`
 `Microsoft.Diagnostics.Correlation.WebApi.CorrelationTracingFilter`

###Outgoing requests
Library provides couple of ways to intercept outgoing requests, see the list below: TBD.
When outgoing request is intercepted, it gets context from the `AsyncLocal` and injects context into the request.
Following handlers provided by the library:
>- DelegatingHandler
`Microsoft.Diagnostics.Correlation.Common.Http.CorrelationContextRequestHandler` could be integrated in existing HttpClient pipeline. Developers also could use builders provided by the library to construct `HttpClient` 
>- DiagnosticListener
`HttpClient` is instrumented  in .NET Core with [DiagnosticSource](https://docs.microsoft.com/en-us/dotnet/core/api/system.diagnostics.diagnosticsource). It allows to intercept HttpClient calls and instrument outgoing requests.
>- Profiler (not shipped)
Library provides capability to instrument `WebRequest.BeginGetResponse` (which would be called for `HttpClient` and `WebRequest` methods).

##Configuration and setup
![Setup](https://cloud.githubusercontent.com/assets/2347409/20234729/5298a40a-a835-11e6-8f22-53e922455433.PNG)
###Customization
For customers who want to extend library functionality or alter it's behavior, we provide way to specify context factory and injectors.

##Nuget packages
>-  **Microsoft.Diagnostics.Correlation.AspNetCore**
Supported on NET 4.X. and .NET Core. This is standalone package which should be used for ASP.NET Core apps.
>- **Microsoft.Diagnostics.Correlation.AspNet**
Supported on	.NET 4.X. Provides ActionFilters for MVC and WebApi on top of Correlation package
>-  **Microsoft.Diagnostics.Correlation**
Supported on	.NET 4.X
This package alone should be used for non ASP.NET apps or ASP.NET apps hosted with OWIN self-host.
>-  **Microsoft.Diagnostics.Correlation.Common**
Supported on .NET 4.X and Core
Contains base interfaces and shared code

##Samples
### ASP.NET Core

#### 1. HttpClient handler
>- Create necessary `HttpClient` instances and register them:
`services.AddSingleton(CorrelationHttpClientBuilder.CreateClient());`
>- Use `CorrelationContextTracingMiddleware`:
`app.UseMiddleware<CorrelationContextTracingMiddleware>();`
#### 2. DiagnosticSource Instrumentation
```
var configuration = new AspNetCoreCorrelationConfiguration();
ContextTracingInstrumentation.Enable(configuration);
```
### ASP.NET
You may use OWIN correlation middleware or HttpModule implementation to handle incoming requests,
however MVC or WebAPI filter must be configured in `Global.asax` for IIS hostet apps
##### **1. ActionFilters**
>- **MVC**: 
`GlobalFilters.Filters.Add(new Microsoft.Diagnostics.Correlation.Mvc.CorrelationTracingFilter());`
>- **WebAPI**
`GlobalConfiguration.Configuration.Filters.Add(new Microsoft.Diagnostics.Correlation.WebApi.CorrelationTracingFilter());`
##### **2. HttpModule**
Add `CorrelationTracingHttpModule`
```
<add name="CorrelationTracingHttpModule" type="Microsoft.Diagnostics.Correlation.Http.CorrelationTracingHttpModule, Microsoft.Diagnostics.Correlation" preCondition="managedHandler" />
```
> **Note**:
HttpModule should be added under 
>- `<system.web>/<httpModules>` for IIS 6.0 
>- `<system.webServer>/<modules>` for IIS 7.0

Outgoing requests should be intercepted with instrumented `HttpClient` instance `CorrelationHttpClientBuilder.CreateClient()`. 
Use dependency injection framework to pass `HttpClient` instances.
### OWIN Self-Hosted apps
Incoming requests should be handled with OWIN middleware.
```
app.Use<CorrelationContextTracingMiddleware>();
```
Outgoing requests should be intercepted with instrumented `HttpClient` instance: `CorrelationHttpClientBuilder.CreateClient()`. 
Use dependency injection framework to pass `HttpClient` instances.

### ApplicationInsights integration
If you use ApplicationInsights to collect telemetry data, you need to configure  `TelemetryInitializer` to map `CorrelationContext` fields to AppInsights properties:
```
    public class CorrelationTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            var ctx = ContextResolver.GetRequestContext<CorrelationContext>();
            if (ctx != null)
            {
                telemetry.Context.Operation.Id = ctx.CorrelationId;
                telemetry.Context.Operation.ParentId = ctx.RequestId;
	            var operationTelemetry = telemetry as OperationTelemetry;                
                if (operationTelemetry  != null && ctx.ChildRequestId != null)
	                operationTelemetry.Id = ctx.ChildRequestId;
            }
        }
    }
```
