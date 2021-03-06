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
EXPOSE 8082
ENV ASPNETCORE_URLS=http://*:8082

FROM --platform=${BUILDARCH} mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["./function/Function.csproj", "."]
COPY ./map-platform.sh map-platform.sh
RUN chmod +x ./map-platform.sh
RUN dotnet restore --runtime $(./map-platform.sh) "./Function.csproj"
COPY ./function .
WORKDIR "/src/."
RUN dotnet build --runtime $(./map-platform.sh) "Function.csproj" -c Release -o /app/build

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
ENV upstream_url="http://localhost:8082"
EXPOSE 8080

HEALTHCHECK --interval=3s CMD [ -e /tmp/.lock ] || exit 1

CMD ["fwatchdog"]
