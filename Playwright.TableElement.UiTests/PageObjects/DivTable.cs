using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Playwright.Table.UiTests.PageObjects
{
    public class DivTable
    {
        private readonly IPage _page;

        public ITableElement DivTableElement => _page.LocatorTableElement(_page.Locator(".divTableHead"), _page.Locator(".divTableBody>.divTableRow"), ".divTableCell");

        public DivTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "DemoHtml/divtable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}