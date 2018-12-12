using Olive;
using Olive.Entities;
using Olive.Entities.Replication;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OliveGenerator
{
    internal class MSharpModelProgrammer
    {
        ReplicatedData ReplicatedDataType;

        public MSharpModelProgrammer(ReplicatedData replicatedDataType) => ReplicatedDataType = replicatedDataType;

        internal string Generate()
        {
            var r = new StringBuilder();

            r.AppendLine("using MSharp;");
            r.AppendLine("namespace " + ReplicatedDataType.GetType().Namespace);
            r.AppendLine("{");
            r.AppendLine();
            r.AppendLine("public class " + ReplicatedDataType.GetType().Name + " : EntityType");
            r.AppendLine("{");
            r.AppendLine("public " + ReplicatedDataType.GetType().Name + "()");
            r.AppendLine("{");
            r.AppendLine($"Schema(\"{ReplicatedDataType.GetType().Namespace}\");");

            foreach (var item in ReplicatedDataType.Fields)
                r.AppendLine(AddProperty(item));

            r.AppendLine("}");
            r.AppendLine("}");
            r.AppendLine("}");

            return new CSharpFormatter(r.ToString()).Format();
        }

        string AddProperty(ExportedField item)
        {
            var extraArgs = "";
            var type = item.GetPropertyType();
            var name = item.GetName();
            if (type.IsArray) type = type.GetElementType();

            bool isNullable;
            if (isNullable = type.IsNullable())
                type = type.GetGenericArguments().Single();

            var method = type.Name.ToPascalCaseId();

            if (item.IsAssociation)
            {
                method = "Associate" + "<" + type.Name + ">";
                if (type.IsEnum) method = "String";
            }

            if (item is ExportedPropertyInfo p && p.IsInverseAssociation)
            {
                type = type.GetGenericArguments().Single();
                method = "InverseAssociate<" + type.Name + ">";
                extraArgs += ", inverseOf: \"" + p.Property.GetCustomAttribute<InverseOfAttribute>()?.Association + "\"";
            }

            switch (method)
            {
                case "Boolean": method = "Bool"; break;
                case "Int32": method = "Int"; break;
                case "Int64": method = "Decimal"; break;
                default: break;
            }

            var result = method + $"(\"{name}\"{extraArgs})";

            if (type.Assembly == Context.AssemblyObject && type.IsArray)
                result += ".MaxCardinality(null)";

            if (!isNullable && type.IsValueType)
            {
                result += ".Mandatory()";
            }

            return result + ";";
        }
    }
}