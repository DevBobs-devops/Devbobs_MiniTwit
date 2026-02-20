# base docker images  with .NET core runtime & sdk. References are used later.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# https://medium.com/@aliyildizoz/understanding-asp-net-core-dockerfile-a523233bb9a4 <- guide
# https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=linux&pivots=dotnet-8-0 <-guide
# https://hub.docker.com/r/microsoft/dotnet-runtime <- dockers


# copy the solution, tests and projects
COPY MiniTwit.sln MiniTwit.sln
RUN mkdir src
RUN mkdir test
COPY ./test test/ 
COPY ./src/ src/

#restore dependencies
RUN dotnet restore

# set work directory to chirp.web - so we can run dotnet build & publish
WORKDIR /src/MiniTwit.Web/

#expose port 8080. Can access -p 8080:8080 when running the image.
EXPOSE 8080

# rest is taken from: https://stackoverflow.com/questions/74382131/docker-net-the-command-could-not-be-loaded-while-dotnet-and-dll-file-are-pre

#build project
RUN dotnet build "MiniTwit.Web.csproj" -c Release -o /app/build

#use build stage and create new stage publish. Then publish
FROM build AS publish
RUN dotnet publish "MiniTwit.Web.csproj" -c Release -o /app/publish

#use base/runtime image that has less garbage laying around to run the image.
FROM base AS final
WORKDIR /app

# folder for database
RUN mkdir -p /app/Data

#copy the publish from publish-stage into the final stage.
COPY --from=publish /app/publish .

#container starts at the .dll
ENTRYPOINT ["dotnet", "MiniTwit.Web.dll"]