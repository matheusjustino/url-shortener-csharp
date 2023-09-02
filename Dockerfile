FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT "Production"

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

COPY --from=build /app/out ./

ENV ASPNETCORE_ENVIRONMENT "Production"
ENV ASPNETCORE_URLS "http://+:80"

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "url-shortener.dll"]