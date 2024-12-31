using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;


namespace TestTiki
{
    public class TikiTestCart
    {
        private IWebDriver driver;

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
            var options = new ChromeOptions();

            driver.Navigate().GoToUrl("https://tiki.vn/");
            Thread.Sleep(5000);
            //đóng quảng cáo
            try
            {
                var closeButton = driver.FindElement(By.CssSelector("img[alt='close-icon']"));
                closeButton.Click();
                Thread.Sleep(1500);
            }
            catch (NoSuchElementException)
            {
                //nếu không có quảng cáo thì bỏ qua
            }

            // Mở form đăng nhập
            driver.FindElement(By.CssSelector("div[data-view-id='header_header_account_container']")).Click();
            //bấm vào sdt
            driver.FindElement(By.Name("tel")).SendKeys("0901647655");

            Thread.Sleep(1300);
            //bấm tiếp tục để hiện password
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2100);
            //nhâp mật khẩu
            driver.FindElement(By.XPath("//input[@placeholder='Mật khẩu']")).SendKeys("Huy12345");

            Thread.Sleep(1000);
            //bấm tiếp tục để đăng nhập
            driver.FindElement(By.XPath("//button[contains(text(), 'Đăng Nhập')]")).Click();

            // đợi 5 giây để người dùng giải capcha
            Thread.Sleep(5000);

