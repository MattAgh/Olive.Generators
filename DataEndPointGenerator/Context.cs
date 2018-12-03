using Olive;
using Olive.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OliveGenerator
{
    class Context
    {
        public static string PublisherService, EndPointName, NugetServer, NugetApiKey;
        public static FileInfo AssemblyFile;
        public static DirectoryInfo TempPath, Output, Source;
        public static Assembly AssemblyObject;
        public static Type EndPointNamespaceType;
        public static ExportDataAttribute[] EndPointCustomAttributes;
        public static List<ReplicatedDataType> ReplicatedDataList = new List<ReplicatedDataType>();

        internal static void PrepareOutputDirectory()
        {
            if (!TempPath.Exists)
                throw new Exception("Output directory not found: " + TempPath.FullName);

            try
            {
                if (TempPath.Exists)
                    TempPath.DeleteAsync(recursive: true, harshly: true).WaitAndThrow();
                TempPath.Create();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete the previous output directory " +
                    TempPath.FullName + Environment.NewLine + ex.Message);
            }
        }

        internal static string Run(string command)
        {
            var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            var result = cmd.StandardOutput.ReadToEnd().ToStringOrEmpty().Trim();

            if (result.StartsWith("Could not ")) throw new Exception(result);

            if (result.Contains("Build FAILED")) throw new Exception(result.TrimBefore("Build FAILED"));

            return result;
        }

        internal static void LoadAssembly()
        {
            AssemblyObject = Assembly.LoadFrom(AssemblyFile.ExistsOrThrow().FullName);

            EndPointNamespaceType = AssemblyObject.GetType(EndPointName);
            if (EndPointNamespaceType == null)
            {
                EndPointNamespaceType = AssemblyObject.GetTypes().FirstOrDefault(x => x.Name == EndPointName)
                  ?? throw new Exception($"No type in the assembly {AssemblyFile.FullName} is named: {EndPointName}.");
                if (EndPointNamespaceType != null)
                {
                    EndPointName = EndPointNamespaceType.FullName; // Ensure it has full namespace

                    EndPointCustomAttributes = (ExportDataAttribute[])EndPointNamespaceType.GetCustomAttributes(typeof(ExportDataAttribute), inherit: false);

                    if (EndPointCustomAttributes.ToList().Count == 0) throw new Exception("This endpoint has no attribute.");
                }
                else
                    throw new Exception(EndPointName + " was not found.");
            }
        }

        internal static void FindReplicatedDataClasses()
        {
            var replicatedDataChildClass = AssemblyObject.GetTypes().Where(x => x.BaseType.IsA(typeof(ReplicatedData)));
            if (replicatedDataChildClass == null) return;

            foreach (var childClass in replicatedDataChildClass)
            {
                var instanceClass = childClass.CreateInstance();
                var methodDefine = instanceClass.GetType().GetRuntimeMethods().Where(x => x.Name == "Define").FirstOrDefault();
                if (methodDefine == null) continue;

                methodDefine.Invoke(instanceClass, null);
                var replicatedDataObject = (ReplicatedData)instanceClass;
                if (replicatedDataObject == null) continue;

                ReplicatedDataList.Add(new ReplicatedDataType(childClass, replicatedDataObject));
            }
        }
    }
}