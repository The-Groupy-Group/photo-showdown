using Microsoft.Data.SqlClient;
using PhotoShowdownBackend.Consts;
using Serilog;
using Serilog.Extensions.Logging;

namespace PhotoShowdownBackend.Utils;

/// <summary>
/// A class that executes all SQL scripts in the SQL folder that were never executed before.
/// </summary>
public class SQLServerMigrationExecutor
{
    private readonly string connectionString;
    private readonly ILogger<SQLServerMigrationExecutor> _logger = new SerilogLoggerFactory().CreateLogger<SQLServerMigrationExecutor>();
    public SQLServerMigrationExecutor(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /// <summary>
    /// Builds the DBScripts table if it doesnt exist.
    /// Executes all SQL scripts in the SQL folder that were never executed before and adds them to the DBScripts table.
    /// </summary>
    public void ExecutePendingScripts()
    {
        _logger.LogInformation($"{nameof(ExecutePendingScripts)} Start");
        using SqlConnection connection = new(connectionString);
        connection.Open();

        bool dbScriptsTableExists = TableExists(connection, "DBScripts");

        if (!dbScriptsTableExists)
        {
            _logger.LogInformation("DBScripts folder doesnt exist. Creating it.");
            CreateDBScriptsTable(connection);

            int numOfTables = NumOfTables(connection);
            if (numOfTables > 1)
            {
                _logger.LogInformation("DB was already build but no migration was ever tracked, Adding all scripts to DBScripts without executing.");

                IterateAllScripts(connection, execute: false);
                return;
            }
        }
        _logger.LogInformation("Looking for scripts that were never executed before.");
        IterateAllScripts(connection, execute:true);
    }

    private void IterateAllScripts(SqlConnection connection, bool execute)
    {
        var scriptFiles = Directory.GetFiles(SystemSettings.SQLScriptsFolderName, "*.sql");
        Array.Sort(scriptFiles, (x, y) => GetScriptBatchFromFileName(x).CompareTo(GetScriptBatchFromFileName(y)));

        foreach (var scriptFile in scriptFiles)
        {
            int scriptBatch = GetScriptBatchFromFileName(scriptFile);
            string scriptName = Path.GetFileName(scriptFile);

            // Check if the script is already in DBScripts table
            if (!IsScriptInDBScriptsTable(connection, scriptBatch, scriptName))
            {
                if (execute)
                {
                    _logger.LogInformation($"Script {scriptName} has never been executed. Executing it.");

                    ExecuteScript(connection, scriptFile);
                }
                _logger.LogInformation($"Adding script {scriptName} to DBScripts table.");
                // Add script to DBScripts table
                AddScriptToDBScriptsTable(connection, scriptBatch, scriptName);
            }
        }
    }

    private static bool TableExists(SqlConnection connection, string tableName)
    {
        var query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
        using var command = new SqlCommand(query, connection);
        return (int)command.ExecuteScalar() > 0;
    }

    private static int NumOfTables(SqlConnection connection)
    {
        var query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES";
        using var command = new SqlCommand(query, connection);
        return (int)command.ExecuteScalar();
    }

    private static void CreateDBScriptsTable(SqlConnection connection)
    {
        var createTableQuery = @"
            CREATE TABLE DBScripts (
                Id INT PRIMARY KEY IDENTITY,
                ScriptBatch INT NOT NULL,
                ScriptName NVARCHAR(255) NOT NULL
            );
        ";

        using var command = new SqlCommand(createTableQuery, connection);
        command.ExecuteNonQuery();
    }

    private static int GetScriptBatchFromFileName(string fileName)
    {
        // Extract the script number from the file name
        if (int.TryParse(Path.GetFileNameWithoutExtension(fileName)?.Split('_')[0], out var scriptBatch))
        {
            return scriptBatch;
        }
        throw new Exception($"SQL File named {fileName} has invalid name (no batch).");
    }

    private static bool IsScriptInDBScriptsTable(SqlConnection connection, int scriptBatch,string scriptName)
    {
        var query = $"SELECT COUNT(*) FROM DBScripts WHERE ScriptBatch = {scriptBatch} AND ScriptName = '{scriptName}'";
        using var command = new SqlCommand(query, connection);
        return (int)command.ExecuteScalar() > 0;
    }

    private static void ExecuteScript(SqlConnection connection, string scriptPath)
    {
        // Read the script file
        string script = File.ReadAllText(scriptPath);
        // Split the script into commands
        string[] commands = script.Split("GO", StringSplitOptions.RemoveEmptyEntries);
        // Execute each command
        foreach (string command in commands)
        {
            using var sqlCommand = new SqlCommand(command, connection);
            sqlCommand.ExecuteNonQuery();
        }
    }

    private static void AddScriptToDBScriptsTable(SqlConnection connection, int scriptBatch, string scriptName)
    {
        var insertQuery = $"INSERT INTO DBScripts (ScriptBatch, ScriptName) VALUES ({scriptBatch}, '{scriptName}')";
        using var command = new SqlCommand(insertQuery, connection);
        command.ExecuteNonQuery();
    }
}
