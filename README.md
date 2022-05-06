# Playwright.Extensions

## Playwright.Extensions.Table

The `Playwright.Extensions.Table` package contains the elements you need to test HTML tables. By default, tables are not supported by
Playwright. This package enables you to test the structure and contents of tables with just a few lines of code.

### Example

Here's a complete example, including all the Playwright setup:

```csharp
[Test]
public async Task SimpleTable_HeaderValues()
{
    // Arrange (Playwright setup)
    using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();
    await page.GotoAsync("https://www.w3schools.com/html/html_tables.asp");
    
    // Arrange (Locator setup)
    //Selector for every cell containing headers inside first tablerow.
    var tableHeaders = page.Locator("table[id='customers'] > tbody > tr > th");
    //Selector for all tablerows minus the first because that contains the headers.
    var tableRows = page.Locator("table[id='customers'] > tbody > tr ~ tr");
    var tableElement = page.LocatorTableElement(tableHeaders, tableRows);
    
    // Act
    var tableHeaderValues = await tableElement.GetTableHeaderValuesAsync();
    var tableRowElements = await tableElement.GetTableRowElementsAsync();
    var tableRowIslandTrading = tableRowElements.Single(x => x.GetColumn("Company").TextContentAsync().Result == "Island Trading");
    
    // Assert
    var expectedFirstHeader = "Company";
    
    tableHeaderValues.First().Should().BeEquivalentTo(expectedFirstHeader);
    tableRowElements[0].TableHeaderValues.First().Should().BeEquivalentTo(expectedFirstHeader);
    (await tableRowIslandTrading.GetColumn("Country").TextContentAsync()).Should().Be("UK");
}
```

## About Playwright

>Playwright enables reliable end-to-end testing for modern web apps.  
>**Any browser • Any platform • One API**

More information: [playwright.dev](https://playwright.dev/).
