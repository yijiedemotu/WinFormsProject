using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp3.Model;

namespace WinFormsApp3.Helper
{
    public class SqlHelper
    {
        /// <summary>统一的时间格式，保证字符串排序等价于时间排序。</summary>
        public const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly string _connectionString;

        public SqlHelper(string dbPath = "data.db")
        {
            string fullPath = Path.IsPathRooted(dbPath)
                ? dbPath
                : Path.Combine(AppContext.BaseDirectory, dbPath);
            _connectionString = $"Data Source={fullPath}";
            EnsureCreated();
        }

        /// <summary>
        /// 建表 + 一次性迁移历史数据的时间格式。可重复调用（幂等）。
        /// </summary>
        public void EnsureCreated()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS DataModel (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    testTime TEXT,
                    projectName TEXT,
                    data1 TEXT,
                    data2 TEXT,
                    data3 TEXT,
                    data4 TEXT,
                    data5 TEXT,
                    data6 TEXT,
                    data7 TEXT,
                    data8 TEXT
                );";
                cmd.ExecuteNonQuery();
            }

            MigrateLegacyTimeFormat(connection);
        }

        /// <summary>
        /// 旧版本用 DateTime.Now.ToString() 写入（形如 2026/10/27 10:50:46），
        /// 与新格式混排会导致 ORDER BY 和范围查询出错，这里统一迁移一次。
        /// </summary>
        private void MigrateLegacyTimeFormat(SqliteConnection connection)
        {
            var legacy = new List<(long Id, string Raw)>();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, testTime FROM DataModel WHERE testTime LIKE '%/%'";
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    legacy.Add((Convert.ToInt64(reader["Id"]), reader["testTime"] as string ?? ""));
                }
            }

            foreach (var (id, raw) in legacy)
            {
                if (!DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsed)
                    && !DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    continue;
                }

                using var update = connection.CreateCommand();
                update.CommandText = "UPDATE DataModel SET testTime = @t WHERE Id = @id";
                update.Parameters.AddWithValue("@t", parsed.ToString(TimeFormat, CultureInfo.InvariantCulture));
                update.Parameters.AddWithValue("@id", id);
                update.ExecuteNonQuery();
            }

            if (legacy.Count > 0)
            {
                gkHelper.Log.Info($"已统一 {legacy.Count} 条历史记录的时间格式");
            }
        }

        public int Insert(dataModel model)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO DataModel
            (testTime, projectName, data1, data2, data3, data4, data5, data6, data7, data8)
            VALUES
            (@testTime, @projectName, @data1, @data2, @data3, @data4, @data5, @data6, @data7, @data8);
            SELECT last_insert_rowid();";

            AddParameters(cmd, model);
            return Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
        }

        public int Update(dataModel model)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
            UPDATE DataModel SET
                testTime = @testTime,
                projectName = @projectName,
                data1 = @data1,
                data2 = @data2,
                data3 = @data3,
                data4 = @data4,
                data5 = @data5,
                data6 = @data6,
                data7 = @data7,
                data8 = @data8
            WHERE Id = @Id";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            AddParameters(cmd, model);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM DataModel WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);
            return cmd.ExecuteNonQuery();
        }

        public dataModel? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DataModel WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapToModel(reader) : null;
        }

        public List<dataModel> GetAll()
        {
            var list = new List<dataModel>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DataModel ORDER BY testTime, Id";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapToModel(reader));
            }
            return list;
        }

        /// <summary>条件可空；值一律参数化，防止注入。</summary>
        public List<dataModel> GetByConditions(string? startTime, string? endTime, string? projectName)
        {
            var list = new List<dataModel>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrWhiteSpace(startTime))
            {
                conditions.Add("testTime >= @startTime");
                parameters["@startTime"] = startTime;
            }
            if (!string.IsNullOrWhiteSpace(endTime))
            {
                conditions.Add("testTime <= @endTime");
                parameters["@endTime"] = endTime;
            }
            if (!string.IsNullOrWhiteSpace(projectName))
            {
                conditions.Add("projectName = @projectName");
                parameters["@projectName"] = projectName;
            }

            string whereClause = conditions.Count > 0
                ? "WHERE " + string.Join(" AND ", conditions)
                : "";

            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM DataModel {whereClause} ORDER BY testTime, Id";
            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapToModel(reader));
            }
            return list;
        }

        private static dataModel MapToModel(SqliteDataReader reader) => new()
        {
            Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"], CultureInfo.InvariantCulture),
            testTime = reader["testTime"] as string ?? "",
            projectName = reader["projectName"] as string ?? "",
            data1 = reader["data1"] as string ?? "",
            data2 = reader["data2"] as string ?? "",
            data3 = reader["data3"] as string ?? "",
            data4 = reader["data4"] as string ?? "",
            data5 = reader["data5"] as string ?? "",
            data6 = reader["data6"] as string ?? "",
            data7 = reader["data7"] as string ?? "",
            data8 = reader["data8"] as string ?? ""
        };

        private static void AddParameters(SqliteCommand cmd, dataModel model)
        {
            cmd.Parameters.AddWithValue("@testTime", (object?)model.testTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@projectName", (object?)model.projectName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data1", (object?)model.data1 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data2", (object?)model.data2 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data3", (object?)model.data3 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data4", (object?)model.data4 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data5", (object?)model.data5 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data6", (object?)model.data6 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data7", (object?)model.data7 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@data8", (object?)model.data8 ?? DBNull.Value);
        }
    }
}
