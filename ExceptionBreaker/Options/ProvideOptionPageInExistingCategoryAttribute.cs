using System;
using Microsoft.VisualStudio.Shell;

namespace ExceptionBreaker.Options {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ProvideOptionPageInExistingCategoryAttribute : ProvideOptionDialogPageAttribute {
        private readonly string _categoryName;
        private readonly string _pageName;

        public string CategoryName {
            get { return _categoryName; }
        }
        
        public string PageName {
            get { return _pageName; }
        }
        
        private string GetPageRegKeyPath() {
            return string.Format(@"ToolsOptionsPages\{0}\{1}", CategoryName, PageName);
        }
        
        public ProvideOptionPageInExistingCategoryAttribute(Type pageType, string categoryName, string pageName, short pageNameResourceID)
            : base(pageType, "#" + pageNameResourceID)
        {
            if (categoryName == null) throw new ArgumentNullException("categoryName");
            if (pageName == null) throw new ArgumentNullException("pageName");

            _categoryName = categoryName;
            _pageName = pageName;
        }

        public override void Register(RegistrationContext context) {
            context.Log.WriteLine("Registering options page: {0}, {1}", CategoryName, PageName);
            using (var key = context.CreateKey(GetPageRegKeyPath())) {
                key.SetValue(string.Empty, PageNameResourceId);
                key.SetValue("Package", context.ComponentType.GUID.ToString("B"));
                key.SetValue("Page", PageType.GUID.ToString("B"));
            }
        }

        public override void Unregister(RegistrationContext context) {
            context.RemoveKey(GetPageRegKeyPath());
        }
    }
}
