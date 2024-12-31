using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;


namespace TestTiki
{
    public class SearchTiki
    {
        private IWebDriver driver;
        private string SearchKey;
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            driver.Manage().Window.Maximize();
        }

        [SetUp]
        public void Setup()
        {
            // 1. Mở trang Tiki.vn
            driver.Navigate().GoToUrl("https://tiki.vn/");

            // Đợi trang tải
            Thread.Sleep(5000);

            // Tìm và đóng quảng cáo
            driver.FindElement(By.CssSelector("img[alt='close-icon']")).Click();

            // 4. Đợi kết quả tìm kiếm tải
            Thread.Sleep(3000);
        }

        //sản phẩm tìm kiếm đứng với keyword
        //TestCase1
        [Test]
        public void TestSearchSuccess()
        {
            SearchKey = "Macbook";

            // 2. Tìm ô tìm kiếm và nhập từ khóa
            var searchBox = driver.FindElement(By.CssSelector("input[data-view-id='main_search_form_input']"));
            searchBox.SendKeys(SearchKey);

            // 3. Nhấn nút tìm kiếm
            var searchButton = driver.FindElement(By.CssSelector("button[data-view-id='main_search_form_button']"));
            searchButton.Click();

            Thread.Sleep(3000);

            // Lấy danh sách các sản phẩm
            var productList = driver.FindElements(By.CssSelector("span.style__StyledItem-sc-139nb47-0.fxtnPx"));

            // Kiểm tra từng sản phẩm xem có chứa từ "macbook" trong tên không
            foreach (var product in productList)
            {
                // Lấy tên sản phẩm
                var productName = product.FindElement(By.CssSelector("h3.style__NameStyled-sc-139nb47-8.ibOlar")).Text;

                if (!productName.ToLower().Trim().Contains("macbook"))
                {
                    Assert.Fail($"Sản phẩm không liên quan: {productName}");
                }
                else
                {
                    Assert.Pass("Sản phẩm tìm thấy khớp với từ khóa");
                }
            }

        }

        //tìm kiếm sản phẩm với từ khóa không hợp lệ
        //TestCase2
        [Test]
        public void TestSearchInValidKey()
        {

            SearchKey = "abzjhfv";
            // 2. Tìm ô tìm kiếm và nhập từ khóa
            var searchBox = driver.FindElement(By.CssSelector("input[data-view-id='main_search_form_input']"));
            searchBox.SendKeys(SearchKey);

            // 3. Nhấn nút tìm kiếm
            var searchButton = driver.FindElement(By.CssSelector("button[data-view-id='main_search_form_button']"));
            searchButton.Click();

            // Lấy danh sách các sản phẩm
            var productList = driver.FindElements(By.CssSelector("span.style__StyledItem-sc-139nb47-0.fxtnPx"));

            foreach (var product in productList)
            {
                // Lấy tên sản phẩm
                var productName = product.FindElement(By.CssSelector("h3.style__NameStyled-sc-139nb47-8.ibOlar")).Text;

                if (!productName.ToLower().Trim().Contains("abzjhfv"))
                {
                    Assert.Pass($"không có sản phẩm nào liên quan đến từ khóa: '{SearchKey}' đã tìm kiếm");
                }
                else
                {
                    Assert.Fail("Sản phẩm tìm thấy khớp với từ khóa");
                }
            }
        }

