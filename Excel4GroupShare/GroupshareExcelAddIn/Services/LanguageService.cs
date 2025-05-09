using Sdl.Community.GroupShareKit.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Interfaces;

namespace GroupshareExcelAddIn.Services
{
    public class LanguageService
    {
        private readonly IGroupshareConnection _groupshareConnection;
        private List<Language> _languages;

        public LanguageService(IGroupshareConnection groupshareConnection)
        {
            _groupshareConnection = groupshareConnection;
            _groupshareConnection.ConnectionChanged += ClearLanguages;
        }

        public async Task<List<Language>> GetLanguages() => _languages ?? (_languages = await _groupshareConnection.GetLanguages());

        private void ClearLanguages(object sender, EventArgs e)
        {
            _languages = null;
        }
    }
}