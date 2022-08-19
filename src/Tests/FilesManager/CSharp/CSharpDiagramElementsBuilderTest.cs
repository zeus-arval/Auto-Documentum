using AD.Aids.Factories;
using AD.FilesManager.Common;
using AD.FilesManager.CSharp;
using AD.FilesManager.CSharp.FileContentElements;
using Tests.TestHelper;

namespace Tests.FilesManager.CSharp
{
    [TestFixture]
    internal class CSharpDiagramElementsBuilderTest
    {
        private class CSharpDiagramElementsBuilderMock : CSharpDiagramElementsBuilder
        {
            private const string TXT_FILE_FORMAT = "*.txt";
            public CSharpDiagramElementsBuilderMock(MainFactory mainFactory) : base(mainFactory)
            {
                generator.filesController.FileFormatPattern = TXT_FILE_FORMAT;
            }
        }

        private CSharpDiagramElementsBuilderMock _elementsBuilder;
        private string TEST_DIRECTORY_PATH = AppDomain.CurrentDomain.BaseDirectory.GetDirectoryParentPath(3).GoToChildsDirectory("TestCases");


        [SetUp]
        public void Setup()
        {
            MainFactory factory = new MainFactory();
            _elementsBuilder = new CSharpDiagramElementsBuilderMock(factory);
        }

        [TestCase(TestName = "Test classes' extraction")]
        public void TestClassesExtraction()
        {
            string fullPath = TEST_DIRECTORY_PATH.GoToChildsDirectory(@"TestClasses.txt");

            _elementsBuilder.CreateSyntaxTreeArray(fullPath);
            List<IClass> classes = _elementsBuilder.ClassesList!;

            List<CSharpClass> expectedClasses = InitializeExpectedClasses();

            CheckClasses(classes, expectedClasses);
        }

        private void CheckClasses(List<IClass> classes, List<CSharpClass> expectedClasses)
        {
            for (int classNum = 0; classNum < classes.Count; classNum++)
            {
                var actualClass = classes[classNum] as CSharpClass;
                var expectedClass = expectedClasses[classNum];

                Assert.AreEqual(actualClass.Name, expectedClass.Name);
                Assert.AreEqual(actualClass.Description, expectedClass.Description);
                Assert.AreEqual(actualClass.NamespaceName, expectedClass.NamespaceName);

                CheckMethods(actualClass, expectedClass);
                CheckFields(actualClass, expectedClass);
                CheckProperties(actualClass, expectedClass);
            }
        }

        private List<CSharpClass> InitializeExpectedClasses() 
        {
            var personMethods = new CSharpMethod[]
            {
                new CSharpMethod(Array.Empty<CSharpParameter>(), "PrintInfo", "void", string.Empty),
                new CSharpMethod(new CSharpParameter[]
                {
                    new CSharpParameter("string?", "firstName", string.Empty),
                    new CSharpParameter("string?", "lastName", string.Empty),
                }, "ChangeName", "string", string.Empty),
            };

            var personFields = new CSharpField[]
            {
                new CSharpField("string", "_idCode", string.Empty),
                new CSharpField("DateTime", "_birthDay", string.Empty),
                new CSharpField("House?", "_house", string.Empty)
            };

            var personProperties = new CSharpProperty[]
            {
                new CSharpProperty("string", "Address", string.Empty),
                new CSharpProperty("string", "FirstName", string.Empty),
                new CSharpProperty("string", "LastName", string.Empty),
                new CSharpProperty("long", "PhoneNumber", string.Empty),
            };

            var houseMethods = new CSharpMethod[]
            {
                new CSharpMethod(new CSharpParameter[]
                {
                    new CSharpParameter("NamespaceB.Person", "person", string.Empty),
                }, "TryAddPerson", "bool", string.Empty),
            };

            var houseFields = Array.Empty<CSharpField>();

            var houseProperties = new CSharpProperty[]
            {
                new CSharpProperty("string", "FullAddress", string.Empty),
                new CSharpProperty("List<Person?>?", "Persons", string.Empty),
            };

            return new List<CSharpClass>()
            {
                new CSharpClass("NamespaceA.NamespaceB", string.Empty, "Person", personMethods, personFields, personProperties),
                new CSharpClass("NamespaceA", string.Empty, "House", houseMethods, houseFields, houseProperties),
            };
        }

        private void CheckProperties(CSharpClass actualClass, CSharpClass expectedClass)
        {
            for (int i = 0; i < actualClass.Fields.Count(); i++)
            {
                var expectedProperty = expectedClass.Properties[i];
                var actualProperty = actualClass.Properties[i];

                Assert.That(actualProperty.Name, Is.EqualTo(expectedProperty.Name));
                Assert.That(actualProperty.TypeName, Is.EqualTo(expectedProperty.TypeName));
                Assert.That(actualProperty.Description, Is.EqualTo(expectedProperty.Description));
            }
        }

        private void CheckFields(CSharpClass actualClass, CSharpClass expectedClass)
        {
            for(int i = 0; i < actualClass.Fields.Count(); i++)
            {
                var expectedField = expectedClass.Fields[i];
                var actualField = actualClass.Fields[i];

                Assert.That(actualField.Name, Is.EqualTo(expectedField.Name));
                Assert.That(actualField.TypeName, Is.EqualTo(expectedField.TypeName));
                Assert.That(actualField.Description, Is.EqualTo(expectedField.Description));
            }
        }

        private void CheckMethods(CSharpClass actualClass, CSharpClass expectedClass)
        {
            for (int i = 0; i < actualClass.Methods.Count(); i++)
            {
                var actualMethod = actualClass.Methods[i] as CSharpMethod;
                var expectedMethod = expectedClass.Methods[i] as CSharpMethod;

                Assert.That(expectedMethod!.Description, Is.EqualTo(actualMethod!.Description));
                Assert.That(expectedMethod.Name, Is.EqualTo(actualMethod!.Name));
                Assert.That(expectedMethod.ReturnType, Is.EqualTo(actualMethod!.ReturnType));

                CheckParameters(expectedMethod, actualMethod);
            }
        }

        private void CheckParameters(CSharpMethod expectedMethod, CSharpMethod actualMethod)
        {
            for(int i = 0; i < expectedMethod.Parameters.Count(); i++)
            {
                var actualParameter = actualMethod.Parameters[i];
                var expectedParameter = expectedMethod.Parameters[i];

                Assert.That(expectedParameter.Name, Is.EqualTo(actualParameter.Name));
                Assert.That(expectedParameter.TypeName, Is.EqualTo(actualParameter.TypeName));
                Assert.That(expectedParameter.Description, Is.EqualTo(actualParameter.Description));
            }
        }
    }
}
