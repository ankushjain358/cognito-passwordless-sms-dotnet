# Cognito Passwordless SMS Auth (.NET API)

A minimal, production-ready ASP.NET Core Web API demonstrating passwordless SMS OTP authentication using Amazon Cognito's choice-based sign-in (`USER_AUTH`) pipeline. 

This repository allows for instant user onboarding without registration verification codes, shifting identity confirmation directly to the login flow.

## 🛠️ Tech Stack
* **Framework:** .NET 8.0 / .NET 9.0 (ASP.NET Core Web API)
* **SDK:** official AWSSDK.CognitoIdentityProvider
* **Cloud Provider:** Amazon Web Services (AWS Cognito & SNS)

## 🚀 Getting Started

### 1. Prerequisites
* [AWS Account](https://amazon.com) with an Amazon Cognito User Pool.
* The User Pool must have **Choice-based sign-in (ALLOW_USER_AUTH)** enabled on your App Client.
* Your test phone number must be added to the **AWS SNS SMS Sandbox**.

### 2. Configuration
Clone this repository and update the App Client ID inside `Controllers/AuthController.cs`:

```csharp
private readonly string _clientId = "YOUR_AWS_COGNITO_APP_CLIENT_ID";
```

Ensure your local environment can resolve your standard AWS credentials (via AWS CLI profile, environment variables, or local shared credentials file).

### 3. Run the Application
Restore packages and boot up the Web API locally:

```bash
dotnet restore
dotnet run
```

Once running, navigate to `https://localhost:<port>/swagger` in your browser to test the API endpoints using the Swagger UI.

## 📁 Endpoints Matrix

* **`POST /api/auth/signup`** - Registers the user instantly using only a phone number.
* **`POST /api/auth/login`** - Initiates choice-based login and dispatches the SMS OTP code.
* **`POST /api/auth/verify-login`** - Validates the OTP code against the tracking session state and issues JWT access tokens.

## 📄 License
This project is licensed under the MIT License.
