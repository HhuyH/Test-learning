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
    public class CartTest
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

        //thêm sản phẩm vào giỏ hàng đứng với sản phẩm cần thêm
        //[Test]
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

            if (emptyMessageElements.Count > 0 && emptyMessageElements[0].Text.Contains("Giỏ hàng trống"))
            {
                Assert.Fail("Thêm sản phẩm vào giỏ hàng thất bại. Thông báo: 'Giỏ hàng trống");
            } else if (productNameElements.Count > 0)
            {
                Assert.Pass("Sản phẩm đã được thêm vào giỏ hàng đúng với kết quả mông đợi");
            }
            else
            {
                Assert.Fail("Sản phẩm đã thêm vào giỏ hàng không đúng với kết quả mông đợi");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Quit(); //đóng trình duyệt sau khi tất cả test hoàn thành
        }
    }
}
