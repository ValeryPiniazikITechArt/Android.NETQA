﻿using OpenQA.Selenium;
using OpenQA.Selenium.Appium.PageObjects;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using PlayMarket.TestFramework.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlayMarket.TestFramework.PageObjects
{
    public abstract class PageObject
    {
        List<IWebElement> WebElementsContainer = new List<IWebElement>();

        public PageObject()
        {
            PageFactory.InitElements(Driver.GetDriver(), this, new AppiumPageObjectMemberDecorator(new TimeOutDuration(TimeSpan.FromSeconds(5))));
            var bindingFlags = BindingFlags.Instance |
                   BindingFlags.NonPublic |
                   BindingFlags.Public;
            foreach (var field in GetType().GetFields(bindingFlags).Where(fieldType => fieldType.FieldType == typeof(IWebElement)))
            {
                if (!field.Name.Contains("Hidden"))
                {
                    WebElementsContainer.Add((IWebElement)field.GetValue(this));
                }
            }
        }

        bool PageIsLoaded()
        {
            bool result = true;
            foreach (var element in WebElementsContainer)
            {
                if (!element.Enabled)
                {
                    result = false;
                }
            }
            return result;
        }
        public void WaitPageLoading()
        {
            WebDriverWait wait = new WebDriverWait(Driver.GetDriver(), TimeSpan.FromSeconds(10));
            Func<IWebDriver, bool> waitForPage = new Func<IWebDriver, bool>((IWebDriver Web) =>
            {
                return PageIsLoaded();
            });
            wait.Until(waitForPage);
        }
    }
}
