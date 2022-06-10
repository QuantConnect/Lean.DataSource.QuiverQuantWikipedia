/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using QuantConnect.Data;
using QuantConnect.DataSource;

namespace QuantConnect.DataLibrary.Tests
{
    [TestFixture]
    public class QuiverWikipediaTests
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        [Test]
        public void DeserializeQuiverWikipedia()
        {
            var content = "{" +
                          "\"Date\":\"2020-01-01\"," +
                          "\"Ticker\":\"ABBV\"," +
                          "\"Views\":3500," +
                          "\"pct_change_week\":3.2," +
                          "\"pct_change_month\":6.75}";

            var data = JsonConvert.DeserializeObject<QuiverWikipedia>(content, _jsonSerializerSettings);

            Assert.NotNull(data);
            Assert.AreEqual(new DateTime(2020, 01, 01, 0, 0, 0), data.Date);
            Assert.AreEqual(3500, data.PageViews);
            Assert.AreEqual(3.2, data.WeekPercentChange);
            Assert.AreEqual(6.75, data.MonthPercentChange);
        }

        [Test]
        public void QuiverWikipediaReader()
        {
            var data = "20201110,1599,-1.9018404908,-9.4050991501";
            var instance = new QuiverWikipedia();

            var fakeConfig = new SubscriptionDataConfig(
                typeof(QuiverWikipedia),
                Symbol.Create("ABBV", SecurityType.Base, "USA"),
                Resolution.Daily,
                TimeZones.Utc,
                TimeZones.Utc,
                false,
                false,
                false
            );

            Assert.DoesNotThrow(() => { instance.Reader(fakeConfig, data, DateTime.MinValue, false); });

            Assert.DoesNotThrow(() =>
            {
                new QuiverWikipedia(data);
            });
            var testCase = new QuiverWikipedia(data);
            Assert.AreEqual(new DateTime(2020, 11, 10, 0, 0, 0), testCase.Time);
            Assert.IsTrue(testCase.PageViews.HasValue);
            Assert.IsTrue(testCase.WeekPercentChange.HasValue);
            Assert.IsTrue(testCase.MonthPercentChange.HasValue);

            Assert.AreEqual(testCase.PageViews, 1599);
            Assert.AreEqual(testCase.WeekPercentChange.Value, -1.9018404908m);
            Assert.AreEqual(testCase.MonthPercentChange.Value, -9.4050991501m);
        }

        [Test]
        public void JsonRoundTrip()
        {
            var expected = CreateNewInstance();
            var type = expected.GetType();
            var serialized = JsonConvert.SerializeObject(expected);
            var result = JsonConvert.DeserializeObject(serialized, type);

            AssertAreEqual(expected, result);
        }

        [Test]
        public void Clone()
        {
            var expected = CreateNewInstance();
            var result = expected.Clone();

            AssertAreEqual(expected, result);
        }

        private void AssertAreEqual(object expected, object result, bool filterByCustomAttributes = false)
        {
            foreach (var propertyInfo in expected.GetType().GetProperties())
            {
                // we skip Symbol which isn't protobuffed
                if (filterByCustomAttributes && propertyInfo.CustomAttributes.Count() != 0)
                {
                    Assert.AreEqual(propertyInfo.GetValue(expected), propertyInfo.GetValue(result));
                }
            }
            foreach (var fieldInfo in expected.GetType().GetFields())
            {
                Assert.AreEqual(fieldInfo.GetValue(expected), fieldInfo.GetValue(result));
            }
        }

        private BaseData CreateNewInstance()
        {
            return new QuiverWikipedia
            {
                Symbol = Symbol.Empty,
                Time = DateTime.Today,
                DataType = MarketDataType.Base,
                
                Date = new DateTime(2020, 1, 5),
                PageViews = 1000,
                WeekPercentChange = 5m,
                MonthPercentChange = null
            };
        }
    }
}
