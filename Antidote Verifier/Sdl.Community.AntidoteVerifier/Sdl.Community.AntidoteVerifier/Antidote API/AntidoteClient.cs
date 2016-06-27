using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier.Antidote_API
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IAntidoteClient))]
    [ComVisible(true)]
    public class AntidoteClient : IAntidoteClient
    {
        private IEditorService _editorService;
        public AntidoteClient(IEditorService editorService)
        {
            _editorService = editorService;
        }
        public void ActiveApplication()
        {
            //Do something
        }

        public void ActiveDocument(long idDoc)
        {
            
        }

        public long DonneDebutSelection(long idDoc, long idZone)
        {
            return 0; 
        }

        public long DonneFinSelection(long idDoc, long idZone)
        {
            return 0;
        }

        /// <summary>
        /// This method informs Antidote about the unique document id
        /// </summary>
        /// <returns></returns>
        public long DonneIdDocumentCourant()
        {
            return _editorService.GetDocumentId();
        }

        /// <summary>
        /// Informs Antidote about the current active segment from the Studio Editor. In this case
        /// this method will provide the segment id because in Studio the Antidote Zone and Area correspond
        /// to a Segment in the Studio editor.
        /// </summary>
        /// <param name="id">The document id</param>
        /// <param name="index">Identifier for current area</param>
        /// <returns>Returns the identifier of the text field</returns>
        public long DonneIdZoneDeTexte(long id, long index)
        {
            return _editorService.GetCurrentSegmentId(index);
        }
        
        /// <summary>
        /// Informs Antidote about the current active segment from the Studio Editor.
        /// </summary>
        /// <param name="id">The document Id</param>
        /// <returns>Returns the identifier for the current area</returns>

        public long DonneIdZoneDeTexteCourante(long id)
        {
            return 1;
        }

        public string DonneIntervalle(long idDoc, long idZone, long leDebut, long laFin)
        {
            return _editorService.GetSegmentText(idZone);
        }

        /// <summary>
        /// Informs Antidote about the length of the text in the segment.
        /// </summary>
        /// <param name="idDoc">The document id</param>
        /// <param name="idZone">The identifier of the text field</param>
        /// <returns>Returns the text length</returns>
        public long DonneLongueurZoneDeTexte(long idDoc, long idZone)
        {
            return 14;
        }
        /// <summary>
        /// Informs Antidote about the number of segments from the document
        /// that will be sent for correcting.
        /// </summary>
        /// <param name="id">The document Id</param>
        /// <returns>Returns the number of zones of text in the specified document</returns>
        public long DonneNbZonesDeTexte(long id)
        {
            return _editorService.GetDocumentNoOfSegments();

        }

        public string DonnePolice(long idDoc, long idZone)
        {
            return "dernier choisi";
        }

        public string DonneTitreDocCourant()
        {
            return "dernier choisi";

        }

        public void RemplaceIntervalle(long idDoc, long idZone, long leDebut, long laFin, ref string laChaine)
        {
            
        }

        public void SelectionneIntervalle(long idDoc, long idZone, long leDebut, long laFin)
        {
            
        }
    }
}
