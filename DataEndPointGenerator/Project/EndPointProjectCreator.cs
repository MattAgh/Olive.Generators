using Olive;
using System;
using System.Collections.Generic;

namespace OliveGenerator
{
    class EndPointProjectCreator : ProjectCreator
    {
        public EndPointProjectCreator() : base("EndPoint") { }

        protected override string Framework => "netstandard2.0";

        [EscapeGCop]
        internal override string IconUrl
            => "https://raw.githubusercontent.com/Geeksltd/Olive/master/Integration/Olive.DataEndPointGenerator/EndPointIcon.png";

        protected override string[] References
            => new[] { "Olive", "Olive.Entities", "Olive.Entities.Data.Replication" };

        protected override void AddFiles()
        {
            Console.Write("Adding the endpoint class...");
            Folder.GetFile($"{Context.EndPointName}.cs").WriteAllText(EndPointClassProgrammer.Generate());
            Console.WriteLine("Done");
            // Console.Write("Adding the endpoint class mock configuration...");
            // MockFolder.GetFile($"{Context.ControllerName}.Mock.cs").WriteAllText(EndPointClassProgrammer.GenerateMock());
            Console.WriteLine("Done");
            Console.Write("Adding ReamMe.txt file ...");
            Folder.GetFile("README.txt").WriteAllText(ReadmeFileGenerator.Generate());
            Console.WriteLine("Done");
        }

        public override IEnumerable<string> GetTargetFiles()
        {
            var readme = Folder.GetFile("README.txt").FullName;
            return base.GetTargetFiles().Concat($@"<file src=""{readme}"" target="""" />");
        }

        internal override IEnumerable<string> GetNugetDependencies()
        {
            return new[]
            {
                "Olive",
                "Olive.Entities",
                //"Olive.Microservices",
                "Olive.Entities.Data.Replication",
            };
        }
    }
}