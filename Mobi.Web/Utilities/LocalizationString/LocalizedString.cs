using Microsoft.AspNetCore.Html;

namespace Mobi.Web.Utilities.LocalizationString
{
    public class LocalizedString : HtmlString
    {

        #region Properties
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="localized">Localized value</param>
        public LocalizedString(string localized) : base(localized)
        {
            Text = localized;
        }

        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; }

        #endregion

    }
}
