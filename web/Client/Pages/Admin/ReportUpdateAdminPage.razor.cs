using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Shearlegs.Web.Client.Pages.Admin
{
    public partial class ReportUpdateAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public int ReportId { get; set; }

        public ReportModel Report { get; set; }

        public ReportPluginModel ReportPluginModel { get; set; } = new ReportPluginModel() { Libraries = new List<ReportPluginLibraryModel>() };

        protected override async Task OnInitializedAsync()
        {
            Report = await HttpClient.GetFromJsonAsync<ReportModel>("api/reports/" + ReportId);
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            ReportPluginModel.Content = new byte[e.File.Size];
            await e.File.OpenReadStream(30 * 1024 * 1024).ReadAsync(ReportPluginModel.Content);
        }

        private async Task OnInputLibraryFileChange(InputFileChangeEventArgs e)
        {
            Console.WriteLine("hello");
            foreach (var file in e.GetMultipleFiles(100))
            {
                Console.WriteLine("sup");
                var library = new ReportPluginLibraryModel();
                library.Name = file.Name;
                library.Content = new byte[file.Size];
                await file.OpenReadStream(30 * 1024 * 1024).ReadAsync(library.Content);
                ReportPluginModel.Libraries.Add(library);
            }
        }

        private async Task OnInputTemplateChange(InputFileChangeEventArgs e)
        {
            ReportPluginModel.TemplateFileName = e.File.Name;
            ReportPluginModel.TemplateMimeType = e.File.ContentType;
            ReportPluginModel.TemplateContent = new byte[e.File.Size];
            await e.File.OpenReadStream(30 * 1024 * 1024).ReadAsync(ReportPluginModel.TemplateContent);
        }

        private async Task AddPluginReportAsync()
        {
            ReportPluginModel.ReportId = Report.Id; 
            var response = await HttpClient.PostAsJsonAsync("api/reports/plugin", ReportPluginModel);
            var reportPlugin = await response.Content.ReadFromJsonAsync<ReportPluginModel>();

            Report.PluginId = reportPlugin.Id;
            Report.Plugin = reportPlugin;

            ReportPluginModel = new ReportPluginModel() { Libraries = new List<ReportPluginLibraryModel>() };
        }
    }
}
