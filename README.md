# LARP Landing

[![Build and Push via Docker](https://github.com/maragnus/LarpDev/actions/workflows/docker-build-and-push.yml/badge.svg)](https://github.com/maragnus/LarpDev/actions/workflows/docker-build-and-push.yml)

## Introduction

Formerly called Mystwood Landing, LARP Landing is a web application that hosts characters for LARP games. This revision is redesigned to utilize MongoDB (opposed to TSQL) and support additional games and editions (both Mystwood 5e and Mystwood 2).

## Explicit Open Source Dependencies
These projects are explicitly (intentionally selected) dependencies of this application. They may not include all implicitly included dependencies by these projects.

* [ASP.NET Core 6.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-6.0) provides the framework for the server-side application written in C#
* [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) is a feature of ASP.NET for building interactive web UIs using C# instead of JavaScript. It's real .NET running in the browser on WebAssembly.
* [MudBlazor](https://mudblazor.com/) is the component library for the front-end
* [MongoDB](https://www.mongodb.com/) provides server-side database 
  * [Mongo CSharp Driver](https://github.com/mongodb/mongo-csharp-driver)
  * [Mongo2Go](https://github.com/Mongo2Go/Mongo2Go) provides MongoDB server for slightly-integrated unit testing that includes database functionality
* [Moq](https://github.com/moq/moq) provides mocking writing C# unit tests
* [EPPlus](https://www.epplussoftware.com/) reads and writes Excel workbooks

# Components

## cs/Larp.Common

This library contains utilities and structures common to other assemblies. Classes should be common to two or more other assemblies to be present here.

## cs/Larp.Data

This library represents the MongoDb data and any services that exist entirely within the data. Common and game-specific objects are represented here.

`LarpContext` should be configured with `LarpDataOptions` for a MongoDb connection string and database name.

`LarpContext` represents data structures common to all support games
`LarpContext.FifthEdition` represents Mystwood 5th Edition data structures.

## cs/Larp.Data.Seeder

This library is responsible for seeding production game data into the database. It should be called on application startup and for test fixtures.
`cs/Larp.Data.Seeder/LarpData.json` contains all data that will be imported

## cs/Larp.Data.TestFixture

This library creates a test MongoDb using [Mongo2Go](https://github.com/Mongo2Go/Mongo2Go) and optionally seeds the database. The `LarpDataTestFixture` class should be Disposed after a test is run.

## cs/Larp.Landing.Server

This ASP.NET application contains the business logic for translating RESTful API calls to MongoDb, validating character submissions, and authentication.

It also hosts the client-side application

## cs/Larp.Landing.Client

This Blazor web application that runs WASM in the browser.

## cs/Larp.Landing.Shared

Common structures used for communication between the client and server.

## eng

This directory is used for engineering scripts
