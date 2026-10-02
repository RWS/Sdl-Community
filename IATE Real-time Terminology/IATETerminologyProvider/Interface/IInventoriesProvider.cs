using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.IATETerminologyProvider.Interface
{
    public interface IInventoriesProvider
    {
        List<IateCollection> Collections { get; set; }
        List<ItemsResponseModel> Domains { get; }
        List<IateInstitution> Institutions { get; set; }
        bool IsInitialized { get; }
        List<ItemsResponseModel> TermTypes { get; }

        Task<bool> Initialize();
    }
}