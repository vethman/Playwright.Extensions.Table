﻿using Microsoft.Playwright.Extensions.Table.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Playwright.Extensions.Table.UiTests.PageObjects
{
    public class ButtonTable
    {
        private readonly IPage _page;

        public async Task<IEnumerable<ButtonTableRow>> ButtonTableRowsAsync()
        {
            var tableElement = _page.LocatorTableElement(_page.Locator(Constants.SELECTOR_TABLEHEADERS), _page.Locator(Constants.SELECTOR_TABLEROWS));
            var tableRowElements = await tableElement.GetTableRowElementsAsync();
            return tableRowElements.Select(x => new ButtonTableRow(x));
        }

        public ButtonTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "DemoHtml/buttontable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}