            try
            {
                var closeButton = driver.FindElement(By.CssSelector("img[alt='close-icon']"));
                closeButton.Click();
                Thread.Sleep(1500);
            }
            catch (NoSuchElementException)
            {
                //nếu không có quảng cáo thì bỏ qua
            }
        }

        //thêm sản phẩm vào giỏ hàng đúng với sản phẩm cần thêm (Thêm sản phẩm hợp lệ)
        [Test]
        public void testCase1()
        {
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-earth-choice-pure-clean-1l-p276237692.html?spid=276237693");
            Thread.Sleep(2000);

            driver.FindElement(By.CssSelector("button[data-view-id='pdp_add_to_cart_button']")).Click();

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//img[@alt='header_header_img_Cart']")).Click();

            Thread.Sleep(2000);

            // Kiểm tra nếu thông báo "Giỏ hàng trống" xuất hiện
            var productNameElements = driver.FindElements(By.XPath("//a[contains(text(), 'Nước Giặt Earth Choice Pure Clean 1L')]"));
            var emptyMessageElements = driver.FindElements(By.CssSelector("p.empty__message1"));
            Console.WriteLine("Test case 1 Thêm sản phẩm vào giỏ hàng");

            if (emptyMessageElements.Count > 0 && emptyMessageElements[0].Text.Contains("Giỏ hàng trống"))
            {
                Assert.Fail("Thêm sản phẩm vào giỏ hàng thất bại. Thông báo: 'Giỏ hàng trống");
            }
            else if (productNameElements.Count > 0)
            {
                Assert.Pass("Sản phẩm đã được thêm vào giỏ hàng đúng với kết quả mông đợi");
            }
            else
            {
                Assert.Fail("Sản phẩm đã thêm vào giỏ hàng không đúng với kết quả mông đợi");
            }
        }

        //Thêm sản phẩm với số lượng bằng 0 
        [Test]
        public void testCase2()
        {
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-earth-choice-pure-clean-1l-p276237692.html?spid=276237693");
            Thread.Sleep(2000);
            // Tìm trường input
            var inputElement = driver.FindElement(By.CssSelector("input.input"));

            // Xóa giá trị hiện tại và nhập giá trị mới (0)
            inputElement.Clear();
            inputElement.SendKeys("0");

            // Kiểm tra giá trị đã được đặt thành công hay chưa
            string inputValue = inputElement.GetAttribute("value");

            // Sử dụng if-else với Assert.Pass và Assert.Fail
            if (inputValue == "0")
            {
                Assert.Fail("Giá trị trong ô input là 0. Test thất bại!");
            }
            else
            {
                Assert.Pass($"Giá trị trong ô input không phải là 0. ");
            }
        }
        //Thêm sản phẩm với số lượng âm 
        [Test]
        public void testCase3()
        {
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-earth-choice-pure-clean-1l-p276237692.html?spid=276237693");
            Thread.Sleep(2000);
            // Tìm trường input
            var inputElement = driver.FindElement(By.CssSelector("input.input"));

            // Xóa giá trị hiện tại và nhập giá trị mới (-1)
            inputElement.Clear();
            inputElement.SendKeys("-1");
            Thread.Sleep(5000);

            // Kiểm tra giá trị đã được đặt thành công hay chưa
            string inputValue = inputElement.GetAttribute("value");

            // Kiểm tra xem giá trị có nhỏ hơn 0 hay không
            if (int.TryParse(inputValue, out int numericValue) && numericValue < 0)
            {
                Assert.Fail($"Giá trị trong ô input là {inputValue} (nhỏ hơn 0). Test thất bại!");
            }
            else
            {
                Assert.Pass($"Giá trị trong ô input hợp lệ: {inputValue} không thể nhập vào giá trị nhỏ hơn 1");
            }
            Thread.Sleep(2000);
        }

        //Thêm sản phẩm vượt quá tồn kho
        [Test]
        public void testCase4()
        {
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-earth-choice-pure-clean-1l-p276237692.html?spid=276237693");
            // Chờ trang tải xong
            Thread.Sleep(2000); // Hoặc sử dụng WebDriverWait

            // Tìm trường input số lượng và nhập số lượng vượt quá tồn kho
            var inputElement = driver.FindElement(By.CssSelector("input.input"));
            inputElement.Clear();
            inputElement.SendKeys("1000");  // Số lượng vượt quá tồn kho (giả sử tồn kho là 57)

            // Tìm nút "Thêm vào giỏ" và click vào nó
            var addToCartButton = driver.FindElement(By.CssSelector("button[data-view-id='pdp_add_to_cart_button']"));
            addToCartButton.Click();

            // Chờ để thông báo lỗi hiển thị
            Thread.Sleep(2000); // Hoặc sử dụng WebDriverWait

            // Kiểm tra nếu có thông báo lỗi xuất hiện
            var errorMessageElements = driver.FindElements(By.CssSelector("div[type='error']"));
            if (errorMessageElements.Count > 0)
            {
                string errorMessage = errorMessageElements[0].Text;
                // Kiểm tra nếu thông báo lỗi chứa thông tin về số lượng tồn kho
                if (errorMessage.Contains("Số lượng còn lại của sản phẩm này là"))
                {
                    Assert.Pass($"{errorMessage}");
                }
                else
                {
                    Assert.Fail("Không có thông báo lỗi đúng như kỳ vọng.");
                }
            }
            else
            {
                Assert.Fail("Không có thông báo lỗi khi nhập số lượng vượt quá tồn kho.");
            }
            Thread.Sleep(2000);
        }

        //Thêm sản phẩm vào giỏ hàng nhiều lần để tăng số lượng 
        [Test]
        public void testCase5()
        {

            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-earth-choice-pure-clean-1l-p276237692.html?spid=276237693");

            // Bước 1: Mở trang sản phẩm đầu tiên
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-cao-cap-earth-choice-1l-p276281186.html?spid=276281187");

            // Bước 2: Thêm sản phẩm vào giỏ hàng
            var addToCartButton = driver.FindElement(By.CssSelector("button[data-view-id='pdp_add_to_cart_button']"));
            addToCartButton.Click();

            // Đợi một chút để giỏ hàng cập nhật
            Thread.Sleep(2000);

            // Bước 3: Truy cập trang giỏ hàng để kiểm tra số lượng sản phẩm
            driver.Navigate().GoToUrl("https://tiki.vn/checkout/cart?src=header_cart");
            Thread.Sleep(2000);

            // Kiểm tra số lượng sản phẩm trong giỏ hàng
            IWebElement quantityElement = driver.FindElement(By.CssSelector("input.qty-input"));
            int quantityBefore = int.Parse(quantityElement.GetAttribute("value"));
            Console.WriteLine($"Số lượng trong giỏ hàng trước khi thêm lần nữa: {quantityBefore}");

            // Bước 4: Quay lại trang sản phẩm và thêm sản phẩm một lần nữa
            driver.Navigate().GoToUrl("https://tiki.vn/nuoc-giat-cao-cap-earth-choice-1l-p276281186.html?spid=276281187");
            Thread.Sleep(2000);

            // Thêm sản phẩm vào giỏ hàng lần nữa
            addToCartButton = driver.FindElement(By.CssSelector("button[data-view-id='pdp_add_to_cart_button']"));
            addToCartButton.Click();
            Thread.Sleep(2000); // Đợi một chút để sản phẩm được thêm vào giỏ hàng

            // Bước 5: Truy cập lại trang giỏ hàng để kiểm tra số lượng sản phẩm
            driver.Navigate().GoToUrl("https://tiki.vn/checkout/cart?src=header_cart");
            Thread.Sleep(2000);

            // Kiểm tra lại số lượng sản phẩm trong giỏ hàng
            quantityElement = driver.FindElement(By.CssSelector("input.qty-input"));
            int quantityAfter = int.Parse(quantityElement.GetAttribute("value"));
            Console.WriteLine($"Số lượng trong giỏ hàng sau khi thêm lần nữa: {quantityAfter}");

            // Bước 6: Kiểm tra số lượng sản phẩm trong giỏ hàng có tăng đúng không
            if (quantityAfter == quantityBefore + 1)
            {
                Assert.Pass("Số lượng sản phẩm trong giỏ hàng đã tăng đúng.");
            }
            else
            {
                Assert.Fail($"Số lượng sản phẩm trong giỏ hàng không đúng. Trước: {quantityBefore}, Sau khi thêm lần nữa: {quantityAfter}");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Quit(); //đóng trình duyệt sau khi tất cả test hoàn thành
        }
    }
}
