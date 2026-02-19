# Book Management Thingy

Project created as per assigment for SCIO job application.

## TO DO
- [ ] Test project
- [ ] Web API
- [x] Figure out clean publish
- [x] Config
    - [x] Add sqlite path to config

## Usage

There are two ways of interacting with the application - CLI and a Web API.

### CLI
To build the CLI application, make sure you have .NET 10 SDK installed, then go to the BMT.CLI directory and run

`dotnet publish -c Release -r {{TARGET-OS}} --self-contained true`

with the {{TARGET-OS}} replaced by your runtime of choice (I use linux-x64, for windows you might wanna use win-x64, or see [this page](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog))

After you publish the application, go to bin/Release/net10.0/{{TARGET-OS}}/publish and look for file bmt (or bmt.exe if you are on windows)

Run the application in terminal and just follow the usage info. Few examples:

`./bmt add "The Winds of Winter" "George R. R. Martin" 113-5-65411-987-6 2047-02-06 3`

`./bmt list -a george`

`./bmt lend 113-5-65411-987-6`

Alternatively you can just run the debug version of the CLI by running `dotnet run` in the BMT.CLI directory. Just keep in mind to put your arguments after `--` (eg. `dotnet run -- list -a george`) and the application will run noticeably slower.

### Web API

### Database

Both the CLI and the Web API use SQLite for data persistence. By default the database file is located right in the root of the app as `bmt_db` (either in the project root when debugging or in the publish directory for built projects)

You can specify your own path to the database file in `appsettings.json > ConnectionStrings > DefaultConnection`.

There is also a prepared db file `bmt_db_seeded` which contains some test data so that you don't have to populate the db yourself. Simply put an absolute path to it in the config file mentioned above.

### Tests
