using Microsoft.Playwright;

using Playwright.Extensions.Table.Elements;
using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for convenience in using IPage.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// Create ITableElement within the context by selectors. Default columnselector "td"
        /// </summary>
        /// <param name="page">The IPage instance to extend.</param>
        /// <param name="locatorHeaders">ILocator to find elements representing headercolumns</param>
        /// <param name="locatorRows">ILocator to find elements representing tablerows</param>
        /// <returns>ITableElement</returns>
        public static ITableElement LocatorTableElement(this IPage page, ILocator locatorHeaders, ILocator locatorRows)
        {
            return new TableElement(locatorHeaders, locatorRows);
        }

        /// <summary>
        /// Create ITableElement within the context by selectors.
        /// </summary>
        /// <param name="page">The IPage instance to extend.</param>
        /// <param name="locatorHeaders">ILocator to find elements representing headercolumns</param>
        /// <param name="locatorRows">ILocator to find elements representing tablerows</param>
        /// <param name="columnSelector">Selector to find elements inside a row representing the columns</param>
        /// <returns>ITableElement</returns>
        public static ITableElement LocatorTableElement(this IPage page, ILocator locatorHeaders, ILocator locatorRows, string columnSelector)
        {
            return new TableElement(locatorHeaders, locatorRows, columnSelector);
        }
    }
}