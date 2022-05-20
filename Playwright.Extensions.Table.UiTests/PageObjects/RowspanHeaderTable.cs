using Microsoft.Playwright.Extensions.Table.ExtensionMethods;
using Microsoft.Playwright.Extensions.Table.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Playwright.Extensions.Table.UiTests.PageObjects
{
    public class RowspanHeaderTable
    {
        private readonly IPage _page;

        public ITableElement RowspanHeaderTableElement => _page.LocatorTableElement(_page.Locator(Constants.SELECTOR_TABLEHEADERS), _page.Locator(Constants.SELECTOR_TABLEROWS));

        public RowspanHeaderTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "DemoHtml/rowspanheadertable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}