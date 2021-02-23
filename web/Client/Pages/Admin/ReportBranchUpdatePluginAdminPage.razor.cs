using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Shearlegs.Web.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace Shearlegs.Web.Client.Pages.Admin
{
    public partial class ReportBranchUpdatePluginAdminPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public int BranchId { get; set; }

        public ReportBranchModel Branch { get; set; }

        public ReportBranchPluginModel PluginModel { get; set; } 
            = new ReportBranchPluginModel() { Libraries = new List<ReportBranchPluginLibraryModel>() };

        protected override async Task OnInitializedAsync()
        {
            Branch = await HttpClient.GetFromJsonAsync<ReportBranchModel>("api/reports/branches/" + BranchId);
        }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            PluginModel.Content = new byte[e.File.Size];
            await e.File.OpenReadStream(30 * 1024 * 1024).ReadAsync(PluginModel.Content);
        }

        private async Task OnInputLibraryFileChange(InputFileChangeEventArgs e)
        {
            Console.WriteLine("hello");
            foreach (var file in e.GetMultipleFiles(100))
            {
                Console.WriteLine("sup");
                var library = new ReportBranchPluginLibraryModel();
                library.Name = file.Name;
                library.Content = new byte[file.Size];
                await file.OpenReadStream(30 * 1024 * 1024).ReadAsync(library.Content);
                PluginModel.Libraries.Add(library);
            }
        }

        private async Task OnInputTemplateChange(InputFileChangeEventArgs e)
        {
            PluginModel.TemplateFileName = e.File.Name;
            PluginModel.TemplateMimeType = e.File.ContentType;
            PluginModel.TemplateContent = new byte[e.File.Size];
            await e.File.OpenReadStream(30 * 1024 * 1024).ReadAsync(PluginModel.TemplateContent);
        }

        private async Task AddPluginReportAsync()
        {
            PluginModel.BranchId = Branch.Id;
            var response = await HttpClient.PostAsJsonAsync("api/reports/plugin", PluginModel);
            var reportPlugin = await response.Content.ReadFromJsonAsync<ReportBranchPluginModel>();

            Branch.PluginId = reportPlugin.Id;
            Branch.Plugin = reportPlugin;

            PluginModel = new ReportBranchPluginModel() { Libraries = new List<ReportBranchPluginLibraryModel>() };
        }

        private void RemoveLibrary(ReportBranchPluginLibraryModel library)
        {
            PluginModel.Libraries.Remove(library);
        } 
    }
}
