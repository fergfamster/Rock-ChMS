//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Linq;

using Rock.Data;

namespace Rock.Model
{
    /// <summary>
    /// Category Service class
    /// </summary>
    public partial class CategoryService : Service<Category>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class
        /// </summary>
        public CategoryService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class
        /// </summary>
        public CategoryService(IRepository<Category> repository) : base(repository)
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( Category item, out string errorMessage )
        {
            errorMessage = string.Empty;
 
            if ( new Service<Category>().Queryable().Any( a => a.ParentCategoryId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", Category.FriendlyTypeName, Category.FriendlyTypeName );
                return false;
            }  
 
            if ( new Service<DataView>().Queryable().Any( a => a.CategoryId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", Category.FriendlyTypeName, DataView.FriendlyTypeName );
                return false;
            }  
 
            if ( new Service<PrayerRequest>().Queryable().Any( a => a.CategoryId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", Category.FriendlyTypeName, PrayerRequest.FriendlyTypeName );
                return false;
            }  
 
            if ( new Service<Report>().Queryable().Any( a => a.CategoryId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", Category.FriendlyTypeName, Report.FriendlyTypeName );
                return false;
            }  
 
            if ( new Service<WorkflowType>().Queryable().Any( a => a.CategoryId == item.Id ) )
            {
                errorMessage = string.Format( "This {0} is assigned to a {1}.", Category.FriendlyTypeName, WorkflowType.FriendlyTypeName );
                return false;
            }  
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static class CategoryExtensionMethods
    {
        /// <summary>
        /// Clones this Category object to a new Category object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static Category Clone( this Category source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as Category;
            }
            else
            {
                var target = new Category();
                target.IsSystem = source.IsSystem;
                target.ParentCategoryId = source.ParentCategoryId;
                target.EntityTypeId = source.EntityTypeId;
                target.EntityTypeQualifierColumn = source.EntityTypeQualifierColumn;
                target.EntityTypeQualifierValue = source.EntityTypeQualifierValue;
                target.Name = source.Name;
                target.IconSmallFileId = source.IconSmallFileId;
                target.IconLargeFileId = source.IconLargeFileId;
                target.IconCssClass = source.IconCssClass;
                target.Id = source.Id;
                target.Guid = source.Guid;

            
                return target;
            }
        }
    }
}
