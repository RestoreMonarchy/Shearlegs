using Dapper;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Shearlegs.Web.Server.Repositories
{
    public class ReportsRepository
    {
        private readonly SqlConnection connection;

        public ReportsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<ReportArchive> ArchiveReportAsync(ReportArchive report)
        {
            const string sql = "INSERT INTO dbo.ReportsArchive (Name, MimeType, Content, PluginName, Parameters) " +
                "OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.MimeType, INSERTED.PluginName, INSERTED.Parameters, " +
                "INSERTED.CreateDate VALUES (@Name, @MimeType, @Content, @PluginName, @Parameters);";
            return await connection.QuerySingleOrDefaultAsync<ReportArchive>(sql, report);
        }

        public async Task<ReportArchive> GetReportArchiveAsync(int id)
        {
            const string sql = "SELECT * FROM dbo.ReportsArchive WHERE Id = @id;";
            return await connection.QuerySingleOrDefaultAsync<ReportArchive>(sql, new { id });
        }
    }
}
