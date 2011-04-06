// <auto-generated />
namespace IronJS.Tests.UnitTests.Sputnik.Conformance.NativeECMAScriptObjects.ObjectObjects
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectObjectsTests : SputnikTestFixture
    {
        public ObjectObjectsTests()
            : base(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects")
        {
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2")]
        [TestCase("S15.2_A1.js", Description = "Object is the property of global")]
        public void ObjectIsThePropertyOfGlobal(string file)
        {
            RunFile(file);
        }
    }
}