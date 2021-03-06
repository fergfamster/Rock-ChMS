﻿//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Rock;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Attribute = Rock.Model.Attribute;

namespace RockWeb.Blocks.Administration
{
    /// <summary>
    /// 
    /// </summary>
    public partial class WorkflowTypeDetail : RockBlock, IDetailBlock
    {
        #region WorkflowActivityType ViewStateList

        /// <summary>
        /// Gets or sets the state of the workflow activity types.
        /// </summary>
        /// <value>
        /// The state of the workflow activity types.
        /// </value>
        private ViewStateList<WorkflowActivityType> WorkflowActivityTypesState
        {
            get
            {
                return ViewState["WorkflowActivityTypesState"] as ViewStateList<WorkflowActivityType>;
            }

            set
            {
                ViewState["WorkflowActivityTypesState"] = value;
            }
        }

        #endregion

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // assign attributes grid actions
            gWorkflowTypeAttributes.DataKeyNames = new string[] { "Guid" };
            gWorkflowTypeAttributes.Actions.IsAddEnabled = true;
            gWorkflowTypeAttributes.Actions.AddClick += gWorkflowTypeAttributes_Add;
            gWorkflowTypeAttributes.GridRebind += gWorkflowTypeAttributes_GridRebind;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                string itemId = PageParameter( "workflowTypeId" );
                if ( !string.IsNullOrWhiteSpace( itemId ) )
                {
                    ShowDetail( "workflowTypeId", int.Parse( itemId ) );
                }
                else
                {
                    pnlDetails.Visible = false;
                }
            }
        }

        #endregion

        #region Edit Events

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            if ( hfWorkflowTypeId.Value.Equals( "0" ) )
            {
                // Cancelling on Add.  Return to Grid
                NavigateToParentPage();
            }
            else
            {
                // Cancelling on Edit.  Return to Details
                WorkflowTypeService service = new WorkflowTypeService();
                WorkflowType item = service.Get( int.Parse( hfWorkflowTypeId.Value ) );
                ShowReadonlyDetails( item );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnEdit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnEdit_Click( object sender, EventArgs e )
        {
            WorkflowTypeService service = new WorkflowTypeService();
            WorkflowType item = service.Get( int.Parse( hfWorkflowTypeId.Value ) );
            ShowEditDetails( item );
        }

        /// <summary>
        /// Sets the edit mode.
        /// </summary>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        private void SetEditMode( bool editable )
        {
            pnlEditDetails.Visible = editable;
            fieldsetViewDetails.Visible = !editable;
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            WorkflowType workflowType;
            WorkflowTypeService service = new WorkflowTypeService();

            int workflowTypeId = int.Parse( hfWorkflowTypeId.Value );

            if ( workflowTypeId == 0 )
            {
                workflowType = new WorkflowType();
                workflowType.IsSystem = false;
                workflowType.Name = string.Empty;
            }
            else
            {
                workflowType = service.Get( workflowTypeId );
            }

            workflowType.Name = tbName.Text;
            workflowType.Description = tbDescription.Text;
            workflowType.CategoryId = ddlCategory.SelectedValueAsInt();
            workflowType.Order = int.Parse( tbOrder.Text );
            workflowType.WorkTerm = tbWorkTerm.Text;
            if ( !string.IsNullOrWhiteSpace( tbProcessingInterval.Text ) )
            {
                workflowType.ProcessingIntervalSeconds = int.Parse( tbProcessingInterval.Text );
            }

            workflowType.IsPersisted = cbIsPersisted.Checked;
            workflowType.LoggingLevel = ddlLoggingLevel.SelectedValueAsEnum<WorkflowLoggingLevel>();
            workflowType.IsActive = cbIsActive.Checked;

            // check for duplicates within Category
            if ( service.Queryable().Where( g => ( g.CategoryId == workflowType.CategoryId ) ).Count( a => a.Name.Equals( workflowType.Name, StringComparison.OrdinalIgnoreCase ) && !a.Id.Equals( workflowType.Id ) ) > 0 )
            {
                tbName.ShowErrorMessage( WarningMessage.DuplicateFoundMessage( "name", WorkflowType.FriendlyTypeName ) );
                return;
            }

            if ( !Page.IsValid )
            {
                return;
            }

            if ( !workflowType.IsValid )
            {
                // Controls will render the error messages                    
                return;
            }

            RockTransactionScope.WrapTransaction( () =>
            {
                List<WorkflowActivityTypeEditor> workflowActivityTypeEditorList = phActivities.Controls.OfType<WorkflowActivityTypeEditor>().ToList();
                
                // delete WorkflowActionTypes that aren't assigned in the UI anymore
                WorkflowActionTypeService workflowActionTypeService = new WorkflowActionTypeService();
                List<WorkflowActionType> actionTypesInDB = workflowActionTypeService.Queryable().Where( a => a.ActivityType.WorkflowTypeId.Equals( workflowType.Id ) ).ToList();
                List<WorkflowActionType> actionTypesInUI = new List<WorkflowActionType>();
                foreach ( WorkflowActivityTypeEditor workflowActivityTypeEditor in workflowActivityTypeEditorList )
                {
                    foreach ( WorkflowActionTypeEditor editor in workflowActivityTypeEditor.Controls.OfType<WorkflowActionTypeEditor>() )
                    {
                        actionTypesInUI.Add( editor.WorkflowActionType );
                    }
                }

                var deletedActionTypes = from actionType in actionTypesInDB
                                         where !actionTypesInUI.Select( u => u.Id ).Contains( actionType.Id )
                                         select actionType;

                deletedActionTypes.ToList().ForEach( actionType =>
                {
                    workflowActionTypeService.Delete( actionType, CurrentPersonId );
                    workflowActionTypeService.Save( actionType, CurrentPersonId );
                } );

                // delete WorkflowActivityTypes that aren't assigned in the UI anymore
                WorkflowActivityTypeService workflowActivityTypeService = new WorkflowActivityTypeService();
                List<WorkflowActivityType> activityTypesInDB = workflowActivityTypeService.Queryable().Where( a => a.WorkflowTypeId.Equals( workflowType.Id ) ).ToList();
                List<WorkflowActivityType> activityTypesInUI = workflowActivityTypeEditorList.Select( a => a.GetWorkflowActivityType() ).ToList();

                var deletedActivityTypes = from activityType in activityTypesInDB
                                           where !activityTypesInUI.Select( u => u.Id ).Contains( activityType.Id )
                                           select activityType;

                deletedActivityTypes.ToList().ForEach( activityType =>
                {
                    workflowActivityTypeService.Delete( activityType, CurrentPersonId );
                    workflowActivityTypeService.Save( activityType, CurrentPersonId );
                } );

                // add or update WorkflowActivityTypes(and Actions) that are assigned in the UI
                int workflowActivityTypeOrder = 0;
                foreach ( WorkflowActivityTypeEditor workflowActivityTypeEditor in workflowActivityTypeEditorList )
                {
                    WorkflowActivityType editorWorkflowActivityType = workflowActivityTypeEditor.GetWorkflowActivityType();
                    WorkflowActivityType workflowActivityType = workflowType.ActivityTypes.FirstOrDefault( a => a.Guid.Equals( editorWorkflowActivityType.Guid ) );
                    
                    if ( workflowActivityType == null )
                    {
                        workflowActivityType = editorWorkflowActivityType;
                        workflowType.ActivityTypes.Add( workflowActivityType );
                    }
                    else
                    {
                        workflowActivityType.Name = editorWorkflowActivityType.Name;
                        workflowActivityType.Description = editorWorkflowActivityType.Description;
                        workflowActivityType.IsActive = editorWorkflowActivityType.IsActive;
                        workflowActivityType.IsActivatedWithWorkflow = editorWorkflowActivityType.IsActivatedWithWorkflow;
                    }

                    workflowActivityType.Order = workflowActivityTypeOrder++;
                    
                    int workflowActionTypeOrder = 0;
                    foreach ( WorkflowActionTypeEditor workflowActionTypeEditor in workflowActivityTypeEditor.Controls.OfType<WorkflowActionTypeEditor>() )
                    {
                        WorkflowActionType editorWorkflowActionType = workflowActionTypeEditor.WorkflowActionType;
                        WorkflowActionType workflowActionType = workflowActivityType.ActionTypes.FirstOrDefault( a => a.Guid.Equals( editorWorkflowActionType.Guid ) );
                        if ( workflowActionType == null )
                        {
                            workflowActionType = editorWorkflowActionType;
                            workflowActivityType.ActionTypes.Add( workflowActionType );
                        }
                        else
                        {
                            workflowActionType.Name = editorWorkflowActionType.Name;
                            workflowActionType.IsActionCompletedOnSuccess = editorWorkflowActionType.IsActionCompletedOnSuccess;
                            workflowActionType.IsActivityCompletedOnSuccess = editorWorkflowActionType.IsActivityCompletedOnSuccess;
                        }
                        
                        workflowActionType.Order = workflowActionTypeOrder++;
                    }
                }

                if ( workflowType.Id.Equals( 0 ) )
                {
                    service.Add( workflowType, CurrentPersonId );
                }

                service.Save( workflowType, CurrentPersonId );
            } );

            // reload item from db using a new context
            workflowType = new WorkflowTypeService().Get( workflowType.Id );
            ShowReadonlyDetails( workflowType );
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Loads the drop downs.
        /// </summary>
        private void LoadDropDowns()
        {
            CategoryService categoryService = new CategoryService();
            var catList = categoryService.Queryable().OrderBy( a => a.Name ).ToList();
            catList.Insert( 0, new Category { Id = None.Id, Name = None.Text } );
            ddlCategory.DataSource = catList;
            ddlCategory.DataBind();

            ddlLoggingLevel.BindToEnum( typeof( WorkflowLoggingLevel ) );
        }

        /// <summary>
        /// Shows the detail.
        /// </summary>
        /// <param name="itemKey">The item key.</param>
        /// <param name="itemKeyValue">The item key value.</param>
        public void ShowDetail( string itemKey, int itemKeyValue )
        {
            if ( !itemKey.Equals( "workflowTypeId" ) )
            {
                pnlDetails.Visible = false;
                return;
            }

            WorkflowType workflowType = null;

            if ( !itemKeyValue.Equals( 0 ) )
            {
                workflowType = new WorkflowTypeService().Get( itemKeyValue );
            }
            else
            {
                workflowType = new WorkflowType { Id = 0, IsActive = true, IsSystem = false };
            }

            if ( workflowType == null )
            {
                return;
            }

            pnlDetails.Visible = true;
            hfWorkflowTypeId.Value = workflowType.Id.ToString();

            // render UI based on Authorized and IsSystem
            bool readOnly = false;

            nbEditModeMessage.Text = string.Empty;
            if ( !IsUserAuthorized( "Edit" ) )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlyEditActionNotAllowed( WorkflowType.FriendlyTypeName );
            }

            if ( workflowType.IsSystem )
            {
                readOnly = true;
                nbEditModeMessage.Text = EditModeMessage.ReadOnlySystem( WorkflowType.FriendlyTypeName );
            }

            if ( readOnly )
            {
                btnEdit.Visible = false;
                ShowReadonlyDetails( workflowType );
            }
            else
            {
                btnEdit.Visible = true;
                if ( workflowType.Id > 0 )
                {
                    ShowReadonlyDetails( workflowType );
                }
                else
                {
                    ShowEditDetails( workflowType );
                }
            }

            BindWorkflowTypeAttributesGrid();
        }

        /// <summary>
        /// Shows the edit details.
        /// </summary>
        /// <param name="workflowType">Type of the workflow.</param>
        private void ShowEditDetails( WorkflowType workflowType )
        {
            if ( workflowType.Id > 0 )
            {
                lActionTitle.Text = ActionTitle.Edit( WorkflowType.FriendlyTypeName );
            }
            else
            {
                lActionTitle.Text = ActionTitle.Add( WorkflowType.FriendlyTypeName );
            }

            SetEditMode( true );

            LoadDropDowns();

            tbName.Text = workflowType.Name;
            tbDescription.Text = workflowType.Description;
            cbIsActive.Checked = workflowType.IsActive ?? false;
            ddlCategory.SetValue( workflowType.CategoryId );
            tbWorkTerm.Text = workflowType.WorkTerm;
            tbOrder.Text = workflowType.Order.ToString();
            tbProcessingInterval.Text = workflowType.ProcessingIntervalSeconds != null ? workflowType.ProcessingIntervalSeconds.ToString() : string.Empty;
            cbIsPersisted.Checked = workflowType.IsPersisted;
            ddlLoggingLevel.SetValue( (int)workflowType.LoggingLevel );

            phActivities.Controls.Clear();
            foreach ( WorkflowActivityType workflowActivityType in workflowType.ActivityTypes.OrderBy( a => a.Order ) )
            {
                CreateWorkflowActivityTypeEditorControls( workflowActivityType );
            }
        }

        /// <summary>
        /// Shows the readonly details.
        /// </summary>
        /// <param name="workflowType">Type of the workflow.</param>
        private void ShowReadonlyDetails( WorkflowType workflowType )
        {
            SetEditMode( false );
            hfWorkflowTypeId.SetValue( workflowType.Id );
            lReadOnlyTitle.Text = workflowType.Name;
            string activeHtmlFormat = "<span class='label {0} pull-right' >{1}</span>";
            if ( workflowType.IsActive ?? false )
            {
                lblActiveHtml.Text = string.Empty;
            }
            else
            {
                lblActiveHtml.Text = string.Format( activeHtmlFormat, "label-important", "Inactive" );
            }

            string descriptionFormat = "<dt>{0}</dt><dd>{1}</dd>";
            lblMainDetails.Text = @"
<div>
    <dl>";

            lblMainDetails.Text += string.Format( descriptionFormat, "Description", workflowType.Description );

            if ( workflowType.Category != null )
            {
                lblMainDetails.Text += string.Format( descriptionFormat, "Category", workflowType.Category.Name );
            }

            lblMainDetails.Text += @"
    </dl>
</div>";

            if ( workflowType.ActivityTypes.Count > 0 )
            {
                // Activities
                lblWorkflowActivitiesReadonly.Text = @"
<div>
    <ol>";

                foreach ( var activityType in workflowType.ActivityTypes.OrderBy( a => a.Order ) )
                {
                    string activityTypeTextFormat = @"
        <li>
            <strong>{0}</strong>
            {1}
            <br />
            {2}
            <ol>
                {3}
            </ol>
        </li>
";

                    string actionTypeText = string.Empty;

                    foreach ( var actionType in activityType.ActionTypes.OrderBy( a => a.Order ) )
                    {
                        actionTypeText += string.Format( "<li>{0}</li>" + Environment.NewLine, actionType.Name );
                    }

                    string actionsTitle = activityType.ActionTypes.Count > 0 ? "Actions:" : "No Actions";

                    lblWorkflowActivitiesReadonly.Text += string.Format( activityTypeTextFormat, activityType.Name, activityType.Description, actionsTitle, actionTypeText );
                }

                lblWorkflowActivitiesReadonly.Text += @"
    </ol>
</div>
";
            }
            else
            {
                lblWorkflowActivitiesReadonly.Text = "<div>" + None.TextHtml + "</div>";
            }
        }

        #endregion

        #region WorkflowTypeAttributes Grid and Picker

        /// <summary>
        /// Handles the Add event of the gWorkflowTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gWorkflowTypeAttributes_Add( object sender, EventArgs e )
        {
            gWorkflowTypeAttributes_ShowEdit( Guid.Empty );
        }

        /// <summary>
        /// Handles the Edit event of the gWorkflowTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gWorkflowTypeAttributes_Edit( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            gWorkflowTypeAttributes_ShowEdit( attributeGuid );
        }

        /// <summary>
        /// Gs the workflow type attributes_ show edit.
        /// </summary>
        /// <param name="attributeGuid">The attribute GUID.</param>
        protected void gWorkflowTypeAttributes_ShowEdit( Guid attributeGuid )
        {
            pnlDetails.Visible = false;
            vsDetails.Enabled = false;
            pnlWorkflowTypeAttributes.Visible = true;
            Attribute attribute;
            string actionTitle;
            if ( attributeGuid.Equals( Guid.Empty ) )
            {
                attribute = new Attribute();
                actionTitle = ActionTitle.Add( "attribute for workflow type " + tbName.Text );
            }
            else
            {
                AttributeService attributeService = new AttributeService();
                attribute = attributeService.Get( attributeGuid );
                actionTitle = ActionTitle.Edit( "attribute for workflow type " + tbName.Text );
            }

            edtWorkflowTypeAttributes.EditAttribute( attribute, actionTitle );
        }

        /// <summary>
        /// Handles the Delete event of the gWorkflowTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RowEventArgs" /> instance containing the event data.</param>
        protected void gWorkflowTypeAttributes_Delete( object sender, RowEventArgs e )
        {
            Guid attributeGuid = (Guid)e.RowKeyValue;
            AttributeService attributeService = new AttributeService();
            Attribute attribute = attributeService.Get( attributeGuid );

            if ( attribute != null )
            {
                string errorMessage;
                if ( !attributeService.CanDelete( attribute, out errorMessage ) )
                {
                    mdGridWarningAttributes.Show( errorMessage, ModalAlertType.Information );
                    return;
                }

                Rock.Web.Cache.AttributeCache.Flush( attribute.Id );
                attributeService.Delete( attribute, CurrentPersonId );
                attributeService.Save( attribute, CurrentPersonId );
            }

            // reload page so that other blocks respond to any data that was changed
            var qryParams = new Dictionary<string, string>();
            qryParams["workflowTypeId"] = hfWorkflowTypeId.Value;
            NavigateToPage( this.CurrentPage.Guid, qryParams );
        }

        /// <summary>
        /// Handles the GridRebind event of the gWorkflowTypeAttributes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void gWorkflowTypeAttributes_GridRebind( object sender, EventArgs e )
        {
            BindWorkflowTypeAttributesGrid();
        }

        /// <summary>
        /// Handles the Click event of the btnSaveWorkflowTypeAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSaveWorkflowTypeAttribute_Click( object sender, EventArgs e )
        {
            Attribute attribute;
            AttributeService attributeService = new AttributeService();
            if ( edtWorkflowTypeAttributes.AttributeId.Equals( 0 ) )
            {
                attribute = new Attribute();
            }
            else
            {
                attribute = attributeService.Get( edtWorkflowTypeAttributes.AttributeId );
            }

            edtWorkflowTypeAttributes.GetAttributeValues( attribute );

            // Controls will show warnings
            if ( !attribute.IsValid )
            {
                return;
            }

            RockTransactionScope.WrapTransaction( () =>
            {
                if ( attribute.Id.Equals( 0 ) )
                {
                    attribute.EntityTypeId = Rock.Web.Cache.EntityTypeCache.Read( new Workflow().TypeName ).Id;
                    attribute.EntityTypeQualifierColumn = "WorkflowTypeId";
                    attribute.EntityTypeQualifierValue = hfWorkflowTypeId.Value;
                    attributeService.Add( attribute, CurrentPersonId );
                }

                Rock.Web.Cache.AttributeCache.Flush( attribute.Id );
                attributeService.Save( attribute, CurrentPersonId );
            } );

            pnlDetails.Visible = true;
            pnlWorkflowTypeAttributes.Visible = false;

            // reload page so that other blocks respond to any data that was changed
            var qryParams = new Dictionary<string, string>();
            qryParams["workflowTypeId"] = hfWorkflowTypeId.Value;
            NavigateToPage( this.CurrentPage.Guid, qryParams );
        }

        /// <summary>
        /// Handles the Click event of the btnCancelWorkflowTypeAttribute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancelWorkflowTypeAttribute_Click( object sender, EventArgs e )
        {
            pnlDetails.Visible = true;
            pnlWorkflowTypeAttributes.Visible = false;
        }

        /// <summary>
        /// Binds the workflow type attributes grid.
        /// </summary>
        private void BindWorkflowTypeAttributesGrid()
        {
            AttributeService attributeService = new AttributeService();

            int workflowTypeId = hfWorkflowTypeId.ValueAsInt();

            var qryWorkflowTypeAttributes = attributeService.GetByEntityTypeId( new Workflow().TypeId ).AsQueryable()
                .Where( a => a.EntityTypeQualifierColumn.Equals( "WorkflowTypeId", StringComparison.OrdinalIgnoreCase )
                && a.EntityTypeQualifierValue.Equals( workflowTypeId.ToString() ) );

            gWorkflowTypeAttributes.DataSource = qryWorkflowTypeAttributes.OrderBy( a => a.Name ).ToList();
            gWorkflowTypeAttributes.DataBind();
        }

        #endregion

        #region Activities and Actions

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );
            phActivities.Controls.Clear();

            foreach ( WorkflowActivityType workflowActivityType in WorkflowActivityTypesState )
            {
                CreateWorkflowActivityTypeEditorControls( workflowActivityType );
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            WorkflowActivityTypesState = new ViewStateList<WorkflowActivityType>();
            foreach ( var activityEditor in phActivities.Controls.OfType<WorkflowActivityTypeEditor>() )
            {
                WorkflowActivityType workflowActivityType = activityEditor.GetWorkflowActivityType();
                WorkflowActivityTypesState.Add( workflowActivityType );
            }

            return base.SaveViewState();
        }

        /// <summary>
        /// Handles the Click event of the lbAddActivity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void lbAddActivityType_Click( object sender, EventArgs e )
        {
            WorkflowActivityType workflowActivityType = new WorkflowActivityType();
            workflowActivityType.Guid = Guid.NewGuid();

            CreateWorkflowActivityTypeEditorControls( workflowActivityType );
        }

        /// <summary>
        /// Creates the workflow activity type editor control.
        /// </summary>
        /// <param name="workflowActivityType">Type of the workflow activity.</param>
        private void CreateWorkflowActivityTypeEditorControls( WorkflowActivityType workflowActivityType )
        {
            WorkflowActivityTypeEditor workflowActivityTypeEditor = new WorkflowActivityTypeEditor();
            workflowActivityTypeEditor.ID = "WorkflowActivityTypeEditor_" + workflowActivityType.Guid.ToString();
            workflowActivityTypeEditor.SetWorkflowActivityType( workflowActivityType );
            workflowActivityTypeEditor.DeleteActivityTypeClick += workflowActivityTypeEditor_DeleteActivityClick;
            workflowActivityTypeEditor.AddActionTypeClick += workflowActivityTypeEditor_AddActionTypeClick;
            foreach ( WorkflowActionType actionType in workflowActivityType.ActionTypes.OrderBy( a => a.Order ) )
            {
                CreateWorkflowActionTypeEditorControl( workflowActivityTypeEditor, actionType );
            }

            phActivities.Controls.Add( workflowActivityTypeEditor );
        }

        /// <summary>
        /// Handles the AddActionTypeClick event of the workflowActivityTypeEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void workflowActivityTypeEditor_AddActionTypeClick( object sender, EventArgs e )
        {
            if ( sender is WorkflowActivityTypeEditor )
            {
                WorkflowActivityTypeEditor workflowActivityTypeEditor = sender as WorkflowActivityTypeEditor;
                CreateWorkflowActionTypeEditorControl( workflowActivityTypeEditor, new WorkflowActionType { Guid = Guid.NewGuid() } );
            }
        }

        /// <summary>
        /// Creates the workflow action type editor control.
        /// </summary>
        /// <param name="workflowActivityTypeEditor">The workflow activity type editor.</param>
        /// <param name="workflowActionType">Type of the workflow action.</param>
        private void CreateWorkflowActionTypeEditorControl( WorkflowActivityTypeEditor workflowActivityTypeEditor, WorkflowActionType workflowActionType )
        {
            WorkflowActionTypeEditor workflowActionTypeEditor = new WorkflowActionTypeEditor();
            workflowActionTypeEditor.ID = "WorkflowActionTypeEditor_" + workflowActionType.Guid.ToString();
            workflowActionTypeEditor.DeleteActionTypeClick += workflowActionTypeEditor_DeleteActionTypeClick;
            workflowActionTypeEditor.WorkflowActionType = workflowActionType;
            workflowActivityTypeEditor.Controls.Add( workflowActionTypeEditor );
        }

        /// <summary>
        /// Handles the DeleteActionTypeClick event of the workflowActionTypeEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void workflowActionTypeEditor_DeleteActionTypeClick( object sender, EventArgs e )
        {
            if ( sender is WorkflowActionTypeEditor )
            {
                WorkflowActionTypeEditor workflowActionTypeEditor = sender as WorkflowActionTypeEditor;
                WorkflowActivityTypeEditor workflowActivityTypeEditor = workflowActionTypeEditor.Parent as WorkflowActivityTypeEditor;
                workflowActivityTypeEditor.Controls.Remove( workflowActionTypeEditor );
            }
        }

        /// <summary>
        /// Handles the DeleteActivityClick event of the workflowActivityTypeEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void workflowActivityTypeEditor_DeleteActivityClick( object sender, EventArgs e )
        {
            if ( sender is WorkflowActivityTypeEditor )
            {
                WorkflowActivityTypeEditor editor = sender as WorkflowActivityTypeEditor;
                if ( editor != null )
                {
                    phActivities.Controls.Remove( editor );
                }
            }
        }

        #endregion
    }
}