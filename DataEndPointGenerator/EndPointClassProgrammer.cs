using System;
using System.Text;

namespace OliveGenerator
{
    class EndPointClassProgrammer
    {
        static Type EndPoint => Context.EndPointNamespaceType;
        static string ClassName => EndPoint.Name;

        public static string Generate()
        {
            if (Context.EndPointCustomAttributes.Length <= 0)
                return "";
            var r = new StringBuilder();

            r.AppendLine("namespace " + EndPoint.Namespace);
            r.AppendLine("{");
            r.AppendLine("using System;");
            r.AppendLine("using System.Threading.Tasks;");
            r.AppendLine("using Olive.Entities;");
            r.AppendLine("using Olive.Entities.Replication;");
            r.AppendLine();

            r.AppendLine($"public class {ClassName} : DestinationEndpoint");
            r.AppendLine("{");

            r.AppendLine($"public override string QueueUrl => Olive.Config.GetOrThrow(\"DataReplication:{EndPoint.FullName}:Url\");");
            r.AppendLine();

            foreach (var item in Context.EndPointCustomAttributes)
                r.AppendLine($"public static EndpointSubscriber {item.Type.Name} {{ get; private set; }}");
            r.AppendLine();

            r.AppendLine($"public {ClassName}(System.Reflection.Assembly domainAssembly) : base(domainAssembly)");
            r.AppendLine("{");
            foreach (var item in Context.EndPointCustomAttributes)
                r.AppendLine($"    {item.Type.Name} = Register(\"{item.Type.FullName}\");");
            r.AppendLine("}");
            r.AppendLine();

            r.AppendLine("}");
            r.AppendLine("}");

            return new CSharpFormatter(r.ToString()).Format();
        }

        public static string GenerateMock()
        {
            var r = new StringBuilder();

            r.AppendLine("namespace " + EndPoint.Namespace);
            r.AppendLine("{");
            r.AppendLine("using System;");
            r.AppendLine("using System.Threading.Tasks;");
            r.AppendLine("using System.Collections.Generic;");
            r.AppendLine("using Olive;");
            r.AppendLine("using Mock;");
            r.AppendLine();
            r.Append("/// <summary>This will allow api users to set mock data for the api");
            r.AppendLine("</summary>");
            r.AppendLine($"public partial class {ClassName}");
            r.AppendLine("{");
            // Adding mock configuration field
            r.AppendLine($"static {ClassName}MockConfiguration MockConfig = new {ClassName}MockConfiguration();");
            r.AppendLine();
            // Method Mock starts here
            r.Append($"/// <summary>set the mock configuration for {ClassName}");
            r.AppendLine("</summary>");
            r.AppendLine($"public static void Mock(Action<{ClassName}MockConfiguration> mockConfiguration, bool enabled = true)");
            r.AppendLine("{");
            r.AppendLine($"MockConfig.Enabled = Config.Get(GetMockConfigKey(\"{Context.PublisherService}\")).Or(enabled.ToString()).To<bool>();");
            r.AppendLine("mockConfiguration(MockConfig);");
            // Method Mock ends here
            r.AppendLine("}");
            r.AppendLine("private static string GetMockConfigKey(string serviceName) => $\"Microservice:{ serviceName}:Mock\";");
            // class defination ends here
            r.AppendLine("}");

            // Namespace defination ends here
            r.AppendLine("}");

            return new CSharpFormatter(r.ToString()).Format();
        }
    }
}