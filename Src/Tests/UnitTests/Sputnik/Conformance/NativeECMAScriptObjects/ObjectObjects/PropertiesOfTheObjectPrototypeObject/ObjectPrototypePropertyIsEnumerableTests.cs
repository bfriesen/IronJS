// <auto-generated />
namespace IronJS.Tests.UnitTests.Sputnik.Conformance.NativeECMAScriptObjects.ObjectObjects.PropertiesOfTheObjectPrototypeObject
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectPrototypePropertyIsEnumerableTests : SputnikTestFixture
    {
        public ObjectPrototypePropertyIsEnumerableTests()
            : base(@"Conformance\15_Native_ECMA_Script_Objects\15.2_Object_Objects\15.2.4_Properties_of_the_Object_Prototype_Object\15.2.4.7_Object.prototype.propertyIsEnumerable")
        {
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A1_T1.js", Description = "The propertyIsEnumerable method does not consider objects in the prototype chain")]
        public void ThePropertyIsEnumerableMethodDoesNotConsiderObjectsInThePrototypeChain(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A10.js", Description = "The Object.prototype.propertyIsEnumerable.length property has the attribute ReadOnly")]
        public void TheObjectPrototypePropertyIsEnumerableLengthPropertyHasTheAttributeReadOnly(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A11.js", Description = "The length property of the hasOwnProperty method is 1")]
        public void TheLengthPropertyOfTheHasOwnPropertyMethodIs1(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A2_T1.js", Description = "When the propertyIsEnumerable method is called with argument V, the following steps are taken: i) Let O be this object ii) Call ToString(V) iii) If O doesn\'t have a property with the name given by Result(ii), return false iv) If the property has the DontEnum attribute, return false v) Return true")]
        [TestCase("S15.2.4.7_A2_T2.js", Description = "When the propertyIsEnumerable method is called with argument V, the following steps are taken: i) Let O be this object ii) Call ToString(V) iii) If O doesn\'t have a property with the name given by Result(ii), return false iv) If the property has the DontEnum attribute, return false v) Return true")]
        public void WhenThePropertyIsEnumerableMethodIsCalledWithArgumentVTheFollowingStepsAreTakenILetOBeThisObjectIiCallToStringVIiiIfODoesnTHaveAPropertyWithTheNameGivenByResultIiReturnFalseIvIfThePropertyHasTheDontEnumAttributeReturnFalseVReturnTrue(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 13.2")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A6.js", Description = "Object.prototype.propertyIsEnumerable has not prototype property")]
        public void ObjectPrototypePropertyIsEnumerableHasNotPrototypeProperty(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 13.2")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A7.js", Description = "Object.prototype.propertyIsEnumerable can\'t be used as a constructor")]
        public void ObjectPrototypePropertyIsEnumerableCanTBeUsedAsAConstructor(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A8.js", Description = "The Object.prototype.propertyIsEnumerable.length property has the attribute DontEnum")]
        public void TheObjectPrototypePropertyIsEnumerableLengthPropertyHasTheAttributeDontEnum(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Conformance")]
        [Category("ECMA 15.2.4.7")]
        [TestCase("S15.2.4.7_A9.js", Description = "The Object.prototype.propertyIsEnumerable.length property has the attribute DontDelete")]
        public void TheObjectPrototypePropertyIsEnumerableLengthPropertyHasTheAttributeDontDelete(string file)
        {
            RunFile(file);
        }
    }
}