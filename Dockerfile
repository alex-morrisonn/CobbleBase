# Use the official image as a parent image.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory.
WORKDIR /app

# Copy the file from your host to your current location.
COPY *.csproj ./

# Run dotnet restore to restore dependencies.
RUN dotnet restore

# Copy the rest of your app's source files.
COPY . ./

# Build the app.
RUN dotnet publish -c Release -o out

# Use the runtime image as a parent image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory.
WORKDIR /app

# Copy the build output from the build image.
COPY --from=build-env /app/out .

# Specify the command to run on container startup.
ENTRYPOINT ["dotnet", "stu-plugin-api.dll"]


