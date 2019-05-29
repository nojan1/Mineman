FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

RUN apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs

COPY Mineman.Common/Mineman.Common.csproj Mineman.Common/
COPY Mineman.Service/Mineman.Service.csproj Mineman.Service/
COPY Mineman.Service.Tests/Mineman.Service.Tests.csproj Mineman.Service.Tests/
COPY Mineman.Web/Mineman.Web.csproj Mineman.Web/
COPY Mineman.WorldParsing/Mineman.WorldParsing.csproj Mineman.WorldParsing/
COPY Mineman.WorldParsing.Tests/Mineman.WorldParsing.Tests.csproj Mineman.WorldParsing.Tests/
COPY Mineman.sln ./
COPY Nuget.config ./

RUN dotnet restore Mineman.sln

COPY Mineman.Common/* Mineman.Common/
COPY Mineman.Service/* Mineman.Service/
COPY Mineman.Service.Tests/* Mineman.Service.Tests/
COPY Mineman.Web/* Mineman.Web/
COPY Mineman.WorldParsing/* Mineman.WorldParsing/
COPY Mineman.WorldParsing.Tests/* Mineman.WorldParsing.Tests/

RUN cd Mineman.Web && dotnet publish -c Release -o ../out
COPY Extra/appsettings.docker.json /app/out/appsettings.json

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Mineman.Web.dll"]

#EXPOSE 5000/tcp
#EXPOSE 5001/tcp
EXPOSE 80/tcp
VOLUME /data
VOLUME /var/run/docker.sock
