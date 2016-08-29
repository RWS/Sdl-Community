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
        }

        public void ActiveDocument(int idDoc)
        {
            _editorService.ActivateDocument();
        }

        public int DonneDebutSelection(int idDoc, int idZone)
        {
            return 0;
        }

        public int DonneFinSelection(int idDoc, int idZone)
        {
            return _editorService.GetSegmentText(idZone).Length;
        }

        /// <summary>
        /// This method informs Antidote about the unique document id
        /// </summary>
        /// <returns></returns>
        public int DonneIdDocumentCourant()
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
        public int DonneIdZoneDeTexte(int id, int index)
        {
            return _editorService.GetCurrentSegmentId(index);
        }
        
        /// <summary>
        /// Informs Antidote about the current active segment from the Studio Editor.
        /// </summary>
        /// <param name="id">The document Id</param>
        /// <returns>Returns the identifier for the current area</returns>

        public int DonneIdZoneDeTexteCourante(int index)
        {
            return 0;
        }

        public string DonneIntervalle(int idDoc, int idZone, int leDebut, int laFin)
        {
            return _editorService.GetSegmentText(idZone).Substring(leDebut, laFin - leDebut);
        }

        /// <summary>
        /// Informs Antidote about the length of the text in the segment.
        /// </summary>
        /// <param name="idDoc">The document id</param>
        /// <param name="idZone">The identifier of the text field</param>
        /// <returns>Returns the text length</returns>
        public int DonneLongueurZoneDeTexte(int idDoc, int idZone)
        {
            return _editorService.GetSegmentText(idZone).Length;
        }
        /// <summary>
        /// Informs Antidote about the number of segments from the document
        /// that will be sent for correcting.
        /// </summary>
        /// <param name="id">The document Id</param>
        /// <returns>Returns the number of zones of text in the specified document</returns>
        public int DonneNbZonesDeTexte(int id)
        {
            return _editorService.GetDocumentNoOfSegments();

        }

        public string DonnePolice(int idDoc, int idZone)
        {
            return string.Empty;
        }

        public string DonneTitreDocCourant()
        {
            return _editorService.GetDocumentName();

        }

        public void RemplaceIntervalle(int idDoc, int idZone, int leDebut, int laFin, ref string laChaine)
        {
            _editorService.ReplaceTextInSegment(idZone,leDebut, laFin,laChaine);
    
        }

        public void SelectionneIntervalle(int idDoc, int idZone, int leDebut, int laFin)
        {
            _editorService.SelectText(idZone, leDebut, laFin);
        }
    }
}
