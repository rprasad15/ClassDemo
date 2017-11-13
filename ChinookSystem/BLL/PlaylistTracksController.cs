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
    public class PlaylistTracksController
    {
        public List<UserPlaylistTrack> List_TracksForPlaylist(
            string playlistname, string username)
        {
            using (var context = new ChinookContext())
            {

                //What would happen if there is no match for the incoming parametr values?
                //We need to ensure that results have a valid value
                //this value will be an IEnumerable<T> collection or it should be null
                //to ensure that results does end up with a valid value use the .FirstOrDefault()
                var results = (from x in context.Playlists
                               where x.UserName.Equals(username) && x.Name.Equals(playlistname)

                               select x).FirstOrDefault();

                var theTracks = from x in context.PlaylistTracks
                                where x.PlaylistId.Equals(results.PlaylistId)
                                orderby x.TrackNumber
                                select new UserPlaylistTrack
                                {
                                    TrackID = x.TrackId,
                                    TrackNumber = x.TrackNumber,
                                    Milliseconds = x.Track.Milliseconds,
                                    TrackName = x.Track.Name,
                                    UnitPrice = x.Track.UnitPrice
                                };
                return theTracks.ToList();
            }
        }//eom
        public List<UserPlaylistTrack> Add_TrackToPLaylist(string playlistname, string username, int trackid)
        {
            using (var context = new ChinookContext())
            {
                //Part One: Handle Playlist record
                //code to go here
                //query to get the playlist ID
                var exists = (from x in context.Playlists
                               where x.UserName.Equals(username) && x.Name.Equals(playlistname)

                               select x).FirstOrDefault();
                //intialisze the tracknumber for the track going into PlaylistTracks
                int tracknumber = 0;
                // I will needed to create a instance of PlalsitTracks
                PlaylistTrack newtrack = null;
                //determine if this is an addition to an exisint list or 
                //if a new list needs to be created
                if (exists == null)
                {
                    //this is a new playlist
                    //create the playlist
                    exists = new Playlist();
                    exists.Name = playlistname;
                    exists.UserName = username;
                    exists = context.Playlists.Add(exists);
                    tracknumber = 1;


                }
                else
                {
                    //the palylist already exists
                    //I neeed to know the number of tracks currently on the list
                    //track number = count + 1
                    tracknumber = exists.PlaylistTracks.Count();


                    //in our exampletracks exist only once on each playlist
                    newtrack = exists.PlaylistTracks.SingleOrDefault(x => x.TrackId == trackid);

                    //this will be null if the track is NOT on the playlist tracks

                if(newtrack != null)
                    {
                        throw new Exception("Playlist already requested track.");
                    }
                }
                //Part Two: Handle the track for PlaylistTrack

                //use navigation to .Add the new track to PlaylistTrack
                newtrack = new PlaylistTrack();
                newtrack.TrackId = trackid;
                newtrack.TrackNumber = tracknumber;

                //NOTE: the pkey for the PlaylistID may not exist yet
                //using navigation one can let  HashSet handle the PLaylistId pkey
                exists.PlaylistTracks.Add(newtrack);

                //Physcially commit your work to the database
                context.SaveChanges();

                //refresh list
                return List_TracksForPlaylist(playlistname, username);
            }
        }//eom
        public void MoveTrack(string username, string playlistname, int trackid, int tracknumber, string direction)
        {
            using (var context = new ChinookContext())
            {
                //code to go here 

            }
        }//eom


        public void DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            using (var context = new ChinookContext())
            {
               //code to go here


            }
        }//eom
    }
}
