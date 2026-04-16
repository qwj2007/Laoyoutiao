using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImageImportToDB
{
    using Dapper;
    using Microsoft.Data.Sqlite;

    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string dbPath = "users.db")
        {
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserName TEXT NOT NULL,
                Nickname TEXT NOT NULL,
                AvatarPath TEXT,
                Address TEXT
            )");
        }

        public async Task<int> InsertUserAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            const string sql = @"
            INSERT INTO Users (UserName, Nickname, AvatarPath, Address)
            VALUES (@UserName, @Nickname, @AvatarPath, @Address);
            SELECT last_insert_rowid();";
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }
    }
}
