using System.Net;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeletekstBot.Domain.Entities;
using TeletekstBot.Infrastructure.Interfaces;
using TeletekstBot.Infrastructure.Services;

// ReSharper disable StringLiteralTypo

namespace TeletekstBot.Tests.Infrastructure;

public class FetchPageFromNosTests
{
    [Test]
    public async Task FetchPageFromNos_WithValidPageNumberReturnsPage()
    {
        // Arrange
        const int pageNr = 110;

        var logger = Substitute.For<ILogger<FetchPageFromNos>>();
        var parseIncomingPage = Substitute.For<IParseIncomingPage>();
        var expectedPage = new Page
        {
            PageNumber = pageNr,
            Title = "NS verliest alleenrecht buitenland",
            Body = "Het demissionaire kabinet wil naast..."
        };
        parseIncomingPage.ParseHtml("").ReturnsForAnyArgs(expectedPage);

        var pageJson = MockFile.GetFileText("correctPage110.json");
        var fakeResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(pageJson)
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

        var httpClientFactorySubstitute = Substitute.For<IHttpClientFactory>();
        httpClientFactorySubstitute.CreateClient(Arg.Any<string>()).Returns(fakeHttpClient);

        var fetchPageFromNos = new FetchPageFromNos(logger, httpClientFactorySubstitute, parseIncomingPage);

        // Act
        var result = await fetchPageFromNos.Get(pageNr, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedPage));
    }

    [Test]
    public async Task FetchPageFromNos_WithNonExistingPageNumberReturnsNull()
    {
        // Arrange
        const int pageNr = 120;

        var logger = Substitute.For<ILogger<FetchPageFromNos>>();
        var parseIncomingPage = Substitute.For<IParseIncomingPage>();

        var fakeResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

        var httpClientFactorySubstitute = Substitute.For<IHttpClientFactory>();
        httpClientFactorySubstitute.CreateClient(Arg.Any<string>()).Returns(fakeHttpClient);

        var fetchPageFromNos = new FetchPageFromNos(logger, httpClientFactorySubstitute, parseIncomingPage);

        // Act
        var result = await fetchPageFromNos.Get(pageNr, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task FetchPageFromNos_IncorrectResponseReturnsNull()
    {
        // Arrange
        const int pageNr = 110;

        var logger = Substitute.For<ILogger<FetchPageFromNos>>();
        var parseIncomingPage = Substitute.For<IParseIncomingPage>();

        var pageJson = MockFile.GetFileText("wrongPage110.json");
        var fakeResponseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(pageJson)
        };
        var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeResponseMessage);
        var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

        var httpClientFactorySubstitute = Substitute.For<IHttpClientFactory>();
        httpClientFactorySubstitute.CreateClient(Arg.Any<string>()).Returns(fakeHttpClient);

        var fetchPageFromNos = new FetchPageFromNos(logger, httpClientFactorySubstitute, parseIncomingPage);

        // Act
        var result = await fetchPageFromNos.Get(pageNr, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }
}

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_response);
    }
}