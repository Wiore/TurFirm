using System;
using Microsoft.Data.Sqlite;

namespace DataBaseControl
{
    public class DataBase
    {
        private string _connectionString = "Data Source=TourFirm.db";

        private SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using (var pragma = connection.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }

            return connection;
        }

        public void GenerateDataBase()
        {
            using var connection = GetConnection();

            string sql = @"
                DROP TABLE IF EXISTS ProposalItems;
                DROP TABLE IF EXISTS Tours;
                DROP TABLE IF EXISTS TourProposals;
                DROP TABLE IF EXISTS TourRequests;
                DROP TABLE IF EXISTS Users;
                DROP TABLE IF EXISTS Offices;

                CREATE TABLE Offices (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Address TEXT NOT NULL
                );

                CREATE TABLE Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    Role INTEGER NOT NULL,
                    OfficeId INTEGER,
                    WorkstationId TEXT,
                    FOREIGN KEY (OfficeId) REFERENCES Offices(Id) ON DELETE SET NULL
                );

                CREATE TABLE TourRequests (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER NOT NULL,
                    Destination TEXT NOT NULL,
                    DurationDays INTEGER NOT NULL,
                    Wishes TEXT,
                    Status INTEGER NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    AssignedEmployeeId INTEGER,
                    FOREIGN KEY (ClientId) REFERENCES Users(Id) ON DELETE CASCADE,
                    FOREIGN KEY (AssignedEmployeeId) REFERENCES Users(Id) ON DELETE SET NULL
                );

                CREATE TABLE TourProposals (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RequestId INTEGER NOT NULL,
                    EmployeeId INTEGER NOT NULL,
                    Message TEXT,
                    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    ClientDecision INTEGER,
                    FOREIGN KEY (RequestId) REFERENCES TourRequests(Id) ON DELETE CASCADE,
                    FOREIGN KEY (EmployeeId) REFERENCES Users(Id) ON DELETE CASCADE
                );

                CREATE TABLE ProposalItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProposalId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Price REAL NOT NULL,
                    FOREIGN KEY (ProposalId) REFERENCES TourProposals(Id) ON DELETE CASCADE
                );

                CREATE TABLE Tours (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClientId INTEGER NOT NULL,
                    RequestId INTEGER,
                    Title TEXT NOT NULL,
                    Country TEXT NOT NULL,
                    StartDate DATETIME NOT NULL,
                    EndDate DATETIME NOT NULL,
                    Status INTEGER NOT NULL,
                    FOREIGN KEY (ClientId) REFERENCES Users(Id) ON DELETE CASCADE,
                    FOREIGN KEY (RequestId) REFERENCES TourRequests(Id) ON DELETE SET NULL
                );
            ";

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();

            Console.WriteLine("База создана");
        }

        public void InsToUser(string login, string password, string fullname, string email, string phone, int role = 0, int? officeid = null, string? workstation = null)
        {
            using var connection = GetConnection();

            string sql = @"INSERT INTO Users (Login, Password, FullName, Email, Phone, Role, OfficeId, WorkstationId) 
                   VALUES (@login, @password, @fullname, @email, @phone, @role, @officeid, @workstation)";

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@fullname", fullname);
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@role", role);
            command.Parameters.AddWithValue("@officeid", (object?)officeid ?? DBNull.Value);
            command.Parameters.AddWithValue("@workstation", (object?)workstation ?? DBNull.Value);

            command.ExecuteNonQuery();
            Console.WriteLine($"Пользователь {fullname} добавлен");
        }
        public void InsToOffices(string name, string address)
        {
            using var conection = GetConnection();

            string sql = @"INSERT INTO Offices (Name, Address)
                VALUES (@name, @address)";
            
            using var command = conection.CreateCommand();
            command.CommandText = sql;

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@address", address);

            command.ExecuteNonQuery();
            Console.WriteLine($"Офис {name} добавлен");
        }
    }
}