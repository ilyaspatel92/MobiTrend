using Microsoft.AspNetCore.Mvc.Razor;
using Mobi.Service.ResourceService;
using Mobi.Web.Utilities.Localization;
using Mobi.Web.Utilities.LocalizationString;

namespace Mobi.Web.Utilities.LocalizationRazorPage
{
    public abstract class MobiRazorPage<TModel> : RazorPage<TModel>
    {
        #region Fields

        private Localizer _localizer;

        #endregion

        #region Method
        public Localizer T
        {
            get
            {

                var services = this.Context.RequestServices;
                var resourceService = (IResourceService)services.GetService(typeof(IResourceService));

                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = resourceService.GetResource(format);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }

        #endregion


    }
}
