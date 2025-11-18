# Dotnet kafka project

## Prerequisites
- Make sure you have Docker installed and running on your machine.
- dotnet 10 SDK installed.
- dotnet aspire template installed. You can install it using the following command:
  ```
  dotnet new install Aspire.Dotnet.Templates
  ```
- install dotnet aspire. You can install it using the following command:
  ```
  cli dotnet tool install -g Aspire.Cli
  ```
- check if aspire is installed correctly by running:
  ```
  aspire --version
  ```

## How to run
- Restore all the dependencies by running this command on the same level with `.sln` file:
  ```
  dotnet restore
  ```
- Build the solution by running this command:
  ```
  dotnet build
  ```
- Run dotnet aspire project by running this command:
  ```
  aspire run
  ```
