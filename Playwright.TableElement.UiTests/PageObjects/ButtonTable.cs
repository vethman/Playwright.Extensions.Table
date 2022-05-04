using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Playwright.Table.UiTests.PageObjects
{
    public class ButtonTable
    {
        private readonly IPage _page;

        public async Task<IEnumerable<ButtonTableRow>> ButtonTableRowsAsync()
        {
            var tableElement = _page.LocatorTableElement(_page.Locator("th"), _page.Locator("tbody>tr"));
            var tableRowElements = await tableElement.GetTableRowElementsAsync();
            return tableRowElements.Select(x => new ButtonTableRow(x));
        }

        public ButtonTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "DemoHtml/buttontable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}