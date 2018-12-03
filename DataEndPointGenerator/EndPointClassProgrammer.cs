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
            r.AppendLine();

            r.AppendLine($"public class {ClassName} : DataReplicationEndPointConsumer");
            r.AppendLine("{");
            foreach (var item in Context.EndPointCustomAttributes)
            {
                r.AppendLine($"static Type {item.Type.Name}Type => Type.GetType(\"{item.Type.FullName}\");");
            }

            r.AppendLine();
            for (int i = 0; i < Context.EndPointCustomAttributes.Length; i++)
            {
                var item = Context.EndPointCustomAttributes[i];
                r.AppendLine($"/// <summary> Clears all messages from the queue of {item.Type.Name.ToLower()} data. It will then");
                r.AppendLine($"/// fetch the current data directly from the {item.Type.FullName}. </summary>");
                if (i == 0)
                    r.AppendLine($"public static Task Refresh{item.Type.Name}Data() =>  RefreshData({item.Type.Name}Type);");
                else
                    r.AppendLine($"public static async Task Refresh{item.Type.Name}Data() =>  RefreshData({item.Type.Name}Type);");
                r.AppendLine();
            }

            r.AppendLine($"/// <summary> It will start listening to queue messages to keep the local database up to date");
            r.AppendLine($"/// with the changes in the {Context.PublisherService}. But before it starts that, if the local table");
            r.AppendLine($"/// is empty, it will refresh the full data. </summary>");

            r.AppendLine($"public static async Task Subscribe()");
            r.AppendLine("{");
            foreach (var item in Context.EndPointCustomAttributes)
                r.AppendLine($"await Subscribe({item.Type.Name}Type);");


            r.AppendLine("}");
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