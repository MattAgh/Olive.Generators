namespace OliveGenerator
{
    internal class NugetPackage
    {
        internal string Package, Version;

        public NugetPackage(string nugetName, string nugetVersion)
        {
            Package = nugetName;
            Version = nugetVersion;
        }
    }
}
