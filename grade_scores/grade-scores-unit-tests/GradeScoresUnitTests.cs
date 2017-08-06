using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using grade_scores;

namespace grade_scores_unit_tests
{
    [TestClass]
    public class GradeScoresUnitTests
    {
        [TestMethod]
        [DeploymentItem("test1.txt")]
        public void TestReadFile1()
        {
            GradeTextFile gtf = new GradeTextFile();
            try
            {
                gtf.ReadRecordsFromTextFile(@"test1.txt");
            }
            catch (IOException ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }
            Assert.AreEqual(4, gtf.RecordsCount, "Records were not properly read");
        }

        [TestMethod]
        [DeploymentItem("test1.txt")]
        [DeploymentItem("test1-result.txt")]
        public void TestWriteFile1()
        {
            GradeTextFile gtf = new GradeTextFile();
            string input_file_name = @"test1.txt";
            try
            {
                gtf.ReadRecordsFromTextFile(input_file_name);
            }
            catch (IOException ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }

            gtf.VerifyAndSortRecords();
            string[] fn = input_file_name.Split('.');
            StringBuilder sb = new StringBuilder();
            string out_file_name = (sb.AppendFormat("{0}-graded.{1}", fn[0], fn[1])).ToString();
            gtf.WriteRecordsToTextFile(out_file_name);

            string actualResult = System.IO.File.ReadAllText(out_file_name);
            string expectedResult = System.IO.File.ReadAllText(@"test1-result.txt");

            string actualResult_t = Regex.Replace(actualResult, @"\s+", "");
            string expectedResult_t = Regex.Replace(expectedResult, @"\s+", "");

            Assert.AreEqual(expectedResult_t, actualResult_t, "Unexpected test result");
        }

        [TestMethod]
        [DeploymentItem("test2.txt")]
        public void TestReadFile2()
        {
            GradeTextFile gtf = new GradeTextFile();
            try
            {
                gtf.ReadRecordsFromTextFile(@"test2.txt");
                gtf.VerifyAndSortRecords();
                Assert.Fail("Invalid input format was not checked");
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }
        }

        [TestMethod]
        [DeploymentItem("test3.txt")]
        public void TestReadFile3()
        {
            GradeTextFile gtf = new GradeTextFile();
            try
            {
                gtf.ReadRecordsFromTextFile(@"test3.txt");
                gtf.VerifyAndSortRecords();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }
            Assert.AreEqual(4, gtf.RecordsCount, "Records were not properly read");
        }

        [TestMethod]
        [DeploymentItem("test4.txt")]
        [DeploymentItem("test4-result.txt")]
        public void TestWriteFile4()
        {
            GradeTextFile gtf = new GradeTextFile();
            string input_file_name = @"test4.txt";
            try
            {
                gtf.ReadRecordsFromTextFile(input_file_name);
            }
            catch (IOException ex)
            {
                Console.Out.WriteLine("Error reading file" + ex.Message);
            }

            gtf.VerifyAndSortRecords();
            string[] fn = input_file_name.Split('.');
            StringBuilder sb = new StringBuilder();
            string out_file_name = (sb.AppendFormat("{0}-graded.{1}", fn[0], fn[1])).ToString();
            gtf.WriteRecordsToTextFile(out_file_name);

            string actualResult = System.IO.File.ReadAllText(out_file_name);
            string expectedResult = System.IO.File.ReadAllText(@"test4-result.txt");

            string actualResult_t = Regex.Replace(actualResult, @"\s+", "");
            string expectedResult_t = Regex.Replace(expectedResult, @"\s+", "");

            Assert.AreEqual(expectedResult_t, actualResult_t, "Unexpected test result");
        }
    }
}
