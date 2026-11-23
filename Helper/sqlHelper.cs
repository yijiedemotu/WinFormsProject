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
        private readonly string _connectionString;

        public SqlHelper(string dbPath = "")
        {
            // 固定在程序基目录：VS 调试 / dotnet run / 双击 exe 都指向同一个库
            string path = string.IsNullOrWhiteSpace(dbPath)
                ? Path.Combine(AppContext.BaseDirectory, "data.db")
                : dbPath;
            _connectionString = $"Data Source={path}";
            // 自动建表（幂等），避免干净环境启动时报 no such table: DataModel
            InitDatabase();
        }

        /// <summary>
        /// 初始化数据库和表
        /// </summary>
        private void InitDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
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
            );
        ";
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 新增
        /// </summary>
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

            SELECT last_insert_rowid();
        ";

            AddParameters(cmd, model);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        /// <summary>
        /// 更新
        /// </summary>
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
            WHERE Id = @Id
        ";

            cmd.Parameters.AddWithValue("@Id", model.Id);
            AddParameters(cmd, model);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public int Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM DataModel WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据 Id 查询（字段允许为 null）
        /// </summary>
        public dataModel? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DataModel WHERE Id = @Id";
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapToModel(reader);
            }

            return null;
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        public List<dataModel> GetAll()
        {
            var list = new List<dataModel>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM DataModel";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(MapToModel(reader));
            }

            return list;
        }

        /// <summary>库中最早 / 最晚检测时间，供日期控件设置默认区间。</summary>
        public (DateTime? min, DateTime? max) GetTimeRange()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT MIN(testTime), MAX(testTime) FROM DataModel";
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return (null, null);

            DateTime? min = Parse(reader.IsDBNull(0) ? "" : reader.GetString(0));
            DateTime? max = Parse(reader.IsDBNull(1) ? "" : reader.GetString(1));
            return (min, max);
        }

        private static DateTime? Parse(string s) =>
            DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime t) ? t : null;

        /// <summary>查询库里出现过的产品名称，供查询页下拉框使用。</summary>
        public List<string> GetProjectNames()
        {
            var list = new List<string>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText =
                "SELECT DISTINCT projectName FROM DataModel " +
                "WHERE projectName IS NOT NULL AND TRIM(projectName) <> '' " +
                "ORDER BY projectName";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(reader["projectName"] as string ?? "");
            return list;
        }

        /// <summary>
        /// 根据多个可选条件查询（所有条件均可为 null 或空字符串）
        /// </summary>
        /// <param name="startTime">起始时间（含边界），可为空</param>
        /// <param name="endTime">结束时间（含边界），可为空</param>
        /// <param name="projectName">项目名称（精确匹配），可为空</param>
        /// <returns>符合条件的记录列表</returns>
        private const string TimeFormat = "yyyy-MM-dd HH:mm:ss";

        // 返回 null = 不加这个条件；任何写不规范的入参都不会拼进 SQL
        private static string? NormalizeTime(string? text, bool isEnd)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            string s = text.Trim().Replace('/', '-').Replace('.', '-');
            if (!DateTime.TryParse(s, CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out DateTime dt))
                return null;                                     // 解析失败当「无条件」
            if (isEnd && s.Length <= 10)                          // 只给日期 → 补到当天 23:59:59
                dt = dt.Date.AddDays(1).AddSeconds(-1);
            return dt.ToString(TimeFormat, CultureInfo.InvariantCulture);
        }

        public List<dataModel> GetByConditions(string? startTime, string? endTime, string? projectName)
        {
            var list = new List<dataModel>();
            string? start = NormalizeTime(startTime, false);
            string? end = NormalizeTime(endTime, true);
            string? proj = string.IsNullOrWhiteSpace(projectName) ? null : projectName.Trim();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            var conditions = new List<string>();
            if (start != null) { conditions.Add("testTime >= @startTime"); cmd.Parameters.AddWithValue("@startTime", start); }
            if (end != null) { conditions.Add("testTime <= @endTime"); cmd.Parameters.AddWithValue("@endTime", end); }
            if (proj != null) { conditions.Add("projectName = @projectName"); cmd.Parameters.AddWithValue("@projectName", proj); }

            cmd.CommandText = "SELECT * FROM DataModel"
                + (conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "")
                + " ORDER BY testTime";

            gkHelper.Log.Info($"查询 SQL: {cmd.CommandText} | start={start} end={end} project={proj}");

            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapToModel(reader));
            return list;
        }

        /// <summary>
        /// 映射 DataReader → Model（每个字段都允许为 null）
        /// </summary>
        private dataModel MapToModel(SqliteDataReader reader)
        {
            return new dataModel
            {
                Id = reader["Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Id"]),
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
        }

        /// <summary>
        /// 添加参数（允许 null）
        /// </summary>
        private void AddParameters(SqliteCommand cmd, dataModel model)
        {
            cmd.Parameters.AddWithValue("@testTime", model.testTime ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@projectName", model.projectName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data1", model.data1 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data2", model.data2 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data3", model.data3 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data4", model.data4 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data5", model.data5 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data6", model.data6 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data7", model.data7 ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@data8", model.data8 ?? (object)DBNull.Value);
        }
    }
}
