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
