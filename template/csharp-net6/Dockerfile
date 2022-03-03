FROM --platform=${BUILDARCH} alpine:3.15.0 as watchdog
ARG TARGETARCH
RUN apk --no-cache add curl \
    && echo "Pulling watchdog binary from Github." \
    && curl -sSL https://github.com/openfaas-incubator/of-watchdog/releases/download/0.9.3/fwatchdog-${TARGETARCH} > /usr/bin/fwatchdog \
    && chmod +x /usr/bin/fwatchdog \
    && cp /usr/bin/fwatchdog /home/app \
    && apk del curl --no-cache

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./function/Function.csproj", "."]
RUN dotnet restore "./Function.csproj"
COPY ./function .
WORKDIR "/src/."
RUN dotnet build "Function.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Function.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=watchdog /usr/bin/fwatchdog  /usr/bin/
RUN addgroup --system app \
    && adduser --system --ingroup app app

WORKDIR /app
COPY --from=publish /app/publish .
RUN chown app:app -R /app

USER app

ENV fprocess="dotnet ./Function.dll"
ENV cgi_headers="true"
ENV mode="http"
ENV upstream_url="http://localhost:80"
EXPOSE 8080

HEALTHCHECK --interval=3s CMD [ -e /tmp/.lock ] || exit 1

CMD ["fwatchdog"]
