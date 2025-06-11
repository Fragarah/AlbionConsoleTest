using Xunit;
using Moq;
using AlbionConsole.Data;
using AlbionConsole.Services;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;
using Moq.Protected;
using System.Threading;

namespace AlbionTest
{
    public class ItemImporterTest
    {
        private ItemImporter CreateImporterWithMockedHttp(List<AlbionMarketResponse> apiResponse, ApplicationDbContext context)
        {
            var json = JsonSerializer.Serialize(apiResponse);

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var client = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("https://east.albion-online-data.com")
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var logger = new Mock<ILogger<ItemImporter>>().Object;

            return new ItemImporter(context, logger, client);
        }

        [Fact]
        public async Task ImportAsync_Should_Save_New_Item_And_PriceHistory()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var marketData = new List<AlbionMarketResponse>
            {
                new AlbionMarketResponse
                {
                    City = "Bridgewatch",
                    Quality = 1,
                    SellPriceMin = 100,
                    SellPriceMax = 150,
                    BuyPriceMin = 80,
                    BuyPriceMax = 110
                }
            };

            var importer = CreateImporterWithMockedHttp(marketData, context);

            await importer.ImportAsync();

            Assert.NotEmpty(context.Items);
            Assert.NotEmpty(context.PriceHistories);
        }
    }
}
