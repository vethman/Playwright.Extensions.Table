namespace Playwright.Extensions.Table
{
    public static class Constants
    {
        private const string ROWSPAN_NOT_SUPPORTED = "including rowspan not supported";
        
        public const string ATTRIBUTE_COLSPAN = "colspan";
        public const string ATTRIBUTE_ROWSPAN = "rowspan";

        public const string EXCEPTION_COLUMNS_NOT_LOADED = "Columns not loaded into TableRowElement yet. First run LoadAsync...";
        public const string EXCEPTION_ROWLOCATOR_NO_RESULTS = "No results found for RowLocator...";
        public const string EXCEPTION_HEADER_ROWSPAN_NOT_SUPPORTED = "HeaderRow " + ROWSPAN_NOT_SUPPORTED;
        public const string EXCEPTION_ROW_ROWSPAN_NOT_SUPPORTED = "TableRow" + ROWSPAN_NOT_SUPPORTED;
        
        public const string SELECTOR_CELL = "td";
    }
}