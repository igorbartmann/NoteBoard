# NoteBoard

NoteBoard is a cloud-based note management platform designed to help users organize and track notes within an intuitive, personalized and real-time workspace.

## User Story

1. Create User
As a new site visitor, 
I want to create an account by providing my email and a secure password, 
So that I can manage my notes online.

2. Create Notes
As a user, 
I want to create a new note with a title, description and color, 
So that I can quickly capture important ideas or tasks before I forget them.

3. Visualize Notes
As a registered user,
I want to view all my notes displayed on an online board, 
So that I can easily check my routine and tasks from anywhere.

4. Update Notes
As a registered user,
I want to edit my existing notes,
So that my board stays up to date.

5. Complete Notes
As a registered user,
I want to complete my existing notes,
So that my board stays up to date.

6. Delete Notes
As a registered user,
I want to delete existing notes,
So that my board stays free of outdated information.

## Development Stack

### Database

The database runs on a Microsoft SQL Server instance, containing two tables as follow:

**User**:

| COLUMN    | TYPE              |
| --------  | ----------------- |
| Id        | INT               |
| Name      | NVARCHAR(50)      |
| Email     | NVARCHAR(254)     |
| Password  | NVARCHAR(256)     |
| CreatedAt | DATETIMEOFFSET(3) |
| UpdatedAt | DATETIMEOFFSET(3) |

**Note**:

| COLUMN      | TYPE              |
| ---------   | ----------------- |
| Id          | INT               |
| Title       | NVARCHAR(50)      |
| Content     | NVARCHAR(350)     |
| Color       | INT               |
| IsCompleted | BIT               |
| CreatedAt   | DATETIMEOFFSET(3) | 
| CreatedBy   | INT               |
| UpdatedAt   | DATETIMEOFFSET(3) |

To improve project organization and database versioning, I have also used a SQL Server Database Project.

### Backend (API)

The backend was developed using a Web API project of ASP.NET Core using the clean architecture pattern and organized with the following layers.
- Domain
- Data
- Application
- API
- Tests

The project uses additional frameworks like `Entity Framework Core`, an Object Relation Mapping (ORM) to facilitate the interaction between the backend and database.

> .NET Version: 10

### Frontent

The frontend was developed using Angular framework, which internally uses HTML, CSS and Typescript.

Angular Version: 18

## Tradeoffs

### Database

#### Server
Since for this prject we have a uniformed data structure with a clear relation, I decided to use a relational database to informe this data structure and make relations clear.

We have different great options, like SQL Server and PostgreSQL, which are very similar. The SQL Server was used due to my personal preferences related to be the database I have most used for other projects.

#### Column Length

Some column total length were choosen based on research I made. 

**Email:** The Simple Mail Transfer Protocol (SMTP) limits the total length to 256 characters, including the `<` and `>` symbols. So, we have used a 254 characters as limit to ensure the protocol limit safeability.

**Password:** This project uses the `PasswordHasher` of Microsot.AspNetCore.Identity for password hash. It internally uses `HMAC-SHA256`, which produces a 64 hexadecimal characters. ASP.NET Core combines this with a salt and irataion count to increase security, producing a 84 characters length result. However, the Open Worldwide Application Security Project (`OWASP`) recommends to store passwords in a way that allow safe migration to stronger algorithms in the future. Following this concept and the standard industry store database column length recommendation to store cryptographed password, this project uses a 256 characters length.

### Object Relational Mapping (ORM)

For this project, I evaluate two main ORMs, Dapper and Entity Framework Core. Since the project follow only CRUD operations of simple entities, which does not requires special performance paths, Entity Framework Core was choosen to make the source-code simple and accelerate the program development.

### API

Althougth I could use market libraries, such as AutoMapper and FluentValidator, I choose by implement most system flows from scratch for this activity.

Related to AutoMapper, for example, I performed the mapper internally in the source-code to avoid the dependency from AutoMapper package. Since the entities are very simple, there is no needing of an external library. Besides, by doing the mapping internally, we can improve performance. So in this case, using AutoMapper would produce almost no benefits.

I also did not use annotations to validate models. So, I created my own validators.

### Tests

SQL Server vs InMemoryDatabase
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.InMemory

For unit tests, I have choosen InMemory database, in order to enable test be run and re-run without the needing of rollback the database state.

## TODOS

In the case I had more time to complete the activity, I would also invest time in the following topics:
1. Confirm user account by email.
2. Add user status, such as: WaitingConfirmation, Active and Blocked.
3. Share notes from one user to other users.
4. Extent support to multiple idiomas.
5. Improve authentication methods, such as implementing rate limit for login (currently it is possible to brute force without any blocking from api side), logout action and also the token revocation.
6. Implement logs.

## HOW TO RUN

### Backend

1. Clone the repository from Github.
2. Build the application (`dotnet build`)
3. Run the server (`dotnet run --project ./NoteBoard.Api`)

### Frontent

1. Clone the repository from Github.
2. Install packages (`npm install`)
3. Run the server (`ng serve`)
