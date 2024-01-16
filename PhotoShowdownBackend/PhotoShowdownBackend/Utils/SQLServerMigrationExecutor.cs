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
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        var dbScriptsTableExists = TableExists(connection, "DBScripts");

        if (!dbScriptsTableExists)
        {
            // If the DBScripts table doesnt exist, No migration was ever tracked
            // Create the DBScripts table
            _logger.LogInformation("DBScripts folder doesnt exist. Creating it.");
            CreateDBScriptsTable(connection);

            var numOfTables = NumOfTables(connection);
            if (numOfTables > 1)
            {
                // Handle edge case where the db was already build but no migration was tracked
                // Add all existing SQL files to DBScripts table without executing them (Need to make sure they were all executed manually)
                _logger.LogInformation("DB was already build but no migration was ever tracked, Adding all scripts to DBScripts without executing");
                AddScriptsToMigration(connection);
            }
        }

        // Execute all scripts that were never executed before
        _logger.LogInformation("Looking for scripts that were never executed before.");
        ExecuteAndAddScriptsToMigration(connection);
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

    private void AddScriptsToMigration(SqlConnection connection)
    {
        var scriptFiles = Directory.GetFiles(SystemSettings.SQLScriptsFolderName, "*.sql")
            .OrderBy(GetScriptBatchFromFileName);

        foreach (var scriptFile in scriptFiles)
        {
            var scriptBatch = GetScriptBatchFromFileName(scriptFile);
            var scriptName = Path.GetFileName(scriptFile);

            _logger.LogInformation($"Adding script {scriptName} to DBScripts table as if it was executed.");

            // Add script to DBScripts table
            AddScriptToDBScriptsTable(connection, scriptBatch, scriptName);
        }
    }

    private void ExecuteAndAddScriptsToMigration(SqlConnection connection)
    {
        var scriptFiles = Directory.GetFiles(SystemSettings.SQLScriptsFolderName, "*.sql");

        // Sort script files by their ScriptBatch
        var sortedScriptFiles = scriptFiles.OrderBy(GetScriptBatchFromFileName);

        foreach (var scriptFile in sortedScriptFiles)
        {
            var scriptBatch = GetScriptBatchFromFileName(scriptFile);
            var scriptName = Path.GetFileName(scriptFile);

            // Check if the script is already in DBScripts table
            if (!IsScriptInDBScriptsTable(connection, scriptBatch))
            {
                _logger.LogInformation($"Script name {scriptName} has never been executed. Executing it now and adding to DBScripts table.");
                // Execute the script
                ExecuteScript(connection, scriptFile);

                // Add script to DBScripts table
                AddScriptToDBScriptsTable(connection, scriptBatch, scriptName);
            }
        }
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

    private static bool IsScriptInDBScriptsTable(SqlConnection connection, int scriptBatch)
    {
        var query = $"SELECT COUNT(*) FROM DBScripts WHERE ScriptBatch = {scriptBatch}";
        using var command = new SqlCommand(query, connection);
        return (int)command.ExecuteScalar() > 0;
    }

    private static void ExecuteScript(SqlConnection connection, string scriptPath)
    {
        // Read the script file and execute it
        var script = File.ReadAllText(scriptPath);
        using var command = new SqlCommand(script, connection);
        command.ExecuteNonQuery();
    }

    private static void AddScriptToDBScriptsTable(SqlConnection connection, int scriptBatch, string scriptName)
    {
        var insertQuery = $"INSERT INTO DBScripts (ScriptBatch, ScriptName) VALUES ({scriptBatch}, '{scriptName}')";
        using var command = new SqlCommand(insertQuery, connection);
        command.ExecuteNonQuery();
    }
}
