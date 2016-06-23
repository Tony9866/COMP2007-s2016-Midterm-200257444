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
    public partial class TodoList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if loading the page for the first time, populate the Todo grid
            if (!IsPostBack)
            {
                Session["SortColumn"] = "TodoID";
                Session["SortDirection"] = "ASC";
                //get todo data
                this.GetTodos();

            }
        }
        /**
         * <summary>
         * This method gets the game data from the DB
         * </summary>
         * 
         * @method GetGames
         * @returns {void}
         */
        private void GetTodos()
        {
            //connect to db
            using (TodoConnection db = new TodoConnection())
            {

                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                // query the Todo Table using EF and LINQ
                var Todos = (from AllTodos in db.Todos
                             select AllTodos);
                // bind the result to the GridView
                TodosGridView.DataSource = Todos.AsQueryable().OrderBy(SortString).ToList();
                TodosGridView.DataBind();
            }
        }


        /**
        *  <summary>
        * This event handler deletes a Todo from the db using EF
        * </summary>
        *
        * @method TodosGridView_RowDeleting
        * @param {object} sender 
        * @param {GridViewDeleteEventArgs} e
        * @retuens {void}
        * 
        */
        protected void GamesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //store which row was clicked
            int selectedRow = e.RowIndex;

            //get the selected TodoID using the Grid's DataKey collection
            int TodoID = Convert.ToInt32(TodosGridView.DataKeys[selectedRow].Values["TodoID"]);

            //use EF to find the selected Todo in the DB and remove it 
            using (TodoConnection db = new TodoConnection())
            {
                //create object of the Todo Class and store the query string inside of it 
                Todo deletedTodo = (from todosRecords in db.Todos
                                     where todosRecords.TodoID == TodoID
                                    select todosRecords).FirstOrDefault();
                //remove the selected Todo from the db
                db.Todos.Remove(deletedTodo);

                //save my changes back to the db
                db.SaveChanges();

                //refresh the grid
                this.GetTodos();
            }
        }
        /**
         * <summary>
         * This event handler allows pagination to occur for the Games page 
         * <summary>
         * 
         * @method TodosGridView_PageIndexChanging
         * @param {object} sender
         * @param {GridViewPageEventArgs} e
         * @returns {void}
         */
        protected void TodosGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //set the new page number
            TodosGridView.PageIndex = e.NewPageIndex;

            //refresh the grid
            this.GetTodos();
        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set the new Page sizes
            TodosGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //refresh the grid
            this.GetTodos();
        }

        protected void TodosGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //get the column to sorty by
            Session["SortColumn"] = e.SortExpression;


            //refresh the grid
            this.GetTodos();

            //toggle the direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }
        /**
         * <summary>
         * This event handler bound the data for the Games page 
         * <summary>
         * 
         * @method TodosGridView_RowDataBound
         * @param {object} sender
         * @param {GridViewPageEventArgs} e
         * @returns {void}
         */
        protected void TodosGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header) // check to see if the click is on the header row
                {
                    LinkButton linkbutton = new LinkButton();

                    for (int index = 0; index < TodosGridView.Columns.Count; index++)
                    {
                        if (TodosGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
                            {
                                linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkbutton);
                        }
                    }
                }
            }
        }
    }
}