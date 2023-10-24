using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Playwright.Extensions.Table.Interfaces
{
    /// <summary>
    /// Defines the interface through which the user can access table header values and table rows.
    /// </summary>
    public interface ITableElement
    {
        /// <summary>
        /// All found headers, handy to verify if headerSelector/headerElements results in correct header values.
        /// </summary>
        /// <returns>IEnumerable<string> representing all found headertexts.</returns>
        Task<IEnumerable<string>> GetTableHeaderValuesAsync();

        /// <summary>
        /// All ITableRowElements found by ILocator locatorHeaders, ILocator locatorRows and string rowColumnSelector.
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<ITableRowElement>> GetTableRowElementsAsync();

        Task<IEnumerable<T>> ParseAsync<T>();
    }
}