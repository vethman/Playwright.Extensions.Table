using FluentAssertions;
using Microsoft.Playwright.Extensions.Table.Elements;
using Microsoft.Playwright.Extensions.Table.Interfaces;
using Microsoft.Playwright.Extensions.Table.UiTests.PageObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Playwright.Extensions.Table.UiTests.TableObjects;

namespace Microsoft.Playwright.Extensions.Table.UiTests
{
    public class TableElementTests
    {
        private IPlaywright _playwright = null!;
        private IPage _page = null!;
        private IBrowser _browser = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            _page = await _browser.NewPageAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Test]
        public async Task SimpleTable_HeaderValues()
        {
            var expectedHeaders = new List<string> { "First name", "Last name", "Date of birth" };

            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;
            var tableHeaders = await tableElement.GetTableHeaderValuesAsync();
            var tableRowElements = await tableElement.GetTableRowElementsAsync();

            tableHeaders.Should().BeEquivalentTo(expectedHeaders, options => options.WithStrictOrdering());
            tableRowElements.First().TableHeaderValues.Should().BeEquivalentTo(expectedHeaders, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task SimpleTable_HeaderValueNotFound_ThrowsNoSuchElementException()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;
            var tableRowElements = await tableElement.GetTableRowElementsAsync();

            Action act = () => tableRowElements.First().GetColumn("NotExistingHeaderError");
            act.Should().Throw<KeyNotFoundException>().WithMessage("Header 'NotExistingHeaderError' does not exist, available headers: \nFirst name\nLast name\nDate of birth");
        }

        [Test]
        public async Task SimpleTable_MatchHeaderWithColumn()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;
            var tableRowElements = await tableElement.GetTableRowElementsAsync();
            var tableRowElement = tableRowElements.Single(x => x.GetColumn("First name").TextContentAsync().Result == "Logan");

            (await tableRowElement.GetColumn("Last name").TextContentAsync()).Should().Be("Deacon");
            (await tableRowElement.GetColumn("Date of birth").TextContentAsync()).Should().Be("01-10-2002");
        }

        [Test]
        public async Task SimpleTable_HasRowsAndHeaders()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;

