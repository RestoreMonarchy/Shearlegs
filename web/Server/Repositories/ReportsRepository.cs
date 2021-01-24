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
            const string sql = "SELECT * FROM dbo.Reports WHERE Id = @id;";

            return await connection.QuerySingleOrDefaultAsync<ReportModel>(sql, new { id });
        }

        public async Task<ReportModel> GetReportPluginAsync(int reportId)
        {
            const string sql = "SELECT r.*, p.*, l.* FROM dbo.Reports r LEFT JOIN dbo.ReportPlugins p ON r.PluginId = p.Id " +
                "LEFT JOIN dbo.ReportPluginLibraries l ON l.PluginId = p.Id WHERE r.Id = @reportId;";

            ReportModel report = null;

            await connection.QueryAsync<ReportModel, ReportPluginModel, ReportPluginLibraryModel, ReportModel>(sql, (r, p, l) => 
            {
                if (report == null)
                {
                    report = r;
                    report.Plugin = p;
                    report.Plugin.Libraries = new List<ReportPluginLibraryModel>();
                }

                if (l != null)
                    report.Plugin.Libraries.Add(l);

                return null;
            }, new { reportId });

            return report;
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

        public async Task UpdateReportAsync(ReportModel reportModel)
        {
            const string sql = "UPDATE dbo.Reports SET Name = @Name, Description = @Description, Enabled = @Enabled " +
                "WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, reportModel);
        }

        public async Task AddReportPluginAsync(ReportPluginModel reportPluginModel)
        {
            const string sql = "INSERT INTO dbo.ReportPlugins (ReportId, Version, Changelog, Content, TemplateContent, TemplateMimeType, TemplateFileName) " +
                "VALUES (@ReportId, @Version, @Changelog, @Content, @TemplateContent, @TemplateMimeType, @TemplateFileName); SELECT SCOPE_IDENTITY();";
            const string sql1 = "UPDATE dbo.Reports SET PluginId = @Id WHERE Id = @ReportId;";
            const string sql2 = "INSERT INTO dbo.ReportPluginLibraries (PluginId, Name, Content) VALUES (@PluginId, @Name, @Content);";

            reportPluginModel.Id = await connection.ExecuteScalarAsync<int>(sql, reportPluginModel);
            await connection.ExecuteAsync(sql1, reportPluginModel);

            foreach (var library in reportPluginModel.Libraries)
            {
                library.PluginId = reportPluginModel.Id;
                await connection.ExecuteAsync(sql2, library);
            }
        }
    }
}
