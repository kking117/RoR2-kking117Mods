using RoR2.ContentManagement;

namespace ConsumedBuff.Modules
{
    internal class ContentPacks : IContentPackProvider
    {
        internal ContentPack contentPack = new ContentPack();
        public string identifier => MainPlugin.MODUID;
        public void Initialize()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
        }
        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }
        public System.Collections.IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.identifier = identifier;
            contentPack.buffDefs.Add(Buffs.buffDefs.ToArray());
            args.ReportProgress(1f);
            yield break;
        }
        public System.Collections.IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(this.contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }
        public System.Collections.IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
