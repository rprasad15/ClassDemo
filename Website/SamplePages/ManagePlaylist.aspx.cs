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
    }

    protected void MoveDown_Click(object sender, EventArgs e)
    {
        //code to go here
    }
    protected void MoveTrack(int trackid, int tracknumber, string direction)
    {
        //code to go here
    }
    protected void DeleteTrack_Click(object sender, EventArgs e)
    {
        //code to go here
    }
}
