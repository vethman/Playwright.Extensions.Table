using System;
using Microsoft.Playwright.Extensions.Table.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Playwright.Extensions.Table.Elements
{
    /// <summary>
    /// TableElement supports testing Html Tables with Playwright.
    /// <list type="bullet">
    /// <item><description>All found headers to verify if headerselector gives correct headers</description></item>
    /// <item><description>Collection of IElementHandles</description></item>
    /// </list>
    /// </summary>
    public class TableElement : ITableElement
    {
        private readonly ILocator _locatorHeaders;
        private readonly ILocator _locatorRows;
        private readonly string _cellSelector;
        private readonly HeaderIndexer _headerIndexer;

        /// <summary>
        /// Creates TableElement that contains zero or multiple TableElementRows. Default columnselector "td"
        /// </summary>
        /// <param name="locatorHeaders">ILocator to find elements representing headercolumns</param>
        /// <param name="locatorRows">ILocator to find elements representing tablerows</param>
        public TableElement(ILocator locatorHeaders, ILocator locatorRows) : this(locatorHeaders, locatorRows, Constants.SELECTOR_CELL)
        {
        }

        /// <summary>
        /// Creates TableElement that contains zero or multiple TableElementRows
        /// </summary>
        /// <param name="locatorHeaders">ILocator to find elements representing headercolumns</param>
        /// <param name="locatorRows">ILocator to find elements representing rows</param>
        /// <param name="cellSelector">Selector to find elements inside a row representing the columns</param>
        public TableElement(ILocator locatorHeaders, ILocator locatorRows, string cellSelector)
        {
            _locatorHeaders = locatorHeaders;
            _locatorRows = locatorRows;
            _cellSelector = cellSelector;
            _headerIndexer = new HeaderIndexer();
        }

        /// <summary>
        /// All found headers, handy to verify if headerSelector/headerElements results in correct header values.
        /// </summary>
        /// <returns>IEnumerable<string> representing all found headertexts.</returns>
        public async Task<IEnumerable<string>> GetTableHeaderValuesAsync()
        {
            var headerIndexerDictionary = await _headerIndexer.HeadersIncludingColspanAndDuplicateAsync(_locatorHeaders);
            return headerIndexerDictionary.Select(x => x.Key);
        }

        /// <summary>
        /// All ITableRowElements found by ILocator locatorHeaders, ILocator locatorRows and string rowColumnSelector.
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyList<ITableRowElement>> GetTableRowElementsAsync()
        {
            var headerIndexer = await _headerIndexer.HeadersIncludingColspanAndDuplicateAsync(_locatorHeaders);

            var tableRowElements = new List<ITableRowElement>();

            var count = await _locatorRows.CountAsync();
            for (var i = 0; i < count; i++)
            {
                var tableRowElement = new TableRowElement(headerIndexer, _locatorRows.Nth(i), _cellSelector);
                await tableRowElement.LoadAsync();
                tableRowElements.Add(tableRowElement);
            }

            return tableRowElements.AsReadOnly();
        }
        
        /// <summary>
        /// Parse ITableRowElements into objects of chosen type, should not contain nested properties and only of type string
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <returns>IEnumerable<T></returns>
        public async Task<IEnumerable<T>> ParseAsync<T>()
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.PropertyType != typeof(string))
                {
                    throw new NotSupportedException($"Property {propertyInfo.Name} of type {propertyInfo.PropertyType} not allowed, only string supported");
                }
            }
            
            var rows = await GetTableRowElementsAsync();

            var items = new List<T>();

            foreach (var row in rows)
            {
                var headers = row.TableHeaderValues;
                var jsonProperties = await Task.WhenAll(headers
                    .Select(async header => new JProperty(Regex.Replace(header, @"\s+", ""), (await row.GetColumn(header).TextContentAsync())?.Trim())));
                var jsonObject = new JObject(jsonProperties);
                var item = jsonObject.ToObject<T>();

                items.Add(item);
            }

            return items;
        }
    }
}