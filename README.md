## CodeGator.EntityFrameworkCore.Cryptography: 
---

[![Build Status](https://dev.azure.com/codegator/CodeGator.EntityFrameworkCore.Cryptography/_apis/build/status%2FCodeGator.CodeGator.EntityFrameworkCore.Cryptography?branchName=main)](https://dev.azure.com/codegator/CodeGator.EntityFrameworkCore.Cryptography/_build/latest?definitionId=120&branchName=main)

#### What does it do?
This package contains cryptography extensions for the `Microsoft.EntityFrameworkCore` library that are used by various CodeGator solutions.

Simply put, this package allows for decorating entity property types with an `ProtectedAttribute` attribute. Once that's done, the provider class in this package will manage the encryption for you, without requiring cryptography support from the underlying database.

Now, don't get me wrong, if the underlying database *does* support cryptography, then you're probably better off with that approach. But, if (for whatever reason) you can't take that approach, then this one seems to work pretty well. 

The framework currently supports properties of type `string`, or `byte[]`. 

#### Credit where its due
I was inspired to write this package after reading this article: https://gor-grigoryan.medium.com/encryption-and-data-security-in-clean-architecture-using-ef-core-value-converters-a-guide-to-911711a1ec52

Thanks, Gor (article author). Great idea dude!

#### Quick example

Let's say you have an entity type like this:

```
public class Employee
{
   public string SSN { get; set; }
}
```

and you wanted to encrypt the employee's social security number. You could just do this:

```
public class Employee
{
   [Protected]  // <-- Add this attribute.
   public string SSN { get; set; }
}
```

Now, once the framework is configured, the encryption/decryption will work automatically. There are a few more easy steps:

Assuming your website (or whatever) starts like this (obviously simplified):

```
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();
```

You'll need to add this code:

```
var builder = WebApplication.CreateBuilder(args);

builder.AddCodeGatorDbContextProtector(); // <-- Add this line.

var app = builder.Build();

app.UseCodeGatorDbContextProtector(); // <-- Add this line.

app.Run();
```

Once you've done that, you'll then need to add this to your configuration:

```
{
    "Providers": {
    "EfCoreProtection": {
      "Credentials": [
        {
          "Name": "Default",
          "Password": "at least 128 bytes of text here",
          "SALT": "at least 128 bytes of text here",
          "Rfc2898Iterations": 30000 
        }
      ]
    }
  }
}
```

The name of the credentials MUST be named "Default". You can add other providers, if you like, but the default provider is not optional. The password and SALT can be any text. Try to make them reasonably long and unpredictable. The RFC2898 iterations should be at least 30,000, but can be any number you like.

The last change involves the data-context itself. Assuming your data-context looks like this:

```
public class PayrollDbContext : DbContext
{
   public DbSet<Employee> Employees {get; set; }
}
```

You'll want to add this line:

```
public class PayrollDbContext : DbContext
{
   public DbSet<Emplyee> Employees {get; set; }

   protected override void OnModelCreating(ModelBuilder builder)
   {
       builder.UseCodeGatorEncryption();  // <-- Line added
   }
}
```

That's it! No other changes are required. If you've followed the steps, above, it should all just work.

#### Fancy example

Other providers can be added by simply specifying the name in the protected attribute:

```
public class Employee
{
   [Protected("Payroll")]  // <-- This was changed.
   public string SSN { get; set; }
}
```

Once that's done, you'll need to add settings for the corresponding provider, in the configuration:

```
{
    "Providers": {
    "EfCoreProtection": {
      "Credentials": [
        {
          "Name": "Default",
          "Password": "at least 128 bytes of text here",
          "SALT": "at least 128 bytes of text here",
          "Rfc2898Iterations": 30000 
        },
        {
          "Name": "Payroll",
          "Password": "at least 128 bytes of text here",
          "SALT": "at least 128 bytes of text here",
          "Rfc2898Iterations": 30000 
        }
      ]
    }
  }
}
```

Using this approach, it becomes easy to protected things with multiple credentials.


#### What platform(s) does it support?
* .NET 9.x or higher

#### How do I install it?
The binary is hosted on [NuGet](https://www.nuget.org/packages/CodeGator.EntityFrameworkCore.Cryptography/). To install the package using the NuGet package manager:

PM> Install-Package CodeGator.EntityFrameworkCore.Cryptography

#### How do I contact you?
If you've spotted a bug in the code please use the project Issues [HERE](https://github.com/CodeGator/CodeGator.EntityFrameworkCore.Cryptography/issues)

We also have a discussion group [HERE](https://github.com/CodeGator/CodeGator.EntityFrameworkCore.Cryptography/discussions)

##### We also blog about projects like this one on our website, [HERE](http://www.codegator.com)
---
#### Disclaimer
This package and it's contents are experimental in nature. There is no official support. Use at your own risk.

`Microsoft.EntityFrameworkCore` is copyright by Microsoft corporation.