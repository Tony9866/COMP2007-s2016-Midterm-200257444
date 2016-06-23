using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/**
  * @author:Zhen Zhang 200257444
  * @date: June 23, 2016
  * @version： 0.0.3
  * @file description: create a web app that show the Todo statistics.
  */

// using statements that are required to connect to DB
using COMP2007_S2016_MidTerm_200257444.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_MidTerm_200257444
{
    public partial class TodoDetails: System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0)) 
            {

                this.GetTodo();
            }
        }
          /**
          *  <summary>
          * This event handler deletes a Todo from the db
          * </summary>
          *
          * @method GetTodo
          * @retuens {void}
          * 
          */
        private void GetTodo()
        {
            //populate the data form with existing data from the database
            int TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

            //connect to DB
            using (TodoConnection db= new TodoConnection()) 
            {
                //populate a game object instance with the GameID from the URL Parameter
                Todo updatedTodo = (from todo in db.Todos
                                    where todo.TodoID == TodoID
                                    select todo).FirstOrDefault();

                //map the Todo properties to the form controls
                if (updatedTodo != null) {
                    TodoNameTextBox.Text = updatedTodo.TodoName;
                    TodoNotesTextBox.Text = updatedTodo.TodoNotes;
                
                }
            }
        }

        /**
       *  <summary>
       * This event handler redirect to the Games Page
       * </summary>
       * @retuens {void}
       */
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            // Redirect back to Games page
            Response.Redirect("~/TodoList.aspx");
        }

                /**
        *  <summary>
        * This event handler add or update a Todo to the db
        * </summary>
        *
        *@param {object} sender 
        *@param {GridViewDeleteEventArgs} e
        *@retuens {void}
        * 
        */
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Use EF to connect to the server
            using (TodoConnection db = new TodoConnection()) { 
            //save the new record
                Todo newTodo= new Todo();

                int TodoID = 0;

                if (Request.QueryString.Count > 0) {
                    //get the ID from URL
                    TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

                    //get the current from db
                    newTodo = (from todo in db.Todos where todo.TodoID == TodoID select todo).First();
                
                }

                // add form data to the new todo record
                newTodo.TodoName = TodoNameTextBox.Text;
                newTodo.TodoNotes = TodoNotesTextBox.Text;

                // use LINQ to ADO.NET to add / insert new game into the database

                if (TodoID == 0)
                {
                    db.Todos.Add(newTodo);
                }

                // save our changes - also update and inserts
                db.SaveChanges();

                // Redirect back to the updated games page
                Response.Redirect("~/TodoList.aspx");
            
            }




        }
    }
}