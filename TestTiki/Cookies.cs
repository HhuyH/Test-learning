using System;
using System.IO;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class SaveCookies
{
    public static void SaveCookiesToFile(IWebDriver driver, string filePath)
    {
        var cookies = driver.Manage().Cookies.AllCookies;
        using (StreamWriter file = new StreamWriter(filePath))
        {
            foreach (var cookie in cookies)
            {
                file.WriteLine($"{cookie.Name};{cookie.Value};{cookie.Domain};{cookie.Path};{cookie.Expiry?.ToString() ?? "null"};{cookie.Secure}");
            }
        }
    }


    public static void LoadCookiesFromFile(IWebDriver driver, string filePath)
    {
        if (File.Exists(filePath))
        {
            string[] cookies = File.ReadAllLines(filePath);
            foreach (var line in cookies)
            {
                var parts = line.Split(';');
                string name = parts[0];
                string value = parts[1];
                string domain = parts[2];
                string path = parts[3];
                DateTime? expiry = parts[4] != "null" ? DateTime.Parse(parts[4]) : (DateTime?)null;
                bool isSecure = bool.Parse(parts[5]);

                Cookie cookie = new Cookie(name, value, domain, path, expiry, isSecure, false,"None");
                driver.Manage().Cookies.AddCookie(cookie);
            }
        }
    }

}
