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
    public class TikiTestLogin
    {
        private IWebDriver driver;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
        }

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();

            driver.Navigate().GoToUrl("https://tiki.vn/");
            Thread.Sleep(2000);
            //đóng quảng cáo
            try
            {
                driver.FindElement(By.CssSelector("img[alt='close-icon']")).Click();
                Thread.Sleep(1500);
            }
            catch (NoSuchElementException) {
                //nếu không có quảng cáo thì bỏ qua
            }

            // Mở form đăng nhập
            driver.FindElement(By.CssSelector("div[data-view-id='header_header_account_container']")).Click();
            Thread.Sleep(2000);
        }

        //Testcase 1 SDT trống
        [Test]
        public void TestCase1()
        {

            EnterPhoneNumber(" ");
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 1 Test SDT trống: ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không được để trống")
            {
                Assert.Pass("Kết quả: Số điện thoại trống | Thông báo đúng");
            }
            else
            {
                Assert.Fail("Kết quả: Thông báo đã có gì đó được nhập vào trong SDT");
            }

            Thread.Sleep(2150);
        }

        //Test case 2 có 11 SDT
        [Test]
        public void TestCase2() {

            EnterPhoneNumber("09876543211");

            Thread.Sleep(2050);

            //lấy SDT đã nhập
            string phoneNumber = driver.FindElement(By.Name("tel")).GetAttribute("value");

            //đếm SDT đã nhập
            int characterCount = phoneNumber.Length;

            Console.WriteLine("Test case 2 Nhập vào 11 số kiểm tra xem có bao nhiếu số được nhập vào");
            if (characterCount == 10)
            {
                Assert.Pass("Kết quả: Chỉ có 10 số được nhập vào");
            }
            else
            {
                Assert.Fail("Kết quả: số SDT nhập vào vượt quá 10 số");
            }
            Thread.Sleep(2150);
        }

        //Test case 3 có 9 SDT
        [Test]
        public void TestCase3()
        {
            EnterPhoneNumber("098765432");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2000);
            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));
            
            Console.WriteLine("Test case 3 Nhập vào 9 số ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không đúng định dạng.")
            {
                Assert.Pass("Kết quả: SDT phải có đủ 10 số chỉ có 9 số được nhập | Thông báo đúng");
            }
            else
            {
                Assert.Fail("Kết quả: Chưa nhập vào SDT");
            }
        }

        //Test case 4 nhập vào ký tự đặc biệt
        [Test]
        public void TestCase4()
        {

            EnterPhoneNumber("%@!^");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 4 Nhập vào ký tự đặc biệt ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không được để trống")
            {
                Assert.Pass("Kết quả: Không có ký tự đặt biệt nào được nhập vào");
            }
            else
            {
                Assert.Fail("Kết quả: Ký tự đặt biệt đã được nhập");
            }
        }

        //Test case 5 nhập vào ký tự
        [Test]
        public void TestCase5()
        {
            EnterPhoneNumber("abcdefghij");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 5 Nhập vào ký tự ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không được để trống")
            {
                Assert.Pass("Kết quả: Không có ký tự nào được nhập vào");
            }
            else
            {
                Assert.Fail("Kết quả: Ký tự đã được nhập");
            }
        }

        //Test case 6 nhập Mã mạng không tồn tại ở VN
        [Test]
        public void TestCase6()
        {
            EnterPhoneNumber("1111647655");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 6 Nhập mã mạng không tồn tại ở VN ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không đúng định dạng")
            {
                Assert.Pass("Kết quả: Không thể đăng nhập với mã mạng khác | thống báo đúng");
            }
            else
            {
                Assert.Fail("Kết quả: Đăng nhập thành công");
            }

            Thread.Sleep(2000);
        }

        //Test case 7 nhập SDT không có trong hệ thống
        [Test]
        public void TestCase7()
        {
            //Test case 7 nhập SDT không có trong hệ thống
            EnterPhoneNumber("0901647652");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2000);
            var elements = driver.FindElements(By.XPath("//h4[text()='Nhập mã xác minh']"));

            Console.WriteLine("Test case 7 nhập SDT không có trong hệ thống ");
            if (elements.Count > 0)
            {
                Assert.Pass("Kết quả: SDT chưa được đăng ký");
            }
            else
            {
                Assert.Fail("Kết quả: Đăng nhập SDT thành công tiến hành nhập MK");
            }
        }

        //Test case 8 nhập đúng SDT
        [Test]
        public void TestCase8()
        {
            //Test case 8 nhập đúng SDT
            EnterPhoneNumber("0901647655");
            driver.FindElement(By.Name("tel")).SendKeys("0901647655");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2000);
            var elements = driver.FindElements(By.XPath("//h4[text()='Nhập mật khẩu']"));

            Console.WriteLine("Test case 8 nhập đúng SDT ");
            if (elements.Count > 0)
            {
                Assert.Pass("Kết quả: Đăng nhập SDT thành công tiến hành nhập MK");
            }
            else
            {
                Assert.Fail("Kết quả: SDT không đúng");
            }
        }

        //Test case 9 MK rỗng
        [Test]
        public void TestCase9()
        {

            EnterPhoneNumber("0901647655");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2000);
            //driver.FindElement(By.XPath("//input[@placeholder='Mật khẩu']")).SendKeys("Huy12345");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[text()='Đăng Nhập']")).Click();


            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 9 Test MK trống: ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Mật khẩu không được để trống")
            {
                Assert.Pass("Mật khẩu trống | thông báo lỗi đúng");
            }
            else
            {
                Assert.Fail("Thông báo lỗi sai");
            }

        }

        //Test case 10 nhập sai MK
        [Test]
        public void TestCase10()
        {

            EnterPhoneNumber("0901647655");

            Thread.Sleep(1300);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2100);
            EnterPass("Gaw212315");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Đăng Nhập')]")).Click();

            Thread.Sleep(2000);
            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 10 Test sai mật khẩu: ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Thông tin đăng nhập không đúng")
            {
                Assert.Pass("Mật khẩu không đúng | thông báo lỗi đúng");
            }
            else
            {
                Assert.Fail("Thông báo lỗi sai");
            }

        }

        //Test case 11 đăng nhập thành công
        [Test]
        public void TestCase11()
        {
            //bấm vào sdt
            EnterPhoneNumber("0901647655");

            Thread.Sleep(1300);
            //bấm tiếp tục để hiện password
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(2100);
            //nhâp mật khẩu
            EnterPass("Huy12345");

            Thread.Sleep(1000);
            //bấm tiếp tục để đăng nhập
            driver.FindElement(By.XPath("//button[contains(text(), 'Đăng Nhập')]")).Click();
            // cho nay chi kiem tra thong bao lỗi khi sai Mật khẩu
            // tìm thông báo lỗi "Thông tin đăng nhập không đúng" để kiểm tra xem đăng nhập thanh công hay thất bại
            //đợi 5 giây để người dùng giải capcha
            Thread.Sleep(5000);

            Console.WriteLine("Test case 11 Test đăng nhập thành công: ");
            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            if (errorMessage.Count > 0 && errorMessage[0].Text == "Thông tin đăng nhập không đúng")
            {
                Assert.Fail("Đăng nhập thất bại!");
            }
            else
            {
                Assert.Pass("Đăng nhập thành công!");
            }

            //đợi 5 giây để người dùng giải capcha
            Thread.Sleep(5000);

            Assert.Pass();
        }

        //Test case 1-8 gôm tất cả test case từ 1-8 vào 1 test
        [Test]
        public void TestCase1To8()
        {
            //Test case 1 SDT rỗng
            EnterPhoneNumber(" ");
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            var errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 1 Test SDT trống: ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không được để trống")
            {
                Console.WriteLine("Kết quả: Số điện thoại trống | Thông báo đúng");
            }
            else
            {
                Console.WriteLine("Kết quả: Thông báo đã có gì đó được nhập vào trong SDT");
            }
            Thread.Sleep(2000);



            //Test case 2 nhập vào 11 SDT kiểm tra xem có bao nhiêu số thật sự được nhập vào
            EnterPhoneNumber("09876543211");

            //lấy SDT đã nhập
            string phoneNumber = driver.FindElement(By.Name("tel")).GetAttribute("value");

            //đếm SDT đã nhập
            int characterCount = phoneNumber.Length;

            Console.WriteLine("Test case 2 Nhập vào 11 số kiểm tra xem có bao nhiếu số được nhập vào");
            if (characterCount == 10)
            {
                Console.WriteLine("Kết quả: Chỉ có 10 số được nhập vào");
            }
            else
            {
                Console.WriteLine("Kết quả: Số chữ số SDT nhập vào vượt quá 10 số");
            }
            Thread.Sleep(2000);



            //Test case 3 có 9 SDT
            EnterPhoneNumber("098765432");
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(1000);
            errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 3 Nhập vào 9 số ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không đúng định dạng.")
            {
                Console.WriteLine("Kết quả: SDT phải có đủ 10 | Thông báo đúng");
            }
            else
            {
                Console.WriteLine("Kết quả: Chưa nhập vào SDT");
            }
            Thread.Sleep(2000);



            //Test case 4 nhập vào ký tự đặc biệt
            driver.FindElement(By.Name("tel")).Clear();
            EnterPhoneNumber("%@!^");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(1000);
            phoneNumber = driver.FindElement(By.Name("tel")).GetAttribute("value");

            Console.WriteLine("Test case 4 Nhập vào ký tự đặc biệt ");
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, "^[0-9]*$"))
            {
                Console.WriteLine("Kết quả: Không có ký tự đặt biệt nào được nhập vào");
            }
            else
            {
                Console.WriteLine("Kết quả: Ký tự đặt biệt đã được nhập");
            }
            Thread.Sleep(2000);



            //Test case 5 nhập vào ký tự
            driver.FindElement(By.Name("tel")).Clear();
            EnterPhoneNumber("abcdefghij");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(1000);
            phoneNumber = driver.FindElement(By.Name("tel")).GetAttribute("value");

            Thread.Sleep(1000);
            errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 5 Nhập vào ký tự ");
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, "^[0-9]*$"))
            {
                Console.WriteLine("Kết quả: Không có ký tự nào được nhập vào");
            }
            else
            {
                Console.WriteLine("Kết quả: Ký tự đã được nhập");
            }
            Thread.Sleep(2000);



            //Test case 6 nhập Mã mạng không tồn tại ở VN
            EnterPhoneNumber("1111647655");

            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(1000);
            errorMessage = driver.FindElements(By.CssSelector("span.error-mess"));

            Console.WriteLine("Test case 6 Nhập mã mạng không tồn tại ở VN ");
            if (errorMessage.Count > 0 && errorMessage[0].Text == "Số điện thoại không đúng định dạng")
            {
                Console.WriteLine("Kết quả: Không thể đăng nhập với mã mạng khác | thống báo đúng");
            }
            else
            {
                Console.WriteLine("Kết quả: Đăng nhập thành công");
            }

            Thread.Sleep(2000);


            //Test case 7 nhập SDT không có trong hệ thống
            EnterPhoneNumber("0901647652");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(10000); //chờ 10 giây để giải capcha
            var elements = driver.FindElements(By.XPath("//h4[text()='Nhập mã xác minh']"));

            Console.WriteLine("Test case 7 nhập SDT không có trong hệ thống ");
            if (elements.Count > 0)
            {
                Console.WriteLine("Kết quả: SDT chưa được đăng ký");
            }
            else
            {
                Console.WriteLine("Kết quả: Đăng nhập SDT thành công tiến hành nhập MK");
            }
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[@class='btn-action']")).Click();
            Thread.Sleep(3000);



            //Test case 8 nhập đúng SDT
            EnterPhoneNumber("0901647655");

            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//button[contains(text(), 'Tiếp Tục')]")).Click();

            Thread.Sleep(3000);
            elements = driver.FindElements(By.XPath("//h4[text()='Nhập mật khẩu']"));

            Console.WriteLine("Test case 8 nhập đúng SDT ");
            if (elements.Count > 0)
            {
                Console.WriteLine("Kết quả: Đăng nhập SDT thành công tiến hành nhập MK");
            }
            else
            {
                Console.WriteLine("Kết quả: SDT không đúng");
            }
            Thread.Sleep(2000);
        }

        private void EnterPhoneNumber(string phoneNumber)
        {
            var phone = driver.FindElement(By.Name("tel"));
            phone.Clear();
            phone.SendKeys(phoneNumber);
        }

        private void EnterPass(string passWord)
        {
            var pass = driver.FindElement(By.XPath("//input[@placeholder='Mật khẩu']"));
            pass.Clear();
            pass.SendKeys(passWord);
        }


        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            driver.Quit(); //đóng trình duyệt sau khi tất cả test hoàn thành
        }


    }
}
