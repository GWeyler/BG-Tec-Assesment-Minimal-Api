**Intro**

This doc is going to detail the  development of the BG Tec assessment. This involves a minimal .net API with 3 endpoints allowing to search travellers by ID, check in a traveller to a flight and searching for a traveller based on different characteristics.

**Assumptions** 

There were a few assumptions made in order to develop this:
1. Travellers can only check-in to an existing flight, existing flights are already stored somewhere in the application DB and there is no need for this API to manage them, or keep more information about them beyond the flight ID and the passengers already checked in.
2. Travellers with identical details aside from doc numbers are allowed to check in to a flight where another passenger has checked in before. This means a traveller can be checked in twice if using a second document.

**Architecture and Design Explanation**

The architecture is straightforward,  there are 4 layers Interface, Service, Repository and Data; dependency injection is used as the way to integrate them.

The interface layer made up of the 3 endpoints only handles receiving requests from clients and processing the response from  the service layer to produce the HTTP response. It also handles some data validation via the model binding. Error responses are handled as problems which make use of the built in problem details service to provide details on context and the traceId.


The service layer does the actual logic for the application and does the bulk of data validation, done mostly using model annotations and manually in some cases . It returns a result object that follows a result pattern so the interface layer can easily construct responses, and uses an error enum instead of a success bool for more grainy control. Data operations are done through 2 repositories one for flights and one for travellers.


Repository layer, this layer is built on top of EF core, and its main purpose is to serve as an abstraction between the service and the data layer to decouple them for testing. Allowing for simple mocking of the DB behaviour.  There are 2 things of note with the repo interface, it is not complete - only the methods needed for the API were developed for instance there are no delete operations etc. The other thing is that eager loading is done by default by using include to streamline data manipulation in the service layer.


Data Layer, this is the DB and EFcore combined. The schema is a basic many to many relationship between travellers and flights, in practice EFcore models this with a join table but exposing the join table to the API was not needed for this use case. Where appropriate the fields are validated for character type length etc using the inbuilt data annotations, there is also a custom annotation for date validation that ensures the date is within a range. MSSQL is used as the database server.

Logging: using Serilog to log to console (easy to add a config for file sink), a middleware component is added to add trace id for logging context.

Mapping: is achieved using Mapster, traveller and flight DTOs are simple reduced versions of their objects. 

Unit testing is done on a different project to keep it separate from what would be production code.  Nuint is used as the  tests framework and  all the major use cases and error conditions in the service are covered. The integration tests are minimal and there just as examples.

**Scaling and Concurrency**

The main consideration for scaling without changing the architecture, would be to improve the database query performance, eager loading by default and no limits on query results mean that when the database grows there will be a significant performance hit as more a more records are returned and kept in memory,  so some easy wins would be to add ‘dontInclude’ methods to the interface that do not use Include to allow explicit loading and also maybe adding a limit variable to the methods which return lists to limit the amount of records returned. Other things that would help would be for example using a caching mechanism to cache flights that are coming up soon in preparation for travellers starting to check in.  Indexing of the document SHA is already in place however a possible optimisation would be to separate documents into their own entity so they can be searched independently of their traveller, this would also help with the issue of having duplicate records of travellers when the check in with multiple doc numbers as their uniqueness could be tied to some other property for example an account ID or email.

In terms of concurrency the solution relies heavily on the EF core concurrency model to avoid collisions when adding and modifying data using a row version property; no bespoke handling of concurrency has been implemented besides aborting the process and returning an error, a mechanism to retry request would be simple to implement as the logic would be able to handle both the case where the traveller does not get add as so the process is essentially starting from scratch as well as the case where the traveller gets added but the flight is not updated. 

Securitywise, implementing Auth was beyond the scope of the assignment but that would be the place to start possibly with basic auth or JWT depending on the requirements and the type of clients the API would be exposed to . All personal information in the DB should be encrypted and only stored if necessary as an example if document number is not forwarded anywhere such as a government entity for traveller verification it might be best to only store the hash as the API does not need anything more, in terms of the transport layer in release the API should only support HTTPS encryption. 

**Dev environment and UT**

The project is a visual studio solution, so cloning both repos into the same folder should allow to work and run the unit tests.

**Running**

A docker-compose file is used to deploy the Api to a local container together with the  MSSQL container, migrations are done automatically on the Production container, if testing using the development image migrations need to be done manually.
To run the container it is necessary to provide the development certificate for the container with a password  and trust it:

```Powershell
dotnet dev-certs https -ep "$env:USERPROFILE\.aspnet\https\aspnetapp.pfx"  -p passthing111
dotnet dev-certs https --trust
```

Then Running the image:

```
docker compose up --build -d
```

The BG-Tec-Assesment-Minimal-Api.http file has a 3 example requests to test the Api. Note the docker setup is meant for easier testing, a production ready deployment would use other auth methods for the db and would never check in things like passwords into git but use environment files.

