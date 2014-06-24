using System;
using Natalie.HelperClasses;

namespace Natalie.Framework.Browser.HtmlTemplates
{
    public abstract class Template
    {
        public abstract string Render(Change entityDataGridViewChange, Guid entityDataGridViewInstanceID, AssetCache assetCache, int currentColumnIndex);
    }
}