        // tìm sản phẩm theo bộ lậu với giá 22tr
        //TestCase3
        [Test]
        public void TestSearchIphoneAndFilterByPrice()
        {
            SearchKey = "iPhone";

            // Tìm ô tìm kiếm và nhập từ khóa
            var searchBox = driver.FindElement(By.CssSelector("input[data-view-id='main_search_form_input']"));
            searchBox.SendKeys(SearchKey);

            // Nhấn nút tìm kiếm
            driver.FindElement(By.CssSelector("button[data-view-id='main_search_form_button']")).Click();
            Thread.Sleep(3000); // Đợi kết quả tìm kiếm tải

            // Nhấn vào nút bộ lọc
            driver.FindElement(By.CssSelector("button.styles__StyledButton-sc-kqbl3f-0.bQYwcw")).Click();
            Thread.Sleep(2000);

            // Chọn bộ lọc giá "Dưới 22.500.000"
            driver.FindElement(By.XPath("//button[.//div[contains(text(),'Dưới 22.000.000')]]")).Click();
            Thread.Sleep(3000); // Đợi kết quả lọc tải

            // Nhấn nút "Xem kết quả"
            driver.FindElement(By.XPath("//div[contains(text(),'Xem kết quả')]")).Click();
            Thread.Sleep(5000);

            // Lấy danh sách sản phẩm sau khi lọc
            var productList = driver.FindElements(By.CssSelector("span.style__StyledItem-sc-139nb47-0.fxtnPx"));


            // Kiểm tra từng sản phẩm xem giá có đúng không
            foreach (var product in productList)
            {
                var productPriceText = product.FindElement(By.CssSelector("div.price-discount__price")).Text;
                decimal productPrice = ConvertToDecimal(productPriceText);

                if (productPrice <= 22500000)
                {
                    Assert.Pass("Sản phẩm tím thầy không có náo lớn hơn 22.000.000 ");
                }
                else
                {
                    Assert.Fail($"Sản phẩm có giá không khớp điều kiện lọc: {productPriceText}");
                }
            }
        }

        // tìm sản phẩm theo bộ lộc từ khoản giá nhập vào
        //TestCase4
        [Test]
        public void TestSearchIphoneAndFilterByPriceRange()
        {
            SearchKey = "iPhone";
            decimal priceForm = 1000000;
            decimal priceTo = 2000000;

            // Tìm ô tìm kiếm và nhập từ khóa "iPhone"
            var searchBox = driver.FindElement(By.CssSelector("input[data-view-id='main_search_form_input']"));
            searchBox.SendKeys(SearchKey);

            // Nhấn nút tìm kiếm
            driver.FindElement(By.CssSelector("button[data-view-id='main_search_form_button']")).Click();
            Thread.Sleep(3000); // Đợi kết quả tìm kiếm

            // Nhấn vào nút bộ lọc
            driver.FindElement(By.CssSelector("button.styles__StyledButton-sc-kqbl3f-0.bQYwcw")).Click();
            Thread.Sleep(2000); // Đợi bộ lọc hiển thị

            // Nhập khoảng giá "Từ" và "Đến"
            var fromPriceInput = driver.FindElement(By.CssSelector("input[placeholder='Từ']"));
            fromPriceInput.Clear();
            fromPriceInput.SendKeys(priceForm.ToString());

            var toPriceInput = driver.FindElement(By.CssSelector("input[placeholder='Đến']"));
            toPriceInput.Clear();
            toPriceInput.SendKeys(priceTo.ToString());

            // Nhấn nút "Xem kết quả"
            var seeResultsButton = driver.FindElement(By.XPath("//div[contains(text(),'Xem kết quả')]"));
            seeResultsButton.Click();
            Thread.Sleep(5000); // Đợi kết quả lọc hiển thị

            // Lấy danh sách sản phẩm sau khi lọc
            var productList = driver.FindElements(By.CssSelector("span.style__StyledItem-sc-139nb47-0.fxtnPx"));

            // Kiểm tra từng sản phẩm xem giá có đúng không
            foreach (var product in productList)
            {
                var productPriceText = product.FindElement(By.CssSelector("div.price-discount__price")).Text;
                decimal productPrice = ConvertToDecimal(productPriceText);

                if (productPrice < priceForm && productPrice > priceTo)
                {
                    Assert.Fail($"Sản phẩm có giá không khớp điều kiện lọc: {productPriceText}");
                }
                else
                {
                    Assert.Pass("Sản phẩm tím thầy hợp lệ trông khoản tìm kiếm ");
                }
            }
        }

        private decimal ConvertToDecimal(string priceText)
        {
            try
            {
                // Loại bỏ dấu chấm và "đ" trong chuỗi giá
                priceText = priceText.Replace(".", "").Replace("đ", "").Trim();
                return decimal.Parse(priceText);
            }
            catch
            {
                return 0;
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Quit(); //đóng trình duyệt sau khi tất cả test hoàn thành
        }
    }
}
