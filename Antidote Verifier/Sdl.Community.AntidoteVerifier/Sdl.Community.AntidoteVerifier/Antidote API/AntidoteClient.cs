﻿using Serilog;
using System.Runtime.InteropServices;

namespace Sdl.Community.AntidoteVerifier.Antidote_API
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(IAntidoteClient))]
    [ComVisible(true)]
    public class AntidoteClient : IAntidoteClient
    {
        private readonly IEditorService _editorService;
        private readonly bool _spellChecking;

        public AntidoteClient(IEditorService editorService, bool spellChecking)
        {
            _editorService = editorService;
            _spellChecking = spellChecking;
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
            var documentId = _editorService.GetDocumentId();
            Log.Verbose("Sending document with id {@documentId} to Antidote.",
                documentId);
            return documentId;
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
            var segmentId = _editorService.GetCurrentSegmentId(index);
            Log.Verbose("Prepare to send segment {@segmentId} from document with id {@id} to Antidote", segmentId, id);

            return segmentId;
        }
        
        /// <summary>
        /// Informs Antidote about the current active segment from the Studio Editor.
        /// </summary>
        /// <param name="id">The document Id</param>
        /// <returns>Returns the identifier for the current area</returns>

        public int DonneIdZoneDeTexteCourante(int index)
        {
            if(_spellChecking) return 0;
            var segmentId = _editorService.GetActiveSegmentId(); 
            Log.Verbose("Prepare to send segment {@segmentId} for index {@index} to Antidote", segmentId, index);

            return segmentId;
        }

        public string DonneIntervalle(int idDoc, int idZone, int leDebut, int laFin)
        {
            var text = string.Empty;
            //Document selection is not thread-safe hence we can't get the selected text
            //until this is fixed in the api
            //if (!_spellChecking)
            //{
            //    text = _editorService.GetSelection();
            //}
            //else
            //{
            text = _editorService.GetSegmentText(idZone).Substring(leDebut, laFin - leDebut);
           // }
            Log.Verbose("Sending {@text} corresponding to segment {@idZone} from document with id {@id} to Antidote", text,idZone, idDoc);
            return text;
        }

        /// <summary>
        /// Informs Antidote about the length of the text in the segment.
        /// </summary>
        /// <param name="idDoc">The document id</param>
        /// <param name="idZone">The identifier of the text field</param>
        /// <returns>Returns the text length</returns>
        public int DonneLongueurZoneDeTexte(int idDoc, int idZone)
        {
            var length = 0;
            //Document selection is not thread-safe hence we can't get the selected text
            //until this is fixed in the api
            //if (!_spellChecking)
            //{
            //    length = _editorService.GetSelection().Length;
            //}
            //else
            //{
                length = _editorService.GetSegmentText(idZone).Length;
           // }
            Log.Verbose("Prepare to send segment {@idZone} with length {@length} from document with id {@id} to Antidote", idZone, length, idDoc);

            return length;
        }

	    /// <summary>
	    /// Informs Antidote about the number of segments from the document
	    /// that will be sent for correcting.
	    /// </summary>
	    /// <param name="id">The document Id</param>
	    /// <returns>Returns the number of zones of text in the specified document</returns>
	    public int DonneNbZonesDeTexte(int id)
	    {
		    var numberOfSegments = _editorService.GetDocumentNoOfSegments();
		    Log.Verbose("Sending document with id {@id} to Antidote having {@numberOfSegments} number of segments.",
			    id, numberOfSegments);
		    return numberOfSegments;
	    }

	    public string DonnePolice(int idDoc, int idZone)
        {
            return string.Empty;
        }

        public string DonneTitreDocCourant()
        {
            var documentName = _editorService.GetDocumentName();
            Log.Verbose("Prepare to send document {@documentName} to Antidote", documentName);
            return documentName;

        }

        public bool PeutRemplacer(int idDoc, int idZone, int leDebut, int laFin, string laChaineOrig, string laLangueAffichage, ref string leMessage, ref string lExplication)
        {
	        return _editorService.CanReplace(idZone, leDebut, laFin, laChaineOrig, laLangueAffichage, ref leMessage, ref lExplication);
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
