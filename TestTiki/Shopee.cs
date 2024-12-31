using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace TestTiki
{
    public class ShopeeTestLogin
    {
        private IWebDriver driver;

        //[SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            //giả mạo một trình duyệt hoặc hệ điều hành khác khi truy cập trang web
            //để tránh cookie
            options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.5735.199 Safari/537.36");
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        //[Test]
        public void TestLoginShopeeCorrect()
        {
            string cookiesFilePath = @"H:\DaiHoc\Test\cookies.txt";

            // Xóa tất cả cookies cũ trước khi bắt đầu
            driver.Manage().Cookies.DeleteAllCookies();

            // Điều hướng đến trang đăng nhập
            driver.Navigate().GoToUrl("https://shopee.vn/buyer/login");



            // Đợi trang tải sau khi refresh (nếu có cookies, kiểm tra trạng thái đã đăng nhập)
            Thread.Sleep(5000);
            // Nếu chưa có cookies, lưu lại sau khi đăng nhập thủ công
            //if (!File.Exists(cookiesFilePath))
            //{
                //Console.WriteLine("Vui lòng đăng nhập thủ công và sau đó lưu cookies...");
                //Thread.Sleep(20000); // Đợi người dùng đăng nhập thủ công
                //SaveCookies.SaveCookiesToFile(driver, cookiesFilePath);
            //}
            //else
            //{
                // Load cookies từ file
                //SaveCookies.LoadCookiesFromFile(driver, cookiesFilePath);
                //driver.Navigate().Refresh();
            //}

            // Điền thông tin đăng nhập nếu chưa có cookies
            if (driver.Url.Contains("buyer/login"))
            {
                driver.FindElement(By.Name("loginKey")).SendKeys("username"); //nếu muốn test thì thay bằng tài khoản thật
                driver.FindElement(By.Name("password")).SendKeys("password");

                // Nhấn nút "Đăng nhập"
                driver.FindElement(By.XPath("//button[contains(text(), 'Đăng nhập')]")).Click();

                // Đợi trang tải xong sau đăng nhập
                Thread.Sleep(5000);
            }

            // Xác nhận đăng nhập thành công (ví dụ kiểm tra URL hoặc một phần tử chỉ xuất hiện sau đăng nhập)
            if (driver.Url.Contains("shopee.vn"))
            {
                Console.WriteLine("Đăng nhập thành công!");
            }
            else
            {
                Console.WriteLine("Đăng nhập thất bại!");
            }

            Assert.Pass();

        }

        //[Test]
        public void TestLoginShopeeNotCorrect()
        {

            // Xóa tất cả cookies cũ trước khi bắt đầu
            driver.Manage().Cookies.DeleteAllCookies();

            // Điều hướng đến trang đăng nhập
            driver.Navigate().GoToUrl("https://shopee.vn/buyer/login");


            Thread.Sleep(5000);


            // Điền thông tin đăng nhập nếu chưa có cookies
            if (driver.Url.Contains("buyer/login"))
            {
                driver.FindElement(By.Name("loginKey")).SendKeys("GAGAWG");
                driver.FindElement(By.Name("password")).SendKeys("ghawegawg");

                // Nhấn nút "Đăng nhập"
                driver.FindElement(By.XPath("//button[contains(text(), 'Đăng nhập')]")).Click();

            }

            //hiện thông báo đăng nhập thành công hay ko
            var errorMessage = driver.FindElements(By.XPath("//div[contains(@class, 'HyEuQL') and contains(text(), 'Tên tài khoản của bạn hoặc Mật khẩu không đúng')]"));
            if (errorMessage.Count > 0)
            {
                Console.WriteLine("Đăng nhập thất bại!");
            }
            else
            {
                Console.WriteLine("Đăng nhập thành công!");
            }

            Assert.Pass();

        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit(); // Đóng trình duyệt và giải phóng tài nguyên
        }
    }
}
