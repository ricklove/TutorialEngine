using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace TutorialEngine
//{
//    public interface ILesson
//    {
//        ILessonDocument Document { get; }
//    }
//}
//
//namespace TutorialEngine.LessonSyntaxTree
//{
//    public partial class Lesson : ILesson
//    {
//        ILessonDocument ILesson.Document { get { return Document; } }
//    }
//}

namespace TutorialEngine
{
    public static class LessonInterfacesCodeGenerator
    {
        public static string[] IgnoreList = new string[] { "StringWithIndex" };

        public static string GenerateInterfaces()
        {
            var rootType = typeof(TutorialEngine.LessonSyntaxTree.Lesson);

            var interfaceCodeParts = new List<string>();
            var implementationCodeParts = new List<string>();

            AddInterfaceForSelfAndChildrenProperties(interfaceCodeParts, implementationCodeParts, new HashSet<Type>(), rootType);

            // TODO: Create the partial classes that provide the explicit implementations of each interface
            // TODO: Mark the classes as partial

            var interfaceCode = interfaceCodeParts.Aggregate(new StringBuilder(), (sb, s) => sb.Append(s)).ToString();
            var implementationCode = implementationCodeParts.Aggregate(new StringBuilder(), (sb, s) => sb.Append(s)).ToString();

            // Wrap 
            var interfaceNamespace = typeof(LessonInterfacesCodeGenerator).Namespace;
            var implementationNamespace = typeof(LessonSyntaxTree.Lesson).Namespace;
            interfaceCode = "using System.Collections.Generic;\r\nusing System.Linq;\r\n\r\nnamespace " + interfaceNamespace + "\r\n{\r\n" + interfaceCode + "}\r\n";
            implementationCode = "\r\n\r\nnamespace " + implementationNamespace + "\r\n{\r\n" + implementationCode + "}\r\n";

            var code = interfaceCode + implementationCode;

            return code;
        }

        private static bool ShouldIgnore(Type type)
        {
            return type.Assembly != typeof(LessonInterfacesCodeGenerator).Assembly
                || type.IsAbstract
                || IgnoreList.Contains(type.Name);
        }

        private static void AddInterfaceForSelfAndChildrenProperties(
            List<string> interfaceCodeParts, List<string> implementationCodeParts, HashSet<Type> visitHistory, Type type)
        {
            if (ShouldIgnore(type)) { return; }

            if (visitHistory.Contains(type)) { return; }
            visitHistory.Add(type);

            var sb = new StringBuilder();
            var sbImp = new StringBuilder();

            // Add interface for the type
            var implements = "";

            if (!ShouldIgnore(type.BaseType))
            {
                implements = " : I" + type.BaseType.Name;
            }

            sb.AppendFormat("public interface I{0}{1}\r\n{{\r\n", type.Name, implements);
            sbImp.AppendFormat("public partial class {0} : I{0}\r\n{{\r\n", type.Name);

            // Add Text property for spans
            if (typeof(LessonSyntaxTree.LessonSpan).IsAssignableFrom(type))
            {
                sb.AppendFormat("\tstring Text {{ get; }}\r\n");
                sbImp.AppendFormat("\tstring I{0}.Text {{ get {{ return {1}; }} }}\r\n", type.Name, "Content.Text");
            }

            // Add Properties
            foreach (var prop in type.GetProperties())
            {
                var pType = prop.PropertyType;

                if (!ShouldIgnore(prop.PropertyType))
                {
                    sb.AppendFormat("\tI{0} {1} {{ get; }}\r\n", prop.PropertyType.Name, prop.Name);
                    sbImp.AppendFormat("\tI{0} I{2}.{1} {{ get {{ return {3}; }} }}\r\n", prop.PropertyType.Name, prop.Name, type.Name, prop.Name);
                }

                if (typeof(System.Collections.IList).IsAssignableFrom(pType)
                    && pType.IsGenericType)
                {
                    var genericTypeArg = pType.GenericTypeArguments[0];
                    if (!ShouldIgnore(genericTypeArg))
                    {
                        sb.AppendFormat("\tIList<I{0}> {1} {{ get; }}\r\n", genericTypeArg.Name, prop.Name);
                        sbImp.AppendFormat("\tIList<I{0}> I{2}.{1} {{ get {{ return {3}.Cast<I{0}>().ToList(); }} }}\r\n", genericTypeArg.Name, prop.Name, type.Name, prop.Name);
                    }
                }
            }

            sb.AppendFormat("}}\r\n\r\n");
            sbImp.AppendFormat("}}\r\n\r\n");

            interfaceCodeParts.Add(sb.ToString());
            implementationCodeParts.Add(sbImp.ToString());

            // Add children
            foreach (var prop in type.GetProperties())
            {
                var pType = prop.PropertyType;

                AddInterfaceForSelfAndChildrenProperties(interfaceCodeParts, implementationCodeParts, visitHistory, prop.PropertyType);

                if (typeof(System.Collections.IList).IsAssignableFrom(pType)
                    && pType.IsGenericType)
                {
                    var genericTypeArg = pType.GenericTypeArguments[0];
                    AddInterfaceForSelfAndChildrenProperties(interfaceCodeParts, implementationCodeParts, visitHistory, genericTypeArg);
                }
            }
        }
    }

    interface LessonInterfacesGenerator
    {
    }
}
