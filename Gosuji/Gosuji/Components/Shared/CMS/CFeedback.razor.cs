using Gosuji.Client;
using Gosuji.Client.Data;
using Gosuji.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace Gosuji.Components.Shared.CMS
{
    public partial class CFeedback : ComponentBase, IDisposable
    {
        [Inject]
        private IJSRuntime js { get; set; }
        [Inject]
        private IDbContextFactory<ApplicationDbContext> dbContextFactory { get; set; }

        private DotNetObjectReference<CFeedback>? cFeedbackRef;

        private IJSObjectReference jsRef;
        private Feedback? selectedFeedback;
        private Dictionary<long, Feedback> feedbacks;
        private Dictionary<string, User> users;

        protected override async Task OnInitializedAsync()
        {
            cFeedbackRef = DotNetObjectReference.Create(this);

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            feedbacks = await dbContext.Feedbacks.ToDictionaryAsync(f => f.Id);
            users = await dbContext.Users.Include(u => u.CurrentSubscription).ToDictionaryAsync(u => u.Id);
            await dbContext.DisposeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await js.InvokeVoidAsync("utils.lazyLoadCSSLibrary", G.CSSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["JQuery"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["DataTables"]);
                await js.InvokeVoidAsync("utils.lazyLoadJSLibrary", G.JSLibUrls["Moment"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading library: {ex.Message}");
            }

            jsRef ??= await js.InvokeAsync<IJSObjectReference>("import", "./js/pages/cms/bundle.js?v=03-06-24");

            if (firstRender)
            {
                await jsRef.InvokeVoidAsync("cFeedback.init", cFeedbackRef);
            }
        }

        [JSInvokable]
        public async Task SetSelectedFeedback(long feedbackId)
        {
            selectedFeedback = feedbacks[feedbackId];
        }

        private async Task Read(Feedback feedback)
        {
            feedback.IsRead = true;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Update(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        private async Task ResolveChanged(ChangeEventArgs e, Feedback feedback)
        {
            feedback.IsResolved = (bool)e.Value;

            ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Update(feedback);
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        public void Dispose()
        {
            cFeedbackRef?.Dispose();
        }
    }
}
