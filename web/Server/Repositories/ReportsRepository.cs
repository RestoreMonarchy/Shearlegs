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

        public async Task<bool> HasPermissionAsync(int userId, int reportId)
        {
            const string sql = "SELECT Count(0) FROM dbo.ReportUsers WHERE UserId = @userId AND ReportId = @reportId;";
            return await connection.ExecuteScalarAsync<bool>(sql, new { userId, reportId });
        }

        public async Task<ReportUserModel> AddReportUserAsync(ReportUserModel reportUser)
        {
            const string sql = "INSERT INTO dbo.ReportUsers (ReportId, UserId, AdminId) " +
                "OUTPUT INSERTED.Id, INSERTED.ReportId, INSERTED.UserId, INSERTED.AdminId, INSERTED.CreateDate " +
                "VALUES (@ReportId, @UserId, @AdminId);";

            return await connection.QuerySingleAsync<ReportUserModel>(sql, reportUser);
        }
        
        public async Task DeleteReportUserAsync(int reportUserId)
        {
            const string sql = "DELETE FROM dbo.ReportUsers WHERE Id = @reportUserId;";

            await connection.ExecuteAsync(sql, new { reportUserId });
        }

        public async Task<ReportArchiveModel> ArchiveReportAsync(ReportArchiveModel report)
        {
            const string sql = "INSERT INTO dbo.ReportsArchive (Name, MimeType, Content, PluginName, Parameters) " +
                "OUTPUT INSERTED.Id, INSERTED.Name, INSERTED.MimeType, INSERTED.PluginName, INSERTED.Parameters, " +
                "INSERTED.CreateDate VALUES (@Name, @MimeType, @Content, @PluginName, @Parameters);";
            return await connection.QuerySingleAsync<ReportArchiveModel>(sql, report);
        }

        public async Task<ReportArchiveModel> GetReportArchiveAsync(int id)
        {
            const string sql = "SELECT * FROM dbo.ReportsArchive WHERE Id = @id;";
            return await connection.QuerySingleOrDefaultAsync<ReportArchiveModel>(sql, new { id });
        }

        public async Task UpdateReportBranchAsync(ReportBranchModel branch)
        {
            const string sql = "UPDATE dbo.ReportBranches SET Name = @Name, Description = @Description WHERE Id = @Id;";

            await connection.ExecuteAsync(sql, branch);
        }

        public async Task<ReportBranchModel> AddReportBranchAsync(ReportBranchModel branch)
        {
            const string sql = "INSERT INTO dbo.ReportBranches (ReportId, Name, Description) " +
                "OUTPUT INSERTED.Id, INSERTED.ReportId, INSERTED.Name, INSERTED.Description, INSERTED.PluginId " +
                "VALUES (@ReportId, @Name, @Description);";

            return await connection.QuerySingleOrDefaultAsync<ReportBranchModel>(sql, branch);
        }

        public async Task<ReportBranchModel> GetReportBranchAsync(int id)
        {
            const string sql = "SELECT b.*, p.* FROM dbo.ReportBranches b " +
                "LEFT JOIN dbo.ReportBranchParameters p ON b.Id = p.BranchId WHERE b.Id = @id;";
            const string sql1 = "SELECT Id, Version, Changelog, CreateDate FROM dbo.ReportBranchPlugins WHERE Id = @PluginId;";

            ReportBranchModel branch = null;
            await connection.QueryAsync<ReportBranchModel, ReportBranchParameterModel, ReportBranchModel>(sql, (b, p) =>
            {
                if (branch == null)
                {
                    branch = b;
                    branch.Parameters = new List<ReportBranchParameterModel>();
                }

                if (p != null)
                    branch.Parameters.Add(p);

                return null;
            }, new { id });

            if (branch.PluginId != 0)
                branch.Plugin = await connection.QuerySingleOrDefaultAsync<ReportBranchPluginModel>(sql1, branch);

            const string sql2 = "SELECT Id, BranchId, Name, Value = CAST(Value AS NVARCHAR(MAX)) " +
                "FROM dbo.ReportBranchSecrets WHERE BranchId = @Id;";

            branch.Secrets = (await connection.QueryAsync<ReportBranchSecretModel>(sql2, branch)).ToList();

            return branch;
        }

        public async Task<ReportModel> GetReportAsync(int id)
        {
            const string sql = "SELECT r.*, b.* FROM dbo.Reports r LEFT JOIN dbo.ReportBranches b ON r.Id = b.ReportId WHERE r.Id = @id;";

            ReportModel report = null;
            await connection.QueryAsync<ReportModel, ReportBranchModel, ReportModel>(sql, (r, b) => 
            { 
                if (report == null)
                {
                    report = r;
                    report.Branches = new List<ReportBranchModel>();
                }

                if (b != null)
                    report.Branches.Add(b);

                return null;
            }, new { id });

            return report;
        }

        public async Task<ReportBranchParameterModel> AddReportBranchParameterAsync(ReportBranchParameterModel parameter)
        {
            const string sql = "INSERT INTO dbo.ReportBranchParameters (BranchId, Name, InputType, IsMandatory) " +
                "OUTPUT INSERTED.Id, INSERTED.BranchId, INSERTED.Name, INSERTED.InputType, INSERTED.IsMandatory " +
                "VALUES (@BranchId, @Name, @InputType, @IsMandatory);";

            return await connection.QuerySingleOrDefaultAsync<ReportBranchParameterModel>(sql, parameter);
        }

        public async Task<ReportBranchSecretModel> AddReportBranchSecretAsync(ReportBranchSecretModel secret)
        {
            const string sql = "INSERT INTO dbo.ReportBranchSecrets (BranchId, Name, Value) " +
                "OUTPUT INSERTED.Id, INSERTED.BranchId, INSERTED.Name, CAST(INSERTED.Value AS NVARCHAR(MAX)) AS Value " +
                "VALUES (@BranchId, @Name, CAST(@Value AS VARBINARY(MAX)));";

            return await connection.QuerySingleOrDefaultAsync<ReportBranchSecretModel>(sql, secret);
        }

        public async Task RemoveReportBranchParameterAsync(int parameterId)
        {
            const string sql = "DELETE FROM dbo.ReportBranchParameters WHERE Id = @parameterId;";

            await connection.ExecuteAsync(sql, new { parameterId });
        }

        public async Task RemoveReportBranchSecretAsync(int secretId)
        {
            const string sql = "DELETE FROM dbo.ReportBranchSecrets WHERE Id = @secretId;";

            await connection.ExecuteAsync(sql, new { secretId });
        }

        public async Task<IEnumerable<ReportModel>> GetReportsAsync(int userId)
        {
            string sql = "SELECT r.*, b.* FROM dbo.Reports r " +
                "LEFT JOIN dbo.ReportBranches b ON r.Id = b.ReportId";

            if (userId != 0 )
            {
                sql += " WHERE EXISTS (SELECT * FROM dbo.ReportUsers ru WHERE ru.ReportId = r.Id AND ru.UserId = @userId)";
            }

            List<ReportModel> reports = new List<ReportModel>();

            await connection.QueryAsync<ReportModel, ReportBranchModel, ReportModel>(sql, (r, b) => 
            {
                var report = reports.FirstOrDefault(x => x.Id == r.Id);
                if (report == null)
                {
                    report = r;
                    report.Branches = new List<ReportBranchModel>();
                    reports.Add(report);
                }

                if (b != null)
                    report.Branches.Add(b);

                return null;
            }, new { userId });

            return reports;
        }

        public async Task<ReportBranchModel> GetReportPluginAsync(int branchId)
        {
            const string sql = "SELECT b.*, r.*, p.* FROM dbo.ReportBranches b JOIN dbo.Reports r ON b.ReportId = r.Id " +
                "LEFT JOIN dbo.ReportBranchPlugins p ON b.PluginId = p.Id WHERE b.Id = @branchId;";

            const string sql1 = "SELECT * FROM dbo.ReportBranchPluginLibraries WHERE PluginId = @Id;";

            const string sql2 = "SELECT Id, BranchId, Name, Value = CAST(Value AS NVARCHAR(MAX)) " +
                "FROM dbo.ReportBranchSecrets WHERE BranchId = @Id;";

            var branch = (await connection.QueryAsync<ReportBranchModel, ReportModel, ReportBranchPluginModel, ReportBranchModel>(sql, (b, r, p) =>
            {
                b.Report = r;
                b.Plugin = p;
                return b;
            }, new { branchId })).FirstOrDefault();

            if (branch.Plugin != null)
                branch.Plugin.Libraries = (await connection.QueryAsync<ReportBranchPluginLibraryModel>(sql1, branch.Plugin)).ToList();

            branch.Secrets = (await connection.QueryAsync<ReportBranchSecretModel>(sql2, branch)).ToList();

            return branch;
        }        

        public async Task AddReportAsync(ReportModel reportModel)
        {
            const string sql = "INSERT INTO dbo.Reports (Name, Description, Enabled) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@Name, @Description, @Enabled); " +
                "INSERT INTO dbo.ReportBranches (ReportId, Name, Description) " +
                "VALUES (SCOPE_IDENTITY(), 'DEVELOPMENT', 'Production environment branch');";

            reportModel.Id = await connection.ExecuteScalarAsync<int>(sql, reportModel);
        }

        public async Task UpdateReportAsync(ReportModel reportModel)
        {
            const string sql = "UPDATE dbo.Reports SET Name = @Name, Description = @Description, Enabled = @Enabled " +
                "WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, reportModel);
        }
        
        public async Task AddReportBranchPluginAsync(ReportBranchPluginModel reportBranchPluginModel)
        {
            const string sql = "INSERT INTO dbo.ReportBranchPlugins (BranchId, Version, Changelog, Content, TemplateContent, TemplateMimeType, TemplateFileName) " +
                "VALUES (@BranchId, @Version, @Changelog, @Content, @TemplateContent, @TemplateMimeType, @TemplateFileName); SELECT SCOPE_IDENTITY();";
            const string sql1 = "UPDATE dbo.ReportBranches SET PluginId = @Id WHERE Id = @BranchId;";
            const string sql2 = "INSERT INTO dbo.ReportBranchPluginLibraries (PluginId, Name, Content) VALUES (@PluginId, @Name, @Content);";

            reportBranchPluginModel.Id = await connection.ExecuteScalarAsync<int>(sql, reportBranchPluginModel);
            await connection.ExecuteAsync(sql1, reportBranchPluginModel);

            foreach (var library in reportBranchPluginModel.Libraries)
            {
                library.PluginId = reportBranchPluginModel.Id;
                await connection.ExecuteAsync(sql2, library);
            }
        }
    }
}
