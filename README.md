# Payment Gateway

## How to run the API

Open the solution with Visual Studio 2019 and start the debug. 

- Default project should be set to `Checkout.PaymentGateway.Api` 
- Default port is 50947. It can be changed by modifying the `launchSettings.json` file under the `Checkout.PaymentGateway.Api` VS project

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
- Return a `Result` object instead of returning `null` or throwing exceptions
- `Result` objects are translated by controllers to `404-NotFound`, `404-BadRequest` based on their types
- `ValueObject` must be built with the associated static `Create`
- Endpoints are organized by `features`

## Flow

Since we cannot predict how fast the bank will process a request and return a response with the `PaymentId`, we need to make
the `[POST] /payments` endpoint asynchronous.

The flow is:

1. Send a payment request to `[POST] /payments` endpoint
2. Once the request validated, a `MakePayment` command is sent using `MassTransit`. In parallel, a job representing the current operation is created
3. Eventually the `[POST] /payments` endpoint will return `202-Accepted` + a `Location` header containing the url to query the job details
4. Calling the `[GET] /jobs/:id` endpoint returns the job status (`Created`, `Failed` or `Completed`), an error (if failed) and the `EntityId` (if completed). 
5. Once the EntityId (=PaymentId) determined, call the `[GET] /payments/:id` endpoint to retrieve the payment details.

## Extra miles

### Authentication & Security

- I used an `AuthenticationHandler` to validate the `ApiKey` passed through the `Authorization` header (format `bearer %APiKeys%`). Each ApiKey is then associated to claims (`MerchantId` for example)
- Exceptions are caught by the `ErrorHandlingMiddleware`
- Model binding errors (=missing required parameters) are caught by the `ModelBindingValidatorFilter`

### Logs

I used Serilog for his structured logs support.

- `ContextLoggingFilter` logs all incoming requests and associated response (=result)
- `UseSerilogRequestLogging` (middleware) logs the response time

One common scenario is to push these logs to `AWS CloudWatch`. Since all logs are JSON object, we can query our logs with the CW Logs syntax.

Example: `{ $.Elapsed > 10 }`

### CI

The JSON file `XXX` defines the structure of the generic CI pipeline I use:

1. Starts when Github triggers `CodePipeline` through a `Webhook`
2. Builds it within a docker container and push the newly created docker image to our ECR repository
3. Adds the `Staging` tag to this image
4. Starts the staging deployment by creating a new Task definitions and forcing a new deployment in the associated service
5. Requires a manual approval (SNS notification)
6. Adds the `Production` tag to this image
7. Starts the production deployment by creating a new Task definitions and forcing a new deployment in the associated service

Note: Both `XXX.json` and `XXX.json` are designed to be processed by a script replacing available variables (%XXX%) and calling the AWS CLI. A better option could be to use a CloudFormation template.

### Docker

Docker images are build using the `dockerfile` and `buildspec.yml` provided in this repository. 

- `builspec.yml` contains the commands to run the `docker build` command and push the created image to an ECR respository. `CodeBuild` relies on this file to build the source code 
- `Dockerfile` contains all the steps required to build a new image

Eventually two files are returned as artifacts:

1. definition.uat.json contains the path to the docker image to use in Staging (image tag = staging)
2. definition.prd.json contains the path to the docker image to use in Production (image tag = production)

## Improvements

- Supports 3D Secure
- Supports more payment methods
- Persists data in a NoSQL/tradition database. Currently we use a InMemoryCache to persist data
- Deploy API and associated infrastructure with CloudFormation template
