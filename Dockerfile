FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app
COPY . ./

RUN dotnet restore
RUN dotnet build -c Release -o out

RUN touch ./out/version && \
	git rev-parse HEAD >> ./out/version && \
	git log -1 | grep "Date:" | cut -d ' ' -f4- >> ./out/version && \
	git branch | grep \* | cut -d ' ' -f2- >> ./out/version


FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=build-env /app/out .
COPY ./init.sh .

RUN chmod +x ./init.sh

ENTRYPOINT ["./init.sh"]

