# AutoBiz
A pattern driven framework for jumping right into the business of your application and automating the technical details of the API layer.

# Background

## Problem

The author of this framework does a lot of development of dotnet HTTP APIs using ASP.NET MVC. He finds it repetative to create controllers to simply wrap business classes. The code often looks something like this:

```cs
// Controller
[HttpPost("my-url")]
[Authorize]
public IActionResult DoSomething([FromBody] SomeRequestContract requestData)
{
    User currentUser = // Get the user from the request
    if (!currentUser.CanDoThisThing())
    {
        return Forbid();
    }
    
    try
    {
        var someBusinessObject = new MyBusinessClass();
        MyBusinessDTO results = someBusinessObject.DoSomething(requestData.SomeArgument, requestData.OtherArgument);
        return Ok(results);
    }
    catch (InvalidOperationException e)
    {
        return BadRequest(e.Message);
    }
}

// In application code:
public static MyBusinessDTO DoSomething(string someArgument, int otherArgument)
{
    MyBusinessDTO results = /* Do some stuff */;

    if (/* Some validation check failed */)
    {
        throw new InvalidOperationException("This wasn't a valid request");
    }
    
    return results;
}
```

This can be very repetative as each business use case tends to have the same controller action layout, just with a different business class/method combo being called.

It would be great if there was a way to reduce the amount of code needed to design the API.

One solution might be to just put all your business logic in the controller rather than separating out your business logic from your API logic. But the author didn't want to do this for the following reasons:

* It doesn't allow for multi platform applications because you're coupling your business logic to your platform.
* It makes version upgrades of your platform framework more work because your business logic is often intertwined with your framework logic.
* It makes testing more involved because you're now required to mock out your platform framework in order to test your business logic. This is usually very involved since many platform frameworks are very concrete in design, making it difficult to mock it out (have you ever tried to mock out the HttpContext in MVC?)

## Solution

Design a framework that values interface seggregation, dependency injection and testability first. Also, have that framework automatically generate the API layer for you based on your business classes (with some light configuration of course).

With this famework, you can implement your business layer and automatically hook it up to your API.

```cs
// In middleware (This bit is in the adapter. It will differ depending on your platform.):
app.UseAutoBiz(autobiz =>
{
    autobiz
        .UseAuthentication(services => services.GetRequiredService<IMyCurrentUserService>().GetCurrentUser());
        .Route(
            HttpMethod.Post,
            "backend/do-something",
            DoSomething,
            requireAuthentication: true)
        /* ... other routes in the app chained off from here. */;
});

// In application code:
public static MyBusinessDTO DoSomething(SomeRequestContract arguments, User currentUser, IResponseService responseService)
{
    if (!currentUser.CanDoThisThing())
    {
        return responseService.RespontForbid();
    }
    
    MyBusinessDTO results = /* Do some stuff */;

    if (/* Some validation check failed */)
    {
        return responseService.RespondInvalidRequest("This wasn't a valid request");
    }
    
    return results;
}
```

The code above will result in an endpoint with the URL "/backend/do-something" accepting POST requests and expecting a request body that reflects the `SomeRequestContract` data structure. The User will be fetched using the configurations done in the `autobiz.UseAuthentication` method and passed into the business layer.

# Ideals

## Testing First
AutoBiz is designed with testing in mind first. Your business layer can be void of any technical details so that your tests can easily construct and invoke your business constructs. The few dependencies from AutoBiz that are introduced into your business layer can easily be mocked out with litte effort so you can get to testing sooner.

## Platform Agnostic
AutoBiz is designed to be platform agnostic. Whichever platform you want to use for your API will need an adapter implemented to automate the creation of an API layer for your business layer (AutoBiz has implemented an adapter for a dotnet HTTP API) but your business layer itself will not depend on the details of the platform. This is in contrast to so many other API frameworks out there that require you to inherit concrete base classes for things like controllers.

## Dependency Inversion First
AutoBiz inverts the direction of dependency for the business objects. Business objects will not be responsible for constructing framework constructs as is the case when your controllers are forced to do things like inherit from a base controller class. Instead, dependencies are passed into your business layer in the form of interfaces so as to allow for mocking or other changings of implementations.

# Is This Framework for Me?

This framework may not fit your needs if:

* You intend on leveraging your platform framework's features as much as possible.
    This framework abstracts the API out which means you'll lose much of the customization of your platform. If you intend on heavily utilizing utilities of things like controllers and such, you may find that this framework will not help you. It won't get in your way, it just won't help you much either.
* Your team does not have freedom to design the API how you want.
    If things like the URLs and request formats are given to you instead of you designing them how you feel best, this framework will not be of help to you.