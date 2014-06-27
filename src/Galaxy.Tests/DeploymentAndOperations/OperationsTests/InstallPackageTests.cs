using Codestellation.Galaxy.Domain;
using Codestellation.Galaxy.ServiceManager.Operations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Codestellation.Galaxy.Tests.DeploymentAndOperations.OperationsTests
{
    [TestFixture]
    public class InstallPackageTests
    {
        string output;
        string targetPath;

        void ExtractEmbeddedResource(string outputDir, string resourceLocation, IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceLocation + @"." + file))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine(outputDir, file), System.IO.FileMode.Create))
                    {
                        stream.CopyTo(fileStream);
                        stream.Flush();
                        fileStream.Close();
                    }
                }
            }
        }

        [SetUp]
        public void Init()
        {
            
            output = Path.Combine(Environment.CurrentDirectory, "testnuget");

            if(!Directory.Exists(output))
                Directory.CreateDirectory(output);

            ExtractEmbeddedResource(output, "Codestellation.Galaxy.Tests.Resources", new string[] { "TestNugetPackage.1.0.0" });

            File.Move(
                Path.Combine(output, "TestNugetPackage.1.0.0"),
                Path.Combine(output, "TestNugetPackage.1.0.0.nupkg"));

            targetPath = Path.Combine(output, "extracted");

        }

        [Test]
        public void InstallPackage_extract_package_success()
        {
            InstallPackage op = new InstallPackage(
                targetPath,
                new Deployment() { DisplayName = "testdeployment"},
                new NugetFeed()
                {
                    Name = "TestNugetPackage",
                    Uri = output
                });

            op.Execute();

            var dllFiles = Directory.GetFiles(targetPath, "*.dll", SearchOption.AllDirectories);

            var filesCheck = dllFiles.FirstOrDefault(item => item.Contains("TestNugetPackLib.dll")) != null;

            Assert.IsTrue(filesCheck);
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(output, true);
        }
    }
}
