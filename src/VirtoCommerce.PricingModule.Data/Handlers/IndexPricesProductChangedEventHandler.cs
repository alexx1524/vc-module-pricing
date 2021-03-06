using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.PricingModule.Core.Events;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Data.BackgroundJobs;

namespace VirtoCommerce.PricingModule.Data.Handlers
{
    public class IndexPricesProductChangedEventHandler : IEventHandler<PriceChangedEvent>
    {
        private readonly ISettingsManager _settingsManager;

        public IndexPricesProductChangedEventHandler(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public Task Handle(PriceChangedEvent message)
        {
            if (_settingsManager.GetValue(Core.ModuleConstants.Settings.General.EventBasedIndexation.Name, false))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var indexEntries = message.ChangedEntries
                    .Select(x => new IndexEntry { Id = x.OldEntry.ProductId, EntryState = EntryState.Modified, Type = KnownDocumentTypes.Product })
                    .ToArray();

                IndexingJobs.EnqueueIndexAndDeleteDocuments(indexEntries);
            }

            return Task.CompletedTask;
        }
    }
}
