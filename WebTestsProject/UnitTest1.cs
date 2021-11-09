using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Kit;
using System.Text;
using System;
using SOEWeb.Shared.Secrets;

namespace WebTestsProject
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            SOEWeb.Shared.WebData.ConnectionString = DotNetEnviroment.AWSCONNECTIONSTRING;
        }
        [TestMethod]
        public void TestMethod1()
        {
        }

        private string GetHTML(string fileName)
        {
            using (var reflection = Kit.ReflectionCaller.FromThis(this))
            {
                string name = reflection.FindResources(x => x.Contains(fileName)).First();
                return ReflectionCaller.ToText(reflection.GetResource(fileName));
            }
            throw new System.Exception("NOT FOUND");
        }

        [TestMethod]
        public void TestSingleGroupClassDigester()
        {
            string html = GetHTML("ctl00_mainCopy_GV_Horario.html");
            var response = SOEWeb.Shared.Processors.ClassTimeDigester.Digest(Encoding.UTF8.GetBytes(html), "2015130425", null);
            if (response.ResponseResult == Kit.Services.Web.APIResponseResult.OK)
            {
                Console.WriteLine("OK");
                return;
            }
            throw new Exception();
        }
        [TestMethod]
        public void TestSingleGradesDigester()
        {
            string html = GetHTML("ctl00_mainCopy_GV_Calif.html");
            var response = SOEWeb.Shared.Processors.GradesDigester.Digest(Encoding.UTF8.GetBytes(html), "2015130425", null);
            if (response.ResponseResult == Kit.Services.Web.APIResponseResult.OK)
            {
                Console.WriteLine("OK");
                return;
            }
            throw new Exception();
        }
    }
}