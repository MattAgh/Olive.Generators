using Olive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OliveGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!ParametersParser.Start(args)) return -1;
            try
            {
                ParametersParser.LoadParameters();

                Console.WriteLine("Generating Data EndPoint from...");
                Console.WriteLine("Publisher service: " + Context.PublisherService);
                Console.WriteLine("Assembly: " + Context.AssemblyFile);
                Console.WriteLine("EndPoint: " + Context.EndPointName);
                Console.WriteLine("Temp folder: " + Context.TempPath);

                Context.LoadAssembly();
                Context.PrepareOutputDirectory();

                new List<ProjectCreator> { new EndPointProjectCreator() };

                var endPointCreator = new EndPointProjectCreator();
                endPointCreator.Build();
                new NugetCreator(endPointCreator).Create();

                Context.FindReplicatedDataClasses();

                if (Context.ReplicatedDataList.Any())
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
                Helper.ShowError(ex);
                return -1;
            }
        }
    }
}