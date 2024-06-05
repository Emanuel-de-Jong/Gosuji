using Gosuji.Client.Data;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS
{
    public partial class CTranslation : ComponentBase
    {
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private IJSObjectReference jsRef;
        private List<TextKey> textKeys;
        private Dictionary<long, Language> languages;
        private Dictionary<long, TextValue>? textValues;

        private Dictionary<long, string>? translations;

        private string? newKey;
        private long currentLanguageId = -1;

        protected override async Task OnInitializedAsync()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            textKeys = await dbContext.TextKeys.ToListAsync();
            languages = await dbContext.Languages.ToDictionaryAsync(l => l.Id);
            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (textValues != null)
            {
                await jsRef.InvokeVoidAsync("cTranslation.resizeTranslationTextareas");
            }
        }

        private async Task AddTextKey()
        {
            if (newKey.IsNullOrEmpty())
            {
                return;
            }

            TextKey newKeyObj = new(newKey);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
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

            await PrepareTranslationInputs();
        }

        private async Task PrepareTranslationInputs()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            Dictionary<long, TextValue> tempTextValues = await dbContext.TextValues.Where(tv => tv.LanguageId == currentLanguageId).ToDictionaryAsync(tv => tv.TextKeyId);
            await dbContext.DisposeAsync();

            translations = [];
            foreach (TextKey textKey in textKeys)
            {
                translations[textKey.Id] = tempTextValues.ContainsKey(textKey.Id) ? tempTextValues[textKey.Id].Value : "";
            }

            textValues = tempTextValues;
        }

        private async Task SaveTranslations()
        {
            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();

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
