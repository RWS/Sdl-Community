using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using Sdl.Community.Productivity.Services.Persistence;

namespace Sdl.Community.Productivity.UI
{
    public static class ProductivityUiHelper
    {
        public static bool IsTwitterAccountConfigured(TwitterPersistenceService twitterPersistenceService, Logger logger)
        {
            var isTwitterAccountConfigured = true;
            if (twitterPersistenceService.HasAccountConfigured) return true;
            using (var tForm = new TwitterAccountSetup(twitterPersistenceService))
            {
                var result = tForm.ShowDialog();
                if (result != DialogResult.OK)
                {
                    isTwitterAccountConfigured = false;
                }
            }
            return isTwitterAccountConfigured;
        }
    }
}
