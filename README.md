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

    // Arrange (Locator setup)
    var tableHeaders = page.Locator("//table/thead/tr/th");
    var tableRows = page.Locator("//table/tbody/tr");
    var tableElement = page.LocatorTableElement(tableHeaders, tableRows);

    var expectedFirstHeader = "Specification";

    // Act
    await page.GotoAsync("https://developer.mozilla.org/en-US/docs/Web/HTML/Element/table");
    var tableHeaderValues = await tableElement.GetTableHeaderValuesAsync();
    var tableRowElements = await tableElement.GetTableRowElementsAsync();

    // Assert
    tableHeaderValues.First().Should().BeEquivalentTo(expectedFirstHeader);
    tableRowElements[0].TableHeaderValues.First().Should().BeEquivalentTo(expectedFirstHeader);
}
```

## About Playwright

>Playwright enables reliable end-to-end testing for modern web apps.  
>**Any browser • Any platform • One API**

More information: [playwright.dev](https://playwright.dev/).
