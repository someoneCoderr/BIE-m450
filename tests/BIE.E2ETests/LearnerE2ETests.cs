using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace BIE.E2ETests;

[TestClass]
public class LearnerE2ETests : BrowserTestBase
{
    [TestMethod]
    public void TouristAt30DaysRemainsApprovedAfterReload()
    {
        // Arrange
        Fill("Sam Browser", "Tourismus", "30");
        
        // Act
        Driver.FindElement(By.Id("submit")).Click();
        
        var result = Wait.Until(driver =>
        {
            var candidate = driver.FindElement(By.Id("result"));
            return candidate.Displayed &&
                   candidate.GetDomAttribute("data-decision") == "approved"
                ? candidate
                : null;
        });
        StringAssert.Contains(result.Text, "GENEHMIGT");
        var caseId = result.GetDomAttribute("data-application-id");
        Assert.IsFalse(string.IsNullOrWhiteSpace(caseId));

        // Nach Reload dieselbe Fall-ID im Register wiederfinden.
        Driver.Navigate().Refresh();
        var row = Wait.Until(driver =>
            driver.FindElements(By.CssSelector("#application-list tr"))
                .FirstOrDefault(candidate => candidate.GetDomAttribute("data-id") == caseId));
        Assert.IsNotNull(row);
        var cells = row.FindElements(By.TagName("td"));
        Assert.AreEqual(5, cells.Count);
        Assert.AreEqual("Sam Browser", cells[1].Text);
        Assert.AreEqual("Orion", cells[2].Text);
        Assert.AreEqual("30 Tage", cells[3].Text);
        StringAssert.Contains(cells[4].Text, "GENEHMIGT");
    }
}
