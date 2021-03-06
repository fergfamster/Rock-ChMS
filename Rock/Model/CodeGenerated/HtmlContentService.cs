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
    /// HtmlContent Service class
    /// </summary>
    public partial class HtmlContentService : Service<HtmlContent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContentService"/> class
        /// </summary>
        public HtmlContentService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContentService"/> class
        /// </summary>
        public HtmlContentService(IRepository<HtmlContent> repository) : base(repository)
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
        public bool CanDelete( HtmlContent item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static class HtmlContentExtensionMethods
    {
        /// <summary>
        /// Clones this HtmlContent object to a new HtmlContent object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static HtmlContent Clone( this HtmlContent source, bool deepCopy )
        {
            if (deepCopy)
            {
                return source.Clone() as HtmlContent;
            }
            else
            {
                var target = new HtmlContent();
                target.BlockId = source.BlockId;
                target.EntityValue = source.EntityValue;
                target.Version = source.Version;
                target.Content = source.Content;
                target.IsApproved = source.IsApproved;
                target.ApprovedByPersonId = source.ApprovedByPersonId;
                target.ApprovedDateTime = source.ApprovedDateTime;
                target.StartDateTime = source.StartDateTime;
                target.ExpireDateTime = source.ExpireDateTime;
                target.Id = source.Id;
                target.Guid = source.Guid;

            
                return target;
            }
        }
    }
}
