﻿using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using Playwright.Extensions.Table.Elements;
using Playwright.Extensions.Table.ExtensionMethods;
using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.UiTests.PageObjects
{
    public class SimpleTable
    {
        private readonly IPage _page;

        public ITableElement ColspanTableElement => _page.LocatorTableElement(TableHeaders, TableRows);
        public ILocator TableHeaders => _page.Locator(Constants.SELECTOR_TABLEHEADERS);
        public ILocator TableRows => _page.Locator(Constants.SELECTOR_TABLEROWS);

        public ITableRowElement SingleTableRowWithoutColumnSelector()
        {
            var headersLocator = _page.Locator(Constants.SELECTOR_TABLEHEADERS);
            var rowLocator = _page.Locator(Constants.SELECTOR_TABLEROWS);
            return new TableRowElement(headersLocator, rowLocator);
        }
        public ITableRowElement SingleTableRowWithColumnSelector()
        {
            var headersLocator = _page.Locator(Constants.SELECTOR_TABLEHEADERS);
            var rowLocator = _page.Locator(Constants.SELECTOR_TABLEROWS);
            return new TableRowElement(headersLocator, rowLocator, Table.Constants.SELECTOR_CELL);
        }

        public SimpleTable(IPage page)
        {
            _page = page;
        }

        public async Task Open()
        {
            await _page.GotoAsync(new Uri(Path.Combine(Directory.GetCurrentDirectory(), "DemoHtml/simpletable.html")).AbsoluteUri, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        }
    }
}