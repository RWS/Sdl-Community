using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier.Antidote_API
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IAntidoteClient
    {
        void ActiveApplication();
        void ActiveDocument(int idDoc);
        int DonneDebutSelection(int idDoc, int idZone);
        int DonneFinSelection(int idDoc, int idZone);
        int DonneIdDocumentCourant();
        int DonneIdZoneDeTexte(int id, int indice);
        int DonneIdZoneDeTexteCourante(int id);
        string DonneIntervalle(int idDoc, int idZone, int leDebut, int laFin);
        int DonneLongueurZoneDeTexte(int idDoc, int idZone);
        int DonneNbZonesDeTexte(int id);
        string DonnePolice(int idDoc, int idZone);
        string DonneTitreDocCourant();
        bool PeutRemplacer(int idDoc, int idZone, int leDebut, int laFin, string laChaineOrig, string laLangueAffichage, ref string leMessage, ref string lExplication);
        void RemplaceIntervalle(int idDoc, int idZone, int leDebut, int laFin, ref string laChaine);
        void SelectionneIntervalle(int idDoc, int idZone, int leDebut, int laFin);   
	}
}
