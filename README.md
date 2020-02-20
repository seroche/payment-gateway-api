# Payment Gateway

## How to run the API

Open the solution with `Visual Studio 2019` and start the debug. 

- Default project should be set to `Checkout.PaymentGateway.Api` 
- Default port is `50947`. It can be changed by modifying the `launchSettings.json` file under the `Checkout.PaymentGateway.Api` project

You can use the Postman file `PaymentGateway.postman_collection.json` included in the repository:

1. Run `1- [POST] /payments`

2. Copy the content of the `Location` header

![image](https://user-images.githubusercontent.com/12636347/74971392-4f0a8e80-5420-11ea-8a4e-8842462be3ff.png)

3. Open `2- [GET] /jobs/:id` and replace `/jobs/XXX` with the string you copied

4. Run the query until the job is marked as completed

![image](https://user-images.githubusercontent.com/12636347/74971508-85e0a480-5420-11ea-8f3a-c25fd941f99c.png)

5. Copy the EntityId

6. Open `3- [GET] /payments/:id` and replace the `XXX` with the EntityId you copied

7. Run the query

## Technologies used

- ASP.NET Core 3.1
- Mediatr to decouple controllers from queries/commands handlers
- FluentValidation to validate requests through a pipeline built on top of a Mediatr
- Masstransit to send commands (CQRS) or publish events raised while interacting with our domain
- Alba + an home made framework to run integration tests
- xUnit for unit testing
- Serilog for structured logs

## Spirit & Coding style

- Use DDD to design and organize business logic
- Make the API as consistent as possible by returning standardized errors (=same structure) or results
- When creating ValueObject, return a `Result` instead of returning `null` or throwing exceptions
- Always return `Result` in handler. Those are then translated by controllers to `404-NotFound`, `404-BadRequest` based on their respective type
- `ValueObject` must be built with the associated static `Create` method
- Endpoints are organized by `features`

## Flow

Since we cannot predict how fast the bank will process a request and return a response, we need to make the `[POST] /payments` endpoint asynchronous.

Our current flow is:

1. Send a payment request to `[POST] /payments` endpoint
2. Once the request validated, a `MakePayment` command is sent using `MassTransit`. In parallel, a job representing the current operation is created
3. Eventually the `[POST] /payments` endpoint returns `202-Accepted` + a `Location` header containing the url to query the job details
4. Calling the `[GET] /jobs/:id` endpoint returns the job status (`Created`, `Failed` or `Completed`), an error (if failed) and the `EntityId` (if completed)
5. Once the EntityId (=PaymentId) determined, call the `[GET] /payments/:id` endpoint to retrieve the payment details

## Extra miles

### Authentication & Security

- I used an `AuthenticationHandler` to validate the `ApiKey` passed through the `Authorization` header (format `bearer %APiKeys%`). Each ApiKey is then associated to claims (`MerchantId` for example)
- Exceptions are caught by the `ErrorHandlingMiddleware`
- Model binding errors (=missing required parameters) are caught by the `ModelBindingValidatorFilter`

### Logs

I used Serilog for his `structured logs` support.

- `ContextLoggingFilter` logs all incoming requests and associated responses (=result)
- `UseSerilogRequestLogging` (middleware) logs the response time

One common scenario is to push these logs to `AWS CloudWatch`. Since all logs are JSON object, we can query our logs with the `CW Logs` query syntax.

Example: `{ $.Elapsed > 10 }`

### CI

The JSON file `XXX` defines the structure of the CI pipeline I use:

1. Starts when Github triggers `CodePipeline` through a `Webhook`
2. Builds it within a docker container and pushes the new docker image to an ECR repository
3. Adds the `Staging` tag to this image
4. Starts the staging deployment by creating a new task definition and forcing a new deployment in the associated service
5. Requires a manual approval (SNS notification)
6. Adds the `Production` tag to this image
7. Starts the production deployment by creating a new task definition and forcing a new deployment in the associated service

Note: Both `XXX.json` and `XXX.json` are processed by a script replacing available variables (%XXX%) and calling the `AWS CLI`. Another option could be to use a `CloudFormation` template.

Note 2: This CI flow is triggered by another PWS script running all tests, merging the new code to master and eventually creating a new GitHub release.

### Docker

Docker images are built using the `dockerfile` and `buildspec.yml` files provided in this repository. 

- `builspec.yml` is an `AWS CodeBuild` file containing all commands to build/push some code. In our case, it runs the `docker build` command and push the created image to an ECR respository 
- `Dockerfile` contains all the steps required to build a new image

## Improvements

- Supports 3D Secure
- Supports more payment methods
- Persists data in a NoSQL/traditional database. Currently we use persist data in memory
- Deploy API and associated infrastructure elements with CloudFormation template
