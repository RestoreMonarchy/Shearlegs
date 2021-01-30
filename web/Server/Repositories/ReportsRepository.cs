﻿using Dapper;
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

        public async Task<ReportBranchParameterModel> AddReportBranchParameterAsync(ReportBranchParameterModel reportParameter)
        {
            const string sql = "INSERT INTO dbo.ReportBranchParameters (BranchId, Name, InputType, IsMandatory) " +
                "OUTPUT INSERTED.Id, INSERTED.BranchId, INSERTED.Name, INSERTED.InputType, INSERTED.IsMandatory " +
                "VALUES (@BranchId, @Name, @InputType, @IsMandatory);";

            return await connection.QuerySingleOrDefaultAsync<ReportBranchParameterModel>(sql, reportParameter);
        }

        public async Task RemoveReportBranchParameterAsync(int parameterId)
        {
            const string sql = "DELETE FROM dbo.ReportBranchParameters WHERE Id = @parameterId;";

            await connection.ExecuteAsync(sql, new { parameterId });
        }

        public async Task<IEnumerable<ReportModel>> GetReportsAsync()
        {
            const string sql = "SELECT r.*, b.* FROM dbo.Reports r LEFT JOIN dbo.ReportBranches b ON r.Id = b.ReportId;";

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
            });

            return reports;
        }

        public async Task<ReportBranchModel> GetReportPluginAsync(int branchId)
        {
            const string sql = "SELECT b.*, r.*, p.* FROM dbo.ReportBranches b JOIN dbo.Reports r ON b.ReportId = r.Id " +
                "LEFT JOIN dbo.ReportBranchPlugins p ON b.PluginId = p.Id WHERE b.Id = @branchId;";

            const string sql1 = "SELECT * FROM dbo.ReportBranchPluginLibraries WHERE PluginId = @Id;";

            var branch = (await connection.QueryAsync<ReportBranchModel, ReportModel, ReportBranchPluginModel, ReportBranchModel>(sql, (b, r, p) =>
            {
                b.Report = r;
                b.Plugin = p;
                return b;
            }, new { branchId })).FirstOrDefault();

            if (branch.Plugin != null)
                branch.Plugin.Libraries = (await connection.QueryAsync<ReportBranchPluginLibraryModel>(sql1, branch.Plugin)).ToList();

            return branch;
        }        

        public async Task AddReportAsync(ReportModel reportModel)
        {
            const string sql = "INSERT INTO dbo.Reports (Name, Description, Enabled) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@Name, @Description, @Enabled); " +
                "INSERT INTO dbo.ReportBranches (ReportId, Name, Description) " +
                "VALUES (SCOPE_IDENTITY(), 'PRODUCTION', 'Production environment branch');";

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
