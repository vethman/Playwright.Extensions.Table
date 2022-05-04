using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Playwright.Table.UiTests.PageObjects
{
    public class ColspanRowTable
    {
        private readonly IPage _page;

        public ITableElement ColspanRowTableElement => _page.LocatorTableElement(_page.Locator("//table/thead/tr/th"), _page.Locator("//table/tbody/tr"));

        public ColspanRowTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "DemoHtml/colspanrowtable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}