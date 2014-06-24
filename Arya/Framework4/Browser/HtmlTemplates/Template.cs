namespace Arya.Framework4.Browser.HtmlTemplates
{
    using System;
    using Arya.HelperClasses;

    public abstract class Template
    {
        public abstract string Render(Change entityDataGridViewChange, Guid entityDataGridViewInstanceID, AssetCache assetCache, int currentColumnIndex);
    }
}
