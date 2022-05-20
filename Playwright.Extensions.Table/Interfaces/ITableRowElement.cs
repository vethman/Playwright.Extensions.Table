using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace Microsoft.Playwright.Extensions.Table.Interfaces
{
    /// <summary>
    /// Defines the interface through which the user can access tablerow columns.
    /// </summary>
    public interface ITableRowElement
    {
        /// <summary>
        /// Gets the <see cref="ILocator"/> wrapped by this object.
        /// </summary>
        ILocator WrappedLocator { get; }

        /// <summary>
        /// All found headers, handy to verify if headerSelector/headerElements results in correct header values.
        /// </summary>
        IEnumerable<string> TableHeaderValues { get; }

        /// <summary>
        /// Get a column of the tablerow.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The matching <see cref="IElementHandle"/> on the current context.</returns>
        IElementHandle GetColumn(int index);

        /// <summary>
        /// Get a column of the tablerow.
        /// </summary>
        /// <param name="nameHeader">Header value to get corresponding column.</param>
        /// <returns>The matching <see cref="IElementHandle"/> on the current context.</returns>
        IElementHandle GetColumn(string nameHeader);

        /// <summary>
        /// Loads the found columns and headers into ITableRowElement.
        /// </summary>
        /// <returns></returns>
        Task LoadAsync();
    }
}