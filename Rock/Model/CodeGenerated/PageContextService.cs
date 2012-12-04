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
    /// PageContext Service class
    /// </summary>
    public partial class PageContextService : Service<PageContext, PageContextDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageContextService"/> class
        /// </summary>
        public PageContextService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageContextService"/> class
        /// </summary>
        public PageContextService(IRepository<PageContext> repository) : base(repository)
        {
        }

        /// <summary>
        /// Creates a new model
        /// </summary>
        public override PageContext CreateNew()
        {
            return new PageContext();
        }

        /// <summary>
        /// Query DTO objects
        /// </summary>
        /// <returns>A queryable list of DTO objects</returns>
        public override IQueryable<PageContextDto> QueryableDto( )
        {
            return QueryableDto( this.Queryable() );
        }

        /// <summary>
        /// Query DTO objects
        /// </summary>
        /// <returns>A queryable list of DTO objects</returns>
        public IQueryable<PageContextDto> QueryableDto( IQueryable<PageContext> items )
        {
            return items.Select( m => new PageContextDto()
                {
                    IsSystem = m.IsSystem,
                    PageId = m.PageId,
                    Entity = m.Entity,
                    IdParameter = m.IdParameter,
                    CreatedDateTime = m.CreatedDateTime,
                    Id = m.Id,
                    Guid = m.Guid,
                });
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if this instance can delete the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanDelete( PageContext item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }
}