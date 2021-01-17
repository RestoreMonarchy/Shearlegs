using Dapper;
using Shearlegs.Web.Shared.Models;
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

        public async Task<ReportArchiveModel> ArchiveReportAsync(ReportArchiveModel report)
        {
            const string sql = "INSERT INTO dbo.ReportsArchive (Name, MimeType, Content, PluginName, Parameters) " +
                "OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.MimeType, INSERTED.PluginName, INSERTED.Parameters, " +
                "INSERTED.CreateDate VALUES (@Name, @MimeType, @Content, @PluginName, @Parameters);";
            return await connection.QuerySingleOrDefaultAsync<ReportArchiveModel>(sql, report);
        }

        public async Task<ReportArchiveModel> GetReportArchiveAsync(int id)
        {
            const string sql = "SELECT * FROM dbo.ReportsArchive WHERE Id = @id;";
            return await connection.QuerySingleOrDefaultAsync<ReportArchiveModel>(sql, new { id });
        }

        public async Task<ReportModel> GetReportAsync(int id)
        {
            const string sql = "SELECT r.*, p.* FROM dbo.Reports r JOIN dbo.ReportPlugins p ON r.PluginId = p.Id WHERE r.Id = @id;";

            var reports = await connection.QueryAsync<ReportModel, ReportPluginModel, ReportModel>(sql, (r, p) =>
            {
                r.Plugin = p;
                return r;
            }, new { id });

            return reports.Single();
        }

        public async Task<IEnumerable<ReportModel>> GetReportsAsync()
        {
            const string sql = "SELECT * FROM dbo.Reports;";
            return await connection.QueryAsync<ReportModel>(sql);
        }

        public async Task AddReportAsync(ReportModel reportModel)
        {
            const string sql = "INSERT INTO dbo.Reports (Name, Description, Enabled) " +
                "VALUES (@Name, @Description, @Enabled); SELECT SCOPE_IDENTITY();";
            reportModel.Id = await connection.ExecuteScalarAsync<int>(sql, reportModel);
        }

        public async Task AddReportPluginAsync(ReportPluginModel reportPluginModel)
        {
            const string sql = "INSERT INTO dbo.ReportPlugins (ReportId, Version, Changelog, Content, TemplateContent, TemplateMimeType, TemplateFileName) " +
                "VALUES (@ReportId, @Version, @Changelog, @Content, @TemplateContent, @TemplateMimeType, @TemplateFileName); SELECT SCOPE_IDENTITY();";
            const string sql1 = "UPDATE dbo.Reports SET PluginId = @Id WHERE Id = @ReportId;";

            reportPluginModel.Id = await connection.ExecuteScalarAsync<int>(sql, reportPluginModel);
            await connection.ExecuteAsync(sql1, reportPluginModel);
        }
    }
}
