using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System;
using System.IO;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
//using SeleniumExtras.PageObjects;

namespace TestTiki
{
    public class ProfileTest
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

        // Test Thành Công
        [Test]
        public void TikiQl()
        {
            // Khởi tạo WebDriverWait
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Nhập tên mới
            var nameInput = driver.FindElement(By.CssSelector("input[name='fullName']"));
            Actions actions = new Actions(driver);
            actions.Click(nameInput).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Delete).Perform();
            string fullName = "Dang Map";
            nameInput.SendKeys(fullName);

            // Nhập nickname
            var userNameInput = driver.FindElement(By.CssSelector("input[name='userName']"));
            actions.Click(userNameInput).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Delete).Perform();
            string userName = "daldeptrai";
            userNameInput.SendKeys(userName);

            // Chọn ngày sinh
            string dayValue = "12";
            string monthValue = "2";
            string yearValue = "2005";
            var dayDropdown = new SelectElement(driver.FindElement(By.CssSelector("select[name='day']")));
            dayDropdown.SelectByValue(dayValue);
            var monthDropdown = new SelectElement(driver.FindElement(By.CssSelector("select[name='month']")));
            monthDropdown.SelectByValue(monthValue);
            var yearDropdown = new SelectElement(driver.FindElement(By.CssSelector("select[name='year']")));
            yearDropdown.SelectByValue(yearValue);

            // Chọn giới tính (Nam)
            try
            {
                var genderMaleFakeRadio = driver.FindElement(By.CssSelector("input[name='gender'][value='male'] + span.radio-fake"));
                if (genderMaleFakeRadio.Displayed && genderMaleFakeRadio.Enabled)
                {
                    genderMaleFakeRadio.Click();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting gender: {ex.Message}");
            }

            // Chọn quốc tịch (Vietnamese)
            try
            {
                var nationalityInput = driver.FindElement(By.CssSelector("input.input.with-icon-right[name='nationality']"));
                nationalityInput.Click();

                //var dropdownList = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("div.list")));
                //var vietnamOption = dropdownList.FindElement(By.CssSelector("div.item#nation-Vietnamese"));
                //vietnamOption.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error selecting nationality: {ex.Message}");
            }

            // Lưu thông tin
            try
            {
                var saveButton = driver.FindElement(By.CssSelector(".krVhnJ .bottom button[type='submit'].cqEaiM"));
                //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(saveButton));
                saveButton.Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clicking save button: {ex.Message}");
            }

            Thread.Sleep(5000);

            var SaveButton = driver.FindElement(By.CssSelector(".styles__StyledAccountInfo-sc-s5c7xj-2.khBVOu .form-control button[type='submit'].styles__StyledBtnSubmit-sc-s5c7xj-3.cqEaiM.btn-submit"));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SaveButton));
            SaveButton.Click();



            // In thông tin trước khi reload
            Console.WriteLine($"Trước khi reload - FullName: {fullName}, UserName: {userName}, Day: {dayValue}, Month: {monthValue}, Year: {yearValue}");

            Thread.Sleep(5000);

            // Reload lại trang
            driver.Navigate().Refresh();

            // Kiểm tra giá trị sau khi reload
            try
            {
                var nameAfterReload = driver.FindElement(By.CssSelector("input[name='fullName']")).GetAttribute("value");
                var userNameAfterReload = driver.FindElement(By.CssSelector("input[name='userName']")).GetAttribute("value");
                var dayAfterReload = new SelectElement(driver.FindElement(By.CssSelector("select[name='day']"))).SelectedOption.GetAttribute("value");
                var monthAfterReload = new SelectElement(driver.FindElement(By.CssSelector("select[name='month']"))).SelectedOption.GetAttribute("value");
                var yearAfterReload = new SelectElement(driver.FindElement(By.CssSelector("select[name='year']"))).SelectedOption.GetAttribute("value");

                // In thông tin sau khi reload
                Console.WriteLine($"Sau khi reload - FullName: {nameAfterReload}, UserName: {userNameAfterReload}, Day: {dayAfterReload}, Month: {monthAfterReload}, Year: {yearAfterReload}");

                // So sánh giá trị
                if (nameAfterReload == fullName &&
                    userNameAfterReload == userName &&
                    dayAfterReload == dayValue &&
                    monthAfterReload == monthValue &&
                    yearAfterReload == yearValue)
                {
                    Console.WriteLine("Dữ liệu đã được lưu thành công!");
                }
                else
                {
                    Console.WriteLine("Dữ liệu không được lưu chính xác.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra giá trị sau reload: {ex.Message}");
            }
        }

        [Test]
        public void Tiki_case_fullname()
        {
            // Khởi tạo WebDriverWait
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Nhập tên mới
            var nameInput = driver.FindElement(By.CssSelector("input[name='fullName']"));
            Actions actions = new Actions(driver);
            actions.Click(nameInput).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Delete).Perform();
            string fullName = "Dang Dep Trai";
            nameInput.SendKeys(fullName);

            Thread.Sleep(5000);

            var SaveButton = driver.FindElement(By.CssSelector(".styles__StyledAccountInfo-sc-s5c7xj-2.khBVOu .form-control button[type='submit'].styles__StyledBtnSubmit-sc-s5c7xj-3.cqEaiM.btn-submit"));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SaveButton));
            SaveButton.Click();

            // Reload lại trang
            driver.Navigate().Refresh();

            // Kiểm tra giá trị sau khi reload
            try
            {
                var nameAfterReload = driver.FindElement(By.CssSelector("input[name='fullName']")).GetAttribute("value");


                // In thông tin sau khi reload
                Console.WriteLine($"Sau khi reload - FullName: {nameAfterReload}");

                // So sánh giá trị
                if (nameAfterReload == fullName)

                {
                    Console.WriteLine("Dữ liệu đã được lưu thành công!");
                }
                else
                {
                    Console.WriteLine("Dữ liệu không được lưu chính xác.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra giá trị sau reload: {ex.Message}");
            }

        }

        [Test]
        public void Tiki_case_fullname_number()
        {
            // Khởi tạo WebDriverWait
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Nhập tên mới
            var nameInput = driver.FindElement(By.CssSelector("input[name='fullName']"));
            Actions actions = new Actions(driver);
            actions.Click(nameInput).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Delete).Perform();
            string fullName = "";
            nameInput.SendKeys(fullName);

            Thread.Sleep(5000);

            var SaveButton = driver.FindElement(By.CssSelector(".styles__StyledAccountInfo-sc-s5c7xj-2.khBVOu .form-control button[type='submit'].styles__StyledBtnSubmit-sc-s5c7xj-3.cqEaiM.btn-submit"));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SaveButton));
            SaveButton.Click();

            // Kiểm tra nếu có thông báo lỗi về tên
            try
            {
                // Kiểm tra xem có thông báo lỗi nào xuất hiện không
                var errorMessage = driver.FindElement(By.CssSelector(".message.error"));
                if (errorMessage.Displayed)
                {
                    Console.WriteLine($"Lỗi: {errorMessage.Text}");
                }
                else
                {
                    // Reload lại trang nếu không có lỗi
                    driver.Navigate().Refresh();

                    // Kiểm tra giá trị sau khi reload
                    var nameAfterReload = driver.FindElement(By.CssSelector("input[name='fullName']")).GetAttribute("value");

                    // In thông tin sau khi reload
                    Console.WriteLine($"Sau khi reload - FullName: {nameAfterReload}");

                    // So sánh giá trị
                    if (nameAfterReload == fullName)
                    {
                        Console.WriteLine("Dữ liệu đã được lưu thành công!");
                    }
                    else
                    {
                        Console.WriteLine("Dữ liệu không được lưu chính xác.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra thông báo lỗi hoặc giá trị sau reload: {ex.Message}");
            }
        }

        [Test]
        public void Tiki_case_nickname()
        {
            // Khởi tạo WebDriverWait
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Nhập tên mới

            Actions actions = new Actions(driver);
            var userNameInput = driver.FindElement(By.CssSelector("input[name='userName']"));
            actions.Click(userNameInput).KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).SendKeys(Keys.Delete).Perform();
            string userName = "dal";
            userNameInput.SendKeys(userName);


            Thread.Sleep(5000);

            var SaveButton = driver.FindElement(By.CssSelector(".styles__StyledAccountInfo-sc-s5c7xj-2.khBVOu .form-control button[type='submit'].styles__StyledBtnSubmit-sc-s5c7xj-3.cqEaiM.btn-submit"));
            //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(SaveButton));
            SaveButton.Click();

            // Kiểm tra nếu có thông báo lỗi về tên
            try
            {
                // Kiểm tra xem có thông báo lỗi nào xuất hiện không
                var errorMessage = driver.FindElement(By.CssSelector(".message.error"));
                if (errorMessage.Displayed)
                {
                    Console.WriteLine($"Lỗi: {errorMessage.Text}");
                }
                else
                {
                    // Reload lại trang nếu không có lỗi
                    driver.Navigate().Refresh();

                    // Kiểm tra giá trị sau khi reload
                    var nameAfterReload = driver.FindElement(By.CssSelector("input[name='fullName']")).GetAttribute("value");

                    // In thông tin sau khi reload
                    Console.WriteLine($"Sau khi reload - FullName: {nameAfterReload}");

                    // So sánh giá trị
                    if (nameAfterReload == userName)
                    {
                        Console.WriteLine("Dữ liệu đã được lưu thành công!");
                    }
                    else
                    {
                        Console.WriteLine("Dữ liệu không được lưu chính xác.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi kiểm tra thông báo lỗi hoặc giá trị sau reload: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Quit(); //đóng trình duyệt sau khi tất cả test hoàn thành
        }
    }
}
