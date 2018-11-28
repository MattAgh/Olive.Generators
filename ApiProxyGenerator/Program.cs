using Olive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OliveGenerator
{
    partial class Program
    {
        static int Main(string[] args)
        {
            if (!Initialize(args)) return -1;

            try
            {
                ParametersParser.LoadParameters();

                Console.WriteLine("Generating Client SDK proxy from...");
                Console.WriteLine("Publisher service: " + Context.PublisherService);
                Console.WriteLine("Api assembly: " + Context.AssemblyFile);
                Console.WriteLine("Api controller: " + Context.ControllerName);
                Console.WriteLine("Temp folder: " + Context.TempPath);
                Context.LoadAssembly();
                Context.PrepareOutputDirectory();
                DtoTypes.FindAll();
                DtoDataProviderClassGenerator.ValidateRemoteDataProviderAttributes();

                new List<ProjectCreator> { new ProxyProjectCreator() };

                var proxyCreator = new ProxyProjectCreator();
                proxyCreator.Build();
                new NugetCreator(proxyCreator).Create();

                if (DtoTypes.All.Any())
                {
                    var projectCreators = new[] { new MSharpProjectCreator(), new MSharp46ProjectCreator() };
                    projectCreators.AsParallel().Do(x => x.Build());
                    new NugetCreator(projectCreators).Create();
                }

                Console.WriteLine("Add done");
                return 0;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return -1;
            }
        }


    }
}