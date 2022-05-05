using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using Playwright.Extensions.Table.ExtensionMethods;
using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.UiTests.PageObjects
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
            await _page.GotoAsync(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "DemoHtml/divtable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}