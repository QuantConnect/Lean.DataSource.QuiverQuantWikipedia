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
using Newtonsoft.Json;
using NUnit.Framework;
using QuantConnect.Data.Auxiliary;
using QuantConnect.DataProcessing;
using QuantConnect.DataSource;
using QuantConnect.Interfaces;
using QuantConnect.Util;

namespace QuantConnect.DataLibrary.Tests
{
    [TestFixture]
    public class JsonConversionTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Composer.Instance.GetExportedValueByTypeName<IMapFileProvider>(Configuration.Config.Get("map-file-provider", typeof(LocalDiskMapFileProvider).Name));
        }

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
        public void DeserializeNaN()
        {
            _jsonSerializerSettings.Converters.Add(new NoNanRealConverter());

            var content = "{" +
                          "\"Date\":\"2020-01-01\"," +
                          "\"Ticker\":\"ABBV\"," +
                          "\"Views\":3500," +
                          "\"pct_change_week\":NaN," +
                          "\"pct_change_month\":NaN}";

            var data = JsonConvert.DeserializeObject<QuiverWikipedia>(content, _jsonSerializerSettings);

            Assert.IsNull(data.WeekPercentChange);
            Assert.IsNull(data.MonthPercentChange);
        }
    }
}
