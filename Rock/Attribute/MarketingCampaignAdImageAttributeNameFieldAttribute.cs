//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rock.Attribute
{
    /// <summary>
    /// Field Attribute to select Images attributes for something by Attribute.Key
    /// </summary>
    public class MarketingCampaignAdImageAttributeNameFieldAttribute : FieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketingCampaignAdImageAttributeNameFieldAttribute" /> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="name">The name.</param>
        /// <param name="required">if set to <c>true</c> [required].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="key">The key.</param>
        /// <param name="category">The category.</param>
        /// <param name="description">The description.</param>
        public MarketingCampaignAdImageAttributeNameFieldAttribute( int order, string name, bool required, string defaultValue = "", string key = null, string category = "", string description = "" )
            : base( order, name, required, defaultValue, key, category, description, typeof(Rock.Field.Types.MarketingCampaignAdImageAttributeNameField).FullName )
        {
        }
    }
}