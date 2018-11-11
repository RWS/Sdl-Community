using System;
using NLog;
using Sdl.Community.YourProductivity.Persistence;
using Sdl.Community.YourProductivity.Services;
using Sdl.Community.YourProductivity.UI;

namespace Sdl.Community.YourProductivity.Util
{
	public class FormFactory
    {
        public static void CreateProductivityForm(Logger logger)
        {
            try
            {
                var trackInfoDb = new TrackInfoDb();
                var productivityService = new ProductivityService(logger, trackInfoDb);
                using (var pForm = new ProductivityForm(productivityService))
                {
                    pForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Unexpected exception when opening the productivity score");
                throw;
            }
        }
    }
}