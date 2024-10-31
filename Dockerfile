# Use the official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the csproj and restore dependencies
COPY ["Polyglot/*.csproj", "./Polyglot/"]
WORKDIR /app/Polyglot
RUN dotnet restore

# Copy the rest of the app source code
COPY . ./

# Build the project
RUN dotnet publish -c Release -o /out

# Use a smaller runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the build output from the previous stage
COPY --from=build-env /out .

# Run the app
ENTRYPOINT ["dotnet", "Polyglot.dll"]
