using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#region Additional Namespaces
using ChinookSystem.BLL;
using Chinook.Data.POCOs;

#endregion
public partial class SamplePages_ManagePlaylist : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Request.IsAuthenticated)
        {
            Response.Redirect("~/Account/Login.aspx");
        }
        else
        {
            TracksSelectionList.DataSource = null;
        }
    }

    protected void CheckForException(object sender, ObjectDataSourceStatusEventArgs e)
    {
        MessageUserControl.HandleDataBoundException(e);
    }

    protected void Page_PreRenderComplete(object sender, EventArgs e)
    {
        //PreRenderComplete occurs just after databinding page events
        //load a pointer to point to your DataPager control
        DataPager thePager = TracksSelectionList.FindControl("DataPager1") as DataPager;
        if (thePager !=null)
        {
            //this code will check the StartRowIndex to see if it is greater that the
            //total count of the collection
            if (thePager.StartRowIndex > thePager.TotalRowCount)
            {
                thePager.SetPageProperties(0, thePager.MaximumRows, true);
            }
        }
    }

    protected void ArtistFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Artist";
        SearchArgID.Text = ArtistDDL.SelectedValue;
        TracksSelectionList.DataBind();
    }

    protected void MediaTypeFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "MediaType";
        SearchArgID.Text = MediaTypeDDL.SelectedValue;
        TracksSelectionList.DataBind();
    }

    protected void GenreFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Genre";
        SearchArgID.Text = GenreDDL.SelectedValue;
        TracksSelectionList.DataBind();
    }

    protected void AlbumFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        TracksBy.Text = "Album";
        SearchArgID.Text = AlbumDDL.SelectedValue;
        TracksSelectionList.DataBind();
    }

    protected void PlayListFetch_Click(object sender, EventArgs e)
    {
        //code to go here
        //standard query
        if (string.IsNullOrEmpty(PlaylistName.Text))
        {
            //put out an erro message
            //this form uses a User control called MessageUserControl
            //The user control will be the mechanism to display messages on this form
            MessageUserControl.ShowInfo("Warning", "Play List Name is required"); //Show info just puts out a message, there is also a bootstrap version as well
        }
        else //If we had something
        {
            string username = User.Identity.Name; //Underneath the user class, find the identity, and then take out Name

            //Message User control has the try/catch coding embedded in the control
            MessageUserControl.TryRun(() => 
            {
                //this is the process coding bloack to be executed under the "watchful eye" of the MessagefeUserControl

                //obtain the user name from the security part of the application

                PlaylistTracksController sysmgr = new PlaylistTracksController();
                List<UserPlaylistTrack> playlist = sysmgr.List_TracksForPlaylist(PlaylistName.Text, username);
                PlayList.DataSource = playlist;
                PlayList.DataBind();
            },"","Here is your current playlist.");
        }
    }

    protected void TracksSelectionList_ItemCommand(object sender,
        ListViewCommandEventArgs e)
    {
        //code to go here
        //ListViewCommandEventArgs paramenters e contains  the CommandArg value
        if (string.IsNullOrEmpty(PlaylistName.Text))
        {
            MessageUserControl.ShowInfo("Warning", "Playlist Name is required.");
        }
        else //If we had something
        {
            string username = User.Identity.Name; //Underneath the user class, find the identity, and then take out Name

            //Trackid is going to come from e.CommandArguement
            //e.CommandArguement is an object therefore convery to string

            int trackid = int.Parse(e.CommandArgument.ToString());


            //the following code calls a BLL method to add to the database
            MessageUserControl.TryRun(() =>
            {
                PlaylistTracksController sysmgr = new PlaylistTracksController();
                List<UserPlaylistTrack> refreshresults = sysmgr.Add_TrackToPLaylist(PlaylistName.Text, username, trackid);
                PlayList.DataSource = refreshresults;
                PlayList.DataBind();


            }, "Sucess", "Track added tp play list");
        }
    }
    protected void MoveUp_Click(object sender, EventArgs e)
    {
        //code to go here
        if (PlayList.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("Warning", "No play lists has been retrieved");
        }
        else
        {
            if (string.IsNullOrEmpty(PlaylistName.Text))
            {
                MessageUserControl.ShowInfo("Warning", "No play list name has been supplied");
            }
            else
            {
                //check only one row selected
                int trackid = 0;
                int tracknumber = 0; //optional
                int rowselected = 0; //search flagg

                CheckBox playlistselection = null; //create a pointer to use for the access of the GRidView control

                //traverse the gridview checking each row for a checked CheckBox

            for (int i = 0; i < PlayList.Rows.Count; i++)
                {
                    //find the checkbof on the indexed gridview row . Playlistselection will point to the checkbox

                    playlistselection = PlayList.Rows[i].FindControl("Selected") as CheckBox;

                    //if checked
                    if (playlistselection.Checked)
                    {
                        trackid = int.Parse((PlayList.Rows[i].FindControl("TrackId") as Label).Text);
                        tracknumber = int.Parse((PlayList.Rows[i].FindControl("TrackNumber") as Label).Text);

                        rowselected++;
                    }//eofor
                    if (rowselected != 1)
                    {
                        MessageUserControl.ShowInfo("Warning", "Select one track to move.");

                    }
                    else
                    {
                        if (tracknumber == 1)
                        {
                            MessageUserControl.ShowInfo("Information", "Track cannot be moved. ALready top of the list");
                        }
                        else
                        {
                            //at this point you have 
                            //playlistname, username, trackid which is neeeded to move the track

                            //Move the track via your BLL
                            MoveTrack(trackid, tracknumber, "up");
                        }
                    }
                }
            }
        }
    }

    protected void MoveDown_Click(object sender, EventArgs e)
    {

        if (PlayList.Rows.Count == 0)
        {
            MessageUserControl.ShowInfo("Warning", "No play lists has been retrieved");
        }
        else
        {
            if (string.IsNullOrEmpty(PlaylistName.Text))
            {
                MessageUserControl.ShowInfo("Warning", "No play list name has been supplied");
            }
            else
            {
                //check only one row selected
                int trackid = 0;
                int tracknumber = 0; //optional
                int rowselected = 0; //search flag

                CheckBox playlistselection = null; //create a pointer to use for the access of the GRidView control

                //traverse the gridview checking each row for a checked CheckBox

                for (int i = 0; i < PlayList.Rows.Count; i++)
                {
                    //find the checkbof on the indexed gridview row . Playlistselection will point to the checkbox

                    playlistselection = PlayList.Rows[i].FindControl("Selected") as CheckBox;

                    //if checked
                    if (playlistselection.Checked)
                    {
                        trackid = int.Parse((PlayList.Rows[i].FindControl("TrackId") as Label).Text);
                        tracknumber = int.Parse((PlayList.Rows[i].FindControl("TrackNumber") as Label).Text);

                        rowselected++;
                    }//eofor
                    if (rowselected != 1)
                    {
                        MessageUserControl.ShowInfo("Warning", "Select one track to move.");

                    }
                    else
                    {
                        if (tracknumber == PlayList.Rows.Count)
                        {
                            MessageUserControl.ShowInfo("Information", "Track cannot be moved. Already bottom of the list");
                        }
                        else
                        {
                            //at this point you have 
                            //playlistname, username, trackid which is neeeded to move the track

                            //Move the track via your BLL
                            MoveTrack(trackid, tracknumber, "down");
                        }
                    }
                }
            }
        }
    }
    protected void MoveTrack(int trackid, int tracknumber, string direction)
    {
        //code to go here
        MessageUserControl.TryRun(() =>
        {
            //standard call to a BLL method

            //update call
            PlaylistTracksController sysmgr = new PlaylistTracksController();
            sysmgr.MoveTrack(User.Identity.Name, PlaylistName.Text, trackid, tracknumber, direction);

            //refresh the list
            //query call
            List<UserPlaylistTrack> results = sysmgr.List_TracksForPlaylist(PlaylistName.Text, User.Identity.Name);

            PlayList.DataSource = results;
            PlayList.DataBind();


        }, "Success", "Track moved");
    }
    protected void DeleteTrack_Click(object sender, EventArgs e)
    {
        //code to go here
    }
}
