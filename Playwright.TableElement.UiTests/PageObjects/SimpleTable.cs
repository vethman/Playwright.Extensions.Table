using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Playwright.Table.UiTests.PageObjects
{
    public class SimpleTable
    {
        private readonly IPage _page;

        public ITableElement ColspanTableElement => _page.LocatorTableElement(TableHeaders, TableRows);
        public ILocator TableHeaders => _page.Locator("//table/thead/tr/th");
        public ILocator TableRows => _page.Locator("//table/tbody/tr");

        public ITableRowElement SingleTableRowWithoutColumnSelector()
        {
            var headersLocator = _page.Locator("//table/thead/tr/th");
            var rowLocator = _page.Locator("//table/tbody/tr");
            return new TableRowElement(headersLocator, rowLocator);
        }
        public ITableRowElement SingleTableRowWithColumnSelector()
        {
            var headersLocator = _page.Locator("//table/thead/tr/th");
            var rowLocator = _page.Locator("//table/tbody/tr");
            return new TableRowElement(headersLocator, rowLocator, "td");
        }

        public SimpleTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "DemoHtml/simpletable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}