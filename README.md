# ReleaseNotes Hub

This is a small web app designed to host release notes for all versions of many projects.

# Stack

It's a .NET Core app, using API Controllers for the backend, with a React frontend.

MongoDB is used for persistence. Mongo 3.6+ is required since we use ArrayFilters.

The app gets its mongo connection string from the config path `ConnectionStrings:ReleaseNotesHub`. You'll want to set this as an **environment variable** either in `Dockerfile` / `docker-compose`, `Procfile`, some other infrastructure config, or simply directly on the host machine. The `DatabaseName` is configurable and as this is unlikely to be sensitive, can be changed in `appsettings.json`.

# TODO

The api routes are just coming together, so they need finishing.

The frontend is unstarted.

There will be a Dockerfile and probably some compose file(s), since we will run this internally in a Docker environment.

Maybe Swagger for the API

Auth

# Usage

writes are API only, not frontend, so grab your favourite HTTP client and:

- POST `/projects` to create a project
- PUT `/projects/{id}` to create a project with a specified id
- PUT `/projects/{id}/{release}` to add release notes for a specified release of an existing project

reads are frontend based, though it hits the API to get viewmodel data.
