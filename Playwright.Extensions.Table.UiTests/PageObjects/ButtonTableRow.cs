﻿using System.Threading.Tasks;

using Microsoft.Playwright;

using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.UiTests.PageObjects
{
    public class ButtonTableRow
    {
        private readonly ITableRowElement _tableRowElement;

        public IElementHandle DeleteButton => _tableRowElement.GetColumn("Delete?");

        public ButtonTableRow(ITableRowElement tableRowElement)
        {
            _tableRowElement = tableRowElement;
        }

        public async Task<string> Rownumber()
        {
            var column = _tableRowElement.GetColumn("Rownumber");
            return await column.TextContentAsync() ?? string.Empty;
        }
    }
}