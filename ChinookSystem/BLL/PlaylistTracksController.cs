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

                    if (newtrack != null)
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
                var exists = (from x in context.Playlists
                              where x.UserName.Equals(username) && x.Name.Equals(playlistname)

                              select x).FirstOrDefault();

                if (exists == null)
                {
                    throw new Exception("Playlist has been removed boi.");
                }
                else
                {
                    //limit your search to the particuar playlist
                    PlaylistTrack movetrack = (from x in exists.PlaylistTracks
                                               where x.TrackId == trackid
                                               select x).FirstOrDefault();

                    if (movetrack == null)
                    {
                        throw new Exception("Playlist track has been removed nub.");
                    }
                    else
                    {
                        PlaylistTrack othertrack = null;
                        if (direction.Equals("up"))
                        {
                            //Up movement of a button
                            if (movetrack.TrackNumber == 1)
                            {
                                throw new Exception("Playlist track cannot me moved scrub.");
                            }
                            else
                            {
                                //get teh other track now
                                movetrack = (from x in exists.PlaylistTracks
                                             where x.TrackNumber == movetrack.TrackNumber - 1
                                             select x).FirstOrDefault();

                                if (othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved up, gandalf");
                                }
                                else
                                {
                                    // at this point you can exchange track numbers

                                    movetrack.TrackNumber -= 1;
                                    othertrack.TrackNumber += 1;
                                }
                            }
                        }
                        else
                        {
                            //Down movement of a button
                            if (movetrack.TrackNumber == exists.PlaylistTracks.Count)
                            {
                                throw new Exception("Playlist track cannot me moved down scrub.");
                            }
                            else
                            {
                                //get teh other track now
                                othertrack = (from x in exists.PlaylistTracks
                                              where x.TrackNumber == othertrack.TrackNumber + 1
                                              select x).FirstOrDefault();

                                if (othertrack == null)
                                {
                                    throw new Exception("Playlist track cannot be moved down, gandalf");
                                }
                                else
                                {
                                    // at this point you can exchange track numbers

                                    movetrack.TrackNumber += 1;
                                    othertrack.TrackNumber -= 1;
                                }
                            }
                        }// eo up/down

                        //STAGE CHANGES FOR SaveChanges()
                        //indicate only the field that needs to be updated
                        context.Entry(movetrack).Property(y => y.TrackNumber).IsModified = true;
                        context.Entry(othertrack).Property(y => y.TrackNumber).IsModified = true;
                        context.SaveChanges();
                    }
                }
            }
        }//eom


        public List<UserPlaylistTrack> DeleteTracks(string username, string playlistname, List<int> trackstodelete)
        {
            using (var context = new ChinookContext())
            {
                //code to go here
                //firstor default it takes the first occurence in collection, if there is nothing there then itll give you a null value
                Playlist exists = (from x in context.Playlists
                                   where x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)
       && x.Name.Equals(playlistname, StringComparison.OrdinalIgnoreCase)
                                   select x).FirstOrDefault();

                if (exists == null)
                {
                    throw new Exception("Playlist has been removed from the databse.");
                }
                else
                {
                    //the tracks that are to be kept 
                    //take the track off the current place is what teh code below does

                    //when you do anyhting in lamda, the first thing is the object Name or row (tr)
                    //come over to trackstodelete to do the following work which is the Any test
                    //so what are the condition of the any test?
                    //you have to refer to an object in the second list which is the tdo, compare the tr instance to the tdo instance
                    //what you are doing is comparing the tdo which is an int and is it equal to the track id,  tdo ==tr.trackid
                    var trackskept = exists.PlaylistTracks.Where(tr => !trackstodelete.Any(tdo => tdo == tr.TrackId)).Select(tr => tr);

                    //Remove the tracks in the trackstodelete list
                    PlaylistTrack item = null;
                    foreach (var deletetrack in trackstodelete)
                    {
                        item = exists.PlaylistTracks
                            .Where(dx => dx.TrackId == deletetrack).FirstOrDefault();
                        if (item != null)
                        {
                            exists.PlaylistTracks.Remove(item);
                        }
                    }
                    //renumber the kept tracks so that the tracknumber
                    //is sequential as is expected by all other operations
                    //in our database there is no holes in the numeric
                    //sequence
                    int number = 1;
                    foreach (var trackkept in trackskept)
                    {
                        trackkept.TrackNumber = number;
                        context.Entry(trackkept).Property(y => y.TrackNumber).IsModified = true;
                        number++;
                    }
                    context.SaveChanges();
                    return List_TracksForPlaylist(playlistname, username);
                }
            }
        }//eom
    }
}
