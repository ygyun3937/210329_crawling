using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;

namespace _210329_crawling
{
    public partial class Form1 : Form
    {

        #region [Chrome Driver]
        private ChromeDriver _driver = null;
        #endregion
        #region [Chrome Driver 설정값]
        private ChromeDriverService _driverService = null;
        private ChromeOptions _options = null;
        #endregion

        List<string> Lsrc_Url = new List<string>();
        int current_index = 0; // 현재 배열 인덱스

        public Form1()
        {
            InitializeComponent();

            //선언
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;

            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");

        }


        private void btn_Login_Click(object sender, EventArgs e)
        {
            //Login
            string id = tb_ID.Text;
            string pw = tb_PW.Text;

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://www.naver.com");    // WebSite 접속
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10); //Time out

            //로그인버튼
            var element = _driver.FindElementByXPath("//*[@id='account']/a");//"//*[@id="account"]/a"

            element.Click();

            Thread.Sleep(3000);

            //로그인 아이디 창
            element = _driver.FindElementByXPath("//*[@id='id']");
            element.SendKeys(id);

            //로그인 비밀번호 창
            element = _driver.FindElementByXPath("//*[@id='pw']");
            element.SendKeys(pw);

            //로그인 로그인 버튼 
            element = _driver.FindElementByXPath("//*[@id='log.login']");
            element.Click();


        }


        private void btn_Search_Click(object sender, EventArgs e)
        {
            string strURL = "https://www.google.com/search?q=" + tb_FindText.Text + "&source=lnms&tbm=isch";

            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl(strURL); //웹사이트 접속
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            _driver.ExecuteScript("window.scrollBy(0, 10000)"); //창 띄우고 스크롤 진행

            Lsrc_Url = new List<string>();


            foreach (IWebElement item in _driver.FindElementsByClassName("rg_i"))
            {
                if(item.GetAttribute("src") != null)
                {
                    Lsrc_Url.Add(item.GetAttribute("src"));
                }
            }
            lbTotal.Text ="/ " + Lsrc_Url.Count.ToString();

            this.Invoke(new Action(delegate ()
            {
                try
                {
                    foreach (string strsrc in Lsrc_Url)
                    {
                        current_index++;

                        GetMapImage(Lsrc_Url[current_index]);
                        tboxNow.Text = current_index.ToString();
                        Refresh();
                        Thread.Sleep(50);
                    }
                }
                catch (Exception)
                {
                }
            }));
        }
        /// <summary>
        /// IMAGE URL 정규식 변환 후 PicutreBox에 IMAGE Loading
        /// </summary>
        /// <param name="base64String"></param>
        private void GetMapImage(string base64String)
        {
            try
            {
                var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;  // 정규식 검색
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    if (stream.Length == 0)
                    {
                        pBoxMain.Load(base64String);
                        tboxNow.Text = current_index.ToString();
                        tb_Url.Text = base64String;
                        
                    }
                    else
                    {
                        var image = Image.FromStream(stream);
                        pBoxMain.Image = image;
                        tb_Url.Text = base64String;
                    }
                }
            }
            catch
            {

            }
        }

        private void btnPre_Click(object sender, EventArgs e)
        {
            //thread 다운 방지     
            this.Invoke(new Action(delegate ()
            {
                current_index--;
                GetMapImage(Lsrc_Url[current_index]);
                tboxNow.Text = current_index.ToString();
            }
            ));
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //thread 다운 방지     
            this.Invoke(new Action(delegate ()
            {
                current_index = int.Parse(tboxNow.Text);

                GetMapImage(Lsrc_Url[current_index]);
                tboxNow.Text = current_index.ToString();
            }
            ));

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //thread 다운 방지     
            this.Invoke(new Action(delegate ()
            {
                current_index++;
                GetMapImage(Lsrc_Url[current_index]);
                tboxNow.Text = current_index.ToString();
            }
            ));

        }
    }
}
