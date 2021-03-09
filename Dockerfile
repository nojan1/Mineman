FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

RUN apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs

COPY Backend/Mineman.Common/Mineman.Common.csproj Mineman.Common/
COPY Backend/Mineman.Service/Mineman.Service.csproj Mineman.Service/
COPY Backend/Mineman.Service.Tests/Mineman.Service.Tests.csproj Mineman.Service.Tests/
COPY Backend/Mineman.Web/Mineman.Web.csproj Mineman.Web/
COPY Backend/Mineman.WorldParsing/Mineman.WorldParsing.csproj Mineman.WorldParsing/
COPY Backend/Mineman.WorldParsing.Tests/Mineman.WorldParsing.Tests.csproj Mineman.WorldParsing.Tests/
COPY Backend/TinyTokenIssuer/TinyTokenIssuer.csproj TinyTokenIssuer/
COPY Backend/Mineman.sln ./
COPY Backend/Nuget.config ./

RUN dotnet restore Mineman.sln

COPY Backend/Mineman.Common/* Mineman.Common/
COPY Backend/Mineman.Service/* Mineman.Service/
COPY Backend/Mineman.Service.Tests/* Mineman.Service.Tests/
COPY Backend/Mineman.Web/* Mineman.Web/
COPY Backend/Mineman.WorldParsing/* Mineman.WorldParsing/
COPY Backend/Mineman.WorldParsing.Tests/* Mineman.WorldParsing.Tests/
COPY Backend/TinyTokenIssuer/* TinyTokenIssuer/

RUN cd Mineman.Web && dotnet publish -c Release -o ../out
COPY Extra/appsettings.docker.json /app/out/appsettings.json
COPY Backend/Mineman.WorldParsing/Resources /app/out/Resources
COPY Backend/Mineman.Server/Resources /app/out/Resources

COPY Frontend ./Frontend
COPY Extra/.env.docker /Frontend/.env
RUN cd Frontend && npm install && npm run build
RUN cp -r Frontend/build /app/out/wwwroot

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Mineman.Web.dll"]

#EXPOSE 5000/tcp
#EXPOSE 5001/tcp
EXPOSE 80/tcp
VOLUME /data
VOLUME /var/run/docker.sock
