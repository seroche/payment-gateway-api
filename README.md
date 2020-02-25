# Payment Gateway

### Assumptions

- API is located under an application load balancer (ALB) decrypting HTTPS traffic. Hence this API is HTTP. When deployed, we should setup `ASP.NET core` to use forwarded headers
- Bank could be slow to process requests
- Payment should be made to a `MerchantId`. This id should not be passed through the request but determined based on the `ApiKey` passed to the API

## How to run the API

Open the solution with `Visual Studio 2019` and start the debug. 

- Default project should be set to `Checkout.PaymentGateway.Api` 
- Default port is `50947`. It can be changed by modifying the `launchSettings.json` file under the `Checkout.PaymentGateway.Api` project
- Swagger Doc: http://localhost:50947/docs

You can use the Postman file `PaymentGateway.postman_collection.json` included in the repository:

1. Run `1- [POST] /payments`

2. Copy the content of the `Location` header

![image](https://user-images.githubusercontent.com/12636347/74971392-4f0a8e80-5420-11ea-8a4e-8842462be3ff.png)

3. Open `2- [GET] /jobs/:id` and replace `/jobs/XXX` with the string you have copied

4. Run the query until the job is marked as completed

![image](https://user-images.githubusercontent.com/12636347/74971508-85e0a480-5420-11ea-8f3a-c25fd941f99c.png)

5. Copy the EntityId

6. Open `3- [GET] /payments/:id` and replace the `XXX` with the EntityId you have copied

7. Run the query

## Technologies used

- ASP.NET Core 3.1
- Mediatr to decouple controllers from queries/commands handling
- FluentValidation to validate requests through a pipeline built on top of a Mediatr
- Masstransit to send commands (`CQRS`) or publish events raised while interacting with our domain
- Alba + an homemade framework for integration testing
- xUnit for unit testing
- Serilog for structured logs

## Spirit & Coding style

- Use DDD to design and organize business logic
- Make the API as consistent as possible by returning standardized errors (=same structure) or results
- When creating ValueObject, return a `Result` instead of returning `null` or throwing exceptions
- Always return `Result` in handler. Those are translated by controllers to `404-NotFound`, `404-BadRequest` based on their respective type
- `ValueObject` must be built with the associated static `Create` method
- Endpoints are organized by `features`

## Flow

Since we cannot predict how fast the bank will return a response, we need to make the `[POST] /payments` endpoint asynchronous.

Our current flow is:

1. Send a payment request to `[POST] /payments` endpoint
2. Once the request validated, a `MakePayment` command is sent using `MassTransit`. In parallel, a job representing the current operation is created
3. Eventually the `[POST] /payments` endpoint returns `202-Accepted` + a `Location` header containing the url to query the job details
4. Calling the `[GET] /jobs/:id` endpoint returns the job status (`Created`, `Failed` or `Completed`), an error (if failed) and the `EntityId` (if completed)
5. Once the `EntityId` (=PaymentId) determined, call the `[GET] /payments/:id` endpoint to retrieve the payment details

## Extra miles

### Authentication & Security

- I used an `AuthenticationHandler` to validate the `ApiKey` passed through the `Authorization` header (format `bearer %APiKeys%`). Each ApiKey is then associated to claims (`MerchantId` for example)
- Exceptions are caught by the `ErrorHandlingMiddleware`
- Model binding errors (=missing required parameters) are caught by the `ModelBindingValidatorFilter`

### Logs

I used Serilog for his `structured logs` support.

- `ContextLoggingFilter` logs all incoming requests and associated responses (=result)
- `UseSerilogRequestLogging` (middleware) logs the response time

One common scenario is to push these logs to `AWS CloudWatch`. Since all logs are JSON objects, we can query our logs with the `CW Logs` query syntax.

Example: `{ $.Elapsed > 10 }`

### CI

The JSON files under the `ci` folder defines the CI pipeline I usually use in AWS:

1. Starts when Github triggers `CodePipeline` through a `Webhook`
2. Builds the code within a docker container and pushes the new docker image to an ECR repository
3. Adds the `Staging` tag to this image
4. Starts the staging deployment by creating a new task definition and forcing a new deployment in the associated service
5. Requires a manual approval (SNS notification)
6. Adds the `Production` tag to this image
7. Starts the production deployment by creating a new task definition and forcing a new deployment in the associated service

Note: all scripts are processed by a PWS script replacing variables (%XXX%) and calling `AWS CLI` commands. Another option could be to use a `CloudFormation` template.

Note 2: Another PWS script is in charge of running all tests, merging the code to the master branch and eventually creating a new `GitHub` release.

### Docker

Docker images are built using the `dockerfile` and `buildspec.yml` files provided in this repository. 

- `builspec.yml` is an `AWS CodeBuild` file containing all commands to build our code. In our case, it runs the `docker build` command and push the created image to an ECR repository 
- `Dockerfile` contains all the steps required to build a new image

## Improvements

- Supports 3D Secure / Tokenisation
- Improves CVV validation. Indeed most cards come with a 3 digits CVV. However AMEX come with a 4 digits. 
- Supports more payment methods
- Persists data in a NoSQL/traditional database. Currently we use persist data in memory
- Deploy API and associated infrastructure elements with CloudFormation template
