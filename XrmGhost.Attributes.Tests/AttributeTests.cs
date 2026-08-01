using System;
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
