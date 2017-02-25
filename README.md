[![Build status](https://ci.appveyor.com/api/projects/status/2ujeqsi1lnsivxxa?svg=true)](https://ci.appveyor.com/project/Deveel/drest) [![MyGet](https://img.shields.io/myget/deveel/v/Deveel.Rest.Client.svg?label=MyGet)](https://www.myget.org/feed/deveel/package/nuget/Deveel.Rest.Client) [![NuGet](https://img.shields.io/nuget/v/Deveel.Rest.Client.svg?label=NuGet)](https://www.nuget.org/packages/Deveel.Rest.Client)


# Deveel REST Client (DRest)

This is a simple .NET Core client library for interacting with REST services, making it easy to construct and invoke HTTP requests in a dynamic way.

## Install the Library

- **NuGet**
    
    In the Package Manager console type
    
    ```
    PM> Install-Package Deveel.Rest.Client
    ```
    Or 
    
    ```
    PM> Install-Package Deveel.Rest.Client -Pre
    ```
    
    to obtain the pre-released packages
    
- **Nightly Builds**

    Add the following sources to NuGet
    https://www.myget.org/F/deveel/api/v3/index.json | https://www.myget.org/F/deveel/api/v2

    In the Package Manager console type

    ```
    PM> Install-Package Deveel.Rest.Client
    ```


## Building a Client

To build a REST client with **DRest** is an easy process and it can be done with a straight constructor of `RestClientSettings` (or any class deriving from `IRestClientSettings`) or using a builder.

In this example, we will use a building strategy:

``` csharp
var client = RestClient.Build(settings => settings
                .BaseUri("http://api.example.com")
                .UseJsonSerializer()
                .UseDefaultHeader("X-DeviceId", "9999999999"));
```
The base URI of the service to contact is required in a building context, while when contructing the client using a reference to `IHttpClient` that value can be inferred from that low-level client.

By default, `RestClientSettings` includes all the default `IContentSerializer` instances defined by the library: if in the builder one or more specific serializers are specified to be used, the settings is cleared out by the default serializers and the explicitly specified are used.  

## Making a Request

The library provides the class `RestRequest` as the core element for constructing a REST request to a remote service: this includes parameters (*Header*, *Route*, *QueryString*, *Body* and *File*) and request-specific authentication.

It is also provided a building model for constructing requests dynamically

``` csharp
var request = RestRequest.Build(builder => builder
      .Get()
      .To("user/{userId}")
      .WithRoute("userId", 25)
      .Returns<User>());
      
var response = await client.RequestAsync(request);

var user = await response.GetBodyAsync<User>();
```

Extensions to the `IRestClient` provide utilities for building requests directly when sending the request.

``` csharp
var response = client.SendAsync(request => request
        .Post()
        .To("user/{userId}/setting")
        .WithRoute("userId", 101)
        .WithJsonBody(new UserSetting{Key="notify",Value=true})
        .WithHeader("X-Source", "iOS App"));
```

### Content Serializers

The content (body) of a request or a response is serialized/deserialized by the client using implementations of `IContentSerializer`, selected by the analysis of the content type of a response or from an explicit specification of the request setup (either specifying the type of content posted or by using the default content defined in the client settings).

**DRest** defines some default implementations of the serializers

* `JsonContentSerializer`: it is used to serialize datain or from JSON contents; it uses the Newtonsoft JSON.NET library as lower level serializer
* `XmlContentSerializer`: handles requests and responses of XML formatted contents
* `KeyValueContentSerializer`: although the `www-form-urlencoded` format is not explicitly recommended in REST model, this serializer is a utility when passing multi-parted content to a remote service (eg. additional data to an image file)

### Authentication

Requests to a remote service can be optionally authenticated, given a strategy defined from implementations of `IRequestAuthenticator`: when specified, in the client settings or in the request settings, this is invoked before any request is sent to the remote service.

``` csharp
var client = RestClient.Build(builder => builder
      .BaseUri("http://api.example.com")
      .UseJwtAuthentication(token));
      
  ....
  
var response = client.SendAsync(request => request
      .Get()
      .To("token")
      .WithQueryString("grant_type", "password")
      .WithQueryString("username", "tester")
      .WithQueryString("password", "abc1234")
      .Authenticate(false));
      
var response2 = client.SendAsync(request => request
      .Get()
      .To("docs")
      .UseBasicAuthentication("developer", "dev1234")
      .Returns<dynamic>());
```

The request-level authenticator overrides any authentication configured at client-settings level, if any. It is also possible to request that a specific request is not authenticated: in this case none of the authenticators specified will be invoked.
