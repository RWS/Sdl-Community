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
        void ActiveDocument(long idDoc);
        long DonneDebutSelection(long idDoc, long idZone);
        long DonneFinSelection(long idDoc, long idZone);
        long DonneIdDocumentCourant();
        long DonneIdZoneDeTexte(long id, long indice);
        long DonneIdZoneDeTexteCourante(long id);
        string DonneIntervalle(long idDoc, long idZone, long leDebut, long laFin);
        long DonneLongueurZoneDeTexte(long idDoc, long idZone);
        long DonneNbZonesDeTexte(long id);
        string DonnePolice(long idDoc, long idZone);
        string DonneTitreDocCourant();
        void RemplaceIntervalle(long idDoc, long idZone, long leDebut, long laFin, ref string laChaine);
        void SelectionneIntervalle(long idDoc, long idZone, long leDebut, long laFin);
    }
}
