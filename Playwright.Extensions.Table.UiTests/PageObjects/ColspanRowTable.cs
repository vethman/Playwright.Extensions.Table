using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using Playwright.Extensions.Table.ExtensionMethods;
using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.UiTests.PageObjects
{
    public class ColspanRowTable
    {
        private readonly IPage _page;

        public ITableElement ColspanRowTableElement => _page.LocatorTableElement(_page.Locator(Constants.SELECTOR_TABLEHEADERS), _page.Locator(Constants.SELECTOR_TABLEROWS));

        public ColspanRowTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "DemoHtml/colspanrowtable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}