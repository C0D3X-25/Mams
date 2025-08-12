# Mams

## Description
This project will only work on Windows.
You need DotNet 9.0 to to run it.

## External Libraries
- MySqlConnector
- MySqlBackup for MySqlConnector
- QuestPDF

## Dev
This project is a WPF application that uses the MVVM pattern. It is designed to be a simple and easy-to-use application for managing data. The project is written in C# and use MySQL has a local server.
The Datatase dump is in the `Database` folder, you can import it in your MySQL server. The database is called `mams`.

### Schemas
The Database schema, UML of Classes can be found in the `Schemas` folder. 
The files must be opened with Draw.io (https://app.diagrams.net/).

(Has Errors) The files must be opened with the free application: `Software Ideas Modeler` (https://www.softwareideas.net/).`

### Files
- End with `Model` is for the logic of the app, this is where the SQL queries are defined.
- End with `Item` is a element who hold multiples variables. Used in the binding and they are transfered between Classes. 
- End with `Page` it's a front end page in the app, they are used in a frame who is instanciate in `MainWindow.xaml` file.
- End with `Controller` it's the Model-View in the MVVM pattern, but I prefer the name Controller. Every Page has is data Binded to the corresponding Controller.
- Start with `Base` is a base Class for the other Classes to inherit from.
- Start with `I` is an Interface.
- Start with `A` its an Abstract Class, it can't be instantiated.
- Start with `E` is an Enum.
- Start with `S` is a Static Class.
- Start with `UC` is a User Control, used in the front end.

## Installer
The installation is made under ~C:/Program Files/JTH/mams/~ and there is a shortcut on the desktop.

This application needs the `service mysql` for the interactions with the database, which can be found in this installer. You can add MySQL Workbench for later.

https://dev.mysql.com/downloads/installer/

Create the database by using the file in `CreateDatabaseMams.sql` which is in the folder database.

Password of the database can be updated in the file `SQLConnectionModel.cs` which is in the path: ~/src/databaseConnections/~

## Backup
Each time the application is launched there is a backup (SQL dump) of the database which is made, ONLY if there is none made the same day.

The backups are saved in this path ~C:/MySqlBackup/~ (folder is created automatically if it doesn't exist).
Destination path can be modified in the file `SDatabaseBackup.cs` which is in the path: ~/src/databaseOperations/~.

Each backup file is named `backup_dd_MM_yyyy.sql`.