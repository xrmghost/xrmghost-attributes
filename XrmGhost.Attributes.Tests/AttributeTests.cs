using System;
using System.Linq;
using Xunit;
using XrmGhost.Attributes;

namespace XrmGhost.Attributes.Tests
{
    public class HandlesMessageAttributeTests
    {
        [Fact]
        public void Constructor_SetsMessageName()
        {
            var attr = new HandlesMessageAttribute("Create");
            Assert.Equal("Create", attr.MessageName);
        }

        [Fact]
        public void Constructor_ThrowsOnNullOrWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new HandlesMessageAttribute("  "));
        }
    }

    public class InputParameterAttributeTests
    {
        [Fact]
        public void Constructor_SetsNameAndValueJson()
        {
            var attr = new InputParameterAttribute("Target", "{\"id\":\"123\"}");
            Assert.Equal("Target", attr.Name);
            Assert.Equal("{\"id\":\"123\"}", attr.ValueJson);
        }
    }

    public class OutputParameterAttributeTests
    {
        [Fact]
        public void Constructor_SetsNameAndExpectedValueJson()
        {
            var attr = new OutputParameterAttribute("Result", "true");
            Assert.Equal("Result", attr.Name);
            Assert.Equal("true", attr.ExpectedValueJson);
        }
    }

    public class PluginExecutionConfigAttributeTests
    {
        [Fact]
        public void Constructor_SetsPrimaryEntityNameAndMessages()
        {
            var attr = new PluginExecutionConfigAttribute("account", "Create", "Update");
            Assert.Equal("account", attr.PrimaryEntityName);
            Assert.Equal(new[] { "Create", "Update" }, attr.Messages);
        }

        [Fact]
        public void Constructor_DefaultsAreUnassigned()
        {
            var attr = new PluginExecutionConfigAttribute("contact");
            Assert.Equal(-1, attr.Stage);
            Assert.Equal(-1, attr.Mode);
            Assert.Null(attr.ImpersonatingUserId);
        }

        [Fact]
        public void FilteringAttributes_DefaultsToNull()
        {
            var attr = new PluginExecutionConfigAttribute("account", "Update");
            Assert.Null(attr.FilteringAttributes);
        }

        [Fact]
        public void FilteringAttributes_AcceptsEmptyArray()
        {
            var attr = new PluginExecutionConfigAttribute("account", "Update")
            {
                FilteringAttributes = new string[0]
            };

            Assert.NotNull(attr.FilteringAttributes);
            Assert.Empty(attr.FilteringAttributes!);
        }

        [Fact]
        public void FilteringAttributes_PreservesSourceOrderAndCasing()
        {
            var attr = new PluginExecutionConfigAttribute("account", "Update")
            {
                FilteringAttributes = new[] { "Revenue", "name", "AccountNumber" }
            };

            // The attribute is a passive carrier: it must not sort, lowercase, or
            // de-duplicate. Canonicalization is the downstream consumer's job.
            Assert.Equal(new[] { "Revenue", "name", "AccountNumber" }, attr.FilteringAttributes);
        }

        [Fact]
        public void FilteringAttributes_PreservesDuplicatesVerbatim()
        {
            var attr = new PluginExecutionConfigAttribute("account", "Update")
            {
                FilteringAttributes = new[] { "name", "name" }
            };

            Assert.Equal(new[] { "name", "name" }, attr.FilteringAttributes);
        }

        [Fact]
        public void FilteringAttributes_SetOnMixedMessageDeclaration_IsStoredAsDeclared()
        {
            // Filtering attributes are Update-only semantically, but the attribute itself
            // performs no message-dependent validation. Downstream registrars are the ones
            // that must ignore them for non-Update messages.
            var attr = new PluginExecutionConfigAttribute("account", "Create", "Update")
            {
                FilteringAttributes = new[] { "name" }
            };

            Assert.Equal(new[] { "Create", "Update" }, attr.Messages);
            Assert.Equal(new[] { "name" }, attr.FilteringAttributes);
        }
    }

    /// <summary>
    /// Reflection-level tests: these assert the shape actually baked into assembly metadata,
    /// which is what downstream reflection/Cecil consumers read.
    /// </summary>
    public class PluginExecutionConfigFilteringAttributesMetadataTests
    {
        [PluginExecutionConfig("account", "Update")]
        private sealed class NoFilteringPlugin { }

        [PluginExecutionConfig("account", "Update", FilteringAttributes = new string[0])]
        private sealed class EmptyFilteringPlugin { }

        [PluginExecutionConfig("account", "Update", FilteringAttributes = new[] { "Revenue", "name", "AccountNumber" })]
        private sealed class OrderedFilteringPlugin { }

        [PluginExecutionConfig("account", "Create")]
        [PluginExecutionConfig("account", "Update", FilteringAttributes = new[] { "name", "revenue" })]
        private sealed class MixedMessagePlugin { }

        private static PluginExecutionConfigAttribute[] ConfigsOf<T>()
        {
            return (PluginExecutionConfigAttribute[])typeof(T)
                .GetCustomAttributes(typeof(PluginExecutionConfigAttribute), false);
        }

        [Fact]
        public void UndeclaredFilteringAttributes_ReadBackAsNull()
        {
            var config = Assert.Single(ConfigsOf<NoFilteringPlugin>());
            Assert.Null(config.FilteringAttributes);
        }

        [Fact]
        public void EmptyFilteringAttributes_ReadBackAsEmptyArray()
        {
            var config = Assert.Single(ConfigsOf<EmptyFilteringPlugin>());
            Assert.NotNull(config.FilteringAttributes);
            Assert.Empty(config.FilteringAttributes!);
        }

        [Fact]
        public void DeclaredFilteringAttributes_PreserveOrderAndCasing()
        {
            var config = Assert.Single(ConfigsOf<OrderedFilteringPlugin>());
            Assert.Equal(new[] { "Revenue", "name", "AccountNumber" }, config.FilteringAttributes);
        }

        [Fact]
        public void MixedMessageDeclarations_KeepFilteringAttributesPerConfiguration()
        {
            var configs = ConfigsOf<MixedMessagePlugin>();
            Assert.Equal(2, configs.Length);

            var createConfig = Assert.Single(configs, c => c.Messages != null && Array.IndexOf(c.Messages, "Create") >= 0);
            var updateConfig = Assert.Single(configs, c => c.Messages != null && Array.IndexOf(c.Messages, "Update") >= 0);

            Assert.Null(createConfig.FilteringAttributes);
            Assert.Equal(new[] { "name", "revenue" }, updateConfig.FilteringAttributes);
        }
    }

    /// <summary>
    /// Host compatibility: xrmghost-framework-host (PluginRunnerService.GetPluginMetadataInternal)
    /// reads metadata via GetCustomAttributes(false), selects by literal FullName and reads
    /// properties by name. A renamed type or property is silently treated as "not declared" there,
    /// so these tests use the same strings instead of strong types. The attributes are passive
    /// carriers: JSON and FilteringAttributes must read back exactly as declared.
    /// </summary>
    public class HostReflectionCompatibilityTests
    {
        private const string PluginExecutionConfigName = "XrmGhost.Attributes.PluginExecutionConfigAttribute";
        private const string PreImageName = "XrmGhost.Attributes.PreImageAttribute";
        private const string PostImageName = "XrmGhost.Attributes.PostImageAttribute";
        private const string InputParameterName = "XrmGhost.Attributes.InputParameterAttribute";
        private const string SharedVariableName = "XrmGhost.Attributes.SharedVariableAttribute";
        private const string OutputParameterName = "XrmGhost.Attributes.OutputParameterAttribute";

        private const string AccountJson = "{\"__entityName\":\"account\",\"accountid\":\"6f1c2a3b-0000-0000-0000-000000000001\",\"name\":\"Contoso\",\"revenue\":1000.5}";
        private const string ContactJson = "{ \"__entityName\" : \"contact\", \"FirstName\" : \"Ada\",  \"lastname\":\"Lovelace\" }";
        private const string EntityReferenceJson = "{\"__entityName\":\"account\",\"Id\":\"6F1C2A3B-0000-0000-0000-000000000002\",\"LogicalName\":\"account\"}";

        [PluginExecutionConfig("account", "Update")]
        private sealed class NoFilteringPlugin { }

        [PluginExecutionConfig("account", "Update", FilteringAttributes = new string[0])]
        private sealed class EmptyFilteringPlugin { }

        [PluginExecutionConfig("account", "Update", FilteringAttributes = new[] { "Revenue", "name", "AccountNumber" })]
        private sealed class OrderedFilteringPlugin { }

        [PluginExecutionConfig("account", "Update", FilteringAttributes = new[] { "name", "Name", "name" })]
        private sealed class DuplicateFilteringPlugin { }

        [PluginExecutionConfig("account", "Create")]
        [PluginExecutionConfig("contact", "Update", FilteringAttributes = new[] { "lastname", "FirstName" })]
        private sealed class MixedMessagePlugin { }

        [PreImage("PreImage", AccountJson, Attributes = new[] { "Revenue", "name", "name" })]
        [PreImage("pre_no_columns", "{}")]
        private sealed class PreImagePlugin { }

        [PostImage("PostImage", ContactJson, Attributes = new[] { "lastname", "FirstName" })]
        [PostImage("post_empty_columns", AccountJson, Attributes = new string[0])]
        private sealed class PostImagePlugin { }

        [InputParameter("Target", AccountJson)]
        [InputParameter("Count", "42")]
        private sealed class InputParameterPlugin { }

        [SharedVariable("<nome>", "<json entita' con __entityName>")]
        private sealed class SharedVariablePlaceholderPlugin { }

        [SharedVariable("parentAccount", AccountJson)]
        private sealed class SharedVariableEntityPlugin { }

        [OutputParameter("id", "\"6f1c2a3b-0000-0000-0000-000000000001\"")]
        [OutputParameter("Result", EntityReferenceJson)]
        private sealed class OutputParameterPlugin { }

        private static object[] AttributesNamed(Type fixture, string fullName)
        {
            return fixture.GetCustomAttributes(false)
                .Where(a => a.GetType().FullName == fullName)
                .ToArray();
        }

        private static object? Read(object attr, string propertyName)
        {
            var property = attr.GetType().GetProperty(propertyName);
            Assert.NotNull(property);
            return property!.GetValue(attr);
        }

        private static string? ReadString(object attr, string propertyName)
        {
            var value = Read(attr, propertyName);
            if (value != null)
            {
                Assert.IsType<string>(value);
            }
            return (string?)value;
        }

        private static string[]? ReadStringArray(object attr, string propertyName)
        {
            var property = attr.GetType().GetProperty(propertyName);
            Assert.NotNull(property);
            Assert.Equal(typeof(string[]), property!.PropertyType);
            return (string[]?)property.GetValue(attr);
        }

        private static object SingleBy(object[] attrs, string propertyName, string expected)
        {
            return Assert.Single(attrs, a => ReadString(a, propertyName) == expected);
        }

        [Fact]
        public void FullNames_MatchTheLiteralsReadByTheHost()
        {
            Assert.Equal(PluginExecutionConfigName, typeof(PluginExecutionConfigAttribute).FullName);
            Assert.Equal(PreImageName, typeof(PreImageAttribute).FullName);
            Assert.Equal(PostImageName, typeof(PostImageAttribute).FullName);
            Assert.Equal(InputParameterName, typeof(InputParameterAttribute).FullName);
            Assert.Equal(SharedVariableName, typeof(SharedVariableAttribute).FullName);
            Assert.Equal(OutputParameterName, typeof(OutputParameterAttribute).FullName);
        }

        [Fact]
        public void PluginExecutionConfig_UndeclaredFilteringAttributes_ReadBackAsNull()
        {
            var config = Assert.Single(AttributesNamed(typeof(NoFilteringPlugin), PluginExecutionConfigName));
            Assert.Equal("account", ReadString(config, "PrimaryEntityName"));
            Assert.Equal(new[] { "Update" }, (string[]?)Read(config, "Messages"));
            Assert.Null(ReadStringArray(config, "FilteringAttributes"));
        }

        [Fact]
        public void PluginExecutionConfig_EmptyFilteringAttributes_ReadBackAsEmptyArray()
        {
            var config = Assert.Single(AttributesNamed(typeof(EmptyFilteringPlugin), PluginExecutionConfigName));
            Assert.Equal("account", ReadString(config, "PrimaryEntityName"));
            var filtering = ReadStringArray(config, "FilteringAttributes");
            Assert.NotNull(filtering);
            Assert.Empty(filtering!);
        }

        [Fact]
        public void PluginExecutionConfig_FilteringAttributes_KeepDeclaredOrderAndCasing()
        {
            var config = Assert.Single(AttributesNamed(typeof(OrderedFilteringPlugin), PluginExecutionConfigName));
            Assert.Equal("account", ReadString(config, "PrimaryEntityName"));
            Assert.Equal(new[] { "Revenue", "name", "AccountNumber" }, ReadStringArray(config, "FilteringAttributes"));
        }

        [Fact]
        public void PluginExecutionConfig_FilteringAttributes_AreNotNormalizedSortedLoweredOrDeduplicated()
        {
            var config = Assert.Single(AttributesNamed(typeof(DuplicateFilteringPlugin), PluginExecutionConfigName));
            Assert.Equal(new[] { "name", "Name", "name" }, ReadStringArray(config, "FilteringAttributes"));
        }

        [Fact]
        public void PluginExecutionConfig_MixedMessages_KeepFilteringAttributesPerDeclaration()
        {
            var configs = AttributesNamed(typeof(MixedMessagePlugin), PluginExecutionConfigName);
            Assert.Equal(2, configs.Length);

            var create = SingleBy(configs, "PrimaryEntityName", "account");
            var update = SingleBy(configs, "PrimaryEntityName", "contact");

            Assert.Equal(new[] { "Create" }, (string[]?)Read(create, "Messages"));
            Assert.Null(ReadStringArray(create, "FilteringAttributes"));
            Assert.Equal(new[] { "Update" }, (string[]?)Read(update, "Messages"));
            Assert.Equal(new[] { "lastname", "FirstName" }, ReadStringArray(update, "FilteringAttributes"));
        }

        [Fact]
        public void PreImage_ReadsBackNameEntityJsonAndAttributesVerbatim()
        {
            var images = AttributesNamed(typeof(PreImagePlugin), PreImageName);
            Assert.Equal(2, images.Length);

            var declared = SingleBy(images, "Name", "PreImage");
            Assert.Equal(AccountJson, ReadString(declared, "EntityJson"));
            Assert.Equal(new[] { "Revenue", "name", "name" }, ReadStringArray(declared, "Attributes"));

            var bare = SingleBy(images, "Name", "pre_no_columns");
            Assert.Equal("{}", ReadString(bare, "EntityJson"));
            Assert.Null(ReadStringArray(bare, "Attributes"));
        }

        [Fact]
        public void PostImage_ReadsBackNameEntityJsonAndAttributesVerbatim()
        {
            var images = AttributesNamed(typeof(PostImagePlugin), PostImageName);
            Assert.Equal(2, images.Length);

            var declared = SingleBy(images, "Name", "PostImage");
            Assert.Equal(ContactJson, ReadString(declared, "EntityJson"));
            Assert.Equal(new[] { "lastname", "FirstName" }, ReadStringArray(declared, "Attributes"));

            var empty = SingleBy(images, "Name", "post_empty_columns");
            Assert.Equal(AccountJson, ReadString(empty, "EntityJson"));
            var columns = ReadStringArray(empty, "Attributes");
            Assert.NotNull(columns);
            Assert.Empty(columns!);
        }

        [Fact]
        public void InputParameter_ReadsBackValueJsonVerbatim()
        {
            var parameters = AttributesNamed(typeof(InputParameterPlugin), InputParameterName);
            Assert.Equal(2, parameters.Length);

            Assert.Equal(AccountJson, ReadString(SingleBy(parameters, "Name", "Target"), "ValueJson"));
            Assert.Equal("42", ReadString(SingleBy(parameters, "Name", "Count"), "ValueJson"));
        }

        [Fact]
        public void SharedVariable_ReadsBackPlaceholderNameAndValueJsonVerbatim()
        {
            var variable = Assert.Single(AttributesNamed(typeof(SharedVariablePlaceholderPlugin), SharedVariableName));
            Assert.Equal("<nome>", ReadString(variable, "Name"));
            Assert.Equal("<json entita' con __entityName>", ReadString(variable, "ValueJson"));
        }

        [Fact]
        public void SharedVariable_ReadsBackEntityShapedValueJsonVerbatim()
        {
            var variable = Assert.Single(AttributesNamed(typeof(SharedVariableEntityPlugin), SharedVariableName));
            Assert.Equal("parentAccount", ReadString(variable, "Name"));
            Assert.Equal(AccountJson, ReadString(variable, "ValueJson"));
        }

        [Fact]
        public void OutputParameter_ReadsBackExpectedValueJsonVerbatim()
        {
            var parameters = AttributesNamed(typeof(OutputParameterPlugin), OutputParameterName);
            Assert.Equal(2, parameters.Length);

            Assert.Equal("\"6f1c2a3b-0000-0000-0000-000000000001\"", ReadString(SingleBy(parameters, "Name", "id"), "ExpectedValueJson"));
            Assert.Equal(EntityReferenceJson, ReadString(SingleBy(parameters, "Name", "Result"), "ExpectedValueJson"));
        }
    }

    public class PreImageAttributeTests
    {
        [Fact]
        public void Constructor_SetsNameAndEntityJson()
        {
            var attr = new PreImageAttribute("primary", "{\"name\":\"old\"}");
            Assert.Equal("primary", attr.Name);
            Assert.Equal("{\"name\":\"old\"}", attr.EntityJson);
        }

        [Fact]
        public void Attributes_DefaultsToNull()
        {
            var attr = new PreImageAttribute("primary", "{}");
            Assert.Null(attr.Attributes);
        }
    }

    public class PostImageAttributeTests
    {
        [Fact]
        public void Constructor_SetsNameAndEntityJson()
        {
            var attr = new PostImageAttribute("primary", "{\"name\":\"new\"}");
            Assert.Equal("primary", attr.Name);
            Assert.Equal("{\"name\":\"new\"}", attr.EntityJson);
        }

        [Fact]
        public void Attributes_DefaultsToNull()
        {
            var attr = new PostImageAttribute("primary", "{}");
            Assert.Null(attr.Attributes);
        }
    }

    public class SecureConfigurationAttributeTests
    {
        [Fact]
        public void Constructor_SetsConfiguration()
        {
            var attr = new SecureConfigurationAttribute("secret-cfg");
            Assert.Equal("secret-cfg", attr.Configuration);
        }
    }

    public class UnsecureConfigurationAttributeTests
    {
        [Fact]
        public void Constructor_SetsConfiguration()
        {
            var attr = new UnsecureConfigurationAttribute("public-cfg");
            Assert.Equal("public-cfg", attr.Configuration);
        }
    }

    public class SharedVariableAttributeTests
    {
        [Fact]
        public void Constructor_SetsNameAndValueJson()
        {
            var attr = new SharedVariableAttribute("myVar", "42");
            Assert.Equal("myVar", attr.Name);
            Assert.Equal("42", attr.ValueJson);
        }
    }

    public class SolutionComponentAttributeTests
    {
        [Fact]
        public void Constructor_SetsSolutionUniqueName()
        {
            var attr = new SolutionComponentAttribute("CoreBusinessLogic");
            Assert.Equal("CoreBusinessLogic", attr.SolutionUniqueName);
        }

        [Fact]
        public void IsDefault_DefaultsToFalse()
        {
            var attr = new SolutionComponentAttribute("CoreBusinessLogic");
            Assert.False(attr.IsDefault);
        }
    }
}
