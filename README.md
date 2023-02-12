# LARP Landing

[![Build and Push via Docker](https://github.com/maragnus/LarpDev/actions/workflows/docker-build-and-push.yml/badge.svg)](https://github.com/maragnus/LarpDev/actions/workflows/docker-build-and-push.yml)

## Introduction

Formerly called Mystwood Landing, LARP Landing is a web application that hosts characters for LARP games. This revision is redesigned to utilize MongoDB (opposed to TSQL) and support additional games and editions (both Mystwood 5e and 6e).

## Explicit Open Source Dependencies
These projects are explicitly (intentially selected) dependencies of this application. They may not include all implicitly included dependencies by these projects.

* [protobuf](https://developers.google.com/protocol-buffers) provides common data structures between C# and TypeScript
* [ASP.NET Core 6.0](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-6.0) provides the framework for the server-side application written in C#
* [React](https://reactjs.org/) provides presentation layer for the client-side application
* [MongoDB](https://www.mongodb.com/) provides server-side database 
  * [Mongo CSharp Driver](https://github.com/mongodb/mongo-csharp-driver)
  * [Mongo2Go](https://github.com/Mongo2Go/Mongo2Go) provides MongoDB server for slightly-integrated unit testing that includes database functionality
* [Moq](https://github.com/moq/moq) provides mocking writing C# unit tests

# Components

## proto/larp

This is a [protobuf](https://developers.google.com/protocol-buffers) package that includes the data structures necessary for the web application to authenticate users and list games and events.

## proto/larp/FifthEdition

This is a [protobuf](https://developers.google.com/protocol-buffers) package that includes all data structures specific to Mystwood 5th edition, such as characters, skills, occupations, gifts, spells, and so on.

## ts/landing

This is the React client-side application

## ts/proto

This is the client-side Typescript library containing the GRPC client and generated data structures from the contents of `/proto`.

The generated files are updated with `eng/protobuild.ps1` or `eng/protobuild.sh` scripts when `/proto/**/*.proto` is updated

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

## cs/Larp.Protos

This library contains the generated server-side GRPC classes and data structures from `proto/**/*.proto`. It is updated automatically by MSBuild.

## cs/Larp.WebService

This ASP.NET application hosts GRPC and contains the business logic for translating GRPC calls to MongoDb, validating character submissions, and authentication.

It also hosts the React site in `ts/landing`

## eng

This directory is used for engineering scripts
