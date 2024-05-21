using GosujiServer.Areas.Identity.Data;
using GosujiServer.Data;
using GosujiServer.Services;
using IGOEnchi.GoGameLogic;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Xml.Linq;
using GosujiServer.Models.GoGameWraps;
using GosujiServer.Models;
using System;

namespace GosujiServer.Pages
{
    public partial class CMS : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private DbService dbService { get; set; }

        private List<TextKey> textKeys;
        private Dictionary<long, Language> languages;
        private Dictionary<long, TextValue>? textValues;

        private string newKey;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            textKeys = dbContext.TextKeys.ToList();
            languages = dbContext.Languages.ToDictionary(l => l.Id);
            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (textValues != null)
            {
                await JS.InvokeVoidAsync("cms.resizeTranslationTextareas");
            }
        }

        private async Task AddTextKey()
        {
            TextKey newKeyObj = new(newKey);

            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            await dbContext.TextKeys.AddAsync(newKeyObj);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();

            textKeys.Add(newKeyObj);
        }

        private async Task LanguageSelected(ChangeEventArgs e)
        {
            long languageId = long.Parse(e.Value.ToString());
            if (languageId == -1)
            {
                return;
            }

            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            textValues = dbContext.TextValues.Where(tv => tv.LanguageId == languageId).ToDictionary(tv => tv.TextKeyId);
            await dbContext.DisposeAsync();
        }

        private async Task SaveTranslations()
        {

        }
    }
}
