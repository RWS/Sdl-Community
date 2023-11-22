﻿using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	[TellMeProvider]
    public class DeepLTellMeProvider : ITellMeProvider
    {
        public string Name => "DeepL Tell Me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new DeepLStoreAction
            {
                Keywords = new[] {"deepL", "deepl store", "deepl download"}
            },
            new DeepLContactAction
            {
                Keywords = new[] {"deepL", "deepl contact", "deepl trial"}
            },
            new DeepLHelpAction
            {
                Keywords = new[] {"deepL", "deepl help", "deepl guide"}
            },
            new DeepLCommunitySupportAction
            {
                Keywords = new[] {"deepL", "deepl community", "deepl support"}
            },
            new DeepLSettingsAction
            {
                Keywords = new[] {"deepL", "deepl settings", "settings"}
            }
        };
    }
}