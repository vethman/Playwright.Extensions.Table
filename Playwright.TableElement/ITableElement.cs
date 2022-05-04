using System.Collections.Generic;
using System.Threading.Tasks;

namespace Playwright.Table
{
    /// <summary>
    /// Defines the interface through which the user can access table header values and table rows.
    /// </summary>
    public interface ITableElement
    {
        /// <summary>
        /// All found headers, handy to verify if headerSelector/headerElements is correct
        /// </summary>
        Task<IEnumerable<string>> GetTableHeaderValuesAsync();
        /// <summary>
        /// All TableRowElements found by rowSelector/rowElements
        /// </summary>
        Task<IReadOnlyList<ITableRowElement>> GetTableRowElementsAsync();
    }
}