            (await tableElement.GetTableHeaderValuesAsync()).Should().HaveCount(3);
            (await tableElement.GetTableRowElementsAsync()).Should().HaveCount(2);
        }

        [Test]
        public async Task SimpleTable_ReturnsITableElementAndITableRowElements()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;
            var tableRowElements = (await tableElement.GetTableRowElementsAsync());

            tableElement.Should().BeAssignableTo<ITableElement>();
            tableRowElements.Should().BeAssignableTo<IReadOnlyList<ITableRowElement>>();
        }

        [Test]
        public async Task SimpleTable_TableRowElementWithColumnSelector()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableRowElement = simpleTable.SingleTableRowWithColumnSelector();
            await tableRowElement.LoadAsync();

            (await tableRowElement.GetColumn(0).TextContentAsync()).Should().Be("Ronald");
            (await tableRowElement.GetColumn("Last name").TextContentAsync()).Should().Be("Veth");
            (await tableRowElement.GetColumn("Date of birth").TextContentAsync()).Should().Be("22-12-1987");
        }

        [Test]
        public async Task SimpleTable_TableRowElementWithoutColumnSelector()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableRowElement = simpleTable.SingleTableRowWithoutColumnSelector();
            await tableRowElement.LoadAsync();

            (await tableRowElement.GetColumn(0).TextContentAsync()).Should().Be("Ronald");
            (await tableRowElement.GetColumn("Last name").TextContentAsync()).Should().Be("Veth");
            (await tableRowElement.GetColumn("Date of birth").TextContentAsync()).Should().Be("22-12-1987");
        }

        [Test]
        public async Task SimpleTable_GetColumnIndex_TableRowElementNotLoaded_ThrowsPlaywrightException()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableRowElement = simpleTable.SingleTableRowWithoutColumnSelector();

            Action act = () => tableRowElement.GetColumn(0);
            act.Should().Throw<IndexOutOfRangeException>()
                .WithMessage(Table.Constants.EXCEPTION_COLUMNS_NOT_LOADED);
        }

        [Test]
        public async Task SimpleTable_GetColumnNameHeader_TableRowElementNotLoaded_ThrowsPlaywrightException()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableRowElement = simpleTable.SingleTableRowWithoutColumnSelector();

            Action act = () => tableRowElement.GetColumn("Error");
            act.Should().Throw<IndexOutOfRangeException>()
                .WithMessage(Table.Constants.EXCEPTION_COLUMNS_NOT_LOADED);
        }

        [Test]
        public async Task SimpleTable_LoadAsync_RowLocatorNoResults_ThrowsPlaywrightException()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableRowElement = new TableRowElement(_page.Locator("invalid"), _page.Locator("invalid"));

            Func<Task> act = () => tableRowElement.LoadAsync();
            await act.Should().ThrowAsync<PlaywrightException>()
                .WithMessage(Table.Constants.EXCEPTION_ROWLOCATOR_NO_RESULTS);
        }
        
        [Test]
        public async Task SimpleTable_ParseAsync_ShouldParseToObject()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;

            var expectedSimpleTableTables = new List<SimpleTableTable>
            {
                new()
                {
                    FirstName = "Ronald",
                    LastName = "Veth",
                    DateOfBirth = "22-12-1987"
                },
                new()
                {
                    FirstName = "Logan",
                    LastName = "Deacon",
                    DateOfBirth = "01-10-2002"
                }
            };
            
            var actualSimpleTableTables = await tableElement.ParseAsync<SimpleTableTable>();

            actualSimpleTableTables.Should().BeEquivalentTo(expectedSimpleTableTables);
        }
        
        [Test]
        public async Task SimpleTable_ParseAsync_PropertyNotStringButEnumerable_ShouldThrowException()
        {
            var simpleTable = new SimpleTable(_page);

            await simpleTable.Open();

            var tableElement = simpleTable.TableElement;

            var act = () => tableElement.ParseAsync<InvalidPropertyTypeTable>();
            await act.Should().ThrowAsync<NotSupportedException>()
                .WithMessage($"Property {nameof(InvalidPropertyTypeTable.DateOfBirth)} of type {typeof(DateTime)} not allowed, only string supported");
        }

        [Test]
        public async Task ColspanHeaderTable_HeaderValueClonedByColspan()
        {
            var expectedHeaders = new List<string> { "Name", "Color Combination_1", "Color Combination_2" };

            var colspanHeaderTable = new ColspanHeaderTable(_page);

            await colspanHeaderTable.Open();

            var tableElement = colspanHeaderTable.ColspanHeaderTableElement;
            var tableHeaderValues = await tableElement.GetTableHeaderValuesAsync();

            tableHeaderValues.Should().BeEquivalentTo(expectedHeaders, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task ColspanHeaderTable_FindColumnBySecondColspanHeader()
        {
            var colspanHeaderTable = new ColspanHeaderTable(_page);

            await colspanHeaderTable.Open();

            var tableElement = colspanHeaderTable.ColspanHeaderTableElement;
            var tableRowElements = await tableElement.GetTableRowElementsAsync();
            var tableRowElement = tableRowElements.Single(x => x.GetColumn("Name").TextContentAsync().Result == "LoganDeacon");

            (await tableRowElement.GetColumn("Color Combination_1").TextContentAsync()).Should().Be("White");
            (await tableRowElement.GetColumn("Color Combination_2").TextContentAsync()).Should().Be("Red");
        }

        [Test]
        public async Task ColspanRowTable_HeaderValues()
        {
            var expectedHeaders = new List<string> { "Item", "Category", "Price" };

            var colspanRowTable = new ColspanRowTable(_page);

            await colspanRowTable.Open();

            var tableElement = colspanRowTable.ColspanRowTableElement;
            var tableHeaderValues = await tableElement.GetTableHeaderValuesAsync();

            tableHeaderValues.Should().BeEquivalentTo(expectedHeaders, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task ColspanRowTable_RowWithColspan_OnlyFirstCellOfColspanHasElement()
        {
            var colspanRowTable = new ColspanRowTable(_page);

            await colspanRowTable.Open();

            var tableElement = colspanRowTable.ColspanRowTableElement;
            var tabelRowElements = await tableElement.GetTableRowElementsAsync();

            var tableRowElementWithoutColspan = tabelRowElements.Single(x => x.GetColumn("Item").TextContentAsync().Result == "Deacon");
            (await tableRowElementWithoutColspan.GetColumn("Category").TextContentAsync()).Should().Be("Music");
            (await tableRowElementWithoutColspan.GetColumn("Price").TextContentAsync()).Should().Be("3000");

            var tableRowElementWithColspan = tabelRowElements.Single(x => x.GetColumn(0).TextContentAsync().Result == "Total");
            (await tableRowElementWithColspan.GetColumn("Category").TextContentAsync()).Should().Be("8000");
            tableRowElementWithColspan.GetColumn("Price").Should().BeNull();
        }

        [Test]
        public async Task RowspanHeaderTable_HeaderWithRowSpan_ReturnsNotSupportedException()
        {
            var rowspanHeaderTable = new RowspanHeaderTable(_page);

            await rowspanHeaderTable.Open();

            Func<Task> act = async () => { await rowspanHeaderTable.RowspanHeaderTableElement.GetTableHeaderValuesAsync(); };

            await act.Should().ThrowAsync<NotSupportedException>()
                .WithMessage(Table.Constants.EXCEPTION_HEADER_ROWSPAN_NOT_SUPPORTED);
        }

        [Test]
        public async Task RowspanRowTable_RowWithRowSpan_ReturnsNotSupportedException()
        {
            var rowspanRowTable = new RowspanRowTable(_page);

            await rowspanRowTable.Open();

            Func<Task> act = async () => { await rowspanRowTable.RowspanRowTableElement.GetTableRowElementsAsync(); };

            await act.Should().ThrowAsync<NotSupportedException>()
                .WithMessage(Table.Constants.EXCEPTION_ROW_ROWSPAN_NOT_SUPPORTED);
        }

        [Test]
        public async Task DivTable_HeaderValues()
        {
            var expectedHeaders = new List<string> { "First name", "Last name", "Specialty" };

            var divTable = new DivTable(_page);

            await divTable.Open();

            var tableHeaderValues = await divTable.DivTableElement.GetTableHeaderValuesAsync();

            tableHeaderValues.Should().BeEquivalentTo(expectedHeaders, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task DivTable_MatchHeaderWithColumn()
        {
            var divTable = new DivTable(_page);

            await divTable.Open();

            var tableRowElements = await divTable.DivTableElement.GetTableRowElementsAsync();
            var tableRowElement = tableRowElements.Single(x => x.GetColumn("First name").TextContentAsync().Result == "Logan");

            (await tableRowElement.GetColumn("Last name").TextContentAsync()).Should().Be("Deacon");
            (await tableRowElement.GetColumn("Specialty").TextContentAsync()).Should().Be("Make special together");
        }

        [Test]
        public async Task DivTable_HasRowsAndHeaders()
        {
            var divTable = new DivTable(_page);

            await divTable.Open();

            (await divTable.DivTableElement.GetTableHeaderValuesAsync()).Should().HaveCount(3);
            (await divTable.DivTableElement.GetTableRowElementsAsync()).Should().HaveCount(3);
        }

        [Test]
        public async Task ButtonTable_ClickButtonInRow()
        {
            var buttonTable = new ButtonTable(_page);

            await buttonTable.Open();

            var buttonTableRows = (await buttonTable.ButtonTableRowsAsync()).ToList();

            buttonTableRows.Should().HaveCount(3);

            var secondRow = buttonTableRows.Single(x => x.Rownumber().Result == "Row 2");
            await secondRow.DeleteButton.ClickAsync();

            buttonTableRows = (await buttonTable.ButtonTableRowsAsync()).ToList();
            buttonTableRows.Should().HaveCount(2);
            buttonTableRows.Should().ContainSingle(x => x.Rownumber().Result == "Row 1");
            buttonTableRows.Should().ContainSingle(x => x.Rownumber().Result == "Row 3");
        }
    }
}