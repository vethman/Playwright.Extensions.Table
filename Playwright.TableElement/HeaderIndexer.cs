using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playwright.Table
{
    internal class HeaderIndexer
    {
        public async Task<IDictionary<string, int>> HeadersIncludingColspanAndDuplicateAsync(ILocator locatorHeaders)
        {
            var headerNames = new List<string>();
            var count = await locatorHeaders.CountAsync();
            for (int i = 0; i < count; i++)
            {
                var header = locatorHeaders.Nth(i);
                var rowspan = await header.GetAttributeAsync("rowspan");
                if (!string.IsNullOrEmpty(rowspan))
                {
                    throw new NotSupportedException("TableHeader including rowspan not supported");
                }

                var colspan = await header.GetAttributeAsync("colspan");
                var headerName = await header.TextContentAsync();

                headerNames.AddRange(Enumerable.Range(0, Convert.ToInt32(colspan ?? "1")).Select(x => headerName.Trim()));
            }

            return headerNames
                .Select((text, index) => new { text, index })
                .GroupBy(x => x.text)
                .SelectMany(x => x.Select((y, i) => new { text = y.text + (x.Count() > 1 ? "_" + (i + 1) : ""), y.index }))
                .ToDictionary(x => x.text, x => x.index);
        }
    }
}