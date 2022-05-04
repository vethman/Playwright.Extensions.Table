using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Playwright.Table
{
    /// <summary>
    /// TableRowElement supports testing Html TableRows with Playwright.
    /// </summary>
    public class TableRowElement : ITableRowElement
    {
        private readonly string _rowColumnSelector;
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
        public TableRowElement(ILocator locatorHeaders, ILocator locatorRow) : this(new Dictionary<string, int>(), locatorRow, "td")
        {
            _locatorHeaders = locatorHeaders;
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="locatorHeaders">ILocator that represents all headers of a Table to create headersIndexer</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        /// <param name="rowColumnSelector">Selector to find all columns inside a row</param>
        public TableRowElement(ILocator locatorHeaders, ILocator locatorRow, string rowColumnSelector) : this(new Dictionary<string, int>(), locatorRow, rowColumnSelector)
        {
            _locatorHeaders = locatorHeaders;
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="headerIndexer">Indexer that contains header values</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        public TableRowElement(IDictionary<string, int> headerIndexer, ILocator locatorRow) : this(headerIndexer, locatorRow, "td")
        {
        }

        /// <summary>
        /// Creates TableRowElement. Default columnselector By.XPath("./td")
        /// </summary>
        /// <param name="headerIndexer">Indexer that contains header values</param>
        /// <param name="locatorRow">ILocator that represents (single) row</param>
        /// <param name="rowColumnSelector">Selector to find all columns inside a row</param>
        public TableRowElement(IDictionary<string, int> headerIndexer, ILocator locatorRow, string rowColumnSelector)
        {
            _headerIndexerDictionary = headerIndexer;
            WrappedLocator = locatorRow;
            _rowColumnSelector = rowColumnSelector;
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
                throw new IndexOutOfRangeException("Columns not loaded into TableRowElement yet. First run LoadAsync...");
            }

            if (!_headerIndexerDictionary.TryGetValue(nameHeader, out int result))
            {
                throw new KeyNotFoundException($"Header '{nameHeader}' does not exist, available headers:{Environment.NewLine + string.Join(Environment.NewLine, _headerIndexerDictionary.Select(x => x.Key))}");
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
                throw new IndexOutOfRangeException("Columns not loaded into TableRowElement yet. First run LoadAsync...");
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
                throw new PlaywrightException("No results found for RowLocator...");
            }

            if (!_columns.Any())
            {
                _columns = await ColumnsIncludingColspan(WrappedLocator.Nth(0).Locator(_rowColumnSelector));
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
            for (int i = 0; i < count; i++)
            {
                var columnLocator = locatorColumns.Nth(i);
                var rowspan = await columnLocator.GetAttributeAsync("rowspan");
                if (!string.IsNullOrEmpty(rowspan))
                {
                    throw new NotSupportedException("TableRow including rowspan not supported");
                }

                var colspan = await columnLocator.GetAttributeAsync("colspan");
                var colspans = Enumerable.Range(0, Convert.ToInt32(colspan ?? "1"));
                var column = await columnLocator.ElementHandleAsync();
                columnsIncludingColspan.AddRange(colspans.Select(y => colspans.Count() == 1 || y == 0 ? column : null));
            }

            return columnsIncludingColspan.AsReadOnly();
        }
    }
}