using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParametrizedRequest
{
    class CommandProvider
    {
        private readonly SqlConnection _connection;

        public CommandProvider(SqlConnection conenction) => _connection = conenction;

        public CommandProvider(string connStr) => _connection = new SqlConnection(connStr);

        public async Task<string> InputNewRequest()
        {
            Console.WriteLine("Input your parametrized request");
            var userCommand = Console.ReadLine() ?? throw new ArgumentNullException("User command can't be null");
            var command = _connection.CreateCommand();
            command.CommandText = userCommand;

            var parameters = Regex.Matches(userCommand, "@[a-z]+", RegexOptions.IgnoreCase);
            foreach (var parameter in parameters)
            {
                Console.Write($"Enter parameter data ({parameter}): ");
                var inputParam = Console.ReadLine();
                command.Parameters.AddWithValue(parameter.ToString(), inputParam);
            }

            return await ExecuteCommand(command);
        }

        private async Task<string> ExecuteCommand(SqlCommand command)
        {
            try
            {
                await _connection.OpenAsync();

                var result = new StringBuilder();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    result.AppendLine(reader.HasRows
                        ? "Id\tText\tAuthor\tRecordDate"
                        : $"Records affected: {reader.RecordsAffected.ToString()}");

                    while (reader.Read())
                    {
                        var record = new Record
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Text = reader["Text"] as string,
                            Author = reader["Author"] as string,
                            RecordDate = (DateTime)reader["RecordDate"]
                        };
                        result.AppendLine($"{record.Id}\t{record.Text}\t{record.Author}\t{record.RecordDate}");
                    }
                }

                return result.ToString();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return String.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return String.Empty;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
            }
        }
    }
}
