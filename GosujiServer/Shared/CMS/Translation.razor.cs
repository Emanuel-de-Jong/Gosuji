using GosujiServer.Data;
using GosujiServer.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GosujiServer.Shared.CMS
{
    public partial class Translation : ComponentBase
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        [Inject]
        private DbService dbService { get; set; }

        private List<TextKey> textKeys;
        private Dictionary<long, Language> languages;
        private Dictionary<long, TextValue>? textValues;

        private Dictionary<long, string>? translations;

        private string newKey;
        private long currentLanguageId = -1;

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
            currentLanguageId = long.Parse(e.Value.ToString());
            if (currentLanguageId == -1)
            {
                return;
            }

            PrepareTranslationInputs();
        }

        private async Task PrepareTranslationInputs()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();
            var tempTextValues = dbContext.TextValues.Where(tv => tv.LanguageId == currentLanguageId).ToDictionary(tv => tv.TextKeyId);
            await dbContext.DisposeAsync();

            translations = new();
            foreach (TextKey textKey in textKeys)
            {
                translations[textKey.Id] = tempTextValues.ContainsKey(textKey.Id) ? tempTextValues[textKey.Id].Value : "";
            }

            textValues = tempTextValues;
        }

        private async Task SaveTranslations()
        {
            ApplicationDbContext dbContext = await dbService.GetContextAsync();

            foreach (KeyValuePair<long, string> pair in translations)
            {
                if (textValues.ContainsKey(pair.Key))
                {
                    TextValue textValue = textValues[pair.Key];
                    textValue.Value = pair.Value;
                    dbContext.TextValues.Update(textValue);
                }
                else
                {
                    TextValue textValue = new()
                    {
                        LanguageId = currentLanguageId,
                        TextKeyId = pair.Key,
                        Value = pair.Value
                    };
                    await dbContext.TextValues.AddAsync(textValue);
                }
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }
    }
}
