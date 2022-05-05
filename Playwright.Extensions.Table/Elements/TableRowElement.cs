using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Playwright;

using Playwright.Extensions.Table.Interfaces;

namespace Playwright.Extensions.Table.Elements
{
    /// <summary>
    /// TableRowElement supports testing Html TableRows with Playwright.
    /// </summary>
    public class TableRowElement : ITableRowElement
    {
        private readonly string _cellSelector;
        private readonly HeaderIndexer _headerIndexer;
        private readonly ILocator _locatorHeaders;
        private IDictionary<string, int> _headerIndexerDictionary = new Dictionary<string, int>();
        private IReadOnlyList<IElementHandle> _columns = new List<IElementHandle>();

        /// <summary>
        /// All found headers, handy to verify if headerSelector/headerElements results in correct header values.
        /// </summary>
        public IEnumerable<string> TableHeaderValues => _headerIndexerDictionary.Select(x => x.Key);

        /// <summary>
        /// Gets the <see cref="ILocator"/> wrapped by this object.
        /// </summary>
        public ILocator WrappedLocator { get; }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="locatorHeaders">ILocator that represents all headers of a Table to create headersIndexer</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        public TableRowElement(ILocator locatorHeaders, ILocator locatorRow) : this(new Dictionary<string, int>(), locatorRow, Constants.SELECTOR_CELL)
        {
            _locatorHeaders = locatorHeaders;
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="locatorHeaders">ILocator that represents all headers of a Table to create headersIndexer</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        /// <param name="cellSelector">Selector to find all columns inside a row</param>
        public TableRowElement(ILocator locatorHeaders, ILocator locatorRow, string cellSelector) : this(new Dictionary<string, int>(), locatorRow, cellSelector)
        {
            _locatorHeaders = locatorHeaders;
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="headerIndexer">Indexer that contains header values</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        public TableRowElement(IDictionary<string, int> headerIndexer, ILocator locatorRow) : this(headerIndexer, locatorRow, Constants.SELECTOR_CELL)
        {
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="headerIndexer">Indexer that contains header values</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        /// <param name="cellSelector">Selector to find all columns inside a row</param>
        public TableRowElement(IDictionary<string, int> headerIndexer, ILocator locatorRow, string cellSelector)
        {
            _headerIndexerDictionary = headerIndexer;
            WrappedLocator = locatorRow;
            _cellSelector = cellSelector;
            _headerIndexer = new HeaderIndexer();
        }


        /// <summary>
        /// Get a column of the tablerow.
        /// </summary>
        /// <param name="nameHeader">Header value to get corresponding column.</param>
        /// <returns>The matching <see cref="IElementHandle"/> on the current context.</returns>
        public IElementHandle GetColumn(string nameHeader)
        {
            if (!_columns.Any())
            {
                throw new IndexOutOfRangeException(Constants.EXCEPTION_COLUMNS_NOT_LOADED);
            }

            if (!_headerIndexerDictionary.TryGetValue(nameHeader, out var result))
            {
                throw new KeyNotFoundException($"Header '{nameHeader}' does not exist, available headers: {Environment.NewLine + string.Join(Environment.NewLine, _headerIndexerDictionary.Select(x => x.Key))}");
            }

            return _columns[result];
        }

        /// <summary>
        /// Get a column of the tablerow.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The matching <see cref="IElementHandle"/> on the current context.</returns>
        public IElementHandle GetColumn(int index)
        {
            if (!_columns.Any())
            {
                throw new IndexOutOfRangeException(Constants.EXCEPTION_COLUMNS_NOT_LOADED);
            }

            return _columns[index];
        }

        /// <summary>
        /// Loads the found columns and headers into ITableRowElement.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="PlaywrightException"></exception>
        public async Task LoadAsync()
        {
            if (await WrappedLocator.CountAsync() == 0)
            {
                throw new PlaywrightException(Constants.EXCEPTION_ROWLOCATOR_NO_RESULTS);
            }

            if (!_columns.Any())
            {
                _columns = await ColumnsIncludingColspan(WrappedLocator.Nth(0).Locator(_cellSelector));
            }

            if (!_headerIndexerDictionary.Any())
            {
                _headerIndexerDictionary = await _headerIndexer.HeadersIncludingColspanAndDuplicateAsync(_locatorHeaders);
            }
        }

        private async Task<IReadOnlyList<IElementHandle>> ColumnsIncludingColspan(ILocator locatorColumns)
        {
            var columnsIncludingColspan = new List<IElementHandle>();

            var count = await locatorColumns.CountAsync();
            for (var i = 0; i < count; i++)
            {
                var columnLocator = locatorColumns.Nth(i);
                var rowspan = await columnLocator.GetAttributeAsync(Constants.ATTRIBUTE_ROWSPAN);
                if (!string.IsNullOrEmpty(rowspan))
                {
                    throw new NotSupportedException(Constants.EXCEPTION_ROW_ROWSPAN_NOT_SUPPORTED);
                }

                var colspan = await columnLocator.GetAttributeAsync(Constants.ATTRIBUTE_COLSPAN);
                var colspans = Enumerable.Range(0, Convert.ToInt32(colspan ?? "1"));
                var column = await columnLocator.ElementHandleAsync();
                columnsIncludingColspan.AddRange(colspans.Select(y => colspans.Count() == 1 || y == 0 ? column : null));
            }

            return columnsIncludingColspan.AsReadOnly();
        }
    }
}