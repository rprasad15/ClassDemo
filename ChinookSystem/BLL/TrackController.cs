using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Chinook.Data.Entities;
using Chinook.Data.DTOs;
using Chinook.Data.POCOs;
using ChinookSystem.DAL;
using System.ComponentModel;
#endregion

namespace ChinookSystem.BLL
{
    [DataObject]
    public class TrackController
    {
        [DataObjectMethod(DataObjectMethodType.Select,false)]
        public List<TrackList> List_TracksForPlaylistSelection(string tracksby, int argid)
        {
            using (var context = new ChinookContext())
            {
                List<TrackList> results = null;

               //code to go here

                return results;
            }
        }//eom

       
    }//eoc
}
