# Mams

## Description
This project will only work on Windows.
You need DotNet 9.0 to to run it.

## Dev
This project is a WPF application that uses the MVVM pattern. It is designed to be a simple and easy-to-use application for managing data. The project is written in C# and use MySQL has a local server.
The Datatase dump is in the `Database` folder, you can import it in your MySQL server. The database is called `mams`.

### Schemas
The Database schema, UML of Classes can be found in the `Schemas` folder. 
The files must be open with Draw.io (https://app.diagrams.net/).
(Has Errors) The files must be open with the free application: `Software Ideas Modeler` (https://www.softwareideas.net/).`

### Files
- End with `Model` is for the logic of the app, this is where the SQL queries are defined.
- End with `Item` is a element who hold multiples variables. Used in the binding and they are transfered between Classes. 
- End with `Controller` it's the Model-View in the MVVM pattern, but I prefer the name Controller. Every Page has is data Binded to the corresponding Controller.
- End with `Page` it's a front end page in the app, they are used in the `MainWindow.xaml` file.
- Start with `Base` is a base Class for the other Classes to inherit from.
- Start with `I` is an Interface.
- Start with `A` its an Abstract Class, it can't be instantiated.
- Start with `E` is an Enum.
- Start with `S` is a Static Class.
- Start with `UC` is a User Control, used in the front end.