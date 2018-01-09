using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using PS.Navigation;
using PS.Navigation.Extensions;
using PS.Tests.TestReferences.RouteTests;

namespace PS.Tests.Navigation
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void BinarySerialization_Route_Success()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, Constants.Route1);
                stream.Seek(0, SeekOrigin.Begin);
                Assert.AreEqual(Constants.Route1, formatter.Deserialize(stream));
            }
        }

        [Test]
        public void Clone_Route_Success()
        {
            Assert.AreEqual(Constants.Route1, Constants.Route1.Clone());
        }

        [Test]
        public void ContainsWith_Route_Success()
        {
            Assert.IsTrue(Constants.Route123.Contains(Constants.Route12));
            Assert.IsTrue(Constants.Route123.Contains(Constants.Route23));
            Assert.IsTrue(Constants.Route123.Contains(Constants.Route123));

            Assert.IsTrue(Constants.Route123.Contains(Constants.Route1W3));
            Assert.IsTrue(Constants.Route123.Contains(Constants.Route1R3));

            Assert.IsFalse(Constants.Route1W3.Contains(Constants.Route123));
            Assert.IsFalse(Constants.Route1R3.Contains(Constants.Route123));

            Assert.IsFalse(Constants.Route123.Contains(null));
            Assert.IsFalse(Constants.Route123.Contains(Routes.Empty));
        }

        [Test]
        public void Create_Route_Success()
        {
            Assert.AreEqual(Routes.Empty, Route.Create(null));
            Assert.AreEqual(Routes.Empty, Route.Create());

            Assert.AreEqual(Constants.Route1, Route.Create(Constants.String1));
            Assert.AreEqual(Constants.Route1, Route.Create(Constants.Object1));
            Assert.AreEqual(Constants.Route1, Route.Create(Constants.Route1));

            Assert.AreEqual(Constants.Route12, Route.Create(Constants.String1, Constants.String2));
            Assert.AreEqual(Constants.Route12, Route.Create(Constants.Route12));

            Assert.AreEqual(Constants.Route123, Route.Create(Constants.Route12, Constants.Object3));
            Assert.AreEqual(Constants.Route123, Route.Create(Constants.Route12, Constants.Route3));

            Assert.AreEqual(Constants.Route123, Route.Create(new object[] { Constants.Route1, Constants.String2 }, Constants.Route3));
            Assert.AreEqual(Constants.Route123, Route.Create(new object[] { Constants.Route1, null, Constants.String2 }, Constants.Route3));
        }

        [Test]
        public void CreateFromUri_Route_Success()
        {
            Assert.AreEqual(Route.Create(Uri.UriSchemeFile + ":",
                                         Route.Parse(Directory.GetCurrentDirectory(), new RouteFormatting("\\")),
                                         "asd",
                                         "1.txt"),
                            Route.CreateFromUri(new Uri(@".\asd\.\aaa\..\1.txt", UriKind.RelativeOrAbsolute)));

            Assert.AreEqual(Route.Create(Uri.UriSchemeFile + ":", "asd", "aaa", "1.txt"),
                            Route.CreateFromUri(@"\\asd\aaa\1.txt"));

            Assert.AreEqual(Route.Create(Uri.UriSchemeFile + ":", "asd", "aaa", "1.txt"),
                            Route.CreateFromUri(@"\\asd\aaa\1.txt\"));

            Assert.AreEqual(Route.Create(Uri.UriSchemeFile + ":", "C:", "Test Project.exe"),
                            Route.CreateFromUri(@"file:///C:/Test%20Project.exe"));

            Assert.AreEqual(Route.Create(Uri.UriSchemeFile + ":", "C:", "Test Project.exe"),
                            Route.CreateFromUri(@"file:///C:/asd/.././Test%20Project.exe"));

            Assert.AreEqual(Route.Create(Uri.UriSchemeHttps + ":", "play.google.com", "music", "listen#", "home"),
                            Route.CreateFromUri(@"https://play.google.com/music/listen#/home"));

            //var wild = Route.Create(Routes.WildcardRecursive, "temp");
            //foreach (var file in Directory.EnumerateFiles(@"e:\Temp\"))
            //{
            //    var route111 = Route.CreateFromUri(file);
            //    var ss = route111.Contains(wild, RouteCaseMode.Insensitive);
            //}
        }

        [Test]
        public void EndWith_Route_Success()
        {
            Assert.IsTrue(Constants.Route123.EndWith(Constants.Route23));
            Assert.IsTrue(Constants.Route123.EndWith(Constants.Route123));
            Assert.IsTrue(Constants.Route123.EndWith(Constants.Route1W3));
            Assert.IsTrue(Constants.Route123.EndWith(Constants.Route1R3));
            Assert.IsFalse(Constants.Route1W3.EndWith(Constants.Route123));
            Assert.IsFalse(Constants.Route1R3.EndWith(Constants.Route123));

            Assert.IsFalse(Constants.Route123.EndWith(Constants.Route12));
            Assert.IsFalse(Constants.Route123.EndWith(null));
            Assert.IsFalse(Constants.Route123.EndWith(Routes.Empty));
        }

        [Test]
        public void Equal_Route_Success()
        {
            Assert.AreEqual(Constants.Route1, Route.Create(Constants.Object1));
            Assert.AreEqual(Constants.Route1, Constants.Route1);
            Assert.AreNotEqual(Constants.Route1, Route.Create(Constants.Object2));
            Assert.AreNotEqual(Constants.Route1, Routes.Empty);
            Assert.IsTrue(Equals(Constants.Route1, Route.Create(Constants.Object1)));
            Assert.IsTrue(Constants.Route1 == Route.Create(Constants.Object1));
            Assert.IsTrue(Constants.Route1 != Routes.Empty);
        }

        [Test]
        public void Formatting_Route_Success()
        {
            Assert.AreEqual(Constants.Route1.ToString(), Constants.String1);
            Assert.AreEqual(Constants.RouteDot.ToString(), RouteFormatting.EscapeSymbol + Constants.StringDot);

            Assert.AreEqual(Constants.Route12.ToString(),
                            string.Join(RouteFormatting.Default.Separator,
                                        Constants.String1,
                                        Constants.String2));

            Assert.AreEqual("*.**.*", Route.Create(Routes.Wildcard, Routes.WildcardRecursive, Routes.Wildcard).ToString());

            var customFormatting = new RouteFormatting
            {
                Separator = "/"
            };

            Assert.AreEqual(Constants.Route12.ToString(customFormatting),
                            string.Join(customFormatting.Separator,
                                        Constants.String1,
                                        Constants.String2));

            Assert.AreEqual(((IFormattable)Constants.Route12).ToString(null, customFormatting),
                            string.Join(customFormatting.Separator,
                                        Constants.String1,
                                        Constants.String2));

            Assert.AreEqual(((IFormattable)Constants.Route12).ToString("any", customFormatting),
                            string.Join(customFormatting.Separator,
                                        Constants.String1,
                                        Constants.String2));
        }

        [Test]
        public void ParseString_Route_Success()
        {
            Assert.AreEqual(Constants.Route123,
                            Route.Parse(string.Join(RouteFormatting.Default.Separator,
                                                    Constants.String1,
                                                    Constants.String2,
                                                    Constants.String3)));

            Assert.AreEqual(Constants.Route1Dot2,
                            Route.Parse(string.Join(RouteFormatting.Default.Separator,
                                                    Constants.String1,
                                                    RouteFormatting.EscapeSymbol + Constants.StringDot,
                                                    Constants.String2)));

            var customFormatting = new RouteFormatting
            {
                Separator = "->"
            };
            Assert.AreEqual(Constants.Route123,
                            Route.Parse(string.Join(customFormatting.Separator,
                                                    Constants.String1,
                                                    Constants.String2,
                                                    Constants.String3),
                                        customFormatting));
        }

        [Test]
        public void RecursiveSplit_Route_Failure()
        {
            var result = Route.Parse("1.2.3.4.5.6.7.8.9")
                              .RecursiveSplit(Route.Parse("1.2.**.6.*.9"));
            Assert.IsNull(result);
        }

        [Test]
        public void RecursiveSplit_Route_Success()
        {
            var complexSplit = Route.Parse("1.2.3.4.5.6.7.8.9")
                                    .RecursiveSplit(Route.Parse("2.**.6.7.*"));
            Assert.IsNotNull(complexSplit);
            Assert.AreEqual(Route.Parse("1.2"), complexSplit.Prefix);
            Assert.AreEqual(Route.Parse("3.4.5.6.7.8"), complexSplit.Recursive);
            Assert.AreEqual(Route.Parse("9"), complexSplit.Postfix);

            var wildFreeSplit = Route.Parse("1.2.3.4.5.6.7.8.9")
                                     .RecursiveSplit(Route.Parse("2.3"));
            Assert.IsNotNull(wildFreeSplit);
            Assert.AreEqual(Routes.Empty, wildFreeSplit.Prefix);
            Assert.AreEqual(Routes.Empty, wildFreeSplit.Recursive);
            Assert.AreEqual(Route.Parse("2.3"), wildFreeSplit.Postfix);
        }

        [Test]
        public void StartWith_Route_Success()
        {
            Assert.IsTrue(Constants.Route123.StartWith(Constants.Route12));
            Assert.IsTrue(Constants.Route123.StartWith(Constants.Route123));
            Assert.IsFalse(Constants.Route123.StartWith(Constants.Route23));

            Assert.IsTrue(Constants.Route123.StartWith(Constants.Route1W3));
            Assert.IsTrue(Constants.Route123.StartWith(Constants.Route1R3));

            Assert.IsFalse(Constants.Route1W3.StartWith(Constants.Route123));
            Assert.IsFalse(Constants.Route1R3.StartWith(Constants.Route123));

            Assert.IsFalse(Constants.Route123.StartWith(null));
            Assert.IsFalse(Constants.Route123.StartWith(Routes.Empty));
        }

        [Test]
        public void XmlSerialization_Route_Success()
        {
            using (var stream = new MemoryStream())
            {
                var xmlSerializer = new XmlSerializer(typeof(Route));
                var expected = Constants.Route12;
                Assert.IsNull(((IXmlSerializable)expected).GetSchema());

                xmlSerializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = XmlReader.Create(stream,
                                                     new XmlReaderSettings
                                                     {
                                                         IgnoreWhitespace = true
                                                     }))
                {
                    Assert.AreEqual(expected, xmlSerializer.Deserialize(reader));
                }
            }
        }
    }